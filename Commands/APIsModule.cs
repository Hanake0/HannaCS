using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using Hanna.Configuration.APIs;
using Hanna.Util;
using Hanna.Util.JsonModels;

namespace Hanna.Commands {
	public class APIsModule : BaseCommandModule {
		public Bot Hanna { private get; set; }

		public APIsModule(Bot hanna) {
			hanna.Client.ComponentInteractionCreated += this.OnButtonInteraction;
		}

		private async Task OnButtonInteraction(DiscordClient client, ComponentInteractionCreateEventArgs args)
		=> Task.Run(async () => {
			string[] action = args.Id.Split("_");

			Task interactionTask = action[0] switch {
				"randomfox" => args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
					new DiscordInteractionResponseBuilder(await this.GetRandomFoxAsync())),
				
				"httpcats" => args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
					new DiscordInteractionResponseBuilder(this.GetHttpCats())),

				"mcserver" => args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
					new DiscordInteractionResponseBuilder(
						await this.GetMCServerAsync(action[2], int.Parse(action[3])))
				),

				_ => Task.CompletedTask
			};

			await interactionTask;
		});

		#region Random Fox API
		[Command("randomfox"), Aliases("fox")]
		public async Task RandomFox(CommandContext ctx) {
			await ctx.TriggerTypingAsync();
			await ctx.RespondAsync(await this.GetRandomFoxAsync());
		}

		private async Task<DiscordMessageBuilder> GetRandomFoxAsync() {
			RandomFoxAPIResponse randomFox = await WebClient.GetRandomFoxImage();

			DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
				.WithAuthor("Random Fox", randomFox.link, RandomFoxAPIConfig.Logo)
				.WithImageUrl(randomFox.image);

			return new DiscordMessageBuilder()
				.WithEmbed(builder)
				.AddComponents(new DiscordComponent[] {
					new DiscordButtonComponent(ButtonStyle.Primary, "randomfox_refresh",
						"Refresh", false, new DiscordComponentEmoji("📥")),
					new DiscordLinkButtonComponent(randomFox.link, "API Link"),
				});
		}
		#endregion

		#region HTTP Cats
		[Command("httpcats"), Aliases("http")]
		public async Task HttpCats(CommandContext ctx, int código = 0) {
			await ctx.TriggerTypingAsync();
			DiscordMessageBuilder msg = this.GetHttpCats(código);
			await ctx.RespondAsync(msg);
		}

		private DiscordMessageBuilder GetHttpCats(int código = 0) {
			if (código == 0)
				código = HttpCatsAPI.ResponseCodes[
					new Random().Next(0, HttpCatsAPI.ResponseCodes.Length)
				];

			else if (!HttpCatsAPI.ResponseCodes.Contains(código))
				código = 404;

			DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
				.WithAuthor("HTTP Cats", HttpCatsAPI.Link, HttpCatsAPI.Logo)
				.WithImageUrl(HttpCatsAPI.Link + código);

			return new DiscordMessageBuilder()
				.WithEmbed(builder)
				.AddComponents(new DiscordComponent[] {
					new DiscordButtonComponent(ButtonStyle.Primary, "httpcats_refresh",
						"Refresh", false, new DiscordComponentEmoji("📥")),
					new DiscordLinkButtonComponent(HttpCatsAPI.Link, "API Link"),
				});
		}
		#endregion

		#region MC Server
		[Command("mcserver"), Aliases("server"),
			Description("Verifica se um servidor do minecraft está online")]
		public async Task MCServerAsync(CommandContext ctx,
			[Description("IP do servidor a checar")]string ip,
			[Description("Porta do servidor")]int porta = 0) {
			await ctx.TriggerTypingAsync();
			DiscordMessageBuilder msg = await this.GetMCServerAsync(ip, porta);
			await ctx.RespondAsync(msg);
		}

		public async Task<DiscordMessageBuilder> GetMCServerAsync(string ip, int porta = 0) {
			// Usa o WebClient do bot para acessar a API
			McStatusAPIResponse result = await WebClient.GetMcServerInfoAsync(ip, porta);

			DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
				.WithColor(result.online ? new("#07a602")/** Verde */ : new("#bd1b0f")) // Vermelho
				.WithAuthor($"Server: {ip}", null,
					$"https://mcstatus.snowdev.com.br/api/favicon/{ip}/favicon.png");

			// Caso o server esteja offline, não tenta mostrar nenhuma informação
			string embedDesc = "";
			if (result.online) embedDesc +=
				$"**Players**: {result.players_online}/{result.max_players}\n" +
				$"**Versão**: {result.version}\n" +
				$"**Ping**: {result.ping} ms";

			else embedDesc += "**Servidor offline**";

			return new DiscordMessageBuilder()
				.WithEmbed(builder.WithDescription(embedDesc))
				.AddComponents(new DiscordComponent[] {
					new DiscordButtonComponent(ButtonStyle.Primary, $"mcserver_refresh_{ip}_{porta}",
						"Refresh", false, new DiscordComponentEmoji("📥")),
					new DiscordLinkButtonComponent("https://mcstatus.snowdev.com.br", "API Link"),
				});
		}
		#endregion
	}
}
