using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

using Hanna.Shop;

namespace Hanna.Commands
{
	class ShopCommand : BaseCommandModule
	{
		private bool Initialized { get; set; }
		public ShopManager Manager { get; private set; }

		// Inicializa as classes
		private void Initialize(CommandContext ctx)
		{
			if (!this.Initialized)
			{
				this.Manager = new ShopManager(ctx);
			}
		}

		// ----------------------------------------------------------------> BASE <---------------------------------------------------------------------
		private async Task<bool> ReactAll(DiscordMessage msg, List<DiscordEmoji> emojis)
		{
			try
			{
				// Reage com cada emoji
				foreach (DiscordEmoji emoji in emojis)
					await msg.CreateReactionAsync(emoji);
			}
			catch { return false; }
			return true;
		}
		private async Task<DiscordEmoji> AwaitReaction(DiscordMessage msg, DiscordUser user, List<DiscordEmoji> validEmojis)
		{
			do
			{
				// Espera uma reação qualquer na mensagem
				InteractivityResult<MessageReactionAddEventArgs> reaction
					= await msg.WaitForReactionAsync(user);

				// Verifica se não ocorreu Timeout
				if (reaction.TimedOut)
					throw new TimeoutException();

				// Verifica se a reação é válida
				if(validEmojis.Any(emoji => emoji.Name == reaction.Result.Emoji.Name))
					return reaction.Result.Emoji;
			} while (true);
		}
		private async Task<DiscordMessage> SendEmbed(DiscordMessage msg, DiscordEmbed embed, string content = "")
		{
			// Tenta editar a mensagem
			DiscordMessage sentMsg;
			try
			{
				await msg.DeleteAllReactionsAsync();
				sentMsg = await msg.ModifyAsync(content, embed);
			}
			// Senão, manda uma mensagem nova
			catch
			{
				sentMsg = await msg.Channel.SendMessageAsync(embed);
			}
			return sentMsg;
		}
		private async Task<(DiscordEmoji, bool)> ReactAndAwait(DiscordMessage msg, DiscordUser user, DiscordEmbed embed, List<DiscordEmoji> emojis)
		{
			// Reage com os emojis e inicia o listener
			Task<bool> reacted = this.ReactAll(msg, emojis);
			Task<DiscordEmoji> emoji = this.AwaitReaction(msg, user, emojis);

			await Task.WhenAll(reacted, emoji);

			// Retorna os resultados
			return (emoji.Result, reacted.Result);
		}
		// ----------------------------------------------------------------> BASE <---------------------------------------------------------------------

		// -------------------------------------------------------------> NAVEGAÇÃO <-------------------------------------------------------------------
		[Command("loja")]
		[Description("Loja do servidor")]
		public async Task Run(CommandContext ctx)
		{
			// Inicializa as classes
			this.Initialize(ctx);

			// Envia a primeira mensagem que vai ser editada
			DiscordMessage msg = await ctx.RespondAsync("Carregando...");

			// Inicia o comando
			try
			{ await this.SendCategories(ctx, msg, ctx.Message.Author);

			} catch (Exception err)
			{ await this.Error(msg, err); }
		}

		private async Task<bool> SendCategories(CommandContext ctx, DiscordMessage msg, DiscordUser user)
		{
			// Cria o embed
			DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
				.WithAuthor($"Bem vindo(a) a loja {user.Username}", null, ctx.Client.CurrentUser.GetAvatarUrl(ImageFormat.Png))
				.AddField("🎨 Cores", "Cargos bonitos para mudar a cor do seu nome", true)
				.AddField("🎫 VIP", "Cargos com vantagens bonitas", true)
				.AddField("🧩 Misc", "Coisas diversas para você comprar", true)
				.WithFooter("Carteira: 💵 000 • 💎 000", "https://twemoji.maxcdn.com/2/72x72/1f4b0.png");

			// Separa os emojis
			List<DiscordEmoji> emojis = new()
			{
				DiscordEmoji.FromName(ctx.Client, ":art:"),
				DiscordEmoji.FromName(ctx.Client, ":ticket:"),
				DiscordEmoji.FromName(ctx.Client, ":jigsaw:")
			};

			// Envia o embed
			DiscordMessage sentMsg = await this.SendEmbed(msg, builder.Build());

			// Reage e espera a reação correta
			DiscordEmoji emoji;
			bool reacted;
			try { (emoji, reacted) = await this.ReactAndAwait(sentMsg, user, builder, emojis); }

			// Termina em caso de timeout
			catch { return await this.End(sentMsg); }

			// Caso tenha reagido, passa para outro handler
			Category category = this.Manager.GetCategory(emoji.GetDiscordName());
			return await this.SendItens(ctx, msg, user, category);
		}

		private async Task<bool> SendItens(CommandContext ctx, DiscordMessage msg, DiscordUser user, Category category, int index = 0)
		{
			DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
			List<DiscordEmoji> emojis = new List<DiscordEmoji>();
			ShopItem[] itens = this.Manager.GroupBy(category, 3, index * 3);
			
			// Adiciona os fields
			foreach (ShopItem item in itens)
			{
				emojis.Add(item.Emoji);
				builder.AddField(item.Name, 
					(item.Temporary ? "**Temporário**" : "**Permanente**") + "\n" +
					(item.Temporary ? $"7 Dias:\n" : "" ) + 
					$"💵 {item.GetValue(Currency.Coins, Time.SeteDias)}\n💎 {item.GetValue(Currency.Gems, Time.SeteDias)}", true);
			}

			int count = this.Manager.GetList(category).Count;

			// Adiciona ⤴️ ◀️ e ▶️ respectivamente
			emojis.Insert(0, DiscordEmoji.FromName(ctx.Client, ":arrow_heading_up:"));
			if (index > 0) emojis.Insert(0, DiscordEmoji.FromName(ctx.Client, ":arrow_backward:"));
			if (count > ((index + 1)* 3))
				emojis.Add(DiscordEmoji.FromName(ctx.Client, ":arrow_forward:"));

			// Envia o embed
			builder
				.WithColor(itens.Length!=0 ? itens.First().DiscColor : new DiscordColor("#000000"))
				.WithAuthor($"{category} {new string(' ', builder.Fields.Count * 15)} {index + 1}/{(int)Math.Ceiling((decimal)count / 3)}", null, this.Manager.GetImageUrl(ctx, category))
				.WithFooter("Carteira: 💵 000 • 💎 000", "https://twemoji.maxcdn.com/2/72x72/1f4b0.png")
				.WithTimestamp(DateTime.Now);
			DiscordMessage sentMsg = await this.SendEmbed(msg, builder.Build());

			// Reage e espera a reação correta
			DiscordEmoji emoji;
			bool reacted;
			try { (emoji, reacted) = await this.ReactAndAwait(sentMsg, user, builder, emojis); }

			// Termina em caso de timeout
			catch { return await this.End(sentMsg); }

			// Caso seja alguma opção de navegação
			string name = emoji.GetDiscordName();
			if (name == ":arrow_heading_up:") return await this.SendCategories(ctx, sentMsg, user);
			if (name == ":arrow_backward:") return await this.SendItens(ctx, sentMsg, user, category, index - 1);
			if (name == ":arrow_forward:") return await this.SendItens(ctx, sentMsg, user, category, index + 1);

			// Descobre o item escolhido
			ShopItem reactedItem = itens.First(i => i.Emoji.GetDiscordName() == name);

			// Passa para outro handler
			return await this.SendItem(ctx, sentMsg, user, reactedItem);
		}

