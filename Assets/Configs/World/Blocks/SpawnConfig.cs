using System;
using UnityEngine;

namespace Configs.World.Blocks
{
    [Serializable]
    public class SpawnConfig
    {
        [SerializeField] private int _minHeight;
        [SerializeField] private int _maxHeight;
        [SerializeField] private int _spawnChance;

        public int MinHeight => _minHeight;
        public int MaxHeight => _maxHeight;
        public int SpawnChance => _spawnChance;

    }
}