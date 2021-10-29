using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Enums;

using Hanna.Configuration;
using Hanna.Shop;
using Hanna.Util;

namespace Hanna.Commands {
	public class ShopCommand : BaseCommandModule {
		public ShopManager Manager { get; private set; }
		public Bot         Hanna   { get; private set; }

		public ShopCommand(Bot hanna) {
			this.Hanna                                    =  hanna;
			this.Manager                                  =  new ShopManager(hanna);
			this.Hanna.Client.ComponentInteractionCreated += this.OnButtonInteraction;
		}

		private async Task OnButtonInteraction(DiscordClient client, ComponentInteractionCreateEventArgs args)
			=> await Task.Run(async () => {
				// Separa o path por "_"
				string[] actions = args.Id.Split("_");
				if (actions[0] != "loja") return Task.CompletedTask;

				// Busca a mensagem referenciado no canal ja que ela pode nao estar no cache
				DiscordMessage reference = await args.Channel.GetMessageAsync(args.Message.Reference.Message.Id);

				if (reference is null || (reference.Author.Id != args.User.Id))
					return args.Channel.SendMessageAsync(this.GetWrongAuthorMessage(args.User));

				return actions[1] switch {
					// Volta para a lista de categorias
					"categories" => Task.Run(async () => {
						DiscordMessageBuilder iResponse;

						try {
							iResponse = await this.GetCategories(args.User);
						} catch (Exception e) {
							iResponse = ShopCommand.ShowError(e);
						}
						
						await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
							new DiscordInteractionResponseBuilder(iResponse));
					}),

					// Mostra a lista de itens de uma categoria
					"itens" => Task.Run(async () => {
						DiscordMessageBuilder iResponse;

						try {
							if (actions[2] == "picker") iResponse = await this.GetItens(args.User, args.Values[0]);
							else iResponse                        = await this.GetItens(args.User, actions[2], actions[3]);
						} catch (Exception e) {
							iResponse = ShopCommand.ShowError(e);
						}
						
						await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
							new DiscordInteractionResponseBuilder(iResponse));
					}),

					// Mostra as informaçoes de um item especifico
					"item" => Task.Run(async () => {
						DiscordMessageBuilder iResponse;

						try {
							iResponse = await this.GetItem(args.User, actions[2], actions[3]);
						} catch (Exception e) {
							iResponse = ShopCommand.ShowError(e);
						}
						
						await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
							new DiscordInteractionResponseBuilder(iResponse));
					}),

					// Confirma a compra de um item
					"confirm" => Task.Run(async () => {
						DiscordMessageBuilder iResponse;

						try {
							iResponse = await this.GetConfirm(args.User, actions[2], actions[3], actions[4]);
						} catch (Exception e) {
							iResponse = ShopCommand.ShowError(e);
						}
						
						await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
							new DiscordInteractionResponseBuilder(iResponse));
					}),

