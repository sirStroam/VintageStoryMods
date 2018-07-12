
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace BbB
{

    class LampPostBehavior : BlockBehavior {
		private string ownFirstCodePart;

        public LampPostBehavior(Block block) : base(block) {
			this.ownFirstCodePart = ((CollectibleObject) block).FirstCodePart(0);
		}

        public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref EnumHandling handling)
        {
            handling = EnumHandling.PreventDefault;
            if (!world.BlockAccessor.GetBlock(blockSel.Position).IsReplacableBy(this.block))
            {
                return false;
            }

            var code = getCode(world, blockSel.Position);
            world.BlockAccessor.SetBlock(world.BlockAccessor.GetBlock(block.CodeWithParts(code)).BlockId, blockSel.Position);
            return true;
        }

        public override void OnNeighourBlockChange(IWorldAccessor world, BlockPos pos, BlockPos neibpos, ref EnumHandling handling)
        {
            handling = EnumHandling.PreventDefault;
            if (pos.Y == neibpos.Y - 1 || pos.Y == neibpos.Y + 1 || IsLantern(world, neibpos))
            {
                return;
            }

            world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(block.CodeWithParts(getCode(world, pos))).BlockId, pos);
        }

        private string getCode(IWorldAccessor world, BlockPos pos)
        {
            var code = "";

            // This is really screwy, and I don't feel like figuring out why. It works, don't mess with it.
            // (It probably has something to do with the block rotation)
            // Also, I can't use SideSolid or it won't connect to lanterns and signs.
            if (world.BlockAccessor.GetBlock(pos.WestCopy()).Id != 0)
            {
                code += "n";
            }
            if (world.BlockAccessor.GetBlock(pos.NorthCopy()).Id != 0)
            {
                code += "e";
            }
            if (world.BlockAccessor.GetBlock(pos.EastCopy()).Id != 0)
            {
                code += "s";
            }
            if (world.BlockAccessor.GetBlock(pos.SouthCopy()).Id != 0)
            {
                code += "w";
            }

            // Now hande the "no connections" case.
            if (code == "")
            {
                code = "empty";
            }
            return code;
        }

        public bool ShouldConnectAt(IWorldAccessor world, BlockPos ownPos, BlockFacing side)
        {
            Block block = world.BlockAccessor.GetBlock(ownPos.AddCopy(side));
            
            if (ownFirstCodePart == block.FirstCodePart())
                    return true;
            return (bool)block.SideSolid[side.GetOpposite().Index]; //test if neighbor face is solid
        }

        public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropChanceMultiplier, ref EnumHandling handling)
        {
            handling = EnumHandling.PreventDefault;
            Block block = world.BlockAccessor.GetBlock(this.block.CodeWithParts("e"));
            return new ItemStack[] { new ItemStack(block) };
        }

        public override ItemStack OnPickBlock(IWorldAccessor world, BlockPos pos, ref EnumHandling handling)
        {
            handling = EnumHandling.PreventDefault;
            return new ItemStack(world.BlockAccessor.GetBlock(block.CodeWithParts("e")));
        }
        private bool IsLantern(IWorldAccessor world, BlockPos blockPos)
        {
            return ((AssetLocation)((CollectibleObject)world.BlockAccessor.GetBlock(blockPos)).Code).Path.Contains("lantern");
        }

        private BlockFacing GetDirection(BlockPos origin, BlockPos neighbor)
        {
            if (origin.Z == neighbor.Z - 1) { return BlockFacing.SOUTH; }
            if (origin.Z == neighbor.Z + 1) { return BlockFacing.NORTH; }
            if (origin.X == neighbor.X - 1) { return BlockFacing.EAST; }
            if (origin.X == neighbor.X + 1) { return BlockFacing.WEST; }
            return null;
        }

    }

}
