namespace Hanna {
	class Program {
		static void Main() {
			Bot Hanna = new();
			Hanna.RunAsync().GetAwaiter().GetResult();
		}
	}
}
