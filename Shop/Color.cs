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
		public override async Task<bool> Buy(CommandContext ctx, DiscordUser user, Currency currency, Time time)
		{
			await Task.Delay(2);
			return false;
		}
	}
}
