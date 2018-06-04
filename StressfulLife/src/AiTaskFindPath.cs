using System.Collections.Generic;
using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Priority_Queue;

namespace stressfullife
{
    public class AiTaskSeekEntity : AiTaskBase
    {
        EntityAgent targetEntity;
        Vec3d targetPos;
        float moveSpeed = 0.02f;
        float seekingRange = 25f;
        float maxFollowTime = 60;
        
        bool stuck = false;
        string[] seekEntityCodesExact = new string[] { "player" };
        string[] seekEntityCodesBeginsWith = new string[0];

        float currentFollowTime = 0;

        bool alarmHerd = false;

        Dictionary<BlockPos,Block> area;

        public AiTaskSeekEntity(EntityAgent entity) : base(entity)
        {
        }

        public override void LoadConfig(JsonObject taskConfig, JsonObject aiConfig)
        {
            base.LoadConfig(taskConfig, aiConfig);

            if (taskConfig["movespeed"] != null)
            {
                moveSpeed = taskConfig["movespeed"].AsFloat(0.02f);
            }

            if (taskConfig["seekingRange"] != null)
            {
                seekingRange = taskConfig["seekingRange"].AsFloat(25);
            }

            if (taskConfig["maxFollowTime"] != null)
            {
                maxFollowTime = taskConfig["maxFollowTime"].AsFloat(60);
            }

            if (taskConfig["alarmHerd"] != null)
            {
                alarmHerd = taskConfig["alarmHerd"].AsBool(false);
            }

            if (taskConfig["entityCodes"] != null)
            {
                string[] codes = taskConfig["entityCodes"].AsStringArray(new string[] { "player" });

                List<string> exact = new List<string>();
                List<string> beginswith = new List<string>();

                for (int i = 0; i < codes.Length; i++)
                {
                    string code = codes[i];
                    if (code.EndsWith("*")) beginswith.Add(code.Substring(0, code.Length - 1));
                    else exact.Add(code);
                }

                seekEntityCodesExact = exact.ToArray();
                seekEntityCodesBeginsWith = beginswith.ToArray();
            }
        }


