using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

[
	assembly: ModInfo( "Fill the Gaps", "fillthegaps",
	Description = "Extends vanilla without straying from it",
	Website     = "https://github.com/sirStroam/VintageStoryMods",
	Authors     = new []{ "Stroam" })
]


namespace FillTheGaps
{
	public class FillTheGaps : ModSystem
	{

		
		
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