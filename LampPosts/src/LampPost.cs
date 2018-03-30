// RotateBehavior by Milo Christiansen
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

namespace LampPost {
	public class LampPostMod : ModBase {
		public override void Start(ICoreAPI api) {
			base.Start(api);

			//api.RegisterBlockBehaviorClass("LampBaseBehavior", typeof(LampBaseBehavior));
			api.RegisterBlockBehaviorClass("LampPostBehavior", typeof(LampPostBehavior));
			api.RegisterBlockBehaviorClass("LampConnectorBehavior", typeof(LampConnectorBehavior));
		}
	}

	// Used for the post base, should change shape based on the type of block above.
	/* Not really working.
	class LampBaseBehavior : BlockBehavior {
		private string defaultMaterial;
		private Regex matrialRe;

		public LampBaseBehavior(Block block) : base(block) {
			// NOP
		}

		public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref EnumHandling handling) {
			handling = EnumHandling.PreventDefault;
			if (!world.BlockAccessor.GetBlock(blockSel.Position).IsReplacableBy(this.block)) {
				return false;
			}

			var bAbove = world.BlockAccessor.GetBlock(blockSel.Position.UpCopy());
			world.BlockAccessor.SetBlock(world.BlockAccessor.GetBlock(block.CodeWithParts(getCode(bAbove))).BlockId, blockSel.Position);
			return true;
		}

		public override void OnNeighourBlockChange(IWorldAccessor world, BlockPos pos, BlockPos neibpos, ref EnumHandling handling) {
			if (pos.Y != neibpos.Y - 1) {
				return;
			}

			var bAbove = world.BlockAccessor.GetBlock(neibpos);
			world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(block.CodeWithParts(getCode(bAbove))).BlockId, pos);
			return;
		}

		private string getCode(Block bAbove) {
			// If block above is air, place basic foundation
			if (bAbove.Id == 0) {
				return "none";
			}

			// Check if the block is a known material and use the proper varient.
			var m = matrialRe.Match(bAbove.Code.Path);
			if (m.Success) {
				return m.Result("${mat}");
			}

			// Else, use default post style.
			return defaultMaterial;
		}

		public override void Initialize(JsonObject properties) {
			base.Initialize(properties);
			defaultMaterial = properties["defaultMaterial"].AsString("base");

			// Vile.
			var matRe = ".+-(?<mat>" + Regex.Escape(defaultMaterial);
			foreach (var mat in properties["materials"].AsString("").Split(',')) {
				matRe += "|" + Regex.Escape(mat);
			}
			matRe += ")(?:-.+)?";
			matrialRe = new Regex(matRe);
		}

		public override ItemStack OnPickBlock(IWorldAccessor world, BlockPos pos, ref EnumHandling handling) {
			return new ItemStack(world.BlockAccessor.GetBlock(block.CodeWithParts("none")));
		}
	}
	*/

	// Used for the post, should change shape based on side connections.
	class LampPostBehavior : BlockBehavior {
		private string ownFirstCodePart;
		public LampPostBehavior(Block block) : base(block) {
			this.ownFirstCodePart = ((CollectibleObject) block).FirstCodePart(0);
		}

		public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref EnumHandling handling) {
			handling = EnumHandling.PreventDefault;
			if (!world.BlockAccessor.GetBlock(blockSel.Position).IsReplacableBy(this.block)) {
				return false;
			}

			var code = getCode(world, blockSel.Position);
			world.BlockAccessor.SetBlock(world.BlockAccessor.GetBlock(block.CodeWithParts(code)).BlockId, blockSel.Position);
			return true;
		}

