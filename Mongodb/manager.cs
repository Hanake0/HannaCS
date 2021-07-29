using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Driver;

namespace Hanna.MongoDB
{
	public class DBManager
	{
		public MongoClient Client { get; private set; }

		public DBManager()
		{

			this.Client = new MongoClient("mongodb+srv://Hanna:<password>@cluster0.ekjrf.mongodb.net/<dbname>?retryWrites=true&w=majority");
			_ = this.Client.GetDatabase("test");

		}
	}
}
