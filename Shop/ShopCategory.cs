namespace Hanna.Shop {
	public struct ShopCategory {
		public string Name;
		public string ImageUrl;
		public string DisplayName { get => $"{this.EmojiName} {this.Name}"; }
		public string SmallDescription;
		public string EmojiName;
	}
}