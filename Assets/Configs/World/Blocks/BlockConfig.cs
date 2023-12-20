using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Configs.World.Blocks
{
    [CreateAssetMenu(fileName = "BlockConfig", menuName = "Config/BlockConfig")]
    public class BlockConfig : ScriptableObject
    {
        [SerializeField] private Material _material;
        [SerializeField] private BlockTypes _type;
        [SerializeField] private List<SpawnConfig> _spawnConfigs;

        public Material Material => _material;
        public BlockTypes Type => _type;

        public int GetSpawnChanceForHeight(int height)
        {
            if (height < 0) return 0;

            var sortedConfigs = _spawnConfigs.OrderBy(config => config.MinHeight);

            if (height < sortedConfigs.First().MinHeight)
                return 0;
                
            if (height > sortedConfigs.Last().MaxHeight) 
                return sortedConfigs
                    .Last()
                    .SpawnChance;

            return sortedConfigs
                .First(config => height >= config.MinHeight && height <= config.MaxHeight)
                .SpawnChance;
        }
    }
}