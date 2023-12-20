using System.Collections.Generic;
using System.Linq;
using Configs.World.Blocks;
using Configs.World.Chunks;
using UnityEngine;

namespace Scripts.World.Components
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class ChunkRenderer : MonoBehaviour
    {
        private readonly Vector3Int[] _sideOffsets  = new[] { Vector3Int.up, Vector3Int.down, Vector3Int.forward, Vector3Int.back, Vector3Int.left, Vector3Int.right };
        private readonly List<Vector3>[] _sides = new[] { BlockVertices.TOP, BlockVertices.DOWN, BlockVertices.FRONT, BlockVertices.BACK, BlockVertices.LEFT, BlockVertices.RIGHT };
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        public void RenderChunk(Chunk chunk)
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();

            _meshFilter.mesh = RenderMesh(chunk);
        }

        private Mesh RenderMesh(Chunk chunk)
        {
            var mesh = new Mesh()
            {
                name = "Chunk mesh"
            };

            var vertices = new List<Vector3>();
            var subMeshDatas = new Dictionary<Material, List<int>>();

            var xBound = chunk.Dimensions[0];
            var yBound = chunk.Dimensions[1];
            var zBound = chunk.Dimensions[2];

            for (var x = 0; x < xBound; x++)
            {
                for (var z = 0; z < zBound; z++)
                {
                    for (var y = 0; y < yBound; y++)
                    {
                        GenerateVerticiesAndTriangles(chunk, vertices, subMeshDatas, x, z, y);   
                    }
                }
            }

            _meshRenderer.materials = subMeshDatas.Select(data => data.Key).ToArray();

            mesh.vertices = vertices.ToArray();
            mesh.subMeshCount = subMeshDatas.Count;

            var i = 0;
            foreach(var key in subMeshDatas.Keys)
            {
                mesh.SetTriangles(subMeshDatas[key].ToArray(), i++);
            }

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
        
        private void GenerateVerticiesAndTriangles(Chunk chunk, List<Vector3> vertices, Dictionary<Material, List<int>> subMeshDatas, int x, int z, int y)
        {
            if (!chunk.HasBlockAt(new(x, y, z)) || !chunk.IsOnEdge(new(x, y, z))) return;
            
            var blockMaterial = chunk.GetBlockAt(new (x, y, z)).Config.Material;

            if (subMeshDatas.ContainsKey(blockMaterial) is false) subMeshDatas[blockMaterial] = new List<int>();

            for (int i = 0; i < _sideOffsets.Length; i++)
            {
                var blockPosition = new Vector3Int(x, y, z);

                if (chunk.HasBlockAt(blockPosition + _sideOffsets[i])) continue;

                var sideVertices = GetVerticesForSide(blockPosition, _sides[i]);
                vertices.AddRange(sideVertices);
                var count = vertices.Count;
                subMeshDatas[blockMaterial].AddRange(GetIndexesForTriangles(count));
            }
        }
        
        private int[] GetIndexesForTriangles(int count) =>
            new [] {
                count - 4, count - 3, count - 2,
                count - 3, count - 1, count - 2
            };

        private Vector3[] GetVerticesForSide(Vector3 position, List<Vector3> side) =>
            side.Select(vertic => vertic + position).ToArray();
    }
}