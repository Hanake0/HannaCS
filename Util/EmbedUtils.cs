using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace Hanna.Util
{
	public static class EmbedUtils
	{
		// <======================================================== Navegação por Emoji ========================================================>
		public static async Task<bool> ReactAll(DiscordMessage msg, List<DiscordEmoji> emojis)
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
		public static async Task<DiscordEmoji> AwaitReaction(DiscordMessage msg, DiscordUser user, List<DiscordEmoji> validEmojis)
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
				if (validEmojis.Any(emoji => emoji.Name == reaction.Result.Emoji.Name))
					return reaction.Result.Emoji;
			} while (true);
		}
		public static async Task<DiscordMessage> SendEmbed(DiscordMessage msg, DiscordEmbed embed, string content = "")
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
		public static async Task<(DiscordEmoji, bool)> ReactAndAwait(DiscordMessage msg, DiscordUser user, List<DiscordEmoji> emojis)
		{
			// Reage com os emojis e inicia o listener
			Task<bool> reacted = ReactAll(msg, emojis);
			Task<DiscordEmoji> emoji = AwaitReaction(msg, user, emojis);

			await Task.WhenAll(reacted, emoji);

			// Retorna os resultados
			return (emoji.Result, reacted.Result);
		}
		// <======================================================== Navegação por Emoji ========================================================>
	}
}
