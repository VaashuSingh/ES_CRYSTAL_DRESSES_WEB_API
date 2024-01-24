using ESCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRYSTAL_DRESSES_API.Models
{
    public class BusyVoucher
    {
        public class PostProductionOrder
        {
            public int AccCode { get; set; }
            public string AccName { get; set; }
            public int SAccCode { get; set; }
            public string SAccName { get; set; }
            public string PONo { get; set; }
            public List<InvoiceItemDT> ItemInvDetails { get; set; }
        }
        public class InvoiceItemDT
        {
            public string PlanNo { get; set; }
            public int FGItemCode { get; set; }
            public int SFGItemCode { get; set; }
            public int ItemCode { get; set; }
            public string ItemName { get; set; }
            public double Qty { get; set; }
            public double Price { get; set; }
            public double Amount { get; set; }
        }
        public class STPTData
        {
            public int GSTType { get; set; }
            public bool MultiTax { get; set; }
            public bool TaxType { get; set; }
        }
        public class MaterialReceipt
        {
            public string VchSeriesName { get; set; }
            public string Date { get; set; }
            public int VchType { get; set; }
            public string StockUpdationDate { get; set; }
            public string VchNo { get; set; }
            public string STPTName { get; set; }
            public string MasterName1 { get; set; }
            public string MasterName2 { get; set; }
            public string TranCurName { get; set; }
            public bool BrokerInvolved { get; set; }
            public string BrokerName { get; set; }
            public BillingDetails BillingDetails { get; set; }
            public VchOtherInfoDetails VchOtherInfoDetails { get; set; }
            public List<ItemDetail> ItemEntries { get; set; }
            public List<BSDetail> BillSundries { get; set; }
            public int TmpVchCode { get; set; }
            public int TmpVchSeriesCode { get; set; }
        }
        public class MaterialIssue
        {
            public string VchSeriesName { get; set; }
            public string Date { get; set; }
            public int VchType { get; set; }
            public string StockUpdationDate { get; set; }
            public string VchNo { get; set; }
            public string STPTName { get; set; }
            public string MasterName1 { get; set; }
            public string MasterName2 { get; set; }
            public string TranCurName { get; set; }
            public bool BrokerInvolved { get; set; }
            public string BrokerName { get; set; }
            public BillingDetails BillingDetails { get; set; }
            public VchOtherInfoDetails VchOtherInfoDetails { get; set; }
            public List<ItemDetail> ItemEntries { get; set; }
            public List<BSDetail> BillSundries { get; set; }
            public int TmpVchCode { get; set; }
            public int TmpVchSeriesCode { get; set; }
        }
        public class Sale
        {
            public string VchSeriesName { get; set; }
            public string Date { get; set; }
            public int VchType { get; set; }
            public string StockUpdationDate { get; set; }
            public string VchNo { get; set; }
            public string STPTName { get; set; }
            public string MasterName1 { get; set; }
            public string MasterName2 { get; set; }
            public string TranCurName { get; set; }
            public bool BrokerInvolved { get; set; }
            public string BrokerName { get; set; }
            public BillingDetails BillingDetails { get; set; }
            public VchOtherInfoDetails VchOtherInfoDetails { get; set; }
            public List<ItemDetail> ItemEntries { get; set; }
            public List<BSDetail> BillSundries { get; set; }
            public int TmpVchCode { get; set; }
            public int TmpVchSeriesCode { get; set; }
        }
        public class BillingDetails
        {
            public string PartyName { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string Address3 { get; set; }
            public string Address4 { get; set; }
            public string MobileNo { get; set; }
            public string ITPAN { get; set; }
            public int TypeOfDealer { get; set; }
            public string TmpVchCode { get; set; }
        }
        public class VchOtherInfoDetails
        {
            //public string OFInfo { get; set; }
            public string PurchaseBillNo { get; set; }
            public string Narration1 { get; set; }
        }
        public class ItemDescInfo
        {
            //public string OFInfo { get; set; }
            public string Description1 { get; set; }
            public string Description2 { get; set; }
            public int tmpSrNo { get; set; }
        }
        public class ItemDetail
        {
            public string Date { get; set; }
            public int VchType { get; set; }
            public string VchNo { get; set; }
            public int SrNo { get; set; }
            public string ItemName { get; set; }
            public string UnitName { get; set; }
            public string AltUnitName { get; set; }
            public double ConFactor { get; set; }
            public double Qty { get; set; }
            public double QtyMainUnit { get; set; }
            public double QtyAltUnit { get; set; }
            public ItemDescInfo ItemDescInfo { get; set; }
            public double Price { get; set; }
            public double PriceAltUnit { get; set; }
            public double ListPrice { get; set; }
            public double Amt { get; set; }
            public double NettAmount { get; set; }
            public int TmpVchCode { get; set; }
            public string MC { get; set; }
            public string AF { get; set; }
            public List<ParamStockDetails> ParamStockEntries { get; set; }
        }
        public class ParamStockDetails
        {
            public int ItemCode { get; set; }
            public int MCCode { get; set; }
            public string Party { get; set; }
            public int VchItemGridSN { get; set; }
            public int SrNo { get; set; }
            public string BCN { get; set; }
            public string Param1 { get; set; }
            public string Param2 { get; set; }
            public string Param3 { get; set; }
            public double MainQty { get; set; }
            public double AltQty { get; set; }
        }
        public class BSDetail
        {
            public string SrNo { get; set; }
            public string BSName { get; set; }
            public string PercentVal { get; set; }
            public string PercentOperatedOn { get; set; }
            public string Amt { get; set; }
            public string Date { get; set; }
            public string VchNo { get; set; }
            public string VchType { get; set; }
            public string TmpVchCode { get; set; }
        }
        public class StockTransfer
        {
            public string VchSeriesName { get; set; }
            public string Date { get; set; }
            public int VchType { get; set; }
            public string StockUpdationDate { get; set; }
            public string VchNo { get; set; }
            public string STPTName { get; set; }
            public string MasterName1 { get; set; }
            public string MasterName2 { get; set; }
            public string TranCurName { get; set; }
            public BillingDetails BillingDetails { get; set; }

            public VItemData VItemData { get; set; }
            public BillWiseDetails1 BillWiseDetails1 { get; set; }
            public VchOtherInfoDetails VchOtherInfoDetails { get; set; }
            public List<ItemDetail> ItemEntries { get; set; }
            public List<BSDetail> BillSundries { get; set; }
            public int TmpVchCode { get; set; }
            public int TmpVchSeriesCode { get; set; }
        }
        public class StockJournal
        {
            public string VchSeriesName { get; set; }
            public string Date { get; set; }
            public int VchType { get; set; }
            public string StockUpdationDate { get; set; }
            public string VchNo { get; set; }
            public string STPTName { get; set; }
            public string MasterName1 { get; set; }
            public string MasterName2 { get; set; }
            public string TranCurName { get; set; }
            public BillingDetails BillingDetails { get; set; }

            public VItemData VItemData { get; set; }
            public BillWiseDetails1 BillWiseDetails1 { get; set; }

            public List<BillWiseDetails1> billWiseDetails1 { get; set; }
            public VchOtherInfoDetails VchOtherInfoDetails { get; set; }
            public List<ItemDetail> ItemEntries { get; set; }
            public List<ItemDetail> ItemEntries1 { get; set; }
            public List<BSDetail> BillSundries { get; set; }
            public int TmpVchCode { get; set; }
            public int TmpVchSeriesCode { get; set; }
        }
        public class Purchase
        {
            public string VchSeriesName { get; set; }
            public string Date { get; set; }
            public int VchType { get; set; }
            public string StockUpdationDate { get; set; }
            public string VchNo { get; set; }
            public string STPTName { get; set; }
            public string MasterName1 { get; set; }
            public string MasterName2 { get; set; }
            public string TranCurName { get; set; }
            public BillingDetails BillingDetails { get; set; }

            public VchOtherInfoDetails VchOtherInfoDetails { get; set; }
            public List<ItemDetail> ItemEntries { get; set; }
            public List<BSDetail> BillSundries { get; set; }
            public int TmpVchCode { get; set; }
            public int TmpVchSeriesCode { get; set; }
        }
        public class ServerInfo
        {
            public string BusyAppPath { get; set; }
            public string ServerName { get; set; }
            public string SUserName { get; set; }
            public string SPassword { get; set; }
            public string CompCode { get; set; }
            public int FinYear { get; set; }
        }
        public class BillByBillRef
        {
            public int RefCode { get; set; }
            public string RefNo { get; set; }
            public string Date { get; set; }
            public double Amount { get; set; }
            public string DueDate { get; set; }
            public double AdjAmount { get; set; }
        }
        public class VchReceipt
        {
            public int SeriesCode { get; set; }
            public string SeriesName { get; set; }
            public string PartyName { get; set; }
            public int PDC { get; set; }
            public String PDCDate { get; set; }
            public string Remarks { get; set; }
            public int Mode { get; set; }
            public string chequeNo { get; set; }
            public string ChequeDt { get; set; }
            public double Amount { get; set; }
            public string Image { get; set; }
            public List<BillByBillRef> BillByBill { get; set; }
        }
        public class AlertOrder
        {
            public int Status { get; set; }
            public string Msg { get; set; }
            //public string Message { get; set; }
            //public int Success { get; set; }
            public int OrderId { get; set; }
        }
        public class Receipt
        {
            public string VchNo { get; set; }
            public string VchSeriesName { get; set; }
            public string Date { get; set; }
            public string PDCDate { get; set; }
            public string StockUpdationDate { get; set; }
            public int VchType { get; set; }
            public string TranCurName { get; set; }
            public int TranType { get; set; }
            public int tmpVchCode { get; set; }
            public int tmpVchSeriesCode { get; set; }
            public double tmpTotalAmt { get; set; }
            public int tmpOldVchSeriesCode { get; set; }
            public string tmpTranCurString { get; set; }
            public string tmpTranCurSubString { get; set; }
            public VchOtherInfoDetails VchOtherInfoDetails { get; set; }
            public List<AccDetail> AccEntries { get; set; }
            public List<BillDetail> PendingBillDetails { get; set; }
        }
        public class BillDetail
        {
            public string MasterName1 { get; set; }
            public int tmpMasterCode1 { get; set; }
            public List<BillRefs> BillRefDT { get; set; }
        }
        public class BillRefs
        {
            public int Method { get; set; }
            public int SrNo { get; set; }
            public string RefNo { get; set; }
            public string Date { get; set; }
            public string DueDate { get; set; }
            public double Value1 { get; set; }
            public int ItemSrNo { get; set; }
            public int tmpRefCode { get; set; }
            public int tmpRecType { get; set; }
            public int tmpMasterCode1 { get; set; }
        }
        public class AccDetail
        {
            public string Date { get; set; }
            public int VchType { get; set; }
            public int SrNo { get; set; }
            public string AccountName { get; set; }
            public string ShortNar { get; set; }
            public int AmountType { get; set; }
            public double AmtMainCur { get; set; }
            public double CashFlow { get; set; }
            public int tmpVchCode { get; set; }
            public string tmpGroupName { get; set; }
            public int tmpAccCode { get; set; }
            //public List<BillDetails> BillRefs { get; set; }
        }
        public class Payment
        {
            public string VchNo { get; set; }
            public string VchSeriesName { get; set; }
            public string Date { get; set; }
            public string PDCDate { get; set; }
            public string StockUpdationDate { get; set; }
            public int VchType { get; set; }
            public string TranCurName { get; set; }
            public int TranType { get; set; }
            public int tmpVchCode { get; set; }
            public int tmpVchSeriesCode { get; set; }
            public double tmpTotalAmt { get; set; }
            public int tmpOldVchSeriesCode { get; set; }
            public string tmpTranCurString { get; set; }
            public string tmpTranCurSubString { get; set; }
            public VchOtherInfoDetails VchOtherInfoDetails { get; set; }
            public List<AccDetail> AccEntries { get; set; }
            public List<BillDetail> PendingBillDetails { get; set; }
        }
        public class BillWiseDetails1
        {
            public string IEMI { get; set; }

            public string AccCode { get; set; }

            public string AccName { get; set; }
            public string SrNo { get; set; }

            public string RefNo { get; set; }

            public string VchType { get; set; }

            public string Date { get; set; }

            public string TotalAmt { get; set; }

            public string Receivable { get; set; }

            public string Payable { get; set; }

            public string DueDate { get; set; }

            public string DDays { get; set; }
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
        public class AccInvVoucher
        {
            public string VchSeriesName { get; set; }
            public string Date { get; set; }
            public string VchNo { get; set; }
            public string Narration1 { get; set; }
            public string Narration2 { get; set; }
            public double DrAmount { get; set; }
            public double CrAmount { get; set; }
            public List<InvAccDetail> AccEntries { get; set; }
        }
        public class InvAccDetail
        {
            public int SrNo { get; set; }
            public string AccName { get; set; }
            public string CrAmount { get; set; }
            public string DrAmount { get; set; }
            public string ShortNar { get; set; }
        }
        public class InvVoucher
        {
            public string VchSeriesName { get; set; }
            public string Date { get; set; }
            public string VchNo { get; set; }
            public string PartyName { get; set; }
            public string Narration { get; set; }
            public double TQty { get; set; }
            public double TAmount { get; set; }
            public double NetAmount { get; set; }
            public List<InvItemDetail> ItemEntries { get; set; }
            public List<InvBSDetail> BSEntries { get; set; }
        }
        public class InvItemDetail
        {
            public int SrNo { get; set; }
            public string ItemName { get; set; }
            public string UnitName { get; set; }
            public double QtyMainUnit { get; set; }
            public double QtyAltUnit { get; set; }
            public double Price { get; set; }
            public double Amount { get; set; }
        }
        public class InvBSDetail
        {
            public int SrNo { get; set; }
            public string BSName { get; set; }
            public double BSPer { get; set; }
            public double Amount { get; set; }
        }
        public string GetMasterCodeToName(string ConStr, int Code)
        {
            string Name = "";
            try
            {
                SQLHELPER obj = new SQLHELPER(ConStr);
                string str = "Select Name From Master1 Where Code = " + Code + "";
                DataTable dt = obj.getTable(str);

                if (dt != null && dt.Rows.Count > 0)
                {
                    Name = clsMain.MyString(dt.Rows[0]["Name"]);
                }
            }
            catch
            {
                return "";
            }
            return Name;
        }
        public string GetAccMasterSTPTType(string ConStr, int Code, int STPTMasterType)
        {
            string Name = "";
            try
            {
                SQLHELPER obj = new SQLHELPER(ConStr);

                string str = "";

                if (STPTMasterType == 13)
                {
                    str = "Select IsNull(B.Name,'') as STPTType from Master1 A Left Join Master1 B On A.CM6 = B.Code Where A.Code = " + Code + "";
                }
                else
                {
                    str = "Select IsNull(B.Name,'') as STPTType from Master1 A Left Join Master1 B On A.CM7 = B.Code Where A.Code = " + Code + "";
                }
                DataTable dt = obj.getTable(str);

                if (dt != null && dt.Rows.Count > 0)
                {
                    Name = clsMain.MyString(dt.Rows[0]["STPTType"]);
                }
            }
            catch
            {
                return "";
            }
            return Name;
        }
        public string GetItemMainUnitName(string ConStr, int ItemCode)
        {
            string Name = "";
            try
            {
                SQLHELPER obj = new SQLHELPER(ConStr);
                string str = "Select B.Name From Master1 A Inner Join Master1 B On A.CM1 = B.Code Where A.Code = " + ItemCode + "";
                DataTable dt = obj.getTable(str);

                if (dt != null && dt.Rows.Count > 0)
                {
                    Name = clsMain.MyString(dt.Rows[0]["Name"]);
                }
            }
            catch
            {
                return "";
            }
            return Name;
        }
        public double GetPlanItemPrice(string ConStr, string PlanNo, int Party, int MCCode, int ItemCode, int RecType)
        {
            double Price = 0;
            try
            {
                SQLHELPER obj = new SQLHELPER(ConStr);
                string str = "Select IsNull(Max(Price),0) as Price From ESReftran A Where A.MCCode = " + clsMain.MyInt(MCCode) + " And Party = " + clsMain.MyInt(Party) + " And ItemCode = " + clsMain.MyInt(ItemCode) + " And VchNo = '" + clsMain.MyString(PlanNo) + "' And A.Rectype = " + RecType + "";
                DataTable dt = obj.getTable(str);

                if (dt != null && dt.Rows.Count > 0)
                {
                    Price = clsMain.MyDouble(dt.Rows[0]["Price"]);
                }
            }
            catch
            {
                return 0;
            }
            return Price;
        }
        public int GetMasterNameToCode(string ConStr, string Name, int MasterType)
        {
            int Code = 0;
            try
            {
                SQLHELPER obj = new SQLHELPER(ConStr);
                string str = "Select Code From Master1 Where Name = '" + Name.Replace("'", "''") + "' And MasterType = " + MasterType + "";
                DataTable dt = obj.getTable(str);

                if (dt != null && dt.Rows.Count > 0)
                {
                    Code = clsMain.MyInt(dt.Rows[0]["Code"]);
                }
            }
            catch
            {
                return 0;
            }
            return Code;
        }
    }
}
