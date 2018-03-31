using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace primitiveconstruction
{
	public class primitiveconstruction : ModBase
	{
		
		public static ModInfo MOD_INFO { get; } = new ModInfo {
			Type		= EnumModType.Code,
			Name        = "Primitive Construction",
			Description = "Adds Primitive Construction Blocks",
			Author      = "Stroam",
		};
		
		public override ModInfo GetModInfo() { return MOD_INFO; }
		
		public override void Start(ICoreAPI api)
		{
			
			base.Start(api);
			api.RegisterBlockBehaviorClass("rotateninety", typeof(rotateninety));
			api.RegisterBlockBehaviorClass("WallBehavior", typeof(WallBehavior));
		}
	}
}