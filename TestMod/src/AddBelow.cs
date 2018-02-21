using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace TestMod 
{
    public class AddBelowBlocks : ModBase
    {

        public override void Start(ICoreAPI api)
        {
            
            api.RegisterBlockBehaviorClass("AddBelow", typeof(AddBelow));
            base.Start(api);
        }

    }
    public class AddBelow : BlockBehavior
    {
        public AddBelow(Block block) : base(block)
        {
        }

        public override bool OnPlayerBlockInteract(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
        {
            Block clickedBlock = world.BlockAccessor.GetBlock(blockSel.Position);
            Block heldBlock = byPlayer.Entity.RightHandItemSlot.Itemstack?.Block;
            if((heldBlock != null) 
            && (heldBlock.Code.Domain == clickedBlock.Code.Domain) //Checks if block is part of mod
            && (heldBlock.Code.FirstPathPart() == clickedBlock.Code.FirstPathPart())) //checks first part of name so block-north == block-west
            {
                BlockPos pos = blockSel.Position.Add(0,-1,0);
                while(world.BlockAccessor.GetBlock(pos).Equals(clickedBlock))
                {
                    pos.Add(0,-1,0);
                }

                /*if(world.BlockAccessor.GetBlock(pos).IsReplacableBy(clickedBlock))
                {
                    //world.BlockAccessor.SetBlock(clickedBlock.BlockId, pos);
                }*/
                return true;
            }
            else
            {
                return false;
            }   
        }
    }
}