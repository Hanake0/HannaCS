using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hanna.Util.JsonModels {
	public record RandomFoxAPIResponse {
#pragma warning disable IDE1006 // Estilos de Nomenclatura
		public string image { get; init; }
		public string link { get; init; }
#pragma warning restore IDE1006 // Estilos de Nomenclatura
	}
}
