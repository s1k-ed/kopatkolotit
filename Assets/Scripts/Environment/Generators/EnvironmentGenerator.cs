using System.Collections.Generic;
using Configs.Environment;
using Configs.World.Blocks;
using Configs.World.Chunks;
using Scripts.World.Generators;
using UnityEngine;

namespace Scripts.Environment
{
    public class EnvironmentGenerator
    {
        private readonly EnvironmentConfig _config;
        private readonly Dictionary<Vector2Int, List<EnvironmentMetaData>> _metaDatas;
        private readonly List<BlockConfig[,,]> _objectBlockConfigs;
        private readonly BlockGenerator _blockGenerator;

        public EnvironmentGenerator(EnvironmentConfig config, BlockGenerator blockGenerator)
        {
            _config = config;
            _blockGenerator = blockGenerator;
            _objectBlockConfigs = new List<BlockConfig[,,]>();
            foreach(var go in _config.EnvironmentGO)
                _objectBlockConfigs.Add(EnvironmentHelper.ParseToBlockConfigs(go, _config.BlockConfigs));
            _metaDatas = new Dictionary<Vector2Int, List<EnvironmentMetaData>>();
        }

        public void GenerateData(Chunk chunk, Vector2Int chunkPosition)
        {
            var environment = _objectBlockConfigs[Random.Range(0, _objectBlockConfigs.Count)];

            var chunkXSize = chunk.Dimensions[0];
            var chunkZSize = chunk.Dimensions[2];
            var objectXSize = environment.GetLength(0);
            var objectZSize = environment.GetLength(2);

            var environmentAmount = _config.Frequency / 100.0 * chunkXSize * chunkZSize / (objectXSize * objectZSize);

            var spawnPoints = new Dictionary<Vector2Int, int>();

            for (var i = 0; i < environmentAmount; i++)
            {
                var spawnPoint = GetRandomSpawnPoint(chunkXSize, chunkZSize);

                if (spawnPoints.ContainsKey(spawnPoint)) continue;

                spawnPoints.Add(spawnPoint, chunk.GetSurface(spawnPoint, BlockTypes.GRASS));
            }

            foreach(var kv in spawnPoints)
            {
                GenerateMetaData(new (kv.Key.x, kv.Value, kv.Key.y), chunkPosition, chunkXSize, chunkZSize, GetRandomEnvironmentId());
            }
        }

        public void HandleData(Chunk chunk, Vector2Int chunkPosition)
        {
            if (_metaDatas.ContainsKey(chunkPosition) is false) return;

            foreach(var metaData in _metaDatas[chunkPosition])
            {
                HandleMetaData(chunk, metaData);
            }

            _metaDatas.Remove(chunkPosition);
        }

        private Vector2Int GetRandomSpawnPoint(int maxX, int maxZ) =>
            new (Random.Range(0, maxX), Random.Range(0, maxZ));

        private int GetRandomEnvironmentId() =>
            Random.Range(0, _objectBlockConfigs.Count);

