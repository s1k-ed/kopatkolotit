using System;
using Configs.World.Blocks;
using Scripts.World.Generators;
using UnityEngine;

namespace Configs.World.Chunks
{
    public class Chunk
    {
        private readonly Block[,,] _blocks;
        private readonly ChunkConfig _config;
        private readonly int[] _dimensions;

        public int[] Dimensions => _dimensions;

        public Chunk(Block[,,] blocks, ChunkConfig config)
        {
            _blocks = blocks;
            _config = config;

            _dimensions = new [] 
            {
                _blocks.GetLength(0),
                _blocks.GetLength(1),
                _blocks.GetLength(2)
            };
        }
        
        public bool TryPlaceBlockAt(Vector3Int position, BlockGenerator blockGenerator, BlockConfig blockConfig)
        {
            if (HasBlockAt(position) || IsOutOfBounds(position)) return false;
            _blocks[position.x, position.y, position.z] = blockGenerator.Generate(blockConfig);
            return true;
        }

        public int GetSurface(Vector2Int position, BlockTypes type)
        {
            for (var y = 1; y < _dimensions[1]; y++)
            {   
                if (HasBlockAt(new (position.x, y, position.y))) continue;
                if (HasBlockAt(new (position.x, y - 1, position.y)) is false) continue;
                if (GetBlockAt(new (position.x, y - 1, position.y)).Config.Type != type) continue;

                return y;
            }

            return -1;
        }

        public bool TryRemoveBlockAt(Vector3Int position)
        {
            if (HasBlockAt(position) is false || IsOutOfBounds(position)) return false;
            _blocks[position.x, position.y, position.z] = null;
            return true;
        }

        public Block GetBlockAt(Vector3Int position)
        {
            if (IsOutOfBounds(position)) return null;
            return _blocks[position.x, position.y, position.z];
        }

        public bool IsOnEdge(Vector3Int blockPosition)
        {
            if (GetBlockAt(blockPosition) is null) return false;

            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
                {
                    for (var z = -1; z <= 1; z++)
                    {
                        if (x == 0 && y == 0 && z == 0) continue;
                        if (Math.Abs(x) == 1 && Math.Abs(y) == 1 && Math.Abs(z) == 1) continue;
                        if (HasBlockAt(blockPosition + new Vector3Int(x, y, z)) is false) return true;
                    }
                }
            }
            return false;
        }

        public bool HasBlockAt(Vector3Int position) =>
            GetBlockAt(position) is not null;

        private bool IsOutOfBounds(Vector3Int blockPosition) =>
                blockPosition.x < 0 ||
                blockPosition.y < 0 ||
                blockPosition.z < 0 ||
                blockPosition.x >= _dimensions[0] ||
                blockPosition.y >= _dimensions[1] ||
                blockPosition.z >= _dimensions[2];
    }
}