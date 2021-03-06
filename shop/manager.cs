﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using Hanna;

namespace Hanna.Shop
{
	class ShopManager
	{
		public List<ShopItem> Colors { get; private set; }
		public List<ShopItem> Miscs { get; private set; }
		public List<ShopItem> VIPs { get; private set; }
		public ShopManager(CommandContext ctx)
		{

			this.Colors = new List<ShopItem>
			{
				/*
				new Color(ctx, "Padrão", Category.Cores, new string[] { "padrao", "padrão", "default", "normal" },
					000, new DiscordColor(), 796475284260913233, "Cor padrão do server"
				),
				*/
				new Color
				{
					Temporary = true, DiscColor = new DiscordColor("#ffebcd"), Name = "🌺Amêndoa",
					Aliases = new string[] { "amêndoa", "amendoa", "almond" },
					ImageLink = "https://twemoji.maxcdn.com/2/72x72/1f33a.png",
					Emoji = DiscordEmoji.FromName(ctx.Client, ":hibiscus:"),
					Category = Category.Cores, DefValue = 2000,
					Role = ctx.Client.Guilds[795053938666766366].Roles[800459707361132604],
					Description = "Poderá usar a cor <@&800459707361132604>\n no servidor, através do comando `hcor`.",
				},
				new Color
				{
					Temporary = true, DiscColor = new DiscordColor("#ffa3f7"), Name = "🩰Rosa Bebê",
					Aliases = new string[] { "rosa bebe", "rosa bebê", "rosabebe", "rosabebê", "baby pink", "babypink" },
					ImageLink = "https://images.emojiterra.com/twitter/v13.0/512px/1fa70.png",
					Emoji = DiscordEmoji.FromName(ctx.Client, ":ballet_shoes:"),
					Category = Category.Cores, DefValue = 2000,
					Role = ctx.Client.Guilds[795053938666766366].Roles[800460708319592478],
					Description = "Poderá usar a cor <@&800460708319592478>\n no servidor, através do comando `hcor`.",
				},
				new Color
				{
					Temporary = true, DiscColor = new DiscordColor("#2dcc70"), Name = "🌿Menta",
					Aliases = new string[] {  "menta", "mint", "mênta" },
					ImageLink = "https://twemoji.maxcdn.com/2/72x72/1f33f.png",
					Emoji = DiscordEmoji.FromName(ctx.Client, ":herb:"),
					Category = Category.Cores, DefValue = 2000,
					Role = ctx.Client.Guilds[795053938666766366].Roles[800460354445377566],
					Description = "Poderá usar a cor <@&800460354445377566>\n no servidor, através do comando `hcor`.",
				},
				new Color(ctx, "🐻Marrom", ":bear:", true, Category.Cores, new string[] { "marrom", "brown" },
				3000, new DiscordColor("#923004"), 800460023385948210, "Poderá usar a cor <@&800460023385948210> no servidor, através do comando `hcor`."
				),
				new Color(ctx, "🌙Meia Noite", ":crescent_moon:", true, Category.Cores, new string[] { "meia noite", "meianoite", "mid night", "midnight" },
				3000, new DiscordColor("#4b0082"), 800457860411555852, "Poderá usar a cor <@&800457860411555852> no servidor, através do comando `hcor`."
				),
				new Color(ctx, "🐸Lima", ":frog:", true, Category.Cores, new string[] { "lima", "lime" },
				3000, new DiscordColor("#10ff00"), 800459707361132604, "Poderá usar a cor <@&800459707361132604> no servidor, através do comando `hcor`."
				),
				new Color(ctx, "🐦Vermelho", ":bird:", true, Category.Cores, new string[] { "vermelho", "red" },
				5000, new DiscordColor("#ff0003"), 800458359471210536, "Poderá usar a cor <@&800458359471210536> no servidor, através do comando `hcor`."
				),
				new Color(ctx, "⭐Amarelo", ":star:", true, Category.Cores, new string[] { "amarelo", "yellow" },
				5000, new DiscordColor("#feff00"), 800458737143119902, "Poderá usar a cor <@&800458737143119902> no servidor, através do comando `hcor`."
				),
				new Color(ctx, "🌊Azul", ":ocean:", true, Category.Cores, new string[] { "azul", "blue" },
				5000, new DiscordColor("#00b3ff"), 800459076755783690, "Poderá usar a cor <@&800459076755783690> no servidor, através do comando `hcor`."
				),
				new Color(ctx, "☘️Verde Grama", ":shamrock:", true, Category.Cores, new string[] { "verde grama", "verdegrama", "grass green", "grassgreen" },
				5000, new DiscordColor("#91ff83"), 800459459440672818, "Poderá usar a cor <@&800459459440672818> no servidor, através do comando `hcor`."
				),
				new Color(ctx, "🍇Púrpura", ":grapes:", true, Category.Cores, new string[] { "púrpura", "purpura", "purple" },
				8000, new DiscordColor("#af00ff"), 800459148952993793, "Poderá usar a cor <@&800459148952993793> no servidor, através do comando `hcor`."
				),
				new Color(ctx, "🍊Laranja", ":tangerine:", true, Category.Cores, new string[] { "laranja", "orange" },
				8000, new DiscordColor("#ff8a00"), 800459444563607564, "Poderá usar a cor <@&800459444563607564> no servidor, através do comando `hcor`."
				),
				new Color(ctx, "🐳Ciano", ":whale:", true, Category.Cores, new string[] { "ciano", "cyan" },
				8000, new DiscordColor("#00faff"), 800461287477477457, "Poderá usar a cor <@&800461287477477457> no servidor, através do comando `hcor`."
				),
				new Color(ctx, "🦢Branco", ":swan:", true, Category.Cores, new string[] { "branco", "white" },
				8000, new DiscordColor("#ffffff"), 800461639152435293, "Poderá usar a cor <@&800461639152435293> no servidor, através do comando `hcor`."
				),
				new Color(ctx, "🕷️Void", ":spider:", true, Category.Cores, new string[] { "black", "void", "preto" },
				10000, new DiscordColor("#050000"), 800458737143119902, "Poderá usar a cor <@&800458737143119902> no servidor, através do comando `hcor`."
				),
				new Color(ctx, "🏅Dourado", ":medal:", true, Category.Cores, new string[] { "dourado", "gold" },
				10000, new DiscordColor("#feff00"), 800459882187849780, "Poderá usar a cor <@&800459882187849780> no servidor, através do comando `hcor`."
				),
			};
		}
		public ShopItem[] GroupBy(Category category, int groupSize, int index = 0)
		{
			return this.Colors.Skip(index).Take(groupSize).ToArray();
		}
		public List<ShopItem> GetList(Category category)
		{
			return category switch
			{
				Category.Cores => this.Colors,
				Category.Miscelânea => this.Miscs,
				Category.VIPs => this.VIPs,
				_ => this.Colors,
			};
		}
		public ShopItem Find(Category category, string alias)
		{
			return this.GetList(category).First(c =>
				Array.Exists(c.Aliases, name => name == alias.ToLower())
			);
		}
		public string FormatTime(Time time)
		{
			return time switch
			{
				Time.UmDia => "1 dia",
				Time.TrêsDias => "3 dias",
				Time.SeteDias => "7 dias",
				_ => "",
			};
		}
		public Category GetCategory(string emojiName)
		{
			return emojiName switch
			{
				":art:" => Category.Cores,
				":ticket:" => Category.VIPs,
				":jigsaw:" => Category.Miscelânea,
				_ => Category.Cores,
			};
		}
		public string GetEmoji(Category category)
		{
			return category switch
			{
				Category.Cores => ":art:",
				Category.Miscelânea => ":jigsaw:",
				Category.VIPs => ":ticket:",
				_ => ":art:",
			};
		}
		public string GetImageUrl(CommandContext ctx, Category category)
		{
			return category switch
            {
                Category.Cores => "https://twemoji.maxcdn.com/2/72x72/1f3a8.png",
                Category.Miscelânea => "https://twemoji.maxcdn.com/2/72x72/1f9e9.png",
                Category.VIPs => "https://twemoji.maxcdn.com/2/72x72/1f3ab.png",
                _ => "https://twemoji.maxcdn.com/2/72x72/1f3a8.png",
            };
		}
	}
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
	class Color : ShopItem
	{
		public DiscordRole Role;
		public ulong RoleId { get; private set; }
		public Color(CommandContext ctx, string name, string emoji, bool temporary, Category category, string[] aliases,
			ulong defValue, DiscordColor color, ulong roleId, string description)
		{
			this.DiscColor = color;
			this.Name = name;
			this.Emoji = DiscordEmoji.FromName(ctx.Client, emoji);
			this.Temporary = temporary;
			this.Category = category;
			this.Aliases = aliases;
			this.DefValue = defValue;
			this.Description = description;
			this.Role = ctx.Client.Guilds[795053938666766366].Roles[roleId];

		}
		public Color(CommandContext ctx)
		{
			this.Role = ctx.Client.Guilds[795053938666766366].Roles[this.RoleId];
		}
		public Color() { }
		public override async Task<bool> Buy(CommandContext ctx, DiscordUser user, Currency currency, Time time)
		{
			await Task.Delay(2);
			return false;
		}
	}
}