        // Не советую сюда смотреть
        private void GenerateMetaData(Vector3Int spawnPoint, Vector2Int chunkPosition, int chunkXSize, int chunkZSize, int environmentId)
        {
            var y = spawnPoint.y;
            var environment = _objectBlockConfigs[environmentId];
            var environmentXSize = environment.GetLength(0);
            var environmentZSize = environment.GetLength(2);

            var positionOfMiddleBlock = EnvironmentHelper.GetPositionOfCenter(environment);

            var topRight = new Vector2Int(spawnPoint.x + (environmentXSize - positionOfMiddleBlock.x - 1), spawnPoint.z + (environmentZSize - positionOfMiddleBlock.y - 1));
            var bottomLeft = new Vector2Int(spawnPoint.x - positionOfMiddleBlock.x, spawnPoint.z - positionOfMiddleBlock.y);

            if (bottomLeft.x >= 0 && bottomLeft.y >= 0 && topRight.x < chunkXSize && topRight.y < chunkZSize)
            {
                var metaData = new EnvironmentMetaData(
                    environmentId,
                    new (0,0), 
                    new (environmentXSize - 1, environmentZSize - 1), 
                    new (bottomLeft.x, y,bottomLeft.y));  

                AddMetaData(chunkPosition, metaData);
            }
            if (bottomLeft.x >= 0 && bottomLeft.y >= 0 && topRight.x < chunkXSize && topRight.y >= chunkZSize)
            {
                var zSize = chunkZSize - spawnPoint.z;

                var metaData = new EnvironmentMetaData(
                    environmentId, 
                    new (0,0), 
                    new (environmentXSize - 1, zSize), 
                    new (bottomLeft.x, y, bottomLeft.y));

                var topMetaData = new EnvironmentMetaData(
                    environmentId, 
                    new (0, zSize + positionOfMiddleBlock.y), 
                    new (environmentXSize - 1, environmentZSize - 1), 
                    new (bottomLeft.x, y, 0));

                var topChunkPosition = chunkPosition + Vector2Int.up;

                AddMetaData(chunkPosition, metaData);
                AddMetaData(topChunkPosition, topMetaData);
            }
            if (bottomLeft.x >= 0 && bottomLeft.y >= 0 && topRight.x >= chunkXSize && topRight.y < chunkZSize)
            {
                var xSize = chunkXSize - spawnPoint.x;

                var metaData = new EnvironmentMetaData(
                    environmentId, 
                    new (0,0), 
                    new (xSize, environmentZSize - 1), 
                    new (bottomLeft.x, y,bottomLeft.y));

                var rightMetaData = new EnvironmentMetaData(
                    environmentId, 
                    new (xSize + positionOfMiddleBlock.x, 0), 
                    new (environmentXSize - 1, environmentZSize - 1), 
                    new (0, y, bottomLeft.y));
                
                var rightChunkPosition = chunkPosition + Vector2Int.right;

                AddMetaData(chunkPosition, metaData);
                AddMetaData(rightChunkPosition, rightMetaData);
            }
            if (bottomLeft.x >= 0 && bottomLeft.y >= 0 && topRight.x >= chunkXSize && topRight.y >= chunkZSize)
            {
                var xSize = chunkXSize - spawnPoint.x;
                var zSize = chunkZSize - spawnPoint.z;

                var metaData = new EnvironmentMetaData(
                    environmentId, 
                    new (0,0), 
                    new (xSize, zSize), 
                    new (bottomLeft.x, y,bottomLeft.y));

                var rightMetaData = new EnvironmentMetaData(
                    environmentId, 
                    new (xSize + + positionOfMiddleBlock.x, 0), 
                    new (environmentXSize - 1, zSize), 
                    new (0, y, bottomLeft.y));

                var topMetaData = new EnvironmentMetaData(
                    environmentId, 
                    new (0, zSize + positionOfMiddleBlock.y), 
                    new (xSize, environmentZSize - 1), 
                    new (bottomLeft.x, y, 0));

                var topRightMetaData = new EnvironmentMetaData(
                    environmentId, 
                    new (xSize + + positionOfMiddleBlock.x, zSize + positionOfMiddleBlock.y), 
                    new (environmentXSize - 1, environmentZSize - 1), 
                    new (0, y, 0));
                
                var rightChunkPosition = chunkPosition + Vector2Int.right;
                var topChunkPosition = chunkPosition + Vector2Int.up;
                var topRightChunkPosition = chunkPosition + Vector2Int.one;

                AddMetaData(chunkPosition, metaData);
                AddMetaData(rightChunkPosition, rightMetaData);
                AddMetaData(topChunkPosition, topMetaData);
                AddMetaData(topRightChunkPosition, topRightMetaData);
            }
            if (bottomLeft.x >= 0 && bottomLeft.y < 0 && topRight.x < chunkXSize && topRight.y < chunkZSize)
            {
                var zSize = -bottomLeft.y;

                var metaData = new EnvironmentMetaData(
                    environmentId, 
                    new (0, zSize), 
                    new (environmentXSize - 1, environmentZSize - 1), 
                    new (bottomLeft.x, y, 0));

                var downMetaData = new EnvironmentMetaData(
                    environmentId, 
                    new (0, 0), 
                    new (environmentXSize - 1, zSize - 1), 
                    new (bottomLeft.x, y, chunkZSize + bottomLeft.y));
                
                var downChunkPosition = chunkPosition + Vector2Int.down;

                AddMetaData(chunkPosition, metaData);
                AddMetaData(downChunkPosition, downMetaData);
            }
            if (bottomLeft.x >= 0 && bottomLeft.y < 0 && topRight.x >= chunkXSize && topRight.y < chunkZSize)
            {
                var xSize = chunkXSize - spawnPoint.x;
                var zSize = -bottomLeft.y;

                var metaData = new EnvironmentMetaData(
                    environmentId, 
                    new (0, zSize), 
                    new (xSize, environmentZSize - 1), 
                    new (bottomLeft.x, y, 0));

                var rightMetaData = new EnvironmentMetaData(
                    environmentId, 
                    new (xSize + positionOfMiddleBlock.x, zSize), 
                    new (environmentXSize - 1, environmentZSize - 1), 
                    new (0, y, 0));

                var downMetaData = new EnvironmentMetaData(
                    environmentId, 
                    new (0, 0), 
                    new (xSize, zSize - 1), 
                    new (bottomLeft.x, y, chunkZSize + bottomLeft.y));

                var downRightMetaData = new EnvironmentMetaData(
                    environmentId, 
                    new (xSize + positionOfMiddleBlock.x, 0), 
                    new (environmentXSize - 1, zSize - 1), 
                    new (0, y, chunkZSize + bottomLeft.y));
                
                var rightChunkPosition = chunkPosition + Vector2Int.right;
                var downChunkPosition = chunkPosition + Vector2Int.down;
                var downRightChunkPosition = chunkPosition + new Vector2Int(1, -1);

                AddMetaData(chunkPosition, metaData);
                AddMetaData(rightChunkPosition, rightMetaData);
                AddMetaData(downChunkPosition, downMetaData);
                AddMetaData(downRightChunkPosition, downRightMetaData);
            }
            if (bottomLeft.x < 0 && bottomLeft.y >= 0 && topRight.x < chunkXSize && topRight.y < chunkZSize)
            {
                var xSize = -bottomLeft.x;

                var metaData = new EnvironmentMetaData(
                    environmentId, 
                    new (xSize, 0), 
                    new (environmentXSize - 1, environmentZSize - 1), 
                    new (0, y, bottomLeft.y));

                var leftMetaData = new EnvironmentMetaData(
                    environmentId, 
                    new (0, 0), 
                    new (xSize - 1, environmentZSize - 1), 
                    new (chunkXSize + bottomLeft.x, y, bottomLeft.y));
                
                var leftChunkPosition = chunkPosition + Vector2Int.left;

                AddMetaData(chunkPosition, metaData);
                AddMetaData(leftChunkPosition, leftMetaData);
            }        
            if (bottomLeft.x < 0 && bottomLeft.y < 0 && topRight.x < chunkXSize && topRight.y < chunkZSize)
            {
                var xSize = -bottomLeft.x;
                var zSize = -bottomLeft.y;

                var metaData = new EnvironmentMetaData(
                    environmentId, 
                    new (xSize, zSize), 
                    new (environmentXSize - 1, environmentZSize - 1), 
                    new (0, y, 0));

                var leftMetaData = new EnvironmentMetaData(
                    environmentId, 
                    new (0, zSize), 
                    new (xSize - 1, environmentZSize - 1), 
                    new (chunkXSize + bottomLeft.x, y, 0));

                var downMetaData = new EnvironmentMetaData(
                    environmentId, 
                    new (xSize, 0), 
                    new (environmentXSize - 1, zSize - 1), 
                    new (0, y, chunkZSize + bottomLeft.y));

                var downLeftMetaData = new EnvironmentMetaData(
                    environmentId, 
                    new (0, 0), 
                    new (xSize - 1, zSize - 1), 
                    new (chunkXSize + bottomLeft.x, y, chunkZSize + bottomLeft.y));
                
                var leftChunkPosition = chunkPosition + Vector2Int.left;
                var downChunkPosition = chunkPosition + Vector2Int.down;
                var downLeftChunkPosition = chunkPosition + new Vector2Int(-1, -1);

                AddMetaData(chunkPosition, metaData);
                AddMetaData(leftChunkPosition, leftMetaData);
                AddMetaData(downChunkPosition, downMetaData);
                AddMetaData(downLeftChunkPosition, downLeftMetaData);
            }
            if (bottomLeft.x < 0 && bottomLeft.y >= 0 && topRight.x < chunkXSize && topRight.y >= chunkZSize)
            {
                var xSize = -bottomLeft.x;
                var zSize = chunkZSize - spawnPoint.z;

                var metaData = new EnvironmentMetaData(
                    environmentId, 
                    new (xSize, 0), 
                    new (environmentXSize - 1, zSize), 
                    new (0, y, bottomLeft.y));

                var leftMetaData = new EnvironmentMetaData(
                    environmentId, 
                    new (0, 0), 
                    new (xSize - 1, zSize - 1), 
                    new (chunkXSize + bottomLeft.x, y, bottomLeft.y));

                var topMetaData = new EnvironmentMetaData(
                    environmentId, 
                    new (xSize, zSize + positionOfMiddleBlock.y), 
                    new (environmentXSize - 1, environmentZSize - 1), 
                    new (0, y, 0));

                var topLeftMetaData = new EnvironmentMetaData(
                    environmentId, 
                    new (0, zSize + positionOfMiddleBlock.y), 
                    new (xSize - 1, environmentZSize - 1), 
                    new (chunkXSize + bottomLeft.x, y, 0));
                
                var leftChunkPosition = chunkPosition + Vector2Int.left;
                var topChunkPosition = chunkPosition + Vector2Int.up;
                var topLeftChunkPosition = chunkPosition + new Vector2Int(-1, 1);

                AddMetaData(chunkPosition, metaData);
                AddMetaData(leftChunkPosition, leftMetaData);
                AddMetaData(topChunkPosition, topMetaData);
                AddMetaData(topLeftChunkPosition, topLeftMetaData);
            }       
        }

