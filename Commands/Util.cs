using DSharpPlus;
using DSharpPlus.CommandsNext;

using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using System.Threading.Tasks;

using Hanna.Util;
using Hanna.Util.JsonModels;

namespace Hanna.Commands {
	public class Util : BaseCommandModule {
		[Command("ping")]
		[Description("Verifica o ping do bot")]
		public async Task PingAsync(CommandContext ctx) {
			await ctx.TriggerTypingAsync();

			int ping = ctx.Client.Ping;
			DiscordColor color = ping < 300 ?
					ping < 150 ? new("#07a602")/** Verde */ : new("#d98e04") // Laranja
				: new("#bd1b0f"); // Vermelho

			DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
				.WithColor(color)
				.WithAuthor("Ping:", null, "https://raw.githubusercontent.com/twitter/twemoji/master/assets/72x72/1f6f0.png")
				.AddField(":satellite: | WS/Discord", $"{ping} ms");

			await ctx.RespondAsync(builder);
		}

		[Command("Online")]
		[Description("Verifica se um servidor do minecraft está online")]
		public async Task OnlineAsync(CommandContext ctx,
			[Description("IP do servidor a checar")]string serverIp,
			[Description("Porta do servidor")]int serverPort = 0) {

			// Usa o WebClient do bot para acessar a API
			McStatusAPIResponse result =  await WebClient.GetMcServerInfoAsync(serverIp, serverPort);

			DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
				.WithColor(result.online ? new("#07a602")/** Verde */ : new("#bd1b0f")) // Vermelho
				.WithAuthor($"Server: {serverIp}", null, 
					$"https://mcstatus.snowdev.com.br/api/favicon/{serverIp}/favicon.png");

			// Caso o server esteja offline, não tenta mostrar nenhuma informação
			string embedDesc = "";
			if (result.online) embedDesc +=
					$"**Players**: {result.players_online}/{result.max_players}\n" +
					$"**Versão**: {result.version}\n" +
					$"**Ping**: {result.ping} ms";

			else embedDesc += "**Servidor offline**";

			await ctx.RespondAsync(builder.WithDescription(embedDesc));
		}

		[Command("say"), Aliases("diga")]
		public async Task Say(CommandContext ctx,
		[Description("O canal para enviar a mensagem")] DiscordChannel channel,
		[Description("O texto á dizer"), RemainingText] string text) {
			if (ctx.Member.PermissionsIn(channel).HasFlag(Permissions.SendMessages)) {
				_ = ctx.Message.DeleteAsync();
				await channel.TriggerTypingAsync();
				await channel.SendMessageAsync(text);
			} else
				await ctx.RespondAsync("Você não tem permissão para enviar mensagens neste canal");
		}

		[Command("say")]
		public async Task Say(CommandContext ctx,
			[Description("O texto á dizer"), RemainingText] string text) {
			_ = ctx.Message.DeleteAsync();
			await ctx.TriggerTypingAsync();
			DiscordMessage referencedMsg = ctx.Message.ReferencedMessage;
			if (referencedMsg != null)
				await referencedMsg.RespondAsync(text);
			else
				await ctx.Channel.SendMessageAsync(text);
		}
	}
}
