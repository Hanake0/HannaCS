namespace Hanna.Util.JsonModels {
	public record McStatusAPI {
#pragma warning disable IDE1006 // Estilos de Nomenclatura
		public bool online { get; init; }
		public string motd { get; init; }
		public string version { get; init; }
		public long players_online { get; init; }
		public long max_players { get; init; }
		public long ping { get; init; }
#pragma warning restore IDE1006 // Estilos de Nomenclatura
	}
}
