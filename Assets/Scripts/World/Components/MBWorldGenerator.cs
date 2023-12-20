using System.Collections.Generic;
using System.Linq;
using Configs.Environment;
using Configs.World;
using Configs.World.Blocks;
using Configs.World.Chunks;
using Scripts.Environment;
using Scripts.World.Generators;
using UnityEngine;

namespace Scripts.World.Components
{
    public class MBWorldGenerator : MonoBehaviour
    {
        [SerializeField] private int _seedBounds;
        [SerializeField] private WorldConfig _worldConfig;
        [SerializeField] private ChunkConfig _chunkConfig;
        [SerializeField] private BlockConfig[] _blockConfigs;
        [SerializeField] private EnvironmentConfig _environmentConfig;
        [SerializeField] private GameObject _player;
        [SerializeField] private int _range;

        private Dictionary<Vector2Int, Chunk> _chunks; 
        private Dictionary<Vector2Int, GameObject> _chunkGOs;
        private Vector2Int _playerLastPosition;

        private async void Start()
        {
            var environmentGenerator = new EnvironmentGenerator(_environmentConfig, new BlockGenerator());
            var worldGenerator = new WorldGenerator(environmentGenerator, _worldConfig, _chunkConfig, _blockConfigs);

            var seed = Random.Range(-_seedBounds, _seedBounds);

            _chunks = await worldGenerator.GenerateChunksAsync(seed);
            worldGenerator.CreateEnvironment(_chunks);
            await worldGenerator.UpdateEnvironmentAsync(_chunks);
            _chunkGOs = new Dictionary<Vector2Int, GameObject>();

            foreach(var chunkPositions in _chunks.Keys)
            {
                var x = chunkPositions.x;
                var z = chunkPositions.y;

                var chunkGO = new GameObject($"Chunk({x},{z})");

                chunkGO.SetActive(false);

                chunkGO.transform.position = new Vector3(x * 16, 0, z * 16);
                chunkGO.AddComponent<ChunkRenderer>().RenderChunk(_chunks[chunkPositions]);
                chunkGO.AddComponent<MeshCollider>();
                
                _chunkGOs.Add(new (x, z), chunkGO);
            }

            SpawnPlayer();
            EnableChunks(_playerLastPosition, _range);
        }

        private void Update()
        {
            var playerPosition = GetPlayerPosition(_player);
            if (playerPosition.Equals(_playerLastPosition) is false)
            {
                EnableChunks(playerPosition, _range);
                DisableChunks(playerPosition, _range);
            }
        }

        private void DisableChunks(Vector2Int playerPosition, int radius)
        {
            foreach(var chunkPosition in _chunkGOs.Keys
                .Where(key => key.x >= playerPosition.x + radius ||
                    key.x <= playerPosition.x - radius ||
                    key.y >= playerPosition.y + radius ||
                    key.y <= playerPosition.y - radius))
            {
                _chunkGOs[chunkPosition].SetActive(false);
            }
        }
        private void EnableChunks(Vector2Int playerPosition, int radius)
        {
            foreach(var chunkPosition in _chunkGOs.Keys
                .Where(key => key.x < playerPosition.x + radius &&
                    key.x > playerPosition.x - radius &&
                    key.y < playerPosition.y + radius &&
                    key.y > playerPosition.y - radius))
            {
                _chunkGOs[chunkPosition].SetActive(true);
            }
        }

        private void SpawnPlayer()
        {
            var _chunksCount = _chunks.Keys.Count / _chunks.Keys.Count * 16;
            _player = Instantiate(_player, new Vector3(_chunksCount,_chunkConfig.HeightLevel + 10,_chunksCount), Quaternion.identity);
            _playerLastPosition = GetPlayerPosition(_player);
        }

        private Vector2Int GetPlayerPosition(GameObject player) =>
            Vector2Int.FloorToInt(new (player.transform.position.x / 16, player.transform.position.z / 16));
    }
}