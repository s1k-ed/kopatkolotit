using UnityEngine;
using Scripts.Environment;
using Configs.World.Blocks;
using Configs.Environment;
using Configs.World.Chunks;
using Scripts.World.Generators;
using Scripts.World.Components;
using System.Linq;

public class ParserTest : MonoBehaviour
{
    [SerializeField] private ChunkConfig _chunkConfig;
    [SerializeField] private EnvironmentConfig _environmentRoot;
    [SerializeField] private BlockConfig[] _environmentBlockConfigs;

    void Start()
    {
        var blockConfigs = EnvironmentHelper.ParseToBlockConfigs(_environmentRoot.EnvironmentGO.First(), _environmentBlockConfigs);
        var blockGenerator = new BlockGenerator();
        var result = new Block[blockConfigs.GetLength(0),blockConfigs.GetLength(1),blockConfigs.GetLength(2)];

        for (var x = 0; x < blockConfigs.GetLength(0); x++)
        {
            for (var z = 0; z < blockConfigs.GetLength(2); z++)
            {
                for (var y = 0; y < blockConfigs.GetLength(1); y++)
                {
                    if (blockConfigs[x,y,z] is null) continue;
                    result[x,y,z] = blockGenerator.Generate(blockConfigs[x,y,z]);
                }
            }
        }
        
        var chunk = new Chunk(result, _chunkConfig);

        var chunkGO = new GameObject($"Chunk(test)");
        chunkGO.transform.position = new Vector3(0,0);
        chunkGO.AddComponent<ChunkRenderer>().RenderChunk(chunk);
    }
}