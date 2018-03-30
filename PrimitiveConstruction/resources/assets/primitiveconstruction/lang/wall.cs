// RotateBehavior by Stroam
//
// To the extent possible under law, the person who associated CC0 with
// this project has waived all copyright and related or neighboring rights
// to this project.
//
// You should have received a copy of the CC0 legalcode along with this
// work.  If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace primitiveconstruction
{
  public class Wall : Block
  {
    private static string[] OneDir = new string[4]
    {
      "n",
      "e",
      "s",
      "w"
    };
    private static string[] TwoDir = new string[2]
    {
      "ns",
      "ew"
    };
    private static string[] AngledDir = new string[4]
    {
      "ne",
      "es",
      "sw",
      "nw"
    };
    private static string[] ThreeDir = new string[4]
    {
      "nes",
      "new",
      "nsw",
      "esw"
    };

    private static Dictionary<string, KeyValuePair<string[], int>> AngleGroups = new Dictionary<string, KeyValuePair<string[], int>>();

    public string GetOrientations(IWorldAccessor world, BlockPos pos)
    {
      string str = this.GetFenceCode(world, pos, (BlockFacing) BlockFacing.NORTH) + this.GetFenceCode(world, pos, (BlockFacing) BlockFacing.EAST) + this.GetFenceCode(world, pos, (BlockFacing) BlockFacing.SOUTH) + this.GetFenceCode(world, pos, (BlockFacing) BlockFacing.WEST);
      if (str.Length == 0)
        str = "empty";
      return str;
    }

	private string GetFenceCode(IWorldAccessor world, BlockPos pos, BlockFacing facing)
    {
      if (this.ShouldConnectAt(world, pos, facing))
        return facing.Code[0].ToString() ?? "";
      return "";
    }

    private bool IsFenceGateAt(IWorldAccessor world, BlockPos blockPos)
    {
      return ((AssetLocation) ((CollectibleObject) world.BlockAccessor.GetBlock(blockPos)).Code).Path.Contains("gate");
    }

    public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel)
    {
      string orientations = this.GetOrientations(world, (BlockPos) blockSel.Position);
      Block block = world.BlockAccessor.GetBlock(((CollectibleObject) this).CodeWithParts(new string[1]
      {
        orientations
      })) ?? (Block) this;
      world.BlockAccessor.SetBlock((ushort) block.BlockId, (BlockPos) blockSel.Position);
      return true;
    }

    public override void OnNeighourBlockChange(IWorldAccessor world, BlockPos pos, BlockPos neibpos)
    {
      AssetLocation assetLocation = ((CollectibleObject) this).CodeWithParts(new string[1]
      {
        this.GetOrientations(world, pos)
      });
      if (((AssetLocation) ((CollectibleObject) this).Code).Equals(assetLocation))
        return;
      Block block = world.BlockAccessor.GetBlock(assetLocation);
      if (block == null)
        return;
      world.BlockAccessor.SetBlock((ushort) block.BlockId, pos);
    }

    public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1f)
    {
      return new ItemStack[1]
      {
        new ItemStack(world.BlockAccessor.GetBlock(((CollectibleObject) this).CodeWithParts(new string[1]
        {
          "ew"
        })), 1)
      };
    }

    public override ItemStack OnPickBlock(IWorldAccessor world, BlockPos pos)
    {
      return new ItemStack(world.BlockAccessor.GetBlock(((CollectibleObject) this).CodeWithParts(new string[1]
      {
        "ew"
      })), 1);
    }

    public bool ShouldConnectAt(IWorldAccessor world, BlockPos ownPos, BlockFacing side)
    {
      Block block = world.BlockAccessor.GetBlock(ownPos.AddCopy(side));
      if (block.BlockId != null)
      {
        if (((AssetLocation) ((CollectibleObject) this).Code).Path.Split('-')[0] == ((AssetLocation) ((CollectibleObject) block).Code).Path.Split('-')[0])
          return true;
      }
      return (bool) block.SideSolid[side.GetOpposite().Index];
    }

    static Wall()
    {
      Wall.AngleGroups["n"] = new KeyValuePair<string[], int>(Wall.OneDir, 0);
      Wall.AngleGroups["e"] = new KeyValuePair<string[], int>(Wall.OneDir, 1);
      Wall.AngleGroups["s"] = new KeyValuePair<string[], int>(Wall.OneDir, 2);
      Wall.AngleGroups["w"] = new KeyValuePair<string[], int>(Wall.OneDir, 3);
      Wall.AngleGroups["ns"] = new KeyValuePair<string[], int>(Wall.TwoDir, 0);
      Wall.AngleGroups["ew"] = new KeyValuePair<string[], int>(Wall.TwoDir, 1);
      Wall.AngleGroups["ne"] = new KeyValuePair<string[], int>(Wall.AngledDir, 0);
      Wall.AngleGroups["nw"] = new KeyValuePair<string[], int>(Wall.AngledDir, 1);
      Wall.AngleGroups["es"] = new KeyValuePair<string[], int>(Wall.AngledDir, 2);
      Wall.AngleGroups["sw"] = new KeyValuePair<string[], int>(Wall.AngledDir, 3);
      Wall.AngleGroups["nes"] = new KeyValuePair<string[], int>(Wall.ThreeDir, 0);
      Wall.AngleGroups["new"] = new KeyValuePair<string[], int>(Wall.ThreeDir, 1);
      Wall.AngleGroups["nsw"] = new KeyValuePair<string[], int>(Wall.ThreeDir, 2);
      Wall.AngleGroups["esw"] = new KeyValuePair<string[], int>(Wall.ThreeDir, 3);
    }

    public virtual AssetLocation GetRotatedBlockCode(int angle)
    {
      string index = ((CollectibleObject) this).LastCodePart(0);
      if (index == "empty" || index == "nesw")
        return (AssetLocation) ((CollectibleObject) this).Code;
      int num = angle / 90;
      KeyValuePair<string[], int> angleGroup = Wall.AngleGroups[index];
      return ((CollectibleObject) this).CodeWithParts(new string[1]
      {
        angleGroup.Key[(num + angleGroup.Value) % angleGroup.Key.Length]
      });
    }

    public class primitiveconstruction : ModBase {
		public override void Start(ICoreAPI api) {
			base.Start(api);

			//api.RegisterBlockBehaviorClass("LampBaseBehavior", typeof(LampBaseBehavior));
			api.RegisterBlockBehaviorClass("Wall", typeof(Wall));
		}
    }
  }
}
