using ArmorTheVehicle.Configs;
using UnityEngine;
using Zenject;

namespace ArmorTheVehicle.Gameplay.Level
{
    public class LevelBuilder : MonoBehaviour
    {
        [SerializeField] private GameObject _groundTilePrefab;
        [SerializeField] private Transform _tilesRoot;
        [SerializeField] private Transform _finish;

        [SerializeField] private int _paddingTiles = 3;

        private LevelConfig _config;

        [Inject]
        private void Construct(LevelConfig config)
        {
            _config = config;
        }

        public void Build()
        {
            GameObject firstTile = Instantiate(_groundTilePrefab, _tilesRoot);

            Bounds bounds = firstTile.GetComponentInChildren<Renderer>().bounds;

            float tileLength = bounds.size.z;
            float pivotToBackEdge = firstTile.transform.position.z - bounds.min.z;

            int tileCount = Mathf.CeilToInt(_config.LevelLength / tileLength) + _paddingTiles * 2;
            float firstBackEdgeZ = -_paddingTiles * tileLength;

            PlaceTile(firstTile.transform, firstBackEdgeZ + pivotToBackEdge);

            for (int i = 1; i < tileCount; i++)
            {
                GameObject tile = Instantiate(_groundTilePrefab, _tilesRoot);

                PlaceTile(tile.transform, firstBackEdgeZ + i * tileLength + pivotToBackEdge);
            }

            _finish.position = new Vector3(0f, 0f, _config.LevelLength);
        }

        private static void PlaceTile(Transform tile, float z)
        {
            tile.position = new Vector3(0f, 0f, z);
        }
    }
}
