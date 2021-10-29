using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

using Hanna.Commands;
using Hanna.Configuration;
using Hanna.Cosmos;
using Hanna.Cosmos.Entitys;
using Hanna.Util;

using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hanna {
	public class Bot {
		// Client e extensões
		public DiscordClient          Client        { get; private set; }
		public InteractivityExtension Interactivity { get; private set; }
		public CommandsNextExtension  Commands      { get; private set; }

		// Cosmos DB
		public static UsersContext UsersContext { get; private set; }

		public async Task RunAsync() {
			// Inicializa o DB
			//UsersContext = new UsersContext();
			//await UsersContext.Database.EnsureCreatedAsync();

			// Define o User-Agent do WebClient
			Util.WebClient.Client.DefaultRequestHeaders
			   .Add("User-Agent", "UnReal - Hanna Bot");

			// Configurações do client
			this.Client = new DiscordClient(new DiscordConfiguration {
				Token              = Environment.GetEnvironmentVariable("DISCORD_TOKEN"),
				TokenType          = TokenType.Bot,
				AutoReconnect      = true,
				MinimumLogLevel    = LogLevel.Information,
				LogTimestampFormat = "MMM dd yyyy - hh:mm:ss tt",
				Intents            = DiscordIntents.All,
			});

			// Depedency Injection
			ServiceProvider services = new ServiceCollection()
			   .AddSingleton(this)
			   .BuildServiceProvider();

			// Configurações do command handler
			this.Commands = this.Client.UseCommandsNext(new CommandsNextConfiguration {
				StringPrefixes       = new string[] {Environment.GetEnvironmentVariable("PREFIX")},
				EnableMentionPrefix  = true,
				EnableDms            = true,
				CaseSensitive        = false,
				IgnoreExtraArguments = true,
				Services             = services,
			});

			this.Interactivity = this.Client.UseInteractivity(new InteractivityConfiguration {
				Timeout = TimeSpan.FromMinutes(1),
			});

			// Registra os eventos
			this.Client.Ready            += this.OnReady;
			this.Client.GuildMemberAdded += this.onGuildMemberAdd;
			//this.Client.MessageCreated += this.OnMessageCreatedAsync;
			this.Commands.CommandErrored += this.OnCommandErroredAsync;

			// Registra as classes de comandos
			this.Commands.RegisterCommands(Assembly.GetExecutingAssembly());
			//_ = new APIsModule(this);

			//this.Commands.RegisterCommands<Commands.ShopCommand>();
			//this.Commands.RegisterCommands<Commands.SuggestionCommand>();
			//this.Commands.RegisterCommands<Commands.UtilsModule>();

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

		private async Task onGuildMemberAdd(DiscordClient client, GuildMemberAddEventArgs args)
			=> await Task.Run(async () => {
				if (Server.ServerId != args.Guild.Id) return;

				DiscordChannel channel = args.Guild.GetChannel(Server.PrimaryChannelId);
				await channel.TriggerTypingAsync();
				foreach (ulong roleId in Server.BaseRoleIds)
					await args.Member
					   .GrantRoleAsync(args.Guild.Roles[roleId], "Member role");

				DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
				   .WithAuthor(args.Member.DisplayName, null, args.Member.AvatarUrl)
				   .WithDescription($"{args.Member.Mention} acaba de entrar, gostosuras ou travessuras?!")
				   .WithImageUrl(
						"https://cdn.discordapp.com/attachments/828500887218946058/895436069825298442/anime-halloween.gif")
				   .WithFooter($"Temos {args.Guild.MemberCount} cadáveres!")
				   .WithTimestamp(DateTime.Now);

				await channel.SendMessageAsync(builder);
			});

		private async Task OnCommandErroredAsync(CommandsNextExtension _, CommandErrorEventArgs args)
			=> await Task.Run(async () => {
				if (args.Exception is CommandNotFoundException or ArgumentException) return;
				
				if (args.Context.Channel.Id == Server.PrimaryChannelId) return;
				await args.Context.TriggerTypingAsync();

				// Caso o motivo do erro seja algum atributo
				if (args.Exception is ChecksFailedException checksException) {
					IReadOnlyList<CheckBaseAttribute> failedChecks = checksException.FailedChecks;

					string errorMsg = "Você não pode executar esse comando pois:\n";
					foreach (CheckBaseAttribute failedCheck in failedChecks)
						switch (failedCheck) {
							// Caso algum dos atributos seja cooldown, retorna uma mensagem de aviso imediatamente
							case CooldownAttribute cdAttribute:
								TimeSpan cooldown = cdAttribute.GetRemainingCooldown(args.Context);

								await args.Context.RespondAsync(EmbedUtils.WarningBuilder
								   .WithDescription(String.Format("Por favor **aguarde {0:F0}m {1:F2}s** ",
										cooldown.TotalMinutes, cooldown.TotalSeconds % 60) + "antes de usar esse comando novamente"));
								return;
							case RequirePermissionsAttribute reqPerm:
								errorMsg += $"\tprecisa das permissoes: {reqPerm.Permissions.ToPermissionString()}\n";
								break;
						}

					await args.Context.RespondAsync(EmbedUtils.ErrorBuilder
					   .WithDescription(errorMsg));

					// Caso seja algum outro erro desconhecido
				} else await EmbedUtils.Error(args.Context.Message, args.Exception);
			});

		/**
		private Task OnMessageCreatedAsync(DiscordClient client, MessageCreateEventArgs args) {
			if (args.Author.IsBot) return Task.CompletedTask;

			Task.Run(async () => {
				using UsersContext usersCtx = new(); WCUser user;
				try {
					user = await usersCtx.Users
						.FindAsync(args.Author.Id);
					usersCtx.Attach(user); // Verificar mudanças

				} catch (CosmosException) {
					user = new WCUser(args.Author.Id, args.Message);
					usersCtx.Add(user); // Add ao DB
				};

				// Carrega a última msg da pessoa salva no db
				usersCtx.Entry(user).Reference(u => u.LastMessage).Load();

				// Se o tempo entre a ultima mensagem e a mensagem atual for maior que 1seg adiciona moedas
				if ((args.Message.Timestamp - user.LastMessage.Timestamp).TotalSeconds >= 1) {

					// Carrega os dados da carteira
					usersCtx.Entry(user).Reference(u => u.Wallet).Load();

					user.Wallet.Coins++;
				}

				await usersCtx.SaveChangesAsync();
			});

			return Task.CompletedTask;
		} */
	}
}