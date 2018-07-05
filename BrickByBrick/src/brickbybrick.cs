using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

[
	assembly: ModInfo( "Brick by Brick", "bbb",
    Description = "Adds primitive materials",
    Website     = "https://github.com/sirStroam/VintageStoryMods",
    Authors     = new []{ "Stroam" })
]

namespace BbB
{
	public class BrickByBrick : ModSystem
	{
		
		public override void Start(ICoreAPI api)
		{
			
			base.Start(api);
			//api.RegisterItemClass("StoneChisel", typeof(ItemStoneChisel));
			//api.RegisterBlockBehaviorClass("rotateninety", typeof(rotateninety));
			//api.RegisterBlockBehaviorClass("WallBehavior", typeof(WallBehavior));
		}
	}
}