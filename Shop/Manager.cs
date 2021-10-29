using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using Hanna;
using Hanna.Configuration;

namespace Hanna.Shop {
	public class ShopManager {
		public Bot Hanna { get; private set; }

		public readonly ShopCategory[] Categories = {
			new() {
				Name             = "Cores",
				ImageUrl         = "https://twemoji.maxcdn.com/2/72x72/1f3a8.png",
				EmojiName        = ":art:",
				SmallDescription = "Cargos bonitos para mudar a cor do seu nome",
			},
			new() {
				Name             = "VIP",
				ImageUrl         = "https://twemoji.maxcdn.com/2/72x72/1f3ab.png",
				EmojiName        = ":ticket:",
				SmallDescription = "Cargos com vantagens bonitas",
			},
			new() {
				Name             = "Misc",
				ImageUrl         = "https://twemoji.maxcdn.com/2/72x72/1f3ab.png",
				EmojiName        = ":jigsaw:",
				SmallDescription = "Coisas diversas para você comprar",
			}
		};

		public readonly TemporaryItemOption[] TemporaryOptions = {
			new() {
				Name           = "1 dia",
				AvailableTime  = TimeSpan.FromDays(1),
				BaseMultiplier = 0.2f,
				EmojiName      = ":one:",
			},
			new() {
				Name           = "3 dias",
				AvailableTime  = TimeSpan.FromDays(3),
				BaseMultiplier = 0.5f,
				EmojiName      = ":three:",
			},
			new() {
				Name           = "7 dias",
				AvailableTime  = TimeSpan.FromDays(7),
				BaseMultiplier = 1f,
				EmojiName      = ":seven:",
			}
		};

		public readonly CurrencyOption[] CurrencyOptions = {
			new() {
				Name = "Coins",
				BaseDivider = 1f,
				EmojiName = ":coin:",
			},
			new() {
				Name = "Gems",
				BaseDivider = 1000f,
				EmojiName = ":gem:",
			}
		};

		public List<ShopItem> ShopItens { get; private set; }

