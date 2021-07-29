using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
					000, new DiscordColor(), 851195394494693388, "Cor padrão do server"
				),
				*/
				new Color
				{
					Temporary = true, DiscColor = new DiscordColor("#ffebcd"), Name = "🌺Amêndoa",
					Aliases = new string[] { "amêndoa", "amendoa", "almond" },
					ImageLink = "https://twemoji.maxcdn.com/2/72x72/1f33a.png",
					Emoji = DiscordEmoji.FromName(ctx.Client, ":hibiscus:"),
					Category = Category.Cores, DefValue = 2000,
					Role = ctx.Client.Guilds[822904923556675656].Roles[851195394494693388],
					Description = $"Poderá usar a cor <@&851195394494693388>\n no servidor, através do comando `hcor`.",
				}, // 🌺Amêndoa
				new Color
				{
					Temporary = true, DiscColor = new DiscordColor("#ffa3f7"), Name = "🩰Rosa Bebê",
					Aliases = new string[] { "rosa bebe", "rosa bebê", "rosabebe", "rosabebê", "baby pink", "babypink" },
					ImageLink = "https://images.emojiterra.com/twitter/v13.0/512px/1fa70.png",
					Emoji = DiscordEmoji.FromName(ctx.Client, ":ballet_shoes:"),
					Category = Category.Cores, DefValue = 2000,
					Role = ctx.Client.Guilds[822904923556675656].Roles[851195394494693388],
					Description = "Poderá usar a cor <@&851195394494693388>\n no servidor, através do comando `hcor`.",
				}, // 🩰Rosa Bebê
				new Color
				{
					Temporary = true, DiscColor = new DiscordColor("#2dcc70"), Name = "🌿Menta",
					Aliases = new string[] {  "menta", "mint", "mênta" },
					ImageLink = "https://twemoji.maxcdn.com/2/72x72/1f33f.png",
					Emoji = DiscordEmoji.FromName(ctx.Client, ":herb:"),
					Category = Category.Cores, DefValue = 2000,
					Role = ctx.Client.Guilds[822904923556675656].Roles[851195394494693388],
					Description = "Poderá usar a cor <@&851195394494693388>\n no servidor, através do comando `hcor`.",
				}, // 🌿Menta
				new Color
				{
					Temporary = true, DiscColor = new DiscordColor("#923004"), Name = "🐻Marrom",
					Aliases = new string[] {  "marrom", "brown" },
					ImageLink = "https://twemoji.maxcdn.com/2/72x72/1f43b.png",
					Emoji = DiscordEmoji.FromName(ctx.Client, ":bear:"),
					Category = Category.Cores, DefValue = 3000,
					Role = ctx.Client.Guilds[822904923556675656].Roles[851195394494693388],
					Description = "Poderá usar a cor <@&851195394494693388>\n no servidor, através do comando `hcor`.",
				}, // 🐻Marrom
				new Color
				{
					Temporary = true, DiscColor = new DiscordColor("#4b0082"), Name = "🌙Meia Noite",
					Aliases = new string[] {  "meia noite", "meianoite", "mid night", "midnight" },
					ImageLink = "https://twemoji.maxcdn.com/2/72x72/1f319.png",
					Emoji = DiscordEmoji.FromName(ctx.Client, ":crescent_moon:"),
					Category = Category.Cores, DefValue = 3000,
					Role = ctx.Client.Guilds[822904923556675656].Roles[851195394494693388],
					Description = "Poderá usar a cor <@&851195394494693388>\n no servidor, através do comando `hcor`.",
				}, // 🌙Meia Noite
				new Color
				{
					Temporary = true, DiscColor = new DiscordColor("#10ff00"), Name = "🐸Lima",
					Aliases = new string[] {  "lima", "lime" },
					ImageLink = "https://twemoji.maxcdn.com/2/72x72/1f438.png",
					Emoji = DiscordEmoji.FromName(ctx.Client, ":frog:"),
					Category = Category.Cores, DefValue = 3000,
					Role = ctx.Client.Guilds[822904923556675656].Roles[851195394494693388],
					Description = "Poderá usar a cor <@&851195394494693388>\n no servidor, através do comando `hcor`.",
				}, // 🐸Lima
				new Color
				{
					Temporary = true, DiscColor = new DiscordColor("#ff0003"), Name = "🐦Vermelho",
					Aliases = new string[] {  "vermelho", "red" },
					ImageLink = "https://twemoji.maxcdn.com/2/72x72/1f426.png",
					Emoji = DiscordEmoji.FromName(ctx.Client, ":bird:"),
					Category = Category.Cores, DefValue = 5000,
					Role = ctx.Client.Guilds[822904923556675656].Roles[851195394494693388],
					Description = "Poderá usar a cor <@&851195394494693388>\n no servidor, através do comando `hcor`.",
				}, // 🐦Vermelho
				new Color
				{
					Temporary = true, DiscColor = new DiscordColor("#feff00"), Name = "⭐Amarelo",
					Aliases = new string[] {  "amarelo", "yellow" },
					ImageLink = "https://twemoji.maxcdn.com/2/72x72/2b50.png",
					Emoji = DiscordEmoji.FromName(ctx.Client, ":star:"),
					Category = Category.Cores, DefValue = 5000,
					Role = ctx.Client.Guilds[822904923556675656].Roles[851195394494693388],
					Description = "Poderá usar a cor <@&851195394494693388>\n no servidor, através do comando `hcor`.",
				}, // ⭐Amarelo
				new Color
				{
					Temporary = true, DiscColor = new DiscordColor("#00b3ff"), Name = "🌊Azul",
					Aliases = new string[] {  "azul", "blue" },
					ImageLink = "https://twemoji.maxcdn.com/2/72x72/1f30a.png",
					Emoji = DiscordEmoji.FromName(ctx.Client, ":ocean:"),
					Category = Category.Cores, DefValue = 5000,
					Role = ctx.Client.Guilds[822904923556675656].Roles[851195394494693388],
					Description = "Poderá usar a cor <@&851195394494693388>\n no servidor, através do comando `hcor`.",
				}, // 🌊Azul
				new Color
				{
					Temporary = true, DiscColor = new DiscordColor("#91ff83"), Name = "☘️Verde Grama",
					Aliases = new string[] {  "verde grama", "verdegrama", "grass green", "grassgreen" },
					ImageLink = "https://twemoji.maxcdn.com/2/72x72/2618-fe0f.png",
					Emoji = DiscordEmoji.FromName(ctx.Client, ":shamrock:"),
					Category = Category.Cores, DefValue = 5000,
					Role = ctx.Client.Guilds[822904923556675656].Roles[851195394494693388],
					Description = "Poderá usar a cor <@&851195394494693388>\n no servidor, através do comando `hcor`.",
				}, // ☘️Verde Grama
				new Color
				{
					Temporary = true, DiscColor = new DiscordColor("#af00ff"), Name = "🍇Púrpura",
					Aliases = new string[] {  "púrpura", "purpura", "purple" },
					ImageLink = "https://twemoji.maxcdn.com/2/72x72/1f347.png",
					Emoji = DiscordEmoji.FromName(ctx.Client, ":grapes:"),
					Category = Category.Cores, DefValue = 8000,
					Role = ctx.Client.Guilds[822904923556675656].Roles[851195394494693388],
					Description = "Poderá usar a cor <@&851195394494693388>\n no servidor, através do comando `hcor`.",
				}, // 🍇Púrpura
				new Color
				{
					Temporary = true, DiscColor = new DiscordColor("#ff8a00"), Name = "🍊Laranja",
					Aliases = new string[] {  "laranja", "orange" },
					ImageLink = "https://twemoji.maxcdn.com/2/72x72/1f34a.png",
					Emoji = DiscordEmoji.FromName(ctx.Client, ":tangerine:"),
					Category = Category.Cores, DefValue = 8000,
					Role = ctx.Client.Guilds[822904923556675656].Roles[851195394494693388],
					Description = "Poderá usar a cor <@&851195394494693388>\n no servidor, através do comando `hcor`.",
				}, // 🍊Laranja
				new Color
				{
					Temporary = true, DiscColor = new DiscordColor("#00faff"), Name = "🐳Ciano",
					Aliases = new string[] {  "ciano", "cyan" },
					ImageLink = "https://twemoji.maxcdn.com/2/72x72/1f433.png",
					Emoji = DiscordEmoji.FromName(ctx.Client, ":whale:"),
					Category = Category.Cores, DefValue = 8000,
					Role = ctx.Client.Guilds[822904923556675656].Roles[851195394494693388],
					Description = "Poderá usar a cor <@&851195394494693388>\n no servidor, através do comando `hcor`.",
				}, // 🐳Ciano
				new Color
				{
					Temporary = true, DiscColor = new DiscordColor("#ffffff"), Name = "🦢Branco",
					Aliases = new string[] {  "branco", "white" },
					ImageLink = "https://twemoji.maxcdn.com/2/72x72/1f9a2.png",
					Emoji = DiscordEmoji.FromName(ctx.Client, ":swan:"),
					Category = Category.Cores, DefValue = 8000,
					Role = ctx.Client.Guilds[822904923556675656].Roles[851195394494693388],
					Description = "Poderá usar a cor <@&851195394494693388>\n no servidor, através do comando `hcor`.",
				}, // 🦢Branco
				new Color
				{
					Temporary = true, DiscColor = new DiscordColor("#050000"), Name = "🕷️Void",
					Aliases = new string[] {  "black", "void", "preto" },
					ImageLink = "https://twemoji.maxcdn.com/2/72x72/1f577-fe0f.png",
					Emoji = DiscordEmoji.FromName(ctx.Client, ":spider:"),
					Category = Category.Cores, DefValue = 10000,
					Role = ctx.Client.Guilds[822904923556675656].Roles[851195394494693388],
					Description = "Poderá usar a cor <@&851195394494693388>\n no servidor, através do comando `hcor`.",
				}, // 🕷️Void
				new Color
				{
					Temporary = true, DiscColor = new DiscordColor("#feff00"), Name = "🏅Dourado",
					Aliases = new string[] {  "dourado", "gold" },
					ImageLink = "https://twemoji.maxcdn.com/2/72x72/1f3c5.png",
					Emoji = DiscordEmoji.FromName(ctx.Client, ":medal:"),
					Category = Category.Cores, DefValue = 10000,
					Role = ctx.Client.Guilds[822904923556675656].Roles[851195394494693388],
					Description = "Poderá usar a cor <@&851195394494693388>\n no servidor, através do comando `hcor`.",
				}, // 🏅Dourado
			};
			this.Miscs = new List<ShopItem>();
			this.VIPs = new List<ShopItem>();
		}
		public ShopItem[] GroupBy(Category category, int groupSize, int index = 0)
		{
			return category switch
			{
				Category.Cores => this.Colors.Skip(index).Take(groupSize).ToArray(),
				Category.Miscelânea => this.Miscs.Skip(index).Take(groupSize).ToArray(),
				Category.VIPs => this.VIPs.Skip(index).Take(groupSize).ToArray(),
				_ => Array.Empty<ShopItem>(),
			};
		}
		public List<ShopItem> GetList(Category category)
		{
			return category switch
			{
				Category.Cores => this.Colors,
				Category.Miscelânea => this.Miscs,
				Category.VIPs => this.VIPs,
				_ => new List<ShopItem>(),
			};
		}
		public ShopItem Find(Category category, string alias)
		{
			return this.GetList(category).First(c =>
				Array.Exists(c.Aliases, name => name == alias.ToLower())
			);
		}
		public static string FormatTime(Time time) => time switch
		{
			Time.UmDia => "1 dia",
			Time.TrêsDias => "3 dias",
			Time.SeteDias => "7 dias",
			_ => "",
		};
		public static Category GetCategory(string emojiName) => emojiName switch
		{
			":art:" => Category.Cores,
			":ticket:" => Category.VIPs,
			":jigsaw:" => Category.Miscelânea,
			_ => Category.Cores,
		};
		public static string GetEmoji(Category category) => category switch
		{
			Category.Cores => ":art:",
			Category.Miscelânea => ":jigsaw:",
			Category.VIPs => ":ticket:",
			_ => ":art:",
		};
		public static string GetImageUrl(Category category) => category switch
		{
			Category.Cores => "https://twemoji.maxcdn.com/2/72x72/1f3a8.png",
			Category.Miscelânea => "https://twemoji.maxcdn.com/2/72x72/1f9e9.png",
			Category.VIPs => "https://twemoji.maxcdn.com/2/72x72/1f3ab.png",
			_ => "https://twemoji.maxcdn.com/2/72x72/1f3a8.png",
		};
	}
}
