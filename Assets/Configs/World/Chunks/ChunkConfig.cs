using UnityEngine;

namespace Configs.World.Chunks
{
    [CreateAssetMenu(fileName = "ChunkConfig", menuName = "Config/ChunkConfig")]
    public class ChunkConfig : ScriptableObject
    {
        [SerializeField] private int _x = 16;
        [SerializeField] private int _y = 128;
        [SerializeField] private int _z = 16;
        [SerializeField] private int _heightLevel = 16;
        [SerializeField] private float _xScale = 0.1f;
        [SerializeField] private float _zScale = 0.1f;
        [SerializeField] private float _yScale = 10f;

        public int X => _x;
        public int Y => _y;
        public int Z => _z;

        public int HeightLevel => _heightLevel;

        public float XScale => _xScale;
        public float ZScale => _zScale;
        public float YScale => _yScale;
    }
}