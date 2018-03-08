using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace TestMod
{
	public class BlockBehaviorRopeLadder : BlockBehavior
	{
		public static string NAME { get; } = "RopeLadder";

        private string dropBlockFace = "north";
        private string ownFirstCodePart;
		
		public BlockBehaviorRopeLadder(Block block)
			: base(block) 
            {  
                this.ownFirstCodePart = block.FirstCodePart(0);
            }
		
		public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, IItemStack itemstack, BlockSelection blockSel, ref EnumHandling handling)
        {
            handling = EnumHandling.PreventDefault;
            BlockPos position = blockSel.Position;
            Block block1 = world.BlockAccessor.GetBlock(position);
            BlockPos blockPos = blockSel.DidOffset ? position.AddCopy(blockSel.Face.GetOpposite()) : blockSel.Position;
            if (block1.IsReplacableBy(this.block))
            {
                AssetLocation code1 = this.block.CodeWithParts(Block.SuggestedHVOrientation(byPlayer, blockSel)[0].Code);
                Block block3 = world.BlockAccessor.GetBlock(code1);
                Block block4 = world.BlockAccessor.GetBlock(position.X, position.Y + 1, position.Z);
                if (block4.FirstCodePart(0) == this.ownFirstCodePart && this.HasSupport(block4, world.BlockAccessor, position))
                {
                    block4.DoPlaceBlock(world, position, blockSel.Face);
                    return true;
                }
                Block block5 = world.BlockAccessor.GetBlock(position.X, position.Y - 1, position.Z);
                if (block5.FirstCodePart(0) == this.ownFirstCodePart && this.HasSupport(block5, world.BlockAccessor, position) && !CollisionTester.AabbIntersect(byPlayer.Entity.CollisionBox, byPlayer.Entity.Pos.X, byPlayer.Entity.Pos.Y, byPlayer.Entity.Pos.Z, Cuboidf.Default(), new Vec3d((double)position.X, (double)position.Y, (double)position.Z)))
                {
                    block5.DoPlaceBlock(world, position, blockSel.Face);
                    return true;
                }
                BlockPos pos1 = blockPos.UpCopy(1);
                Block block6 = world.BlockAccessor.GetBlock(pos1);
                Block block2 = blockSel.DidOffset ? world.BlockAccessor.GetBlock(position.AddCopy(blockSel.Face.GetOpposite())) : block1;
                if (block2.FirstCodePart(0) == this.ownFirstCodePart && block6.IsReplacableBy(this.block) && !CollisionTester.AabbIntersect(byPlayer.Entity.CollisionBox, byPlayer.Entity.Pos.X, byPlayer.Entity.Pos.Y, byPlayer.Entity.Pos.Z, Cuboidf.Default(), new Vec3d((double)pos1.X, (double)pos1.Y, (double)pos1.Z)))
                {
                    block2.DoPlaceBlock(world, pos1, blockSel.Face);
                    return true;
                }
                BlockPos pos2 = blockPos.DownCopy(1);
                Block block7 = world.BlockAccessor.GetBlock(pos2);
                if (block2.FirstCodePart(0) == this.ownFirstCodePart && block7.IsReplacableBy(this.block) && !CollisionTester.AabbIntersect(byPlayer.Entity.CollisionBox, byPlayer.Entity.Pos.X, byPlayer.Entity.Pos.Y, byPlayer.Entity.Pos.Z, Cuboidf.Default(), new Vec3d((double)pos2.X, (double)pos2.Y, (double)pos2.Z)))
                {
                    block2.DoPlaceBlock(world, pos2, blockSel.Face);
                    return true;
                }
                if (this.HasSupport(block3, world.BlockAccessor, position))
                {
                    block3.DoPlaceBlock(world, position, blockSel.Face);
                    return true;
                }
                AssetLocation code2 = this.block.CodeWithParts(blockSel.Face.GetOpposite().Code);
                Block block8 = world.BlockAccessor.GetBlock(code2);
                if (block8 != null && this.HasSupport(block8, world.BlockAccessor, position))
                {
                    block8.DoPlaceBlock(world, position, blockSel.Face);
                    return true;
                }
            }
            return false;
    }

    public override void OnNeighourBlockChange(IWorldAccessor world, BlockPos pos, BlockPos neibpos, ref EnumHandling handling)
    {
      if (!this.HasSupport(this.block, world.BlockAccessor, pos))
      {
        handling = EnumHandling.Last;
        world.BlockAccessor.BreakBlock(pos, (IPlayer) null, 1f);
        world.BlockAccessor.MarkBlockUpdated(pos);
      }
      else
        base.OnNeighourBlockChange(world, pos, neibpos, ref handling);
    }

    public bool HasSupportUp(Block forBlock, IBlockAccessor blockAccess, BlockPos pos)
    {
      BlockFacing facing = BlockFacing.FromCode(forBlock.LastCodePart(0));
      BlockPos pos1 = pos.UpCopy(1);
      if (this.SideSolid(blockAccess, pos, facing) || this.SideSolid(blockAccess, pos1, BlockFacing.UP))
        return true;
      if (pos.Y < blockAccess.MapSizeY - 1 && blockAccess.GetBlock(pos1) == forBlock)
        return this.HasSupportUp(forBlock, blockAccess, pos1);
      return false;
    }

    public bool HasSupportDown(Block forBlock, IBlockAccessor blockAccess, BlockPos pos)
    {
      BlockFacing facing = BlockFacing.FromCode(forBlock.LastCodePart(0));
      BlockPos pos1 = pos.DownCopy(1);
      if (this.SideSolid(blockAccess, pos, facing) || this.SideSolid(blockAccess, pos1, BlockFacing.DOWN))
        return true;
      if (pos.Y > 0 && blockAccess.GetBlock(pos1) == forBlock)
        return this.HasSupportDown(forBlock, blockAccess, pos1);
      return false;
    }

    public bool HasSupport(Block forBlock, IBlockAccessor blockAccess, BlockPos pos)
    {
      BlockFacing facing = BlockFacing.FromCode(forBlock.LastCodePart(0));
      BlockPos pos1 = pos.DownCopy(1);
      BlockPos pos2 = pos.UpCopy(1);
      if (this.SideSolid(blockAccess, pos, facing) || this.SideSolid(blockAccess, pos1, BlockFacing.DOWN) || this.SideSolid(blockAccess, pos2, BlockFacing.UP) || pos.Y < blockAccess.MapSizeY - 1 && blockAccess.GetBlock(pos2) == forBlock && this.HasSupportUp(forBlock, blockAccess, pos2))
        return true;
      if (pos.Y > 0 && blockAccess.GetBlock(pos1) == forBlock)
        return this.HasSupportDown(forBlock, blockAccess, pos1);
      return false;
    }

    public bool SideSolid(IBlockAccessor blockAccess, BlockPos pos, BlockFacing facing)
    {
      return blockAccess.GetBlock(pos.X + facing.Normali.X, pos.Y, pos.Z + facing.Normali.Z).SideSolid[facing.GetOpposite().Index];
    }

    public override void Initialize(JsonObject properties)
    {
      base.Initialize(properties);
      if (!properties["dropBlockFace"].Exists)
        return;
      this.dropBlockFace = properties["dropBlockFace"].AsString((string) null);
    }

    public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier, ref EnumHandling handled)
    {
      handled = EnumHandling.PreventDefault;
      return new ItemStack[1]
      {
        new ItemStack(world.BlockAccessor.GetBlock(this.block.CodeWithParts(this.dropBlockFace)), 1)
      };
    }

    public override ItemStack OnPickBlock(IWorldAccessor world, BlockPos pos, ref EnumHandling handled)
    {
      handled = EnumHandling.PreventDefault;
      return new ItemStack(world.BlockAccessor.GetBlock(this.block.CodeWithParts(this.dropBlockFace)), 1);
    }

    public override AssetLocation GetRotatedBlockCode(int angle, ref EnumHandling handled)
    {
      handled = EnumHandling.PreventDefault;
      int index = GameMath.Mod(BlockFacing.FromCode(this.block.LastCodePart(0)).HorizontalAngleIndex - angle / 90, 4);
      return this.block.CodeWithParts(BlockFacing.HORIZONTALS_ANGLEORDER[index].Code);
    }

    public override AssetLocation GetHorizontallyFlippedBlockCode(EnumAxis axis, ref EnumHandling handling)
    {
      handling = EnumHandling.PreventDefault;
      BlockFacing blockFacing = BlockFacing.FromCode(this.block.LastCodePart(0));
      if (blockFacing.Axis != axis)
        return this.block.Code;
      return this.block.CodeWithParts(blockFacing.GetOpposite().Code);
    }
		
	}
}
