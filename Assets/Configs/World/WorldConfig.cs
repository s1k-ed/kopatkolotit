using UnityEngine;

namespace Configs.World
{
    [CreateAssetMenu(fileName = "WorldConfig", menuName = "Config/WorldConfig")]
    public class WorldConfig : ScriptableObject
    {
        [SerializeField] private int _chunksAmountOnSide;

        public int ChunksAmountOnSide => _chunksAmountOnSide;
    }
}