// Wall by Stroam
//
// To the extent possible under law, the person who associated CC0 with
// this project has waived all copyright and related or neighboring rights
// to this project.
//
// You should have received a copy of the CC0 legalcode along with this
// work.  If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace primitiveconstruction
{

    class WallBehavior : BlockBehavior
    {

        Block refBlock;
        public WallBehavior(Block block) : base(block)
        {
            refBlock = this.block;
        }

        public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref EnumHandling handling)
        {
            handling = EnumHandling.PreventDefault;
            if (!world.BlockAccessor.GetBlock(blockSel.Position).IsReplacableBy(this.block))
            {
                return false;
            }

            string orientations = this.GetConnections(world, (BlockPos)blockSel.Position);
            world.BlockAccessor.SetBlock(world.BlockAccessor.GetBlock(block.CodeWithParts(orientations)).BlockId, blockSel.Position);
            return true;
        }

        public override void OnNeighourBlockChange(IWorldAccessor world, BlockPos pos, BlockPos neibpos, ref EnumHandling handling)
        {
            string orientations = this.GetConnections(world, (BlockPos)pos);
            world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(block.CodeWithParts(orientations)).BlockId, pos);
        }

        private string GetConnections(IWorldAccessor world, BlockPos pos)
        {
            string str = this.GetCode(world, pos, (BlockFacing)BlockFacing.NORTH) +
            this.GetCode(world, pos, (BlockFacing)BlockFacing.EAST) +
            this.GetCode(world, pos, (BlockFacing)BlockFacing.SOUTH) +
            this.GetCode(world, pos, (BlockFacing)BlockFacing.WEST);

            if (str.Length == 0)
                str = "empty";
            return str;
        }

        private string GetCode(IWorldAccessor world, BlockPos pos, BlockFacing facing)
        {
            if (this.ShouldConnectAt(world, pos, facing))
                return facing.Code[0].ToString() ?? "";
            return "";
        }

        public bool ShouldConnectAt(IWorldAccessor world, BlockPos ownPos, BlockFacing side)
        {
            Block block = world.BlockAccessor.GetBlock(ownPos.AddCopy(side));
            Block B1 = refBlock;

            String s1 = block.FirstCodePart();
            String s2 = B1.Code.FirstPathPart();

            if (block.BlockId != null)
            {//test same base block 
                if (refBlock.FirstCodePart() == block.FirstCodePart())
                    return true;
            }
            return (bool)block.SideSolid[side.GetOpposite().Index]; //test if neighbor face is solid
        }

        public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropChanceMultiplier, ref EnumHandling handling)
        {
          handling = EnumHandling.PreventDefault;
            Block block = world.BlockAccessor.GetBlock(this.block.CodeWithParts("ew"));
            return new ItemStack[] { new ItemStack(block) };
        }

        public override ItemStack OnPickBlock(IWorldAccessor world, BlockPos pos, ref EnumHandling handling)
        {
          handling = EnumHandling.PreventDefault;
            return new ItemStack(world.BlockAccessor.GetBlock(block.CodeWithParts("ew")));
        }

    }

}
