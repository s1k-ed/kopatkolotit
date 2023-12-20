using Configs.World.Blocks;

namespace Scripts.World.Generators
{
    public class BlockGenerator
    {
        public Block Generate(BlockConfig config) =>
            new Block(config);
    }
}

