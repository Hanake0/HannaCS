namespace Hanna
{
	class Program
	{
		static void Main(string[] args)
		{
			Bot Hanna = new Bot();
			Hanna.RunAsync().GetAwaiter().GetResult();

		}
	}
}
