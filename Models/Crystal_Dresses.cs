using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ES_CrystalDresses_WEB.Models
{
    public class MiniAarth
    {

    }
    public class AccList
    {
        public int Code { get; set; }
        public double Price { get; set; }
        public string Name { get; set; }
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
