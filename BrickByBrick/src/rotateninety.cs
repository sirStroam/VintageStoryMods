// RotateNinety by Stroam
//
// To the extent possible under law, the person who associated CC0 with
// this project has waived all copyright and related or neighboring rights
// to this project.
//
// You should have received a copy of the CC0 legalcode along with this
// work.  If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace BbB
{
    // Used for the post, should change shape based on side connections.
    class rotateninety : BlockBehavior
    {
        public rotateninety(Block block) : base(block)
        {
        }

        public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref EnumHandling handling)
        {
            handling = EnumHandling.PreventDefault;
            if (!world.BlockAccessor.GetBlock(blockSel.Position).IsReplacableBy(this.block))
            {
                return false;
            }
            BlockFacing[] blockFacingArray = Block.SuggestedHVOrientation(byPlayer, blockSel);
            string orientation = blockFacingArray[0] == BlockFacing.NORTH || blockFacingArray[0] == BlockFacing.SOUTH ? "n" : "w" ;

            AssetLocation assetLocation = this.block.CodeWithParts(orientation);
            world.BlockAccessor.SetBlock(world.BlockAccessor.GetBlock(assetLocation).BlockId, blockSel.Position);
            return true;
        }

        public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropChanceMultiplier, ref EnumHandling handling)
        {
            handling = EnumHandling.PreventDefault;
            return new ItemStack[1]
            {
                new ItemStack(world.BlockAccessor.GetBlock(this.block.CodeWithPath(this.block.CodeWithoutParts(2) + "-n")), 1)
            };
        }

        public override ItemStack OnPickBlock(IWorldAccessor world, BlockPos pos, ref EnumHandling handling)
        {
            return new ItemStack(world.BlockAccessor.GetBlock(this.block.CodeWithPath(this.block.CodeWithoutParts(2) + "-n")), 1);
        }

        public override AssetLocation GetRotatedBlockCode(int angle, ref EnumHandling handling)
        {

            string[] strArray = new string[2] { "w", "n" };
            int num = angle / 90;
            if (this.block.LastCodePart(0) == "n")
                ++num;
            return this.block.CodeWithParts( strArray[num % 2]);
        }

    }
}