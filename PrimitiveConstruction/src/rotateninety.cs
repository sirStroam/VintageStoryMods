using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace PrimitiveConstruction
{
  public class PrimitiveConstructionMod : ModBase {
		public override void Start(ICoreAPI api) {
			base.Start(api);
			api.RegisterBlockBehaviorClass("WallBehavior", typeof(WallBehavior));
		}
	}


	// Used for the post, should change shape based on side connections.
	class rotateninety : BlockBehavior {
		public rotateninety(Block block) : base(block) {
		}

    public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel)
    {
      if (!world.BlockAccessor.GetBlock(blockSel.Position).IsReplacableBy(this.block)){
        return false;
      }
      BlockFacing[] blockFacingArray = Block.SuggestedHVOrientation(byPlayer, blockSel);
      AssetLocation assetLocation = this.block.CodeWithParts(new string[2]
      {
        blockFacingArray[0] == BlockFacing.NORTH || blockFacingArray[0] == BlockFacing.SOUTH ? "n" : "w",
        ""
      });
      world.BlockAccessor.SetBlock((ushort) world.BlockAccessor.GetBlock(assetLocation).BlockId, (BlockPos) blockSel.Position);
      return true;
    }

    public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1f)
    {
      return new ItemStack[1]
      {
        new ItemStack(world.BlockAccessor.GetBlock(this.block.CodeWithPath(this.block.CodeWithoutParts(2) + "-n")), 1)
      };
    }

    public override ItemStack OnPickBlock(IWorldAccessor world, BlockPos pos)
    {
      return new ItemStack(world.BlockAccessor.GetBlock(this.block.CodeWithPath(this.block.CodeWithoutParts(2) + "-n")), 1);
    }

    public override AssetLocation GetRotatedBlockCode(int angle)
    {/* 
      string[] strArray = new string[2]{ "w", "n" };
      int num = angle / 90;
      if (this.block.LastCodePart(0) == "n")
        ++num;
      return this.block.CodeWithParts(new string[2]
      {
        strArray[num % 2]
      });*/
    }

  }
}