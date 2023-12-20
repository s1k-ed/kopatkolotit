using UnityEngine;

namespace Scripts.Environment
{
    public class EnvironmentMetaData
    {
        private int _environmentId;
        private Vector2Int _startPoint;
        private Vector2Int _endPoint;
        private Vector3Int _spawnPoint;

        public int EnvironmentId => _environmentId;
        public Vector2Int StartPoint => _startPoint;
        public Vector2Int EndPoint => _endPoint;
        public Vector3Int SpawnPoint => _spawnPoint;

        public EnvironmentMetaData(int environmentId, Vector2Int startPoint, Vector2Int endPoint, Vector3Int spawnPoint)
        {
            _environmentId = environmentId;
            _startPoint = startPoint;
            _endPoint = endPoint;
            _spawnPoint = spawnPoint;
        }

    }
}