		private async Task<bool> SendItem(CommandContext ctx, DiscordMessage msg, DiscordUser user, ShopItem item)
		{
			// Adiciona os emojis
			List<DiscordEmoji> emojis = new List<DiscordEmoji>();
			if(item.Temporary)
			{
				emojis.Add(DiscordEmoji.FromName(ctx.Client, ":one:"));
				emojis.Add(DiscordEmoji.FromName(ctx.Client, ":three:"));
				emojis.Add(DiscordEmoji.FromName(ctx.Client, ":seven:"));
			} else
				emojis.Add(DiscordEmoji.FromName(ctx.Client, ":dollar:"));
			

			// Adiciona os fields
			DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
			if(item.Temporary)
			{
				builder
					.AddField("1 dia", $"💵 {item.GetValue(Currency.Coins, Time.UmDia)}\n💎 {item.GetValue(Currency.Gems, Time.UmDia)}", true)
					.AddField("3 dias", $"💵 {item.GetValue(Currency.Coins, Time.TrêsDias)}\n💎 {item.GetValue(Currency.Gems, Time.TrêsDias)}", true)
					.AddField("7 dias", $"💵 {item.GetValue(Currency.Coins, Time.SeteDias)}\n💎 {item.GetValue(Currency.Gems, Time.SeteDias)}", true);
			} else { builder.AddField("Permanente", $"💵 {item.GetValue(Currency.Coins, Time.SeteDias)}\n💎 {item.GetValue(Currency.Gems, Time.SeteDias)}", true); }

			int count = this.Manager.GetList(item.Category).Count;
			int index = this.Manager.GetList(item.Category).FindIndex(i => i == item);

			// Adiciona ⤴️ ◀️ e ▶️ respectivamente
			emojis.Insert(0, DiscordEmoji.FromName(ctx.Client, ":arrow_heading_up:"));
			if (index > 0) emojis.Insert(0, DiscordEmoji.FromName(ctx.Client, ":arrow_backward:"));
			if (count > index)
				emojis.Add(DiscordEmoji.FromName(ctx.Client, ":arrow_forward:"));

			// Envia o embed
			builder
				.WithColor(item.DiscColor)
				.WithAuthor($"{item.Name}{new string(' ', item.Temporary ? 40 : 1)}{index + 1}/{count}", null, item.ImageLink)
				.WithDescription(item.Description)
				.WithFooter("Carteira: 💵 000 • 💎 000", "https://twemoji.maxcdn.com/2/72x72/1f4b0.png")
				.WithTimestamp(DateTime.Now);
			DiscordMessage sentMsg = await this.SendEmbed(msg, builder.Build());

			// Reage e espera a reação correta
			DiscordEmoji emoji;
			bool reacted;
			try { (emoji, reacted) = await this.ReactAndAwait(sentMsg, user, builder, emojis); }

			// Termina em caso de timeout
			catch { return await this.End(sentMsg); }

			// Caso seja alguma opção de navegação
			string name = emoji.GetDiscordName();
			if (name == ":arrow_heading_up:") return await this.SendItens(ctx, sentMsg, user, item.Category, index / 3);
			if (name == ":arrow_backward:") return await this.SendItem(ctx, sentMsg, user, this.Manager.GetList(item.Category)[index - 1]);
			if (name == ":arrow_forward:") return await this.SendItem(ctx, sentMsg, user, this.Manager.GetList(item.Category)[index + 1]);

			// Caso queira comprar o item
			return await this.Confirm(ctx, sentMsg, user, item, name switch 
			{
				":one:" => Time.UmDia,
				":three:" => Time.TrêsDias,
				":seven:" => Time.SeteDias,
				_ => Time.SeteDias,
			});
		}
		// -------------------------------------------------------------> NAVEGAÇÃO <-------------------------------------------------------------------

