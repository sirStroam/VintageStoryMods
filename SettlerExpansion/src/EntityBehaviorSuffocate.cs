using System;
using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace settler
{
    public class EntityBehaviorSuffocate : EntityBehavior
    {
        ITreeAttribute breatheTree;
        EntityAgent entityAgent;

        public enum EnumSuffocationType
        {
            None = 0,
            Burried = 1,
            Drowning = 2
        }

        float breatheCounter;

        internal float Oxygen
        {
            get { return breatheTree.GetFloat("currentoxygen"); }
            set { breatheTree.SetFloat("currentoxygen", value); entity.WatchedAttributes.MarkPathDirty("breathe"); }
        }

        internal float MaxOxygen
        {
            get { return breatheTree.GetFloat("maxoxygen"); }
            set { breatheTree.SetFloat("maxoxygen", value); entity.WatchedAttributes.MarkPathDirty("breathe"); }
        }

        internal float HealthLocked
        {
            get { return entity.WatchedAttributes.GetTreeAttribute("health").GetFloat("healthlocked"); }
            set { entity.WatchedAttributes.GetTreeAttribute("health").SetFloat("healthlocked", value); entity.WatchedAttributes.MarkPathDirty("health"); }
        }

        internal float Health
        {
            get { return entity.WatchedAttributes.GetTreeAttribute("health").GetFloat("currenthealth"); }
            set { entity.WatchedAttributes.GetTreeAttribute("health").SetFloat("currenthealth", value); entity.WatchedAttributes.MarkPathDirty("health"); }
        }

        public EntityBehaviorSuffocate(Entity entity) : base(entity)
        {
            entityAgent = entity as EntityAgent;
        }

        public override void Initialize(EntityType config, JsonObject typeAttributes)
        {
            breatheTree = entity.WatchedAttributes.GetTreeAttribute("breathe");

            if (breatheTree == null)
            {
                entity.WatchedAttributes.SetAttribute("breathe", breatheTree = new TreeAttribute());
                
                Oxygen = typeAttributes["currentoxygen"].AsFloat(30);
                MaxOxygen = typeAttributes["maxoxygen"].AsFloat(30);

            }
        }



        public override void OnEntityDespawn(EntityDespawnReason despawn)
        {
            base.OnEntityDespawn(despawn);
        }

        public void OnEntityReceiveOxygen(float oxygen)
        {
            Oxygen = Math.Min(MaxOxygen, Oxygen + oxygen);
        }

        public override void OnGameTick(float deltaTime)
        {
            breatheCounter += deltaTime;
            if(breatheCounter < 1 || !entity.Alive) { return; }
            

            if (entity is EntityPlayer)
            {
                EntityPlayer plr = (EntityPlayer)entity;
                EnumGameMode mode = entity.World.PlayerByUid(plr.PlayerUID).WorldData.CurrentGameMode;
                if (mode == EnumGameMode.Creative || mode == EnumGameMode.Spectator) return;
            }

            EnumSuffocationType danger = checkSuffocation();

            if(danger == EnumSuffocationType.None)
            {
                this.Oxygen = Math.Min(this.MaxOxygen, Oxygen + breatheCounter);
            } else {
                float prevOxy = Oxygen;
                Oxygen -= depletionRate(breatheCounter);
                triggerSoundEffect(prevOxy,Oxygen, danger);
                
                if(Oxygen < 0 )
                {
                    DamageSource damage = new DamageSource();
                    damage.source = EnumDamageSource.Drown;
                    damage.type = EnumDamageType.Asphyxiation;
                    entity.Die(EnumDespawnReason.Death, damage);
                }
            }
            breatheCounter = 0; 
        }

        public EnumSuffocationType checkSuffocation()
        {
            BlockPos headPos = new BlockPos((int) entity.ServerPos.X,(int) (entity.ServerPos.Y + entity.EyeHeight()),(int) entity.ServerPos.Z);
            Block headblock = entity.World.BlockAccessor.GetBlock(headPos);

            if(headblock.IsLiquid())
            {
                return EnumSuffocationType.Drowning;
            }
            if(headblock.SideSolid[BlockFacing.DOWN.Index] == true)
            {
                return EnumSuffocationType.Burried;
            }
            return EnumSuffocationType.None;
            
        }

        private float depletionRate(float time)
        {
            int depth = Math.Max(1, entity.World.SeaLevel - (int) entity.ServerPos.Y);
            //float amount = ((this.MaxOxygen * (-38/1890)) * (depth*depth))+((this.MaxOxygen * (8511/14)) * depth)+(-1148947/1890);
            return depth * time;
        }

        public override string PropertyName()
        {
            return "oxygen";
        }

        private void triggerSoundEffect(float prev, float cur, EnumSuffocationType effect)
        {
            if( prev >= (MaxOxygen * 0.75))
            {
                if(cur <= (MaxOxygen * 0.75))
                {
                    if(effect == EnumSuffocationType.Drowning)
                    {
                        entity.World.PlaySoundAt(new AssetLocation("stressfullife:sounds/player/bubble1.ogg"), entity);
                    }
                    else
                    {
                        entity.World.PlaySoundAt(new AssetLocation("stressfullife:sounds/player/doublecough.ogg"), entity);
                    }
                }
            }
            else if( prev >= (MaxOxygen * 0.5))
            {
                if(cur <= (MaxOxygen * 0.5))
                {
                    if(effect == EnumSuffocationType.Drowning)
                    {
                        entity.World.PlaySoundAt(new AssetLocation("stressfullife:sounds/player/bubble2.ogg"), entity);
                    }
                    else
                    {
                        entity.World.PlaySoundAt(new AssetLocation("stressfullife:sounds/player/cough.ogg"), entity);
                    }
                }
            }
            else if( prev >= (MaxOxygen * 0.3))
            {
                if(cur <= (MaxOxygen * 0.3))
                {
                    if(effect == EnumSuffocationType.Drowning)
                    {
                        entity.World.PlaySoundAt(new AssetLocation("stressfullife:sounds/player/bubble3.ogg"), entity);
                    }
                    else
                    {
                        entity.World.PlaySoundAt(new AssetLocation("stressfullife:sounds/player/quickgasp1.ogg"), entity);
                    }
                }
            }
            else if( prev >= (MaxOxygen * 0.1))
            {
                if(cur <= (MaxOxygen * 0.1))
                {
                    if(effect == EnumSuffocationType.Drowning)
                    {
                        entity.World.PlaySoundAt(new AssetLocation("stressfullife:sounds/player/bubble4.ogg"), entity);
                    }
                    else
                    {
                        entity.World.PlaySoundAt(new AssetLocation("stressfullife:sounds/player/quickgasp2.ogg"), entity);
                    }
                }
            }
            else if( prev >= 1.5)
            {
                if(cur <= 1.5 )
                {
                    if(effect == EnumSuffocationType.Drowning)
                    {
                        entity.World.PlaySoundAt(new AssetLocation("stressfullife:sounds/player/drowned.ogg"), entity);
                    }
                    else
                    {
                        entity.World.PlaySoundAt(new AssetLocation("stressfullife:sounds/player/lastgasp.ogg"), entity);
                    }
                }
            }

        }

        public override void OnEntityReceiveDamage(DamageSource damageSource, float damage)
        {
            if (damageSource.type == EnumDamageType.Heal && damageSource.source == EnumDamageSource.Respawn)
            {
                Oxygen = MaxOxygen / 2;
            }
        }
    }
    
}
