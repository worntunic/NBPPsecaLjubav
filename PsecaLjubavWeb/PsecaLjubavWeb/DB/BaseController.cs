using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PsecaLjubavWeb.DB
{
    public class BaseController
    {
        protected IGraphClient graphClient;

        public BaseController(Database db) { 
            this.graphClient = db.GetGraphClient();
        }
    }
    public class ControllerException : Exception
    {
        public ControllerException(string message) : base(message)
        {
        }
    }
}
