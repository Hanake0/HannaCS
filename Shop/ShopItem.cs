using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace Hanna.Shop
{
	enum Category { Cores, VIPs, Miscelânea }
	enum Currency { Coins, Gems }
	enum Time { UmDia, TrêsDias, SeteDias }

	abstract class ShopItem {
		public DiscordColor DiscColor;
		public DiscordEmoji Emoji;
		public bool Temporary;
		public string ImageLink;
		public string Name;
		public Category Category;
		public string[] Aliases;
		public ulong DefValue;
		public string Description;

		public abstract Task<bool> Buy(CommandContext ctx, DiscordUser user, Currency currency, Time time);
		public ulong GetValue( Currency currency, Time time)
		{
			ulong def = currency switch
			{
				Currency.Gems => (ulong)Math.Ceiling((double)this.DefValue / 1000),
				Currency.Coins => this.DefValue,
				_ => this.DefValue,
			};
			return time switch
			{
				Time.UmDia => (ulong)Math.Ceiling((double)def / 5),
				Time.TrêsDias => (ulong)Math.Ceiling((double)def / 2),
				Time.SeteDias => def,
				_ => def,
			};
		}
	}
}
