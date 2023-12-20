using UnityEngine;
using Configs.World.Chunks;
using Configs.World.Blocks;
using System.Linq;

namespace Scripts.World.Generators
{
    public class ChunkGenerator
    {
        private readonly ChunkConfig _chunkConfig;
        private readonly BlockGenerator _blockGenerator;
        private readonly BlockConfig[] _blockConfigs;
        private readonly int _seed;

        public ChunkGenerator(ChunkConfig chunkConfig, BlockGenerator blockGenerator, BlockConfig[] blockConfigs, int seed)
        {
            _chunkConfig = chunkConfig;
            _blockGenerator = blockGenerator;
            _blockConfigs = blockConfigs;
            _seed = seed;
        }

        public Chunk Generate(Vector2Int position)
        {
            var blocks = new Block[_chunkConfig.X, _chunkConfig.Y, _chunkConfig.Z];

            for (var x = 0; x < _chunkConfig.X; x++)
            {
                for (var z = 0; z < _chunkConfig.Z; z++) 
                {
                    var height = (int)GetHeight(x, position.x, z, position.y);
                    for (var y = 0; y < _chunkConfig.Y; y++) 
                    {
                        if (y > height) break;
                        blocks[x,y,z] = _blockGenerator.Generate(ChooseBlockConfigForHeight(height - y));
                    }
                }
            }

            return new Chunk(blocks, _chunkConfig);
        }
        
        private float GetHeight(int x, int xOffset, int z, int zOffset) =>
            Mathf.PerlinNoise((x + xOffset + _seed) * _chunkConfig.XScale, (z + zOffset + _seed) * _chunkConfig.ZScale)
                * _chunkConfig.YScale
                + _chunkConfig.HeightLevel;

        private BlockConfig ChooseBlockConfigForHeight(int height)
        {
            var sortedConfigs = _blockConfigs.OrderBy(config => config.GetSpawnChanceForHeight(height));

            int randomValue = new System.Random().Next(1, 101);

            foreach(var config in sortedConfigs)
            {
                if (randomValue <= config.GetSpawnChanceForHeight(height)) return config;
            }

            return sortedConfigs.Last();
        }
    }
}