using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ES_CrystalDresses_WEB.Models;

namespace ES_CrystalDresses_WEB.Repository
{
    public interface IRepository
    {
        dynamic ValidateUser(string UName, string Pass, string CompCode, int FY);
        List<AccList> GetBusyMaster(int MasterType, int VchType, string CompCode, string FY);
        List<AccList> GetPartyItems(string CompCode, string FY, int AccCode);
    }
}
