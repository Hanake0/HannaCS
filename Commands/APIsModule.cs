using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Hanna.Configuration.APIs;
using Hanna.Util;
using Hanna.Util.JsonModels;

namespace Hanna.Commands {
	public class APIModule : BaseCommandModule {
	
		[Command("httpcats"), Aliases("http")]
		public async Task HttpCats(CommandContext ctx, int código = 0) {
			await ctx.TriggerTypingAsync();
			
			if (código == 0)
				código = HttpCatsAPI.ResponseCodes[
					new Random().Next(0, HttpCatsAPI.ResponseCodes.Length)
				];

			else if (!HttpCatsAPI.ResponseCodes.Contains(código))
				código = 404;

			DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
				.WithAuthor("HTTP Cats", HttpCatsAPI.Link, HttpCatsAPI.Logo)
				.WithImageUrl(HttpCatsAPI.Link + código);

			await ctx.RespondAsync(builder);
		}
		
		[Command("mcserver"), Aliases("server"),
			Description("Verifica se um servidor do minecraft está online")]
		public async Task MCServerAsync(CommandContext ctx,
			[Description("IP do servidor a checar")]string ip,
			[Description("Porta do servidor")]int porta = 0) {
			await ctx.TriggerTypingAsync();
			
			// Usa o WebClient do bot para acessar a API
			McStatusAPIResponse result =  await WebClient.GetMcServerInfoAsync(ip, porta);

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

			await ctx.RespondAsync(builder.WithDescription(embedDesc));
		}


		[Command("randomfox"), Aliases("fox")]
		public async Task RandomFox(CommandContext ctx) {
			await ctx.TriggerTypingAsync();
			RandomFoxAPIResponse randomFox = await WebClient.GetRandomFoxImage();

			DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
				.WithAuthor("Random Fox", randomFox.link, RandomFoxAPIConfig.Logo)
				.WithImageUrl(randomFox.image);

			await ctx.RespondAsync(builder);
		}
	}
}
