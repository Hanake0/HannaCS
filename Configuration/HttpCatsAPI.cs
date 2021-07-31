using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hanna.Configuration {
	public static class HttpCatsAPI {
		public static readonly string Link = "https://http.cat/";
		public static readonly int[] ResponseCodes = new int[] {
			// Information
			100, 101, 102,

			// Success
			200, 201, 202, 203,
			204, 206, 207,

			// Redirect
			300, 301, 302, 303,
			304, 305, 307, 308,

			// Client Error
			400, 401, 402, 403, 404,
			405, 406, 407, 408, 409,
			410, 411, 412, 413, 414,
			415, 416, 417, 418, 420,
			421, 422, 423, 424, 425,
			426, 429, 431, 444, 450,
			451, 497, 498, 499,

			// Server Error
			500, 501, 502, 503, 504,
			506, 507, 508, 509, 510,
			511, 521, 523, 525, 599
		};
	}
}
