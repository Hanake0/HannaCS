﻿using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using Hanna.Util.JsonModels;

namespace Hanna.Util {
	public static class WebClient {
		// Um mesmo HttpClient deve ser usado para evitar utilizar todas as portas do bot
		public static readonly HttpClient Client = new();

		public static async Task<McStatusAPI> GetMcServerInfoAsync(string ip, int port = 0) {
			Stream stream = await Client
				.GetStreamAsync($"https://mcstatus.snowdev.com.br/api/query/v3/{ip}" +
					(port == 0 ? "" : $":{port}"));
			
			return await JsonSerializer
				.DeserializeAsync<McStatusAPI>(stream);
		}
	}
}
