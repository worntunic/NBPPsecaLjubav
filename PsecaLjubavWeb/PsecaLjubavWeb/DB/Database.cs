using Neo4j.Driver;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PsecaLjubavWeb.DB
{

    public class Database : IDisposable
    {
        private readonly GraphClient client;
        private const string uri = "bolt://localhost:7687";
        private const string username = "neo4j";
        private const string password = "password";


        public Database()
        {
            client = new GraphClient(new Uri(uri), username, password);
            try
            {
                client.Connect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        
        public void Dispose()
        {
            client.Dispose();
        }
    }
}