		public ShopManager(Bot hanna) {
			this.Hanna = hanna;

			DiscordGuild guild = this.Hanna.Client
			   .GetGuildAsync(Server.ServerId).GetAwaiter().GetResult();

			this.ShopItens = new List<ShopItem> {
				/*
				new Color(hanna, "Padrão", "Cores", new string[] { "padrao", "padrão", "default", "normal" },
					000, new DiscordColor(), 851194848325533737, "Cor padrão do server"
				),
				*/
				new Color {
					Temporary   = false, DiscColor = new DiscordColor("#ffebcd"), Name = "🌺Amêndoa",
					Aliases     = new[] {"amêndoa", "amendoa", "almond"},
					ImageLink   = "https://twemoji.maxcdn.com/2/72x72/1f33a.png",
					Emoji       = DiscordEmoji.FromName(hanna.Client, ":hibiscus:"),
					Category    = "Cores", DefValue = 2000, Role = guild.Roles[851194848325533737],
					Description = $"Poderá usar a cor <@&851194848325533737>\n no servidor, através do comando `hcor`.",
				}, // 🌺Amêndoa
				new Color {
					Temporary   = true, DiscColor = new DiscordColor("#ffa3f7"), Name = "🩰Rosa Bebê",
					Aliases     = new[] {"rosa bebe", "rosa bebê", "rosabebe", "rosabebê", "baby pink", "babypink"},
					ImageLink   = "https://images.emojiterra.com/twitter/v13.0/512px/1fa70.png",
					Emoji       = DiscordEmoji.FromName(hanna.Client, ":ballet_shoes:"),
					Category    = "Cores", DefValue = 2000, Role = guild.Roles[851194848325533737],
					Description = "Poderá usar a cor <@&851194848325533737>\n no servidor, através do comando `hcor`.",
				}, // 🩰Rosa Bebê
				new Color {
					Temporary   = true, DiscColor = new DiscordColor("#2dcc70"), Name = "🌿Menta",
					Aliases     = new[] {"menta", "mint", "mênta"},
					ImageLink   = "https://twemoji.maxcdn.com/2/72x72/1f33f.png",
					Emoji       = DiscordEmoji.FromName(hanna.Client, ":herb:"),
					Category    = "Cores", DefValue = 2000, Role = guild.Roles[851194848325533737],
					Description = "Poderá usar a cor <@&851194848325533737>\n no servidor, através do comando `hcor`.",
				}, // 🌿Menta
				new Color {
					Temporary   = true, DiscColor = new DiscordColor("#923004"), Name = "🐻Marrom",
					Aliases     = new[] {"marrom", "brown"},
					ImageLink   = "https://twemoji.maxcdn.com/2/72x72/1f43b.png",
					Emoji       = DiscordEmoji.FromName(hanna.Client, ":bear:"),
					Category    = "Cores", DefValue = 3000, Role = guild.Roles[851194848325533737],
					Description = "Poderá usar a cor <@&851194848325533737>\n no servidor, através do comando `hcor`.",
				}, // 🐻Marrom
				new Color {
					Temporary   = true, DiscColor = new DiscordColor("#4b0082"), Name = "🌙Meia Noite",
					Aliases     = new[] {"meia noite", "meianoite", "mid night", "midnight"},
					ImageLink   = "https://twemoji.maxcdn.com/2/72x72/1f319.png",
					Emoji       = DiscordEmoji.FromName(hanna.Client, ":crescent_moon:"),
					Category    = "Cores", DefValue = 3000, Role = guild.Roles[851194848325533737],
					Description = "Poderá usar a cor <@&851194848325533737>\n no servidor, através do comando `hcor`.",
				}, // 🌙Meia Noite
				new Color {
					Temporary   = true, DiscColor = new DiscordColor("#10ff00"), Name = "🐸Lima",
					Aliases     = new[] {"lima", "lime"},
					ImageLink   = "https://twemoji.maxcdn.com/2/72x72/1f438.png",
					Emoji       = DiscordEmoji.FromName(hanna.Client, ":frog:"),
					Category    = "Cores", DefValue = 3000, Role = guild.Roles[851194848325533737],
					Description = "Poderá usar a cor <@&851194848325533737>\n no servidor, através do comando `hcor`.",
				}, // 🐸Lima
				new Color {
					Temporary   = true, DiscColor = new DiscordColor("#ff0003"), Name = "🐦Vermelho",
					Aliases     = new[] {"vermelho", "red"},
					ImageLink   = "https://twemoji.maxcdn.com/2/72x72/1f426.png",
					Emoji       = DiscordEmoji.FromName(hanna.Client, ":bird:"),
					Category    = "Cores", DefValue = 5000, Role = guild.Roles[851194848325533737],
					Description = "Poderá usar a cor <@&851194848325533737>\n no servidor, através do comando `hcor`.",
				}, // 🐦Vermelho
				new Color {
					Temporary   = true, DiscColor = new DiscordColor("#feff00"), Name = "⭐Amarelo",
					Aliases     = new[] {"amarelo", "yellow"},
					ImageLink   = "https://twemoji.maxcdn.com/2/72x72/2b50.png",
					Emoji       = DiscordEmoji.FromName(hanna.Client, ":star:"),
					Category    = "Cores", DefValue = 5000, Role = guild.Roles[851194848325533737],
					Description = "Poderá usar a cor <@&851194848325533737>\n no servidor, através do comando `hcor`.",
				}, // ⭐Amarelo
				new Color {
					Temporary   = true, DiscColor = new DiscordColor("#00b3ff"), Name = "🌊Azul",
					Aliases     = new[] {"azul", "blue"},
					ImageLink   = "https://twemoji.maxcdn.com/2/72x72/1f30a.png",
					Emoji       = DiscordEmoji.FromName(hanna.Client, ":ocean:"),
					Category    = "Cores", DefValue = 5000, Role = guild.Roles[851194848325533737],
					Description = "Poderá usar a cor <@&851194848325533737>\n no servidor, através do comando `hcor`.",
				}, // 🌊Azul
				new Color {
					Temporary   = true, DiscColor = new DiscordColor("#91ff83"), Name = "☘️Verde Grama",
					Aliases     = new[] {"verde grama", "verdegrama", "grass green", "grassgreen"},
					ImageLink   = "https://twemoji.maxcdn.com/2/72x72/2618-fe0f.png",
					Emoji       = DiscordEmoji.FromName(hanna.Client, ":shamrock:"),
					Category    = "Cores", DefValue = 5000, Role = guild.Roles[851194848325533737],
					Description = "Poderá usar a cor <@&851194848325533737>\n no servidor, através do comando `hcor`.",
				}, // ☘️Verde Grama
				new Color {
					Temporary   = true, DiscColor = new DiscordColor("#af00ff"), Name = "🍇Púrpura",
					Aliases     = new[] {"púrpura", "purpura", "purple"},
					ImageLink   = "https://twemoji.maxcdn.com/2/72x72/1f347.png",
					Emoji       = DiscordEmoji.FromName(hanna.Client, ":grapes:"),
					Category    = "Cores", DefValue = 8000, Role = guild.Roles[851194848325533737],
					Description = "Poderá usar a cor <@&851194848325533737>\n no servidor, através do comando `hcor`.",
				}, // 🍇Púrpura
				new Color {
					Temporary   = true, DiscColor = new DiscordColor("#ff8a00"), Name = "🍊Laranja",
					Aliases     = new[] {"laranja", "orange"},
					ImageLink   = "https://twemoji.maxcdn.com/2/72x72/1f34a.png",
					Emoji       = DiscordEmoji.FromName(hanna.Client, ":tangerine:"),
					Category    = "Cores", DefValue = 8000, Role = guild.Roles[851194848325533737],
					Description = "Poderá usar a cor <@&851194848325533737>\n no servidor, através do comando `hcor`.",
				}, // 🍊Laranja
				new Color {
					Temporary   = true, DiscColor = new DiscordColor("#00faff"), Name = "🐳Ciano",
					Aliases     = new[] {"ciano", "cyan"},
					ImageLink   = "https://twemoji.maxcdn.com/2/72x72/1f433.png",
					Emoji       = DiscordEmoji.FromName(hanna.Client, ":whale:"),
					Category    = "Cores", DefValue = 8000, Role = guild.Roles[851194848325533737],
					Description = "Poderá usar a cor <@&851194848325533737>\n no servidor, através do comando `hcor`.",
				}, // 🐳Ciano
				new Color {
					Temporary   = true, DiscColor = new DiscordColor("#ffffff"), Name = "🦢Branco",
					Aliases     = new[] {"branco", "white"},
					ImageLink   = "https://twemoji.maxcdn.com/2/72x72/1f9a2.png",
					Emoji       = DiscordEmoji.FromName(hanna.Client, ":swan:"),
					Category    = "Cores", DefValue = 8000, Role = guild.Roles[851194848325533737],
					Description = "Poderá usar a cor <@&851194848325533737>\n no servidor, através do comando `hcor`.",
				}, // 🦢Branco
				new Color {
					Temporary   = true, DiscColor = new DiscordColor("#050000"), Name = "🕷️Void",
					Aliases     = new[] {"black", "void", "preto"},
					ImageLink   = "https://twemoji.maxcdn.com/2/72x72/1f577-fe0f.png",
					Emoji       = DiscordEmoji.FromName(hanna.Client, ":spider:"),
					Category    = "Cores", DefValue = 10000, Role = guild.Roles[851194848325533737],
					Description = "Poderá usar a cor <@&851194848325533737>\n no servidor, através do comando `hcor`.",
				}, // 🕷️Void
				new Color {
					Temporary   = true, DiscColor = new DiscordColor("#feff00"), Name = "🏅Dourado",
					Aliases     = new[] {"dourado", "gold"},
					ImageLink   = "https://twemoji.maxcdn.com/2/72x72/1f3c5.png",
					Emoji       = DiscordEmoji.FromName(hanna.Client, ":medal:"),
					Category    = "Cores", DefValue = 10000, Role = guild.Roles[851194848325533737],
					Description = "Poderá usar a cor <@&851194848325533737>\n no servidor, através do comando `hcor`.",
				}, // 🏅Dourado
			};
		}
	}
}