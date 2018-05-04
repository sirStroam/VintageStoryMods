
using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

[
	assembly: ModInfo( "testmod", "testmod",
    Description = "this is a test",
    Website     = "https://github.com/sirStroam/VintageStoryMods",
    Authors     = new []{ "Stroam" })
]
namespace TestMod
{
	public class TestMod : ModSystem
	{	

        // Client

        //Server

        //Common
        
		public override void Start(ICoreAPI api)
		{
			//api.RegisterBlockBehaviorClass(BlockBehaviorRopeLadder.NAME, typeof(BlockBehaviorRopeLadder));
			//api.RegisterBlockBehaviorClass("RotateBehavior", typeof(RotateBehavior));
			base.Start(api);
		}
	}
}

