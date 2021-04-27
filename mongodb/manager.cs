using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hanna.MongoDB
{
	public class DBManager
	{
		public MongoClient Client { get; private set; }

		public DbManager()
		{

			var client = new MongoClient("mongodb+srv://Hanna:<password>@cluster0.ekjrf.mongodb.net/<dbname>?retryWrites=true&w=majority");
			var database = client.GetDatabase("test");

		}
	}
}
