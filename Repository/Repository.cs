using ES_CrystalDresses_WEB.Models;
using ESCommon;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ES_CrystalDresses_WEB.Repository
{
    public class Repository: IRepository
    {
        string servername = ConfigurationManager.AppSettings["servername"].ToString();
        string Suser = ConfigurationManager.AppSettings["SUserName"].ToString();
        string Spass = ConfigurationManager.AppSettings["SPassword"].ToString();
        string BusyAppPath = ConfigurationManager.AppSettings["BusyAppPath"].ToString();

        public dynamic ValidateUser(string UName, string Pass, string CompCode, int FY)
        {

            BusyHelper helper = new BusyHelper();

            Boolean SuperUser = false;
            string err = "";
            bool Validate = helper.ValidateUser(UName, Pass, BusyAppPath, servername, Suser, Spass, FY, CompCode, ref SuperUser, ref err);
            bool branchYN = true;
            return new { validate = Validate, SuperUser = SuperUser, branchYN = branchYN };
        }
    }
}
