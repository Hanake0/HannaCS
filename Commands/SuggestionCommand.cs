using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Hanna.Util;

namespace Hanna.Commands {
	public class SuggestionCommand : BaseCommandModule{
		public static readonly ulong SuggestionChannelId = 892849941125398528;
		public static readonly int MinSize = 10;

		[Command("suggestion")]
		[Cooldown(1, 120, CooldownBucketType.User)]
		[Aliases("sugestao", "sugestão")]
		[Description("Usado para sugerir algo á staff")]
		public async Task RunAsync(CommandContext ctx,
			[Description("sugestão"), RemainingText] string sugestão) {
			await ctx.TriggerTypingAsync();

			// Envia um aviso caso a mensagem tenha menos que {MinSize} caracteres
			if (sugestão.Length < MinSize) {
				await ctx.RespondAsync(EmbedUtils.WarningBuilder
					.WithDescription($"Sua sugestão deve ter mais que {MinSize} caracteres."));
				return;
			}

			DiscordMessage lastMsg = await ctx.RespondAsync("Sua sugestão está sendo enviada...");
			DiscordEmbedBuilder builder = EmbedUtils.WarningBuilder
					.WithAuthor("Sugestão")
					.AddField("Autor:", $"{ctx.User.Username}#{ctx.User.Discriminator}")
					.AddField($"Texto da sugestão:", sugestão)
					.WithFooter($"Id: {ctx.User.Id}", ctx.User.AvatarUrl)
					.WithTimestamp(DateTime.Now);

			try { // Tenta enviar a mensagem e as reações para o canal de sugestoes
				DiscordChannel suggestionChannel = await ctx.Client.GetChannelAsync(SuggestionChannelId);
				DiscordMessage suggestionMsg = await suggestionChannel.SendMessageAsync("", builder.Build());
				await EmbedUtils.ReactAllAsync(suggestionMsg, new List<DiscordEmoji>() {
					DiscordEmoji.FromName(ctx.Client, ":white_check_mark:"),
					DiscordEmoji.FromName(ctx.Client, ":negative_squared_cross_mark:"),
				});

			// Se algo der errado, mostra uma mensagem de erro
			} catch (Exception e) {
				await EmbedUtils.Error(lastMsg, e);
				return;

			} // Senão, confirma o envio
			await lastMsg.ModifyAsync("", EmbedUtils.SuccesBuilder
				.WithDescription("Sugestão enviada com sucesso!")
				.Build());
		}
	}
}