        public override bool ShouldExecute()
        {
            if (cooldownUntilMs > entity.World.ElapsedMilliseconds) return false;
            if (rand.NextDouble() > 0.03f) return false;
            if (whenInEmotionState != null && !entity.HasEmotionState(whenInEmotionState)) return false;

            targetEntity = (EntityAgent)entity.World.GetNearestEntity(entity.ServerPos.XYZ, seekingRange, seekingRange, (e) => {
                if (!e.Alive || !e.IsInteractable || e.Type == null || e.Entityid == this.entity.Entityid) return false;

                for (int i = 0; i < seekEntityCodesExact.Length; i++)
                {
                    if (e.Type.Code.Path == seekEntityCodesExact[i])
                    {
                        if (e.Type.Code.Path == "player")
                        {
                            IPlayer player = entity.World.PlayerByUid(((EntityPlayer)e).PlayerUID);
                            return player == null || player.WorldData.CurrentGameMode != EnumGameMode.Creative;
                        }
                        return true;
                    }
                }


                for (int i = 0; i < seekEntityCodesBeginsWith.Length; i++)
                {
                    if (e.Type.Code.Path.StartsWith(seekEntityCodesBeginsWith[i])) return true;
                }

                return false;
            });

            if (targetEntity != null)
            {
                if (alarmHerd && entity.HerdId > 0)
                {
                    entity.World.GetNearestEntity(entity.ServerPos.XYZ, seekingRange, seekingRange, (e) =>
                    {
                        EntityAgent agent = e as EntityAgent;
                        if (e.Entityid != entity.Entityid && agent != null && agent.Alive && agent.HerdId == entity.HerdId)
                        {
                            agent.Notify("seekEntity", targetEntity);
                        }

                        return false;
                    });
                }

                targetPos = targetEntity.ServerPos.XYZ;

                if (entity.ServerPos.SquareDistanceTo(targetEntity.ServerPos.XYZ) <= MinDistanceToTarget())
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public float MinDistanceToTarget()
        {
            return System.Math.Max(0.1f, (targetEntity.CollisionBox.X2 - targetEntity.CollisionBox.X1) / 2 + (entity.CollisionBox.X2 - entity.CollisionBox.X1) / 2);
        }

        public override void StartExecute()
        {
            base.StartExecute();
            stuck = false;
            entity.PathTraverser.GoTo(targetPos, moveSpeed, MinDistanceToTarget(), OnGoalReached, OnStuck);
            currentFollowTime = 0;
        }

        public override bool ContinueExecute(float dt)
        {
            currentFollowTime += dt;

            entity.PathTraverser.CurrentTarget.X = targetEntity.ServerPos.X;
            entity.PathTraverser.CurrentTarget.Y = targetEntity.ServerPos.Y;
            entity.PathTraverser.CurrentTarget.Z = targetEntity.ServerPos.Z;

            Cuboidd targetBox = targetEntity.CollisionBox.ToDouble().Add(targetEntity.ServerPos.X, targetEntity.ServerPos.Y, targetEntity.ServerPos.Z);
            Vec3d pos = entity.ServerPos.XYZ.Add(0, entity.CollisionBox.Y2 / 2, 0).Ahead((entity.CollisionBox.X2 - entity.CollisionBox.X1) / 2, 0, entity.ServerPos.Yaw);
            double distance = targetBox.ShortestDistanceFrom(pos);
            

            bool inCreativeMode = (targetEntity is EntityPlayer) && entity.World.PlayerByUid(((EntityPlayer)targetEntity).PlayerUID)?.WorldData?.CurrentGameMode == EnumGameMode.Creative;

            float minDist = MinDistanceToTarget();

            return
                currentFollowTime < maxFollowTime &&
                distance < seekingRange * seekingRange &&
                distance > minDist &&
                targetEntity.Alive &&
                !inCreativeMode &&
                !stuck
            ;
        }


        public override void FinishExecute(bool cancelled)
        {
            base.FinishExecute(cancelled);
            entity.PathTraverser.Stop();
        }


        public override bool Notify(string key, object data)
        {
            if (key == "seekEntity")
            {
                targetEntity = (EntityAgent)data;
                targetPos = targetEntity.ServerPos.XYZ;
                return true;
            }

            return false;
        }


        private void OnStuck()
        {
            stuck = true;   
        }

        private void OnGoalReached()
        {
            entity.PathTraverser.Active = true;
        }

        /*public Vec3i FindPath(Vec3i startPosition, Vec3i goalPosition)
        {
            //set of nodes to explore
            HashSet<Vec3i> unexploredNodes = new HashSet<Vec3i> ();

            // represents h(x) or the score from heuristic function
            IDictionary<Vec3i, int> heuristicScore = new Dictionary<Vec3i, int>();

            // represents g(x) or the distance from start to node "x"
            IDictionary<Vec3i, int> distanceFromStart = new Dictionary<Vec3i, int>();

            //A list of all nodes that are walkable
            IEnumerable<Vec3i> validNodes = walkablePositions
                .Where (x =>x.value == true)
                .Select (x => x.key);

            foreach (Vec3i vertex in validNodes)
            {
                heuristicScore.Add(new KeyValuePair<Vec3i,int> (vertex, int.MaxValue));
                distanceFromStart.Add(new KeyValuePair<Vec3i, int>(vertex, int.MaxValue));
            }

            heuristicScore[startPosition] = (int) startPosition.DistanceTo(goalPosition);
            distanceFromStart[startPosition] = 0;

            //nodes already explored
            HashSet<Vec3i> exploredNodes = new HashSet<Vec3i>();

            

            
        }*/

        
        public bool canMoveHere(BlockPos fromPos, BlockPos feetBlockPos, int stepHeight = 1, int dropHeight = 1, bool canSwim = false)
        {
            bool canMoveHere = true;
            Dictionary<BlockPos,Block> column = new Dictionary<BlockPos,Block>();
            BlockPos headPos = feetBlockPos;
            int entityHeight = (int) this.entity.CollisionBox.Y2; //assumes Y2 is the the tallest
            if((float) entityHeight < this.entity.CollisionBox.Y2) {entityHeight++;} //rounds up
            headPos.Up(entityHeight-1);//go from foot position to head position 

            this.entity.World.BulkBlockAccessor.WalkBlocks(feetBlockPos.DownCopy(dropHeight+1), //drop height plus block to stand on
                                                            headPos.UpCopy(stepHeight), //step height plus head room
                                                            (block, pos) => column.Add(pos, block)
                                                          );
            BlockPos temp = headPos;
            if(column[temp].CollisionBoxes != null)
            {//solid at head
                if(stepHeight < entityHeight) {return false;}
                int i = 0;
                bool hasNonSolid = false;
                while(!hasNonSolid && i<stepHeight)
                { //go up looking for non solid block to step into
                    temp.Up();
                    i++;
                    if(column[temp].CollisionBoxes == null)
                    {//check head height
                        for(int j = entityHeight-1; j>0;j--) //array is height plus step so shouldn't go off the end
                        {
                            temp.Up();
                            if(column[temp].CollisionBoxes != null) {return false;}
                        }
                        temp = fromPos.UpCopy(entityHeight); //start at above head
                        this.entity.World.BulkBlockAccessor.WalkBlocks(fromPos.UpCopy(entityHeight-1), //copy from above head
                                                                    fromPos.UpCopy(entityHeight+i),    //to distance going up (should be lower than entityHeight+step)
                                                                    (block, pos) => column.Add(pos, block));
                        for(int j = i; j>0;j--)
                        {//checks colum above entity head to see if there's room to step up.
                            if(column[temp].CollisionBoxes != null) {return false;}
                            temp.Up();
                        }
                        return true;
                    }
                }
            } else 
            {//not solid at head
                if(column[temp].IsWater()){ return false;} //don't want to deal with underwater and waterfalls yet
                int i = 0;
                int depth = 0;
                bool isNonSolid = true;
                while(isNonSolid && i <entityHeight+dropHeight+1)//go down until solid
                {
                    temp.Down();
                    i++;
                    if(column[temp].CollisionBoxes != null){isNonSolid = false;}
                    if(column[temp].IsWater())
                    {
                        depth++;
                        if(depth > 1 && !canSwim) {return false;}
                    }
                }
                if(i<entityHeight)
                {//check head height and room to step
                    temp.Up(i);//go to head height
                    for(int j = 0; j < entityHeight-i; j++)
                    {
                        temp.Up();
                        if(column[temp].CollisionBoxes != null) {return false;}
                    }
                    this.entity.World.BulkBlockAccessor.WalkBlocks(fromPos.UpCopy(entityHeight-1), //copy from above head
                                                                    fromPos.UpCopy(entityHeight*2-i),    
                                                                    (block, pos) => column.Add(pos, block));
                    for(int j = 0; j<entityHeight-i;j++) //checks colum above entity head to see if there's room to step up.
                    {
                        if(column[temp].CollisionBoxes != null) { return false;}
                        temp.Up();
                    }
                    return true;
                } else 
                {
                    if(column[temp].CollisionBoxes == null) {return false;}//if no solid before drop height return false.
                    return true;//else return true
                }
            }

            return canMoveHere;
        }

        public void updateArea(BlockPos bottom, BlockPos top)
        {
            this.entity.World.BulkBlockAccessor.WalkBlocks(bottom,
                top,
                (block, pos) => area.Add(pos,block),
                false);
        }




    }
}
