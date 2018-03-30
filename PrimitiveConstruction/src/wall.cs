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

namespace PrimitiveConstruction {
	public class PrimitiveConstructionMod : ModBase {
		public override void Start(ICoreAPI api) {
			base.Start(api);
			api.RegisterBlockBehaviorClass("WallBehavior", typeof(WallBehavior));
		}
	}


	// Used for the post, should change shape based on side connections.
	class WallBehavior : BlockBehavior {
		public WallBehavior(Block block) : base(block) {
		

		public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref EnumHandling handling) {
			handling = EnumHandling.PreventDefault;
			if (!world.BlockAccessor.GetBlock(blockSel.Position).IsReplacableBy(this.block)) {
				return false;
			}

      string orientations = this.GetConnections(world, (BlockPos) blockSel.Position);
      world.BlockAccessor.SetBlock(world.BlockAccessor.GetBlock(block.CodeWithParts(orientations)).BlockId, blockSel.Position);
      return true;
		}

		public override void OnNeighourBlockChange(IWorldAccessor world, BlockPos pos, BlockPos neibpos, ref EnumHandling handling) {
      string orientations = this.GetConnections(world, (BlockPos) pos);
      world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(block.CodeWithParts(orientations)).BlockId, pos);
		}

    private string GetConnections(IWorldAccessor world, BlockPos pos)
    {
      string str = this.GetCode(world, pos, (BlockFacing) BlockFacing.NORTH) + 
      this.GetCode(world, pos, (BlockFacing) BlockFacing.EAST) + 
      this.GetCode(world, pos, (BlockFacing) BlockFacing.SOUTH) + 
      this.GetCode(world, pos, (BlockFacing) BlockFacing.WEST);

      if (str.Length == 0)
        str = "empty";
      return str;
    }

		private string GetCode(IWorldAccessor world, BlockPos pos, BlockFacing facing) {
      if (this.ShouldConnectAt(world, pos, facing))
        return facing.Code[0].ToString() ?? "";
      return "";
    }

    public bool ShouldConnectAt(IWorldAccessor world, BlockPos ownPos, BlockFacing side)
    {
      Block block = world.BlockAccessor.GetBlock(ownPos.AddCopy(side));
      Block B1 = world.BlockAccessor.GetBlock(ownPos);
      
      if (block.BlockId != null){//test same base block 
        if (this.Code.Path.FirstCodePart() == block.FirstCodePart())
          return true;
      }
      return (bool) block.SideSolid[side.GetOpposite().Index]; //test if neighbor face is solid
    }

		public override ItemStack OnPickBlock(IWorldAccessor world, BlockPos pos, ref EnumHandling handling) {
			return new ItemStack(world.BlockAccessor.GetBlock(block.CodeWithParts("empty")));
		}
    }

	}

}
