using System;
using System.Collections.Generic;
using System.Linq;
using Configs.World.Blocks;
using UnityEngine;

namespace Scripts.Environment
{
    public class EnvironmentHelper
    {
        public static BlockConfig[,,] ParseToBlockConfigs(GameObject environmentGO, BlockConfig[] configs)
        {
            var children = environmentGO.GetComponentsInChildren<Transform>().Skip(1);
            var positions = GetPositions(children);

            var minX = positions.Min(position => position.x);
            var minY = positions.Min(position => position.y);
            var minZ = positions.Min(position => position.z);

            var maxX = positions.Max(position => position.x);
            var maxY = positions.Max(position => position.y);
            var maxZ = positions.Max(position => position.z);

            var xSize = maxX - minX + 1;
            var ySize = maxY - minY + 1;
            var zSize = maxZ - minZ + 1;

            var result = new BlockConfig[xSize, ySize, zSize];
            for (var x = 0; x < xSize; x++)
            {
                for (var z = 0; z < zSize; z++)
                {
                    for (var y = 0; y < ySize; y++)
                    {
                        var blockTransform = children.FirstOrDefault(transform => Vector3Int.FloorToInt(transform.localPosition).Equals(new (x + minX, y + minY, z + minZ)));
                        if (blockTransform is null) continue;

                        var config = configs.FirstOrDefault(config => config.Material == blockTransform.GetComponent<MeshRenderer>().sharedMaterials[0]);

                        if (config is null) throw new ArgumentNullException();

                        result[x,y,z] = config;
                    }
                }
            }

            return result;
        }

        public static Vector2Int GetPositionOfCenter(BlockConfig[,,] configs)
        {
            var xSize = configs.GetLength(0);
            var zSize = configs.GetLength(2);

            for (var x = 0; x < xSize; x++)
            {
                for (var z = 0; z < zSize; z++)
                {
                    if (configs[x, 0, z] is not null) return new (x, z);
                }
            }

            throw new Exception($"Exception in parsing {configs} object");
        }

        private static List<Vector3Int> GetPositions(IEnumerable<Transform> transforms)
        {
            var positions = new List<Vector3Int>();
            foreach (var transform in transforms)
                positions.Add(Vector3Int.FloorToInt(transform.localPosition));
            return positions;
        }
    }
}