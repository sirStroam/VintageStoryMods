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

namespace FillTheGaps
{

    class StonePathBehavior : BlockBehavior {
		private string ownFirstCodePart;

        public StonePathBehavior(Block block) : base(block) {
			this.ownFirstCodePart = ((CollectibleObject) block).FirstCodePart(0);
		}

        public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref EnumHandling handling)
        {
            handling = EnumHandling.PreventDefault;
            if (!world.BlockAccessor.GetBlock(blockSel.Position).IsReplacableBy(this.block))
            {
                return false;
            }
            string type = "normal";
            if(OnPath(world, blockSel.Position))
            {
                type = "step";
            }

            world.BlockAccessor.SetBlock(world.BlockAccessor.GetBlock(block.CodeWithParts(type)).BlockId, blockSel.Position);
            return true;
        }

        public override void OnNeighourBlockChange(IWorldAccessor world, BlockPos pos, BlockPos neibpos, ref EnumHandling handling)
        {
            handling = EnumHandling.PreventDefault;

            if(neibpos != pos.DownCopy())
            {
                return;
            }

            string type = "normal";
            if(OnPath(world, pos))
            {
                type = "step";
            }
            world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(block.CodeWithParts(type)).BlockId, pos);
        }

        private bool OnPath(IWorldAccessor world, BlockPos pos)
        {
            if(world.BlockAccessor.GetBlock(pos.DownCopy()).FirstCodePart() == this.ownFirstCodePart){
                return true;
            }
            return false;
        }


    }
    

}
