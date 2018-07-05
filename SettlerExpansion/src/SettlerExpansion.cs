using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

[
	assembly: ModInfo( "Settler Expansion", "settler",
    Description = "Adds new mechanics to VS",
    Website     = "https://github.com/sirStroam/VintageStoryMods",
    Authors     = new []{ "Stroam" })
]

namespace settler
{
	public class settler : ModSystem
	{
		
		public override void Start(ICoreAPI api)
		{
			
			base.Start(api);
			//api.RegisterEntityBehaviorClass("Suffocate", typeof(EntityBehaviorSuffocate));
			//api.RegisterBlockBehaviorClass("WallBehavior", typeof(WallBehavior));
		}
	}
}