// SoundOnInteract by Milo Christiansen
//
// To the extent possible under law, the person who associated CC0 with
// this project has waived all copyright and related or neighboring rights
// to this project.
//
// You should have received a copy of the CC0 legalcode along with this
// work.  If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace RotateBehavior {
	public class RotateBehaviorMod : ModBase {
		public override void Start(ICoreAPI api) {
			base.Start(api);

			api.RegisterBlockBehaviorClass("RotateBehavior", typeof(RotateBehavior));
		}
	}

	class RotateBehavior : BlockBehavior {
		private bool rotate = false;
		private string rotateDrop = "north";
		private bool flip = false;
		private string flipDrop = "up";

		public RotateBehavior(Block block) : base(block) {
			// NOP
		}

		public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, IItemStack itemstack, BlockSelection blockSel, ref EnumHandling handling) {
			if (!world.BlockAccessor.GetBlock(blockSel.Position).IsReplacableBy(this.block)) {
				return false;
			}

			BlockFacing[] horVer = Block.SuggestedHVOrientation(byPlayer, blockSel);
			if (blockSel.Face.IsVertical) {
				horVer[1] = blockSel.Face;
			} else {
				horVer[1] = blockSel.HitPosition.Y < 0.5 ? BlockFacing.UP : BlockFacing.DOWN;
			}

			AssetLocation blockCode;
			this.block.Code.FirstPathPart();
			if (rotate && flip) {
				blockCode = block.CodeWithParts(horVer[1].Code, horVer[0].Code);
			} else if (rotate) {
				blockCode = block.CodeWithParts(horVer[0].Code);
			} else if (flip) {
				blockCode = block.CodeWithParts(horVer[1].Code);
			} else {
				blockCode = this.block.Code;
			}

			world.BlockAccessor.SetBlock(world.BlockAccessor.GetBlock(blockCode).BlockId, blockSel.Position);
			return true;
		}

		public override ItemStack[] GetDrops(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier, ref EnumHandling handled) {
			handled = EnumHandling.PreventDefault;

			AssetLocation blockCode;
			if (rotate && flip) {
				blockCode = this.block.CodeWithParts(flipDrop, rotateDrop);
			} else if (rotate) {
				blockCode = this.block.CodeWithParts(rotateDrop);
			} else if (flip) {
				blockCode = this.block.CodeWithParts(flipDrop);
			} else {
				blockCode = this.block.Code;
			}
			return new ItemStack[1] { new ItemStack(world.BlockAccessor.GetBlock(blockCode), 1) };
		}

		public override void Initialize(JsonObject properties) {
			base.Initialize(properties);
			rotate = properties["rotate"].AsBool(rotate);
			rotateDrop = properties["rotateDrop"].AsString(rotateDrop);
			flip = properties["flip"].AsBool(flip);
			flipDrop = properties["flipDrop"].AsString(flipDrop);
		}
	}
}
