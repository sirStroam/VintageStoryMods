using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace FillTheGaps
{
    public class ItemStoneChisel : Item
    {

        public override bool OnHeldInteractStart(IItemSlot slot, IEntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            IWorldAccessor world = byEntity.World;
            if(blockSel == null)
            {
                return false;
            }
            BlockPos position = blockSel.Position;
            Block block1 = world.BlockAccessor.GetBlock(position);
            //string temp = block1.Code.Path;
            string str = "";
            if(block1.Code.Path.StartsWith("quartz"))//replace with some sort of tree.
            {
                str = "quartzpillar-ud";
                if(block1.Code.Path.StartsWith("quartzpillar"))
                {
                    str = "quartz-ornate";
                }
            } else if(block1.FirstCodePart(0) == "rock"){ 
                str = "stonebricks-" + block1.CodeEndWithoutParts(1);
            } else
            {
                return false;
            }

            Block block2 = byEntity.World.GetBlock(new AssetLocation(str));
            if(block2 == null) {return false;} //sanity check
            
            IPlayer iplayer = null;
            if(byEntity is IEntityPlayer)
            {
                iplayer = world.PlayerByUid(((IEntityPlayer) byEntity).PlayerUID);
            }
            if(iplayer == null) return false;

            IItemSlot hammerSlot = Gethammer(byEntity);
            if(hammerSlot == null) return false;

            if (block1.Sounds != null)
            {
                world.PlaySoundAt(block1.Sounds.Place, position.X, position.Y, position.Z, iplayer, true, 32f,1f);
            } 
            world.BlockAccessor.SetBlock(block2.BlockId, position);
            world.BlockAccessor.MarkBlockDirty(position);
            hammerSlot.Itemstack.Collectible.DamageItem(world, byEntity, hammerSlot);
            slot.Itemstack.Collectible.DamageItem(world, byEntity, iplayer.InventoryManager.ActiveHotbarSlot);

            return true;
        }

        IItemSlot Gethammer(IEntityAgent byEntity)
        {
            IItemSlot slot = null;
            byEntity.WalkInventory((invslot) =>
            {
                //if (invslot is CreativeSlot) return true;

                if (invslot.Itemstack != null && invslot.Itemstack.Collectible.Code.FirstPathPart(0) == "hammer")
                {
                    slot = invslot;
                    return false;
                }

                return true;
            });

            return slot;
        }
    }
}
