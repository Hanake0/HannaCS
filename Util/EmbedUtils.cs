using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

using Hanna.Configuration;

namespace Hanna.Util {
	public static class EmbedUtils {

		#region Pre-sets para embeds
		/// <summary>
		/// Builder com cor e autor configurados para "Cancelado" em vermelho
		/// </summary>
		public static DiscordEmbedBuilder ErrorBuilder { get => new DiscordEmbedBuilder()
			.WithColor(Embeds.Colors.DefaultRed)
			.WithAuthor("Cancelado", null, Embeds.Images.RedCross);
		}
		/// <summary>
		/// Builder com cor e autor configurados para "Sucesso" em verde
		/// </summary>
		public static DiscordEmbedBuilder SuccesBuilder { get => new DiscordEmbedBuilder()
			.WithColor(Embeds.Colors.DefaultGreen)
			.WithAuthor("Sucesso", null, Embeds.Images.GreenCheck);
		}
		/// <summary>
		/// Builder com cor e autor configurados para "Aviso" em laranja
		/// </summary>
		public static DiscordEmbedBuilder WarningBuilder { get => new DiscordEmbedBuilder()
			.WithColor(Embeds.Colors.DefaultOrange)
			.WithAuthor("Aviso", null, Embeds.Images.OrangeExclamation);
		}
		#endregion

		#region Funções para interatividade com reações
		/// <summary>
		/// Reage cada emoji de uma lista na ordem
		/// </summary>
		/// <param name="msg"> A mensagem que recebera as reações </param>
		/// <param name="emojis"> A lista de emojis a reagir</param>
		/// <returns>Task -> Todas as reações ocorreram com sucesso, ou não</returns>
		public static async Task<bool> ReactAllAsync(DiscordMessage msg, List<DiscordEmoji> emojis) {
			try {
				// Reage com cada emoji
				foreach (DiscordEmoji emoji in emojis)
					await msg.CreateReactionAsync(emoji);
			} catch { return false; }
			return true;
		}
		/// <summary>
		/// Espera qualquer reação especificada em uma lista, de um usuário
		/// </summary>
		/// <param name="msg">A mensagem que possivelmente vai receber a reação</param>
		/// <param name="user">O usuário que vai reagir</param>
		/// <param name="validEmojis">A lista de reações válidas</param>
		/// <returns>Task -> Emoji que foi reagido ou TimeoutException</returns>
		public static async Task<DiscordEmoji> AwaitReactionAsync(DiscordMessage msg, DiscordUser user, List<DiscordEmoji> validEmojis) {
			do {
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
		/// <summary>
		/// Edita a mensagem original se conseguir, caso contrário envia uma nova
		/// </summary>
		/// <param name="msg">A mensagem a ser editada</param>
		/// <param name="embed">O Embed para enviar na nova mensagem</param>
		/// <param name="content">O conteúdo para enviar na nova mensagem</param>
		/// <returns>Task -> Nova mensagem, enviada</returns>
		public static async Task<DiscordMessage> SendEmbedAsync(DiscordMessage msg, DiscordEmbed embed, string content = "") {
			// Tenta editar a mensagem
			DiscordMessage sentMsg;
			try {
				await msg.DeleteAllReactionsAsync();
				sentMsg = await msg.ModifyAsync(content, embed);
			}

			// Senão, manda uma mensagem nova
			catch {
				sentMsg = await msg.Channel.SendMessageAsync(embed);
			}
			return sentMsg;
		}
		/// <summary>
		/// Reage uma lista de emojis em ordem e espera que um usuário clique em uma dessas reações
		/// Junção de EmbedUtils.ReactAllAsync e EmbedUtils.AwaitReactionAsync.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="user"></param>
		/// <param name="emojis"></param>
		/// <returns></returns>
		public static async Task<(DiscordEmoji, bool)> ReactAndAwaitAsync(DiscordMessage msg, DiscordUser user, List<DiscordEmoji> emojis) {
			// Reage com os emojis e inicia o listener
			Task<bool> reacted = ReactAllAsync(msg, emojis);
			Task<DiscordEmoji> emoji = AwaitReactionAsync(msg, user, emojis);

			await Task.WhenAll(reacted, emoji);

			// Retorna os resultados
			return (emoji.Result, reacted.Result);
		}
		#endregion

		#region Funções para respostas padroes
		/// <summary>
		/// Envia um embed de erro através de EmbedUtil.SendEmbedAsync
		/// </summary>
		/// <param name="msg"> Mensagem a responder, ou para ser editada </param>
		/// <param name="err"> Erro a ser reportado</param>
		/// <returns>Task -> Mensagem enviada com sucesso, ou não</returns>
		public static async Task<bool> Error(DiscordMessage msg, Exception err) {
			DiscordEmbedBuilder builder = ErrorBuilder
				.WithDescription($"Infelizmente algo deu errado durante a execução do comando\n\n" +
				$"**Erro:** {err.TargetSite} => {err.GetType()}: `{err.Message}`\n\n```{err.StackTrace}```");

			try { await EmbedUtils.SendEmbedAsync(msg, builder.Build()); return true; }
			catch { return false; }
		}
		#endregion
	}
}