		public override void OnNeighourBlockChange(IWorldAccessor world, BlockPos pos, BlockPos neibpos, ref EnumHandling handling) {
			//Block neibBlock = world.BlockAccessor.GetBlock(neibpos);
			if (pos.Y == neibpos.Y - 1 || pos.Y == neibpos.Y + 1 || IsLantern(world, neibpos)){
				return;
			}

			world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(block.CodeWithParts(getCode(world, pos))).BlockId, pos);
		}

		private string getCode(IWorldAccessor world, BlockPos pos) {
			var code = "";

			// Build a string of the form "nesw" with each letter present only if there is a solid block face on that side.
			// if (world.BlockAccessor.GetBlock(pos.NorthCopy()).SideSolid[BlockFacing.SOUTH.Index]) {
			// 	code += "n";
			// }
			// if (world.BlockAccessor.GetBlock(pos.EastCopy()).SideSolid[BlockFacing.WEST.Index]) {
			// 	code += "e";
			// }
			// if (world.BlockAccessor.GetBlock(pos.SouthCopy()).SideSolid[BlockFacing.NORTH.Index]) {
			// 	code += "s";
			// }
			// if (world.BlockAccessor.GetBlock(pos.WestCopy()).SideSolid[BlockFacing.EAST.Index]) {
			// 	code += "w";
			// }

			// This is really screwy, and I don't feel like figuring out why. It works, don't mess with it.
			// (It probably has something to do with the block rotation)
			// Also, I can't use SideSolid or it won't connect to lanterns and signs.
			if (world.BlockAccessor.GetBlock(pos.WestCopy()).Id != 0) {
				code += "n";
			}
			if (world.BlockAccessor.GetBlock(pos.NorthCopy()).Id != 0) {
				code += "e";
			}
			if (world.BlockAccessor.GetBlock(pos.EastCopy()).Id != 0) {
				code += "s";
			}
			if (world.BlockAccessor.GetBlock(pos.SouthCopy()).Id != 0) {
				code += "w";
			}

			// Now hande the "no connections" case.
			if (code == "") {
				code = "empty";
			}
			return code;
		}

		public override ItemStack OnPickBlock(IWorldAccessor world, BlockPos pos, ref EnumHandling handling) {
			return new ItemStack(world.BlockAccessor.GetBlock(block.CodeWithParts("empty")));
		}
		private bool IsLantern(IWorldAccessor world, BlockPos blockPos)
    	{
      		return ((AssetLocation) ((CollectibleObject) world.BlockAccessor.GetBlock(blockPos)).Code).Path.Contains("lantern");
    	}

		private BlockFacing GetDirection(BlockPos origin, BlockPos neighbor){
			if(origin.Z == neighbor.Z - 1){ return BlockFacing.SOUTH; }
			if(origin.Z == neighbor.Z + 1){ return BlockFacing.NORTH; }
			if(origin.X == neighbor.X - 1){ return BlockFacing.EAST; }
			if(origin.X == neighbor.X + 1){ return BlockFacing.WEST; }
			return null;
		}

		// public override AssetLocation GetHorizontallyFlippedBlockCode(EnumAxis axis, ref EnumHandling handling) {
		//	//
		// }
	}

	// Lamp wall connectors, two types:
	//	A: Flip top or bottom.
	//	B: Swap long or short version based on if something is connected to the end of the block.
	class LampConnectorBehavior : BlockBehavior {
		private bool flipable = true;

		public LampConnectorBehavior(Block block) : base(block) {
			// NOP
		}

		public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref EnumHandling handling) {
			handling = EnumHandling.PreventDefault;
			if (!world.BlockAccessor.GetBlock(blockSel.Position).IsReplacableBy(this.block)) {
				return false;
			}
			if (blockSel.Face.IsVertical) {
				return false;
			}

			var dir = blockSel.Face.GetOpposite().Code;
			if (flipable) {
				var flip = blockSel.HitPosition.Y < 0.5 ? "down" : "up";
				world.BlockAccessor.SetBlock(world.BlockAccessor.GetBlock(block.CodeWithParts(dir, flip)).BlockId, blockSel.Position);
			} else {
				var len = getCode(world, blockSel.Position, dir);
				world.BlockAccessor.SetBlock(world.BlockAccessor.GetBlock(block.CodeWithParts(dir, len)).BlockId, blockSel.Position);
			}
			return true;
		}

		private static Regex dirRe = new Regex(@".*-(?<dir>north|south|east|west)(?:-.*)?");
		public override void OnNeighourBlockChange(IWorldAccessor world, BlockPos pos, BlockPos neibpos, ref EnumHandling handling) {
			if (!flipable) {
				var m = dirRe.Match(block.Code.Path);
				var dir = "north";
				var len = "short";
				if (m.Success) {
					dir = m.Result("${dir}");
					len = getCode(world, pos, dir);
				}

				world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(block.CodeWithParts(dir, len)).BlockId, pos);
			}
		}

		private string getCode(IWorldAccessor world, BlockPos pos, string dir) {
			bool solid;
			switch (dir) {
				case "north":
					solid = world.BlockAccessor.GetBlock(pos.EastCopy()).Id != 0;
					break;
				case "south":
					solid = world.BlockAccessor.GetBlock(pos.WestCopy()).Id != 0;
					break;
				case "east":
					solid = world.BlockAccessor.GetBlock(pos.SouthCopy()).Id != 0;
					break;
				case "west":
					solid = world.BlockAccessor.GetBlock(pos.NorthCopy()).Id != 0;
					break;
				default:
					// Should be impossible.
					solid = false;
					break;
			}
			return solid ? "long" : "short";
		}

		public override void Initialize(JsonObject properties) {
			base.Initialize(properties);
			flipable = properties["type"].AsString("flippable") == "flippable";
		}

		public override ItemStack OnPickBlock(IWorldAccessor world, BlockPos pos, ref EnumHandling handling) {
			if (flipable) {
				return new ItemStack(world.BlockAccessor.GetBlock(block.CodeWithParts("north", "up")));
			}
			return new ItemStack(world.BlockAccessor.GetBlock(block.CodeWithParts("north", "short")));
		}
	}
}
