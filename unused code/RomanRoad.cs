using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace CivicConstruction
{
  public class BlockRomanRoad : Block
  {
    public int Stage
    {
      get
      {
        string str = this.LastCodePart(0);
        if (str == "rocks")
          return 1;
        if (str == "concrete")
          return 2;
          return 3;
      }
    }

    public string NextStageCodePart
    {
      get
      {
        string str = this.LastCodePart(0);
        if (str == "rocks")
          return "concrete";
          return "road";
      }
    }

    public bool Construct(IWorldAccessor world, BlockPos pos, CombustibleProperties props)
    {
      int stage = this.Stage;
      switch (stage)
      {
        case 4:
          if (((AssetLocation) ((CollectibleObject) world.BlockAccessor.GetBlock(pos.DownCopy(1))).Code).Path.Equals("firewoodpile"))
          {
            Block block = world.GetBlock(new AssetLocation("charcoalpit"));
            if (block != null)
            {
              world.BlockAccessor.SetBlock((ushort) block.BlockId, pos);
              return true;
            }
            break;
          }
          break;
        case 5:
          return false;
      }
      Block block1 = world.GetBlock(((CollectibleObject) this).CodeWithParts(new string[1]
      {
        this.NextStageCodePart
      }));
      world.BlockAccessor.ExchangeBlock((ushort) block1.BlockId, pos);
      world.BlockAccessor.MarkBlockDirty(pos);
      if (block1.Sounds != null)
        world.PlaySoundAt((AssetLocation) ((BlockSounds) block1.Sounds).Place, (double) pos.X, (double) pos.Y, (double) pos.Z, (IPlayer) null, true, 32f, 1f);
      if (stage == 4)
      {
        BlockEntity blockEntity = world.BlockAccessor.GetBlockEntity(pos);
       // if (blockEntity is BlockEntityFirepit)
        //  ((BlockEntityFirepit) blockEntity).igniteWithFuel(props, 4f);
      }
      return true;
    }

  }
}