		// ----------------------------------------------------------> INTERATIVIDADE <-----------------------------------------------------------------
		private async Task<bool> Confirm(CommandContext ctx, DiscordMessage msg, DiscordUser user, ShopItem item, Time time = Time.SeteDias)
		{
			ulong carteira = 000;
			if (carteira < item.GetValue(Currency.Coins, time) && carteira < item.GetValue(Currency.Gems, time))
				return await this.Reject(ctx, msg, user, item, time);

			// Adiciona ⤴️
			List<DiscordEmoji> emojis = new List<DiscordEmoji>
			{
				DiscordEmoji.FromName(ctx.Client, ":arrow_heading_up:"),
			};

			// Emojis para escolher a moeda
			if (carteira > item.GetValue(Currency.Coins, time))
				emojis.Add(DiscordEmoji.FromName(ctx.Client, ":dollar:"));
			if (carteira > item.GetValue(Currency.Gems, time))
				emojis.Add(DiscordEmoji.FromName(ctx.Client, ":gem:"));

			// Envia o embed
			DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
				.WithColor(new DiscordColor("#FFFD84"))
				.WithThumbnail(item.ImageLink)
				.WithAuthor("Tem certeza que deseja comprar?", null, "https://garticbot.gg/images/icons/alert.png")
				.WithDescription($"**Nome:** {item.Name}\n" +
					$"{(item.Temporary ? $"**Validade:** { this.Manager.FormatTime(time)}" : "")}\n" +
					$"**Valor:** 💵 {item.GetValue(Currency.Coins, time)} • 💎 {item.GetValue(Currency.Gems, time)}")
				.WithFooter("Carteira: 💵 000 • 💎 000", "https://twemoji.maxcdn.com/2/72x72/1f4b0.png")
				.WithTimestamp(DateTime.Now);
			DiscordMessage sentMsg = await this.SendEmbed(msg, builder.Build());

			// Reage e espera a reação correta
			DiscordEmoji emoji;
			bool reacted;
			try { (emoji, reacted) = await this.ReactAndAwait(sentMsg, user, builder, emojis); }

			// Termina em caso de timeout
			catch { return await this.End(sentMsg); }

			// Caso tenha reação
			string name = emoji.GetDiscordName();
			int index = this.Manager.GetList(item.Category).FindIndex(i => i == item);
			if (name == ":arrow_heading_up:")
				return await this.SendItens(ctx, sentMsg, user, item.Category, index / 3);

			Currency currency = name switch { ":dollar:" => Currency.Coins, ":gem:" => Currency.Coins, _ => Currency.Coins };
			try
			{
				await item.Buy(ctx, user, currency, time);

				// TODO tirar o dinheiro da carteira
			}
			catch (Exception err)
			{ return await this.Error(msg, err, item, time); }

			return await this.Success(ctx, sentMsg, user, item, currency, time);
		}

		private async Task<bool> Success(CommandContext ctx, DiscordMessage msg, DiscordUser user, ShopItem item, Currency currency, Time time = Time.SeteDias)
		{
			// Adiciona ⤴️
			List<DiscordEmoji> emojis = new List<DiscordEmoji>
			{
				DiscordEmoji.FromName(ctx.Client, ":arrow_heading_up:"),
			};

			// Envia o embed
			DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
				.WithColor(new DiscordColor("#91FF84"))
				.WithAuthor("Concluído", null, "https://garticbot.gg/images/icons/hit.png")
				.WithThumbnail(item.ImageLink)
				.WithDescription($"o item {item.Name} {(item.Temporary ? $"({this.Manager.FormatTime(time)})" : "")}\n" +
					"foi comprado com sucesso!\n\n" +
					$"**Item:** {item.Name} {(item.Temporary ? $"({this.Manager.FormatTime(time)})" : "")}\n" +
					$"**Valor:** {(currency == Currency.Coins ? "💵" : "💎")} {item.GetValue(currency, time)}")
				.WithFooter("Carteira: 💵 000 • 💎 000", "https://twemoji.maxcdn.com/2/72x72/1f4b0.png")
				.WithTimestamp(DateTime.Now);
			DiscordMessage sentMsg = await this.SendEmbed(msg, builder.Build());

			// Reage e espera a reação correta
			DiscordEmoji emoji;
			bool reacted;
			try { (emoji, reacted) = await this.ReactAndAwait(sentMsg, user, builder, emojis); }

			// Termina em caso de timeout
			catch { return await this.End(sentMsg); }

			// Caso tenha reação
			string name = emoji.GetDiscordName();
			int index = this.Manager.GetList(item.Category).FindIndex(i => i == item);
			return await this.SendItens(ctx, sentMsg, user, item.Category, index / 3);
		}

