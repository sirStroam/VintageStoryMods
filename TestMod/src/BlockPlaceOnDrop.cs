﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

namespace Vintagestory.GameContent
{
    public class BlockPlaceOnDrop : Block
    {
        public override void OnGroundIdle(EntityItem entityItem)
        {
            if (entityItem.World.Side == EnumAppSide.Client) return;
            if (entityItem.ShouldDespawn) return;

            if (TryPlace(entityItem, 0, 0, 0))
            {
                entityItem.Die(EnumDespawnReason.Removed, null);
                return;
            }
            if (TryPlace(entityItem, 0, 1, 0))
            {
                entityItem.Die(EnumDespawnReason.Removed, null);
                return;
            }
            if (TryPlace(entityItem, 0, -1, 0))
            {
                entityItem.Die(EnumDespawnReason.Removed, null);
                return;
            }

            if (!entityItem.CollidedVertically) return;

            List<BlockPos> offsetsList = new List<BlockPos>();

            for (int x = -1; x < 1; x++)
            {
                for (int y = -1; y < 1; y++)
                {
                    for (int z = -1; z < 1; z++)
                    {
                        offsetsList.Add(new BlockPos(x, y, z));
                    }
                }
            }

            BlockPos[] offsets = offsetsList.ToArray();
            offsets.Shuffle(entityItem.World.Rand);

            for (int i = 0; i < offsets.Length; i++)
            {
                if (TryPlace(entityItem, offsets[i].X, offsets[i].Y, offsets[i].Z))
                {
                    entityItem.Die(EnumDespawnReason.Removed, null);
                    return;
                }
            }   
        }


        bool TryPlace(EntityItem entityItem, int offX, int offY, int offZ)
        {
            IWorldAccessor world = entityItem.World;
            BlockPos pos = entityItem.ServerPos.AsBlockPos.Add(offX, offY, offZ);
            Block block = world.BlockAccessor.GetBlock(pos.DownCopy());
            if (!block.SideSolid[BlockFacing.UP.Index]) return false;

            bool ok = TryPlaceBlock(world, null, entityItem.Itemstack, new BlockSelection()
            {
                Position = pos,
                Face = BlockFacing.UP,
                HitPosition = new Vec3d(0.5, 1, 0.5)
            });

            if (ok) entityItem.World.PlaySoundAt(entityItem.Itemstack.Block.Sounds?.Place, pos.X, pos.Y, pos.Z, null);
            
            return ok;
        }
    }
}
