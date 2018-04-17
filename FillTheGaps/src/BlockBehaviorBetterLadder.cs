
using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace FillTheGaps
{
    public class BlockBehaviorBetterLadder : BlockBehavior
    {
        private string dropBlockFace;
        private string ownFirstCodePart;

        public BlockBehaviorBetterLadder(Block block) : base(block)
        {
            this.ownFirstCodePart = block.FirstCodePart(0);
            this.dropBlockFace = "north";
        }

        public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref EnumHandling handling)
        {
            handling = EnumHandling.PreventDefault;
            BlockPos position = (BlockPos)blockSel.Position;
            Block block1 = world.BlockAccessor.GetBlock(position);
            BlockPos pos = blockSel.DidOffset != null ? position.AddCopy((blockSel.Face).GetOpposite()) : (BlockPos)blockSel.Position;

            AssetLocation assetLocation1 = (this.block).CodeWithParts(new string[1]
            {
        Block.SuggestedHVOrientation(byPlayer, blockSel)[0].Code
            });

            if (blockSel.HitPosition.Y < 0.5 && this.TryStackDown(byPlayer, world, pos, blockSel.Face, itemstack) || (this.TryStackUp(byPlayer, world, pos, blockSel.Face, itemstack) || this.TryStackDown(byPlayer, world, pos, blockSel.Face, itemstack)))
                return true;
            Block block4 = world.BlockAccessor.GetBlock(assetLocation1);
            if (this.HasSupport(block4, world.BlockAccessor, position) && !this.AabbIntersect(byPlayer, block4, position) && block1.IsReplacableBy(this.block))
            {
                block4.DoPlaceBlock(world, position, blockSel.Face, itemstack);
                return true;
            }
            AssetLocation assetLocation2 = (this.block).CodeWithParts(new string[1]
            {
        ( blockSel.Face).GetOpposite().Code
            });
            Block block5 = world.BlockAccessor.GetBlock(assetLocation2);
            if (block5 == null || !this.HasSupport(block5, world.BlockAccessor, position) || !block1.IsReplacableBy(this.block))
                return false;
            block5.DoPlaceBlock(world, position, blockSel.Face, itemstack);
            return true;
        }

        public override bool OnPlayerBlockInteract(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
        {
            handling = EnumHandling.PreventDefault;
            if (!byPlayer.Entity.Controls.Sneak || blockSel.Face.IsHorizontal) return false;
            handling = EnumHandling.Last;

            BlockPos position = blockSel.Position;
            Block block1 = world.BlockAccessor.GetBlock(position);
            //Block tempBlock = blockSel.DidOffset ? world.BlockAccessor.GetBlock(position.AddCopy(blockSel.Face.GetOpposite())) : block1;
            BlockPos pos = blockSel.DidOffset ? position.AddCopy(blockSel.Face.GetOpposite()) : blockSel.Position;

            return this.TryGetBottom(byPlayer, world, pos);

        }

        private bool TryGetBottom(IPlayer byPlayer, IWorldAccessor world, BlockPos pos)
        {
            Block block1 = world.BlockAccessor.GetBlock(pos);
            if (block1.FirstCodePart(0) != this.ownFirstCodePart) //should never happen
                return false;
            BlockPos pos1 = pos.DownCopy();
            Block block2 = null;
            while (pos1.Y > 0)
            {
                block2 = world.BlockAccessor.GetBlock(pos1);
                if (!(block2.FirstCodePart(0) != this.ownFirstCodePart))
                    pos1.Down(1);
                else
                    break;
            }
            if (block2 == null) //should never happen
                return false;

            block2 = world.BlockAccessor.GetBlock(pos1.Up());

            ItemStack ladderstack = new ItemStack(world.BlockAccessor.GetBlock(this.block.CodeWithParts(this.dropBlockFace)), 1);
            bool gotLadder = byPlayer.InventoryManager.TryGiveItemstack(ladderstack);
            if (gotLadder)
            {
                world.BlockAccessor.SetBlock((ushort)0, pos1);
                return true;
            }
            return false;
        }


        private bool TryStackUp(IPlayer byPlayer, IWorldAccessor world, BlockPos pos, BlockFacing face, ItemStack itemstack)
        {
            Block block1 = world.BlockAccessor.GetBlock(pos);
            if (block1.FirstCodePart(0) != this.ownFirstCodePart)
                return false;
            BlockPos pos1 = pos.UpCopy(1);
            Block block2 = null;
            while (pos1.Y < world.BlockAccessor.MapSizeY)
            {
                block2 = world.BlockAccessor.GetBlock(pos1);
                if (!((block2).FirstCodePart(0) != this.ownFirstCodePart))
                    pos1.Up(1);
                else
                    break;
            }
            if (block2 == null || block2.FirstCodePart(0) == this.ownFirstCodePart || (!block2.IsReplacableBy(this.block) || this.AabbIntersect(byPlayer, block1, pos1)))
                return false;
            block1.DoPlaceBlock(world, pos1, face, itemstack); //may be the fake block culprit
            return true;
        }

        private bool TryStackDown(IPlayer byPlayer, IWorldAccessor world, BlockPos pos, BlockFacing face, ItemStack itemstack)
        {
            Block block1 = world.BlockAccessor.GetBlock(pos);
            if (block1.FirstCodePart(0) != this.ownFirstCodePart)
                return false;
            BlockPos pos1 = pos.DownCopy(1);
            Block block2 = null;
            while (pos1.Y > 0)
            {
                block2 = world.BlockAccessor.GetBlock(pos1);
                if (!(block2.FirstCodePart(0) != this.ownFirstCodePart))
                    pos1.Down(1);
                else
                    break;
            }
            if (block2 == null || block2.FirstCodePart(0) == this.ownFirstCodePart || (!block2.IsReplacableBy(this.block) || this.AabbIntersect(byPlayer, block1, pos1)))
                return false;
            block1.DoPlaceBlock(world, pos1, face, itemstack); //may be the fake block culprit
            return true;
        }

        public bool AabbIntersect(IPlayer byPlayer, Block block, BlockPos pos)
        {
            Cuboidf[] collisionBoxes = block.GetCollisionBoxes(byPlayer.Entity.World.BlockAccessor, pos);
            Cuboidf cuboidf = collisionBoxes == null || collisionBoxes.Length == 0 ? null : collisionBoxes[0];
            return CollisionTester.AabbIntersect(byPlayer.Entity.CollisionBox, byPlayer.Entity.Pos.X, byPlayer.Entity.Pos.Y, byPlayer.Entity.Pos.Z, cuboidf, new Vec3d(pos.X, pos.Y, pos.Z));
        }

        public override void OnNeighourBlockChange(IWorldAccessor world, BlockPos pos, BlockPos neibpos, ref EnumHandling handling)
        {
            if (!this.HasSupport(this.block, world.BlockAccessor, pos))
            {
                handling = EnumHandling.Last;
                world.BlockAccessor.BreakBlock(pos, (IPlayer)null, 1f);
                world.BlockAccessor.MarkBlockUpdated(pos);
            }
            else
                base.OnNeighourBlockChange(world, pos, neibpos, ref handling);
        }

        public bool HasSupportUp(Block forBlock, IBlockAccessor blockAccess, BlockPos pos)
        {
            BlockFacing facing = BlockFacing.FromCode((forBlock).LastCodePart(0));
            BlockPos pos1 = pos.UpCopy(1);
            if (this.SideSolid(blockAccess, pos, facing) || this.SideSolid(blockAccess, pos1, BlockFacing.UP))
                return true;
            if (pos.Y < blockAccess.MapSizeY - 1 && blockAccess.GetBlock(pos1) == forBlock)
                return this.HasSupportUp(forBlock, blockAccess, pos1);
            return false;
        }

        public bool HasSupportDown(Block forBlock, IBlockAccessor blockAccess, BlockPos pos)
        {
            BlockFacing facing = BlockFacing.FromCode((forBlock).LastCodePart(0));
            BlockPos pos1 = pos.DownCopy(1);
            if (this.SideSolid(blockAccess, pos, facing) || this.SideSolid(blockAccess, pos1, BlockFacing.DOWN))
                return true;
            if (pos.Y > 0 && blockAccess.GetBlock(pos1) == forBlock)
                return this.HasSupportDown(forBlock, blockAccess, pos1);
            return false;
        }

        public bool HasSupport(Block forBlock, IBlockAccessor blockAccess, BlockPos pos)
        {
            BlockFacing facing = BlockFacing.FromCode((forBlock).LastCodePart(0));
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
            return blockAccess.GetBlock((pos.X + facing.Normali.X), pos.Y, (pos.Z + facing.Normali.Z)).SideSolid[facing.GetOpposite().Index];
        }

        public override void Initialize(JsonObject properties)
        {
            base.Initialize(properties);
            if (!properties.KeyExists("dropBlockFace"))
                return;
            this.dropBlockFace = properties.AsString("dropBlockFace");
        }

        public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier, ref EnumHandling handled)
        {
            handled = EnumHandling.PreventDefault;
            string str = this.dropBlockFace;
            AssetLocation al = this.block.CodeWithParts(this.dropBlockFace);
            return new ItemStack[1]
            {
        new ItemStack(world.BlockAccessor.GetBlock(al), 1) //
            };
        }

        public override ItemStack OnPickBlock(IWorldAccessor world, BlockPos pos, ref EnumHandling handled)
        {
            handled = EnumHandling.PreventDefault;
            AssetLocation al = this.block.CodeWithParts(this.dropBlockFace);
            return new ItemStack(world.BlockAccessor.GetBlock(al), 1);
        }

        public override AssetLocation GetRotatedBlockCode(int angle, ref EnumHandling handled)
        {
            handled = EnumHandling.PreventDefault;
            int index = GameMath.Mod(BlockFacing.FromCode(this.block.LastCodePart(0)).HorizontalAngleIndex - angle / 90, 4);
            return (this.block).CodeWithParts(new string[1]
            {
        ( BlockFacing.HORIZONTALS_ANGLEORDER[index]).Code
            });
        }

        public override AssetLocation GetHorizontallyFlippedBlockCode(EnumAxis axis, ref EnumHandling handling)
        {
            handling = EnumHandling.PreventDefault;
            BlockFacing blockFacing = BlockFacing.FromCode(this.block.LastCodePart(0));
            if (blockFacing.Axis != axis)
                return (AssetLocation)(this.block).Code;
            return this.block.CodeWithParts(new string[1]
            {
        blockFacing.GetOpposite().Code
            });
        }
    }
}
