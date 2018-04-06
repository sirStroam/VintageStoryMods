

using System;
using System.Text;
using Vintagestory.API;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace FillTheGaps
{
   class LevigatorBehavior : BlockBehavior {

       Block blockSand;

        //TODO these need to be dark gray and really small
       public static SimpleParticleProperties particles = new SimpleParticleProperties(
                    1, 1,
                    ColorUtil.ColorFromArgb(50, 220, 220, 220),
                    new Vec3d(),
                    new Vec3d(),
                    new Vec3f(-0.25f, 0.1f, -0.25f),
                    new Vec3f(0.25f, 0.1f, 0.25f),
                    1.5f,
                    -0.075f,
                    0.25f,
                    0.25f,
                    EnumParticleModel.Quad
                );

        public LevigatorBehavior(Block block) : base(block) {
		}

        //This was getting called instead of OnHeldInteractStart
      /*  public override bool OnPlayerBlockInteract(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
        {
            bool isEmptyHanded = byPlayer.Entity.RightHandItemSlot.Empty;
            if(isEmptyHanded)
            {
                handling = EnumHandling.PreventDefault;
                return true;
            }
            return false;
        }*/

        public override bool OnHeldInteractStart(IItemSlot slot, IEntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, ref EnumHandling handling)
        {  //For some reason this doesn't get called
            handling = EnumHandling.PreventDefault;
            bool isAboveRock = byEntity.World.BlockAccessor.GetBlock(blockSel.Position.DownCopy()).FirstCodePart() == "rock"; 
            if(isAboveRock && IsSandHeld(byEntity) )
            {
                
                return true;
            }
            return false;
        }

        public override bool OnHeldInteractStep(float secondsUsed, IItemSlot slot, IEntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, ref EnumHandling handling)
        {
            handling = EnumHandling.PreventDefault;
            if (byEntity.World is IClientWorldAccessor)
            {
                //ModelTransform tf = new ModelTransform();
                //tf.EnsureDefaultValues();

                float speed = 5 + 20 * Math.Max(0, secondsUsed - 0.25f);
                float start = secondsUsed * 120;
                //float rotationZ = Math.Max(-110, start - Math.Max(0, secondsUsed - 0.25f) * 90 * speed);

                // TODO spin block
                
                //tf.Origin.Set(0, 2f, 0);
                //tf.Translation.Set(0, Math.Max(-1f, -5 * Math.Max(0, secondsUsed - 0.25f)), 0);
                //tf.Rotation.Z = rotationZ;
                //byEntity.Controls.UsingHeldItemTransform = tf;


                /* TODO Spawn grit particles

                Vec3d pos =
                            byEntity.Pos.XYZ.Add(0, byEntity.EyeHeight(), 0)
                            .Ahead(1f, byEntity.Pos.Pitch, byEntity.Pos.Yaw)
                        ;

                    Vec3f speedVec = new Vec3d(0, 0, 0).Ahead(5, byEntity.Pos.Pitch, byEntity.Pos.Yaw).ToVec3f();
                    particles.minVelocity = speedVec;
                    Random rand = new Random();
                    particles.color = ColorUtil.ToRGBABytes(ColorUtil.ColorFromArgb(255, rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255)));
                    particles.minPos = pos.AddCopy(-0.05, -0.05, -0.05);
                    particles.addPos.Set(0.1, 0.1, 0.1);
                    particles.minSize = 0.1F;
                    particles.SizeEvolve = EvolvingNatFloat.create(EnumTransformFunction.SINUS, 10);
                    byEntity.World.SpawnParticles(particles);*/

                if (secondsUsed > 0.6)
                {  
                    if(rand.NextDouble() > 0.09)
                    {
                        //TODO Drop sandblock
                    }
                    Block polishedRock = GetPolishedRock(byEntity.World, blockSel);
                    byEntity.World.BlockAccessor.ExchangeBlock(polishedRock.BlockId, blockSel.Position.DownCopy());
                }
            }
            return true;
        }

        private Block GetPolishedRock(IWorldAccessor world, BlockSelection blockSel)
        {
            string name = "rockpolished-"+world.BlockAccessor.GetBlock(blockSel.Position.DownCopy()).LastCodePart();
            Block polishedRock = world.GetBlock(new AssetLocation(name));
            return polishedRock;
        }

        private bool IsSandHeld(IEntityAgent byEntity)
        {
            if(byEntity.RightHandItemSlot.Empty)
            {
                return false;
            }
            ItemStack itst = byEntity.RightHandItemSlot.Itemstack;
            string name = itst.GetName();
            if(name.StartsWith("sand"))
            {
                blockSand = itst.Block;
                byEntity.RightHandItemSlot.TakeOut(1);
                return true;
            }
            return false;
        }
    }
}
