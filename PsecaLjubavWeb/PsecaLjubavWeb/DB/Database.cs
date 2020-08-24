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
        private readonly IGraphClient client;
        private const string uri = "bolt://localhost:7687/db/data";
        private const string username = "neo4j";
        private const string password = "password123";

        public Database()
        {
            client = new BoltGraphClient(uri, username, password);
            Connect();

        }
        public IGraphClient GetGraphClient()
        {
            if (!client.IsConnected)
            {
                Connect();
            }
            return client;
        }

        private void Connect()
        {
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