		private async Task<bool> Reject(CommandContext ctx, DiscordMessage msg, DiscordUser user, ShopItem item, Time time = Time.SeteDias)
		{
			// Adiciona ⤴️
			List<DiscordEmoji> emojis = new List<DiscordEmoji>
			{
				DiscordEmoji.FromName(ctx.Client, ":arrow_heading_up:"),
			};

			// Envia o embed
			DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
				.WithColor(new DiscordColor("#FF8484"))
				.WithAuthor("Cancelado", null, "https://garticbot.gg/images/icons/error.png")
				.WithThumbnail(item.ImageLink)
				.WithDescription("Infelizmente você não tem coins,\nnem gems suficientes para" +
					$"comprar {item.Name} {(item.Temporary ? $"({this.Manager.FormatTime(time)})" : "")}\n\n" +
					$"**Item:** {item.Name} {(item.Temporary ? $"({this.Manager.FormatTime(time)})" : "")}\n" +
					$"**Valor:** 💵 {item.GetValue(Currency.Coins, time)} • 💎 {item.GetValue(Currency.Gems, time)}\n" +
					$"**Faltam:** ..")
				.WithFooter("Carteira: 💵 000 • 💎 000", "https://twemoji.maxcdn.com/2/72x72/1f4b0.png")
				.WithTimestamp(DateTime.Now);
			DiscordMessage sentMsg = await this.SendEmbed(msg, builder.Build());

			// Reage e espera a reação correta
			DiscordEmoji emoji;
			bool reacted;
			try { (emoji, reacted) = await this.ReactAndAwait(sentMsg, user, builder, emojis); }

			// Termina em caso de timeout
			catch { return await this.End(sentMsg); }

			// Caso tenha reação
			string name = emoji.GetDiscordName();
			int index = this.Manager.GetList(item.Category).FindIndex(i => i == item);
			return await this.SendItens(ctx, sentMsg, user, item.Category, index / 3);
		}

		private async Task<bool> Error(DiscordMessage msg, Exception err, ShopItem item, Time time = Time.SeteDias)
		{
			DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
				.WithColor(new DiscordColor("#FF8484"))
				.WithThumbnail(item.ImageLink)
				.WithAuthor("Cancelado", null, "https://garticbot.gg/images/icons/error.png")
				.WithDescription($"Infelizmente algo deu errado durante a compra desse item\n\n" +
				$"**Item:** {item.Name} {(item.Temporary ? $"({this.Manager.FormatTime(time)})" : "")}\n" +
				$"**Erro:** {err.TargetSite} => {err.GetType()}: `{err.Message}`\n\n```{err.StackTrace}```");

			try { await this.SendEmbed(msg, builder.Build()); return true; }
			catch { return false; }
		}
		private async Task<bool> Error(DiscordMessage msg, Exception err)
		{
			DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
				.WithColor(new DiscordColor("#FF8484"))
				.WithAuthor("Cancelado", null, "https://garticbot.gg/images/icons/error.png")
				.WithDescription($"Infelizmente algo deu errado durante a execução do comando\n\n" +
				$"**Erro:** {err.TargetSite} => {err.GetType()}: `{err.Message}`\n\n```{err.StackTrace}```");

			try { await this.SendEmbed(msg, builder.Build()); return true; }
			catch { return false; }
		}

		private async Task<bool> End(DiscordMessage msg)
		{
			DiscordEmbedBuilder builder = new DiscordEmbedBuilder(msg.Embeds.First())
				.WithFooter("Tempo esgotado", "https://garticbot.gg/images/icons/time.png");
			
			try{ await this.SendEmbed(msg, builder.Build()); return true; }
			catch { return false; }
		}
		// ----------------------------------------------------------> INTERATIVIDADE <-----------------------------------------------------------------
	}
}
