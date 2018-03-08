using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace RopeLadder
{
	public class RopeLadder : ModBase
	{
		public static RopeLadder INSTANCE { get; private set; }
		
		public static ModInfo MOD_INFO { get; } = new ModInfo {
			Name        = "Rope Ladder",
			Description = "Adds rope ladder",
			Author      = "Stroam",
		};
		
		
		public override ModInfo GetModInfo() { return MOD_INFO; }
		
		public override void Start(ICoreAPI api)
		{
			base.Start(api);
			INSTANCE = this;
		}
		
		public override void StartServerSide(ICoreServerAPI api)
		{
			
		}
		
		public override void StartClientSide(ICoreClientAPI api)
		{
			
		}
	}
}