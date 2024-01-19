using ES_CrystalDresses_WEB.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ES_CrystalDresses_WEB.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly IRepository _services;

        public ValuesController()
        {
            IRepository services = new Repository.Repository();
            this._services = services;
        }

        [HttpGet]
        public dynamic ValidateUser(string UName, string Pass, string CompCode, int FY)
        {
            return _services.ValidateUser(UName, Pass, CompCode, FY);
        }

    }
}
