using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace primitiveconstruction
{
	public class primitiveconstruction : ModBase
	{
		public static primitiveconstruction INSTANCE { get; private set; }
		
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
			INSTANCE = this;
		}
		/* 
		public override void StartServerSide(ICoreServerAPI api)
		{
			
		}
		
		public override void StartClientSide(ICoreClientAPI api)
		{
			
		}*/
	}
}