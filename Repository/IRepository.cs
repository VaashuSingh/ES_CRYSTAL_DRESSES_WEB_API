using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CRYSTAL_DRESSES_API.Models;

namespace CRYSTAL_DRESSES_API.Repository
{
    public interface IRepository
    {
        dynamic ValidateUser(string UName, string Pass, string CompCode, int FY);
        List<AccList> GetBusyMaster(int MasterType, int VchType, string CompCode, string FY);
        List<AccList> GetPartyItems(string CompCode, string FY, int AccCode);
        GetProductionOrder GetPendingProductionDetails(string CompCode, string FY, int SAccCode, string PONo);
        BusyVoucher.AlertOrder SaveAutoProductionOrderInv(BusyVoucher.PostProductionOrder obj, string CompCode, string FY);
    }

}
