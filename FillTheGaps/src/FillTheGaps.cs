using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace FillTheGaps
{
	public class FillTheGaps : ModBase
	{
		
		public static ModInfo MOD_INFO { get; } = new ModInfo {
			Type		= EnumModType.Code,
			Name        = "FillTheGaps",
			Description = "Adds Quick Content",
			Author      = "Stroam",
		};
		
		
		public override ModInfo GetModInfo() { return MOD_INFO; }
		
		public override void Start(ICoreAPI api)
		{
			api.RegisterBlockBehaviorClass("StonePathBehavior", typeof(StonePathBehavior));
			api.RegisterBlockBehaviorClass("BetterlLadder", typeof(BlockBehaviorBetterLadder));
			api.RegisterItemClass("StoneChisel", typeof(ItemStoneChisel));
			api.RegisterItemClass("PolishingStone", typeof(ItemPolishingStone));
			base.Start(api);

		}
	}
}