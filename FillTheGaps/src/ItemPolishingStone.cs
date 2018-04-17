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
    public class ItemPolishingStone : Item
    {
        private Dictionary<IPlayer, BlockPos> currentEvents = new Dictionary<IPlayer,BlockPos>();

        public override bool OnHeldInteractStart(IItemSlot slot, IEntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {

            IWorldAccessor world = byEntity.World;
            if(blockSel == null)
            {
                return false;
            }
            BlockPos position = blockSel.Position;
            if(!byEntity.Pos.InRangeOf(position, 4)) return false;

            Block block1 = world.BlockAccessor.GetBlock(position);
            if(block1.FirstCodePart(0) != "rock") return false;

            IPlayer byPlayer = null;
            if (byEntity is IEntityPlayer) byPlayer = byEntity.World.PlayerByUid(((IEntityPlayer)byEntity).PlayerUID);

            try
            {
                this.currentEvents.Add(byPlayer,position);
            } catch {
                this.currentEvents[byPlayer] = position;
            }
            byEntity.World.PlaySoundAt(new AssetLocation("fillthegaps","sounds/rubbing-stones"), byEntity, byPlayer, false, 8);
            return true;
        }

        public override bool OnHeldInteractStep(float secondsUsed, IItemSlot slot, IEntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            
            BlockPos position = null;
            IPlayer byPlayer = null;
            if(byEntity is IEntityPlayer)
            {
                byPlayer = byEntity.World.PlayerByUid(((IEntityPlayer) byEntity).PlayerUID);
                position = this.currentEvents[byPlayer];
            }
            BlockPos temp1 = blockSel.Position;
            if( blockSel.Position != position || !byEntity.Pos.InRangeOf(position, 4) ) return false;

            if (secondsUsed > 2f) return false;
            if(secondsUsed > 1 && secondsUsed%10 < 1.05)
                byEntity.World.PlaySoundAt(new AssetLocation("fillthegaps","sounds/rubbing-stones"), byEntity, byPlayer, false, 8);
            return true;
        }
            


        public override bool OnHeldInteractCancel(float secondsUsed, IItemSlot slot, IEntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, EnumItemUseCancelReason cancelReason)
        {
            return true;
        }

        public override void OnHeldInteractStop(float secondsUsed, IItemSlot slot, IEntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            BlockPos position = null;
            IPlayer byPlayer = null;
            if(byEntity is IEntityPlayer)
            {
                byPlayer = byEntity.World.PlayerByUid(((IEntityPlayer) byEntity).PlayerUID);
                try{
                position = this.currentEvents[byPlayer];
                } catch {
                    return;
                }
            }
            this.currentEvents.Remove(byPlayer);

             if (secondsUsed > 1.95f)
             {
                
                string polished = "rockpolished-" + byEntity.World.BlockAccessor.GetBlock(position).CodeEndWithoutParts(1);
                byEntity.World.BlockAccessor.SetBlock(byEntity.World.GetBlock(new AssetLocation(polished)).BlockId, position);
                byEntity.World.BlockAccessor.MarkBlockDirty(position);
                slot.TakeOut(1);
                slot.MarkDirty();
                
             }

        }

    }
}