        private void AddMetaData(Vector2Int chunkPosition, EnvironmentMetaData metaData)
        {
            if (_metaDatas.ContainsKey(chunkPosition))
                _metaDatas[chunkPosition].Add(metaData);
            else
                _metaDatas.Add(chunkPosition, new List<EnvironmentMetaData>() { metaData });
        }
    
        private void HandleMetaData(Chunk chunk, EnvironmentMetaData metaData)
        {
            var environment = _objectBlockConfigs[metaData.EnvironmentId];

            var spawnPoint = metaData.SpawnPoint;

            var startPoint = metaData.StartPoint;
            var endPoint = metaData.EndPoint;

            var environmentYSize = environment.GetLength(1);

            var chunkX = spawnPoint.x;
            var chunkY = spawnPoint.y;

            for (var x = startPoint.x; x <= endPoint.x; x++)
            {
                var chunkZ = spawnPoint.z;
                for (var z = startPoint.y; z <= endPoint.y; z++)
                {
                    for (var y = 0; y < environmentYSize; y++)
                    {
                        var blockConfig = environment[x, y, z];
                        if (blockConfig is null) continue;
                        chunk.TryPlaceBlockAt(new (chunkX, chunkY + y, chunkZ), _blockGenerator, blockConfig);
                    }
                    chunkZ++;
                }
                chunkX++;
            }
        }
    }
}