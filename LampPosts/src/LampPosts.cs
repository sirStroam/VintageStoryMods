
using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace TestMod
{
	public class TestMod : ModBase
	{
		public static TestMod INSTANCE { get; private set; }
		
		public static ModInfo MOD_INFO { get; } = new ModInfo {
			Type		= EnumModType.Code,
			Name        = "LampPosts",
			Description = "Adds Lamp Posts",
			Author      = "Milo, Stroam",
		};
		
		
		public override ModInfo GetModInfo() { return MOD_INFO; }
		
        // Client

        //Server

        //Common
        
		public override void Start(ICoreAPI api)
		{
			//api.RegisterBlockBehaviorClass(BlockBehaviorRopeLadder.NAME, typeof(BlockBehaviorRopeLadder));
			//api.RegisterBlockBehaviorClass("RotateBehavior", typeof(RotateBehavior));
			base.Start(api);
			INSTANCE = this;
		}
	}
}

