using System;
using DSharpPlus;

using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using DSharpPlus.Interactivity.Extensions;

using Hanna.Commands;
using DSharpPlus.CommandsNext.Exceptions;
using System.Collections.Generic;
using DSharpPlus.CommandsNext.Attributes;
using Hanna.Util;
using System.Linq;

namespace Hanna {
	public class Bot {
		// Client e extensões
		public DiscordClient Client { get; private set; }
		public InteractivityExtension Interactivity { get; private set; }
		public CommandsNextExtension Commands { get; private set; }

		public async Task RunAsync() {

			// Define o User-Agent do WebClient
			Util.WebClient.Client.DefaultRequestHeaders
				.Add("User-Agent", "UnReal - Hanna Bot");

			// Configurações do client
			this.Client = new DiscordClient(new DiscordConfiguration {
				Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN"),
				TokenType = TokenType.Bot,
				AutoReconnect = true,
				MinimumLogLevel = LogLevel.Information,
				LogTimestampFormat = "MMM dd yyyy - hh:mm:ss tt",
			});

			// Configurações do command handler
			this.Commands = this.Client.UseCommandsNext(new CommandsNextConfiguration {
				StringPrefixes = new string[] { Environment.GetEnvironmentVariable("PREFIX") },
				EnableMentionPrefix = true,
				EnableDms = true,
				CaseSensitive = false,
				IgnoreExtraArguments = true,
			});

			this.Interactivity = this.Client.UseInteractivity(new InteractivityConfiguration {
				Timeout = TimeSpan.FromMinutes(1),
			});

			// Registra os eventos
			this.Client.Ready += this.OnReady;
			this.Commands.CommandErrored += this.OnCommandErroredAsync;


			// Registra as classes de comandos
			this.Commands.RegisterCommands<Commands.UtilsModule>();
			this.Commands.RegisterCommands<Commands.ShopCommand>();
			this.Commands.RegisterCommands<Commands.SuggestionCommand>();
			this.Commands.RegisterCommands<Commands.APIModule>();

			await this.Client.ConnectAsync();
			await Task.Delay(-1);
		}

		private Task OnReady(DiscordClient client, ReadyEventArgs args) {
			DiscordUser user = this.Client.CurrentUser;

			this.Client.Logger
				.LogInformation(LoggerEvents.Startup, 
					$"Logado como {user.Username}#{user.Discriminator}" +
					$"({user.Id}) com sucesso!");

			return Task.CompletedTask;
		}

		private async Task OnCommandErroredAsync(CommandsNextExtension _, CommandErrorEventArgs args) {

			// TODO: configurar canais sem logging
			if(args.Context.Channel.Id == 827661920974274591) return;
			await args.Context.TriggerTypingAsync();

			// Caso o motivo do erro seja algum atributo
			if(args.Exception is ChecksFailedException checksException) {
				IReadOnlyList<CheckBaseAttribute> failedChecks = checksException.FailedChecks;

				string errorMsg = "Você não pode executar esse comando pois:\n";
				foreach (CheckBaseAttribute failedCheck in failedChecks) {

					// Caso algum dos atributos seja cooldown, retorna uma mensagem de aviso imediatamente
					if(failedCheck is CooldownAttribute cdAttribute) {
						TimeSpan cooldown = cdAttribute.GetRemainingCooldown(args.Context);

						await args.Context.RespondAsync(EmbedUtils.WarningBuilder
							.WithDescription(String.Format("Por favor **aguarde {0:F0}m {1:m\\.ff}s** ",
								cooldown.TotalMinutes, cooldown) + "antes de usar esse comando novamente"));
						return;
					}

					if (failedCheck is RequirePermissionsAttribute reqPerm)
						errorMsg += $"\tprecisa das permissoes: {reqPerm.Permissions.ToPermissionString()}\n";
				}

				await args.Context.RespondAsync(EmbedUtils.ErrorBuilder
					.WithDescription(errorMsg));
				return;

			// Caso seja algum outro erro desconhecido
			} else
				await EmbedUtils.Error(args.Context.Message, args.Exception);
		}
	}
}
