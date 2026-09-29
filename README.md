# Armor the Vehicle

Тестове завдання на позицію Unity Developer: core-механіка гри, де гравець керує туреллю на автомобілі, що їде вперед, і відстрілює ворогів-стікменів.

## Геймплей

1. Авто стоїть на старті, камера позаду, на екрані «Tap to start».
2. Тап: авто плавно розганяється, камера слідує.
3. Вороги стоять в Idle у випадкових місцях. Коли авто наближається, ворог біжить до нього, б'є один раз і вибухає.
4. Гравець крутить турель драгом по горизонталі. Поки палець або кнопка миші затиснуті, турель стріляє автоматично.
5. ХП авто закінчилось: «You lose». Авто доїхало до фінішу: плавно зупиняється, «You win».
6. Тап після результату: рівень починається спочатку без перезавантаження сцени.

## Керування

| Дія | Миша | Тач |
|---|---|---|
| Старт / рестарт | клік | тап |
| Стрільба | затиснути ЛКМ | затиснути палець |
| Поворот турелі | тягнути вліво / вправо | свайп вліво / вправо |

Чутливість нормалізована до ширини екрана, тому однакова на будь-якій роздільній здатності.

## Запуск

1. Unity **6000.3.0f1** (URP).
2. Відкрити проєкт. Пакети підтягнуться автоматично; для git-залежностей потрібен встановлений Git.
3. Сцена: `Assets/Scenes/Game.unity` → Play.

## Стек

| Що | Пакет |
|---|---|
| DI | Extenject (Zenject) 9.2, запінений на коміт `9dca4e0` |
| Async | UniTask 2.5.11 |
| Камера | Cinemachine 3.1.7 |
| Ввід | Input System 1.16 (миша і тач однією прив'язкою `<Pointer>`) |
| Пули | `UnityEngine.Pool.ObjectPool<T>` |
| Рендер | URP 17.3 |

Extenject запінений на коміт, бо поточний `master` містить Addressables-провайдери без `#if`-захисту і не компілюється без пакета Addressables.

## Структура

```
Assets/
├── Art/            Animations (Mixamo), Materials, Models, Textures
├── Configs/        ScriptableObject-конфіги балансу
├── Prefabs/        Bullet, Car, Enemy, Game (GroundTile, Level), UI
├── Scenes/         Game.unity
├── Settings/       URP-асети
└── Scripts/
    ├── Core/       GameInstaller, GameFlow, GameResult
    ├── Configs/    LevelConfig, CarConfig, TurretConfig, EnemyConfig
    ├── Input/      IInputService, InputService
    ├── Gameplay/
    │   ├── Car/      CarController, CarDamageFeedback
    │   ├── Turret/   TurretController
    │   ├── Combat/   Health, Bullet, BulletPool, HitFlash
    │   ├── Enemies/  Enemy, EnemyPool, EnemySpawner, EnemyHealthBar, KillCounter
    │   └── Level/    LevelBuilder
    └── UI/         GameUi, UiPanel, GameplayHud
```

## Архітектура

### Composition root

`GameInstaller` (Zenject `MonoInstaller`) — єдине місце, де описано всі зв'язки: конфіги, об'єкти сцени, сервіси, пули, точка входу. Звичайні класи отримують залежності через конструктор, MonoBehaviour-и — через `[Inject] Construct(...)`. У проєкті немає синглтонів, статичного стану (крім хешів Animator і ID шейдерних властивостей), `Find` і `FindObjectOfType`.

### Флоу гри

`GameFlow` (`IInitializable`, `IDisposable`) — уся гра як один async-сценарій на UniTask:

```csharp
ResetLevel();
while (!cancellation.IsCancellationRequested)
{
    await _ui.ShowStartHintAsync(cancellation);
    await _input.WaitForTapAsync(cancellation);
    _ui.HideStartHintAsync(cancellation).Forget();

    _car.StartDriving();
    _turret.enabled = true;

    GameResult result = await WaitForResultAsync(cancellation); // WhenAny(фініш, смерть)

    _turret.enabled = false;
    _car.Stop();

    await _ui.ShowResultAsync(result, cancellation);
    await _input.WaitForTapAsync(cancellation);

    ResetLevel();                       // за непрозорим overlay
    await _ui.HideResultAsync(cancellation);
}
```

- Стан гри тримається в одному місці — на тому `await`, де зараз стоїть цикл. Окремих класів на стан немає.
- «Фініш чи смерть» — `UniTask.WhenAny` з linked `CancellationTokenSource` на раунд, щоб очікування, що програло, не працювало в наступному раунді.
- Токен скасовується в `Dispose()` при вивантаженні сцени.
- Рестарт без перезавантаження сцени: кожна система скидає себе сама (`ResetState`, `ReleaseAll`, `SpawnAll`).

### Системи

| Клас | Відповідальність |
|---|---|
| `CarController` | Рух уперед із плавним розгоном і гальмуванням (`Mathf.MoveTowards`), `Health`, `WaitForFinishAsync` / `WaitForDeathAsync` |
| `CarDamageFeedback` | Тряска камери (`CinemachineImpulseSource`) і спалах кузова на урон |
| `TurretController` | Поворот за драгом у межах ±`MaxAngle` зі згладженням `SmoothDampAngle`, автоматична стрільба, віддача, muzzle flash |
| `Health` | Plain C#: `Max`, `Current`, `IsDead`, події `Changed` / `Damaged` / `Died`. Спільний для авто і ворогів |
| `Bullet` / `BulletPool` | Куля на фізиці (Rigidbody, Continuous Speculative), урон через `OnCollisionEnter`, повернення в пул за влучанням або часом життя |
| `Enemy` | Стейт-машина `Idle → Chase → Dead` (`enum` + `switch`), удар по авто, спалах, іскри, відкид, вибух |
| `EnemyPool` / `EnemySpawner` | Пул створює ворогів через `DiContainer`; спавнер розставляє їх випадково з мінімальною відстанню (rejection sampling з лімітом спроб) |
| `LevelBuilder` | Зациклює тайл землі на довжину рівня, довжину тайла вимірює з `Renderer.bounds`; ставить фініш |
| `InputService` | Обгортка над Input System: `IsPressed`, `HorizontalDelta`, `WaitForTapAsync` |
| `GameUi` / `UiPanel` | Фасад UI для флоу; показ і приховування панелей через fade + scale на UniTask |
| `GameplayHud` | ХП-бар авто з відстаючим другим шаром і лічильник вбивств |

## Конфіги

Весь баланс — у `Assets/Configs` (ScriptableObject, поля лише для читання з коду).

| Конфіг | Поля |
|---|---|
| `LevelConfig` | довжина рівня 200, ворогів 30, мін. відстань спавну від старту 30, мін. відстань між ворогами 4, півширина дороги 4 |
| `CarConfig` | ХП 100, швидкість 8, розгін 4, гальмування 6 |
| `TurretConfig` | кут ±60°, чутливість 180°/ширина екрана, згладження 0.08 с, інтервал пострілу 0.12 с, швидкість кулі 40, урон 10, життя кулі 2 с |
| `EnemyConfig` | ХП 30, радіус виявлення 20, швидкість 9, поворот 720°/с, дальність удару 2.5, урон 20, відкид 4 / 25, тривалість вибуху 1 с |

