namespace Configs.World.Blocks
{
    public class Block
    {
        private readonly BlockConfig _config;

        public BlockConfig Config => _config;

        public Block(BlockConfig config)
        {
            _config = config;
        }
    }
}