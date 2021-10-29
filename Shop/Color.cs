using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace Hanna.Shop
{
	class Color : ShopItem
	{
		public DiscordRole Role;
		public ulong RoleId { get; private set; }
		public Color() {}

		public override async Task<bool> Buy(DiscordUser user, string cName, string tOptName) {
			throw new NotImplementedException("Ainda nao e possivel comprar cores");
		}
	}
}