					// Compra o item e mostra uma mensagem de erro ou sucesso
					"buy" => Task.Run(async () => {
						//await args.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, null);
						
						DiscordMessageBuilder iResponse;
						ShopItem              item = this.Manager.ShopItens.Single(i => i.Name == actions[2]);

						try {
							await item.Buy(args.User, actions[3], actions[4]);
							iResponse = await this.Success(args.User, actions[2], actions[3], actions[4]);
						} catch (Exception e) {
							iResponse = await ShopCommand.ShowError(e, args.User, item, actions[4]);
						}
						
						await args.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
							new DiscordInteractionResponseBuilder(iResponse));
					}),
					_ => Task.CompletedTask,
				};
			});

		private DiscordMessageBuilder GetWrongAuthorMessage(DiscordUser user) {
			DiscordEmbedBuilder builder = EmbedUtils.ErrorBuilder
			   .WithDescription("Você não pode usar a loja de outra pessoa!");

			return new DiscordMessageBuilder()
			   .WithContent(user.Mention)
			   .WithEmbed(builder);
		}

		// -------------------------------------------------------------> NAVEGAÇÃO <-------------------------------------------------------------------
		[Command("loja"), Aliases("shop"),
		 Description("Loja do servidor")]
		public async Task Loja(CommandContext ctx) {
			await ctx.TriggerTypingAsync();

			// Envia a primeira mensagem que vai ser editada
			await ctx.RespondAsync(await this.GetCategories(ctx.User));
		}

		private async Task<DiscordMessageBuilder> GetCategories(DiscordUser user) {
			// Cria o embed
			DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
			   .WithAuthor($"Bem vindo(a) a loja {user.Username}",
					null, this.Hanna.Client.CurrentUser.GetAvatarUrl(ImageFormat.Png))
			   .WithFooter("Carteira: 💵 000 • 💎 000",
					"https://twemoji.maxcdn.com/2/72x72/1f4b0.png");

			// Adiciona cada categoria como um Field no embed
			foreach (ShopCategory category in this.Manager.Categories)
				builder.AddField(category.DisplayName, category.SmallDescription, true);


			// Create the options for the user to pick
			List<DiscordSelectComponentOption> options = this.Manager.Categories
			   .Select(category => new DiscordSelectComponentOption(
					label: category.Name,
					value: category.Name,
					description: category.SmallDescription,
					isDefault: false,
					emoji: new DiscordComponentEmoji(
						DiscordEmoji.FromName(this.Hanna.Client, category.EmojiName)))
				).ToList();

			// Make the dropdown
			DiscordSelectComponent dropdown = new("loja_itens_picker", null, options);

			return new DiscordMessageBuilder()
			   .WithEmbed(builder)
			   .AddComponents(dropdown);
		}

		private async Task<DiscordMessageBuilder> GetItens(
			DiscordUser user, string categoryName, string indexString = "0") {
			int ipe = 3; // Itens Per Embed

			ShopCategory category = this.Manager.Categories
			   .Single(c => c.Name == categoryName);

			// Pula ate o index e pega 6 itens da loja com essa categoria
			int index = int.Parse(indexString);
			ShopItem[] itens = this.Manager.ShopItens
			   .Where(i => i.Category == categoryName)
			   .Skip(index * ipe)
			   .Take(ipe)
			   .ToArray();


			int count = this.Manager.ShopItens.Count(i => i.Category == categoryName);

			// Adiciona️◀ ⤴ ▶
			List<DiscordButtonComponent> navButtons = new() {
				new DiscordButtonComponent(ButtonStyle.Primary, $"loja_itens_{categoryName}_{index - 1}",
					null, index == 0, new DiscordComponentEmoji("◀")),
				new DiscordButtonComponent(ButtonStyle.Primary, "loja_categories",
					null, false, new DiscordComponentEmoji("↪")),
				new DiscordButtonComponent(ButtonStyle.Primary, $"loja_itens_{categoryName}_{index + 1}",
					null, ((index * ipe) + ipe) >= count, new DiscordComponentEmoji("▶"))
			};

			// Adiciona os fields e botoes
			DiscordEmbedBuilder          builder        = new();
			List<DiscordButtonComponent> optionsButtons = new();
			foreach (ShopItem item in itens) {
				// Adiciona o botão
				optionsButtons.Add(new DiscordButtonComponent(ButtonStyle.Secondary,
					$"loja_item_{item.Name}_none", null, false,
					new DiscordComponentEmoji(item.Emoji)));

				// Adiciona o field
				builder.AddField(item.Name,
					(item.Temporary ? "**Temporário**" : "**Permanente**") + "\n" +
					(item.Temporary ? $"7 Dias:\n" : "") +
					$"💵 {item.GetValue(Currency.Coins, Time.SeteDias)}\n" +
					$"💎 {item.GetValue(Currency.Gems,  Time.SeteDias)}", true);
			}

			// Envia o embed
			builder
			   .WithColor(itens.Length != 0 ? itens.First().DiscColor : new DiscordColor("#000000"))
			   .WithAuthor(
					$"{categoryName} {new string(' ', builder.Fields.Count * 15)}" +
					$"{index + 1}/{(int)Math.Ceiling((decimal)count / ipe)}"
				  , null, category.ImageUrl)
			   .WithFooter("Carteira: 💵 000 • 💎 000", "https://twemoji.maxcdn.com/2/72x72/1f4b0.png")
			   .WithTimestamp(DateTime.Now);

			DiscordMessageBuilder msgBuilder = new DiscordMessageBuilder().WithEmbed(builder);
			if (optionsButtons.Count > 0)
				msgBuilder.AddComponents(optionsButtons);

			return msgBuilder.AddComponents(navButtons);
		}

		private async Task<DiscordMessageBuilder> GetItem(
			DiscordUser user, string itemName, string tOptionName = "none") {
			ShopItem item  = this.Manager.ShopItens.Single(c => c.Name == itemName);
			int      index = this.Manager.ShopItens.IndexOf(item);
			int      count = this.Manager.ShopItens.Count(i => i.Category == item.Category);
			ShopItem prevItem = this.Manager.ShopItens.SingleOrDefault(i =>
				this.Manager.ShopItens.IndexOf(i) == (index - 1)) ?? item;
			ShopItem nextItem = this.Manager.ShopItens.SingleOrDefault(i =>
				this.Manager.ShopItens.IndexOf(i) == (index + 1)) ?? item;

			// Adiciona️◀ ⤴ ▶
			List<DiscordButtonComponent> navButtons = new() {
				new DiscordButtonComponent(ButtonStyle.Primary, $"loja_item_{prevItem.Name}_{tOptionName}",
					null, index == 0, new DiscordComponentEmoji("◀")),
				new DiscordButtonComponent(ButtonStyle.Primary,
					tOptionName == "none"
						? $"loja_itens_{item.Category}_{Math.Floor(index / 3f)}"
						: $"loja_item_{itemName}_none", null, false, new DiscordComponentEmoji("↪")),
				new DiscordButtonComponent(ButtonStyle.Primary, $"loja_item_{nextItem.Name}_{tOptionName}",
					null, (index + 1) >= count, new DiscordComponentEmoji("▶"))
			};

			// Adiciona os fields e botoes
			DiscordEmbedBuilder          builder        = new();
			List<DiscordButtonComponent> optionsButtons = new();

			// Caso seja permanente
			bool hasTOpt = tOptionName != "none";
			if (!item.Temporary || hasTOpt) {
				string fieldCurrencyValues = "";
				TemporaryItemOption tOption = this.Manager.TemporaryOptions
				   .SingleOrDefault(t => t.Name == tOptionName);

				// Adiciona cada moeda disponível
				foreach (CurrencyOption c in this.Manager.CurrencyOptions) {
					optionsButtons.Add(new DiscordButtonComponent(ButtonStyle.Success,
						$"loja_confirm_{itemName}_{c.Name}_{tOptionName}", "Comprar",
						false, new DiscordComponentEmoji(
							DiscordEmoji.FromName(this.Hanna.Client, c.EmojiName))));

					fieldCurrencyValues +=
						$"{c.EmojiName} {Math.Ceiling((item.DefValue / c.BaseDivider) * (item.Temporary && hasTOpt ? tOption.BaseMultiplier : 1))}\n";
				}

				builder.AddField(item.Temporary && hasTOpt ? tOption.Name : "Item permanente", fieldCurrencyValues);

				// Caso seja temporário
			} else
				foreach (TemporaryItemOption itemOption in this.Manager.TemporaryOptions) {
					// Adiciona os botoes de comprar
					optionsButtons.Add(new DiscordButtonComponent(ButtonStyle.Secondary,
						$"loja_item_{itemName}_{itemOption.Name}", null,
						false, new DiscordComponentEmoji(
							DiscordEmoji.FromName(this.Hanna.Client, itemOption.EmojiName))));

					// Adiciona cada moeda disponível
					string fieldCurrencyValues = this.Manager.CurrencyOptions
					   .Aggregate("", (current, c) => current +
							$"{c.EmojiName} {Math.Ceiling((item.DefValue / c.BaseDivider) * itemOption.BaseMultiplier)} {c.Name}\n");

					builder.AddField(itemOption.Name, fieldCurrencyValues, true);
				}


			builder
			   .WithColor(item.DiscColor)
			   .WithAuthor($"{item.Name}{new string(' ', 40)}{index + 1}/{count}", null, item.ImageLink)
			   .WithDescription(item.Description)
			   .WithFooter("Carteira: 💵 000 • 💎 000", "https://twemoji.maxcdn.com/2/72x72/1f4b0.png")
			   .WithTimestamp(DateTime.Now);
			return new DiscordMessageBuilder()
			   .WithEmbed(builder)
			   .AddComponents(optionsButtons)
			   .AddComponents(navButtons);
		}


		// -------------------------------------------------------------> NAVEGAÇÃO <-------------------------------------------------------------------

		// ----------------------------------------------------------> INTERATIVIDADE <-----------------------------------------------------------------
		private async Task<DiscordMessageBuilder> GetConfirm(
			DiscordUser user, string itemName, string currencyName, string tOptionName = "none") {
			ShopItem item = this.Manager
			   .ShopItens.Single(i => i.Name == itemName);

			CurrencyOption currency = this.Manager.CurrencyOptions
			   .SingleOrDefault(o => o.Name == currencyName);

			TemporaryItemOption temporaryOption = this.Manager.TemporaryOptions
			   .SingleOrDefault(o => o.Name == tOptionName);

			List<DiscordButtonComponent> buttonOptions = new() {
				new DiscordButtonComponent(ButtonStyle.Success,
					$"loja_buy_{itemName}_{currency.Name}_{tOptionName}", "Comprar",
					false, // TODO: verificar se a pessoa pode comprar
					new DiscordComponentEmoji("💰")
				),
				new DiscordButtonComponent(ButtonStyle.Danger,
					$"loja_item_{itemName}_{tOptionName}", "Cancelar", false,
					new DiscordComponentEmoji("↪")),
			};

			// Envia o embed
			DiscordEmbedBuilder builder = EmbedUtils.WarningBuilder
			   .WithThumbnail(item.ImageLink)
			   .WithAuthor("Tem certeza que deseja comprar?", null, Embeds.Images.OrangeExclamation)
			   .WithDescription(
					$"**Nome:** {item.Name}\n" +
					$"**Validade:** {(item.Temporary ? $"{tOptionName}" : "Permanente")}\n" +
					$@"**Valor:** {currency.EmojiName} {Math.Ceiling((item.DefValue / currency.BaseDivider)
					  * (item.Temporary ? temporaryOption.BaseMultiplier : 1))}")
			   .WithFooter("Carteira: 💵 000 • 💎 000", "https://twemoji.maxcdn.com/2/72x72/1f4b0.png")
			   .WithTimestamp(DateTime.Now);

			return new DiscordMessageBuilder()
			   .WithEmbed(builder)
			   .AddComponents(buttonOptions);
		}
		
		private async Task<DiscordMessageBuilder> Success(
			DiscordUser user, string itemName, string currencyName, string tOptName = "none") {
			ShopItem            item     = this.Manager.ShopItens.Single(i => i.Name == itemName);
			CurrencyOption      currency = this.Manager.CurrencyOptions.Single(c => c.Name == currencyName);
			TemporaryItemOption tOption  = this.Manager.TemporaryOptions.SingleOrDefault(t => t.Name == tOptName);
			
			// Envia o embed
			DiscordEmbedBuilder builder = EmbedUtils.SuccesBuilder
			   .WithThumbnail(item.ImageLink)
			   .WithDescription($"o item {item.Name} {(tOptName == "none" ? "" : $"({tOption.Name})")}\n" +
					"foi comprado com sucesso!\n\n" +
					$"**Item:** {item.Name} {(tOptName == "none" ? "" : tOption.Name)}\n" +
					$@"**Valor:** {currency.EmojiName} {Math.Ceiling((item.DefValue / currency.BaseDivider)
					  * (item.Temporary ? tOption.BaseMultiplier : 1))}")
			   .WithFooter("Carteira: 💵 000 • 💎 000", "https://twemoji.maxcdn.com/2/72x72/1f4b0.png")
			   .WithTimestamp(DateTime.Now);

			return new DiscordMessageBuilder()
			   .WithEmbed(builder)
			   .AddComponents(new DiscordButtonComponent(
					ButtonStyle.Secondary, $"loja_item_{itemName}_none",
					"Retornar", false, new DiscordComponentEmoji("↪")));
		}

		private static async Task<DiscordMessageBuilder> ShowError(
			Exception err, DiscordUser user, ShopItem item, string tOptName = "none") {
			DiscordEmbedBuilder builder = EmbedUtils.ErrorBuilder
			   .WithThumbnail(item.ImageLink)
			   .WithDescription($"Infelizmente algo deu errado durante a compra desse item\n\n" +
					$"**Item:** {item.Name} {(tOptName == "none" ? "" : $"({tOptName})")}\n" +
					$"**Erro:** {err.TargetSite} => {err.GetType()}: `{err.Message}`\n\n```{err.StackTrace}```");

			return new DiscordMessageBuilder().WithEmbed(builder);
		}
	
		private static DiscordMessageBuilder ShowError(Exception err) {
			DiscordEmbedBuilder builder = EmbedUtils.ErrorBuilder
			   .WithDescription($"Infelizmente algo deu errado durante a execução do comando\n\n" +
					$"**Erro:** {err.TargetSite} => {err.GetType()}: `{err.Message}`\n\n```{err.StackTrace}```");

			return new DiscordMessageBuilder().WithEmbed(builder);
		}
		// ----------------------------------------------------------> INTERATIVIDADE <-----------------------------------------------------------------
	}
}