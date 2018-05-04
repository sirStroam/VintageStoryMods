using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

[
	assembly: ModInfo( "Primitive Construction", "primitiveconstruction",
    Description = "Adds primitive materials",
    Website     = "https://github.com/sirStroam/VintageStoryMods",
    Authors     = new []{ "Stroam" })
]

namespace primitiveconstruction
{
	public class primitiveconstruction : ModSystem
	{
		
		public override void Start(ICoreAPI api)
		{
			
			base.Start(api);
			api.RegisterBlockBehaviorClass("rotateninety", typeof(rotateninety));
			api.RegisterBlockBehaviorClass("WallBehavior", typeof(WallBehavior));
		}
	}
}