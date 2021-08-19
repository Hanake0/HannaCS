using DSharpPlus;
using DSharpPlus.CommandsNext;

using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using System.Threading.Tasks;

using Hanna.Util;
using System;

namespace Hanna.Commands {
	public class UtilsModule : BaseCommandModule {

		[Hidden]
		[Command("test")]
		[RequirePermissions(Permissions.Administrator)]
		public async Task TestAsync(CommandContext ctx) {
			await ctx.TriggerTypingAsync();
			throw new Exception("Deu bom!");
		}		
		
		[Command("ping")]
		[Cooldown(2, 10, CooldownBucketType.User)]
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
		
		[Command("say"), Aliases("diga")]
		public async Task Say(CommandContext ctx,
		[Description("O canal para enviar a mensagem")] DiscordChannel channel,
		[Description("O texto á dizer"), RemainingText] string text) {
			if (ctx.Member.PermissionsIn(channel).HasFlag(Permissions.SendMessages)) {
				_ = ctx.Message.DeleteAsync();
				await channel.TriggerTypingAsync();
				await channel.SendMessageAsync(text);
			} else
				await ctx.RespondAsync(EmbedUtils.ErrorBuilder
					.WithDescription("Você não tem permissão para enviar mensagens neste canal!"));
		}

		[Command("say")]
		public async Task Say(CommandContext ctx,
			[Description("O texto á dizer"), RemainingText] string text) {
			_ = ctx.Message.DeleteAsync();
			await ctx.TriggerTypingAsync();
			DiscordMessage referencedMsg = ctx.Message.ReferencedMessage;
			if (referencedMsg != null){
				await ctx.TriggerTypingAsync();
				await referencedMsg.RespondAsync(text);
			} else {
				await ctx.TriggerTypingAsync();
				await ctx.Channel.SendMessageAsync(text);
			}
		}
	}
}
