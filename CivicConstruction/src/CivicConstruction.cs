
using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

[
	assembly: ModInfo( "Civic Construction", "civicconstruction",
    Description = "Adds medieval structures like lampposts",
    Website     = "https://github.com/sirStroam/VintageStoryMods",
    Authors     = new []{ "Stroam" })
]
	
namespace CivicConstruction
{
	public class CivicConstruction : ModSystem
	{
		
        // Client

        //Server

        //Common
        
		public override void Start(ICoreAPI api)
		{
			api.RegisterBlockBehaviorClass("LampConnectorBehavior", typeof(LampConnectorBehavior));
			api.RegisterBlockBehaviorClass("LampPostBehavior", typeof(LampPostBehavior));
			api.RegisterBlockBehaviorClass("StonePathBehavior", typeof(StonePathBehavior));
			base.Start(api);
		}
	}
}

