using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Configs.Environment;
using Configs.World;
using Configs.World.Blocks;
using Configs.World.Chunks;
using Scripts.Environment;
using UnityEngine;

namespace Scripts.World.Generators
{
    public class WorldGenerator
    {
        private readonly EnvironmentGenerator _environmentGenerator;
        private readonly WorldConfig _worldConfig;
        private readonly ChunkConfig _chunkConfig;
        private readonly BlockConfig[] _blockConfigs;

        public WorldGenerator(EnvironmentGenerator environmentGenerator,
            WorldConfig worldConfig, 
            ChunkConfig chunkConfig, 
            BlockConfig[] blockConfigs)
        {
            _environmentGenerator = environmentGenerator;
            _worldConfig = worldConfig;
            _chunkConfig = chunkConfig;
            _blockConfigs = blockConfigs;
        }

        public async Task<Dictionary<Vector2Int, Chunk>> GenerateChunksAsync(int seed)
        {
            var result = new Dictionary<Vector2Int, Chunk>();
            var tasks = new List<Task>();
            var radius = _worldConfig.ChunksAmountOnSide;

            var chunkGenerator = new ChunkGenerator(_chunkConfig, new BlockGenerator(), _blockConfigs, seed);

            for (var x = 0 - radius; x <= 0 + radius; x++)
            {
                for (var z = 0 - radius; z <= 0 + radius; z++)
                {

                    var currentX = x;
                    var currentZ = z;
                    var chunkPosition = new Vector2Int(currentX, currentZ);

                    tasks.Add(Task.Run(() =>
                    {
                        var chunk = chunkGenerator.Generate(new Vector2Int(currentX * _chunkConfig.X, currentZ * _chunkConfig.Z));
                        result.Add(chunkPosition, chunk);
                    }));
                }
            }

            await Task.WhenAll(tasks);

            return result;
        }

        public void CreateEnvironment(Dictionary<Vector2Int, Chunk> chunks)
        {
            foreach (var chunkPosition in chunks.Keys)
            {
                var chunk = chunks[chunkPosition];
                _environmentGenerator.GenerateData(chunk, chunkPosition);
            }
        }

        public async Task UpdateEnvironmentAsync(Dictionary<Vector2Int, Chunk> chunks)
        {
            var tasks = new List<Task>();

            foreach (var chunkPosition in chunks.Keys)
            {
                var currentX = chunkPosition.x;
                var currentZ = chunkPosition.y;
                tasks.Add(Task.Run(() =>
                {
                    _environmentGenerator.HandleData(chunks[chunkPosition], chunkPosition);
                }));
            }

            await Task.WhenAll(tasks);

            return;
        }
    }
}