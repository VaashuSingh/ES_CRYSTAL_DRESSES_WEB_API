using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRYSTAL_DRESSES_API.Models
{
    public class Crystal_Dresses
    {

    }
    public partial class UserValidate
    {
        public bool Validate { get; set; } = false;
        public int Code { get; set; }
        public string Name { get; set; }
    }

    public class ItemList
    {
        public int Code { get; set; }
        public string Name { get; set; }
    }

    public class AccList
    {
        public int Code { get; set; }
        public double Price { get; set; }
        public string Name { get; set; }
    }
    public class GetProductionOrderDetails
    {
        public string PlanNo { get; set; }
        public int MainFGItem { get; set; }
        public int SRawCode { get; set; }
        public string LotNo { get; set; } 
        public int FGItemCode { get; set; }
        public int SFGItemCode { get; set; }
        public int IGrp { get; set; }
        public int RItemCode { get; set; }
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public double RQty { get; set; }
        public double IQty { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
    }
    public class GetProductionOrder
    {
        public string PONo { get; set; }
        public int CustCode { get; set; }
        public string CustName { get; set; }
        public List<GetProductionOrderDetails> ProductionOrderDetails { get; set; }
    }

    public class GetMRAutoSyncDet
    {
        public int VchCode { get; set; }
        //public int ItemDesc { get; set; }
        public string SName { get; set; }
        public string VchDate { get; set; }
        public string VchNo { get; set; }
        public double GrandTot { get; set; }
    }

    public class GetMRAutoSyncItemDet
    {
        public int SrNo { get; set; }
        public string ItemName { get; set; }
        public string UnitName { get; set; }
        public double Qty { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
    }
    public class SaleOrder
    {
        public string CompCode { get; set; }
        public string FY { get; set; }
        public string vchno { get; set; }
        public int Vchcode { get; set; }
        public string Series { get; set; }
        public int SeriesCode { get; set; }
        public DateTime Date { get; set; }
        public int MnfCode { get; set; }
        public string MnfName { get; set; }
        public int Partycode { get; set; }
        public string PartyName { get; set; }
        public string ItemName { get; set; }
        public int ItemCode { get; set; }
        public double Qty { get; set; }
        public string TName { get; set; }
        public string PMarka { get; set; }
        public string BillingOF { get; set; }
        public string ItemOF { get; set; }
        public DateTime DueDate { get; set; }
        public int OrderApp { get; set; }
    }
    public class VItemData
    {
        public int SrNo { get; set; }
        public string ItemName { get; set; }
        public double Qty { get; set; }
        public string Unit { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
    }
    public class PendingOrder
    {
        public string vchno { get; set; }
        public int Vchcode { get; set; }
        public string Series { get; set; }
        public int SeriesCode { get; set; }
        public DateTime Date { get; set; }
        public int MnfCode { get; set; }
        public string MnfName { get; set; }
        public int Partycode { get; set; }
        public string PartyName { get; set; }
        public string ItemName { get; set; }
        public int ItemCode { get; set; }
        public double Qty { get; set; }
        public double PQty { get; set; }
        public string TName { get; set; }
        public string PMarka { get; set; }
        public string BillingOF { get; set; }
        public string ItemOF { get; set; }
        public DateTime DueDate { get; set; }
    }
    public class LedgerReport
    {
        public int vchcode { get; set; }
        public string date { get; set; }
        public string dtorder { get; set; }
        public string Type { get; set; }
        public string BillNo { get; set; }
        public string Account { get; set; }
        public double Value { get; set; }
        public string Credit { get; set; }
        public string debit { get; set; }
        public string Balance { get; set; }
        public string Shortnar { get; set; }
        public string DueDate { get; set; }
        public int OverDueDays { get; set; }
    }
    public class Pwise
    {
        public string S { get; set; }
        public string A { get; set; }
    }
    public class AccOutstanding
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public double Outstanding { get; set; }
    }
}
