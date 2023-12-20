using Configs.World.Blocks;
using UnityEngine;

namespace Configs.Environment
{
    public abstract class EnvironmentConfig : ScriptableObject
    {
        [SerializeField] private GameObject[] _environmentGO;
        [SerializeField] private BlockConfig[] _blockConfigs;
        [SerializeField] private int _frequency;

        public GameObject[] EnvironmentGO => _environmentGO;
        public BlockConfig[] BlockConfigs => _blockConfigs;
        public int Frequency => _frequency;
    }
}