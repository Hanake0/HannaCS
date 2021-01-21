namespace Hanna
{
	class Program
	{
		static void Main()
		{
			Bot Hanna = new Bot();
			Hanna.RunAsync().GetAwaiter().GetResult();

		}
	}
}
