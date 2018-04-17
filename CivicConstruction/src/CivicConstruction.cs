
using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace LampPosts
{
	public class LampPosts : ModBase
	{
		
		public static ModInfo MOD_INFO { get; } = new ModInfo {
			Type		= EnumModType.Code,
			Name        = "Civic Construction",
			Description = "Adds Lamp Posts",
			Author      = "Milo, Stroam",
		};
		
		
		public override ModInfo GetModInfo() { return MOD_INFO; }
		
        // Client

        //Server

        //Common
        
		public override void Start(ICoreAPI api)
		{
			api.RegisterBlockBehaviorClass("LampConnectorBehavior", typeof(LampConnectorBehavior));
			api.RegisterBlockBehaviorClass("LampPostBehavior", typeof(LampPostBehavior));
			base.Start(api);
		}
	}
}

