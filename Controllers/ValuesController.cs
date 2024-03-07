using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using CRYSTAL_DRESSES_API.Models;
using CRYSTAL_DRESSES_API.Repository;

namespace CRYSTAL_DRESSES_API.Controllers
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

        [HttpGet]
        public List<AccList> GetBusyMasters(int MasterType, int VchType, string CompCode, string FY)
        {
            return _services.GetBusyMaster(MasterType, VchType, CompCode, FY);
        }
        public List<AccList> GetItemList(string CompCode, string FY, int AccCode)
        {
            return _services.GetPartyItems(CompCode, FY, AccCode);
        }

        [HttpGet]
        public GetProductionOrder GetPendingProductionOrder(string CompCode, string FY, int SAccCode, string PONo)
        {
            return _services.GetPendingProductionDetails(CompCode, FY, SAccCode, PONo);
        }

        [HttpPost]
        public BusyVoucher.AlertOrder PostProductionOrder(BusyVoucher.PostProductionOrder obj, string CompCode, string FY)
        {
            return _services.SaveAutoProductionOrderInv(obj, CompCode, FY);
        }

        [HttpGet]
        public dynamic GetMRAutoSyncProdDet(string CompCode, string FY, int SAccCode,string FDate, string TDate)
        {
            return _services.GetMRAutoSyncDetails(CompCode, FY, SAccCode, FDate, TDate);
        }

        [HttpGet]
        public dynamic GetMRAutoSyncProdItemsDet(string CompCode, string FY, int VchCode)
        {
            return _services.GetMRAutoSyncItemsDetails(CompCode, FY, VchCode);
        }
    }
}
