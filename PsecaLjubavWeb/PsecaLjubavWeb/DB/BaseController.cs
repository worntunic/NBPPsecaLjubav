using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PsecaLjubavWeb.DB
{
    public class BaseController
    {
        protected GraphClient graphClient;

        public void Init(GraphClient graphClient)
        {
            this.graphClient = graphClient;
        }
    }
    public class ControllerException : Exception
    {
        public ControllerException(string message) : base(message)
        {
        }
    }
}
