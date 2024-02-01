using CRYSTAL_DRESSES_API.Models;
using ESCommon;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CRYSTAL_DRESSES_API.Models.BusyVoucher;
using Busy2184;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace CRYSTAL_DRESSES_API.Repository
{
    public class Repository: IRepository
    {
        string servername = ConfigurationManager.AppSettings["servername"].ToString();
        string Suser = ConfigurationManager.AppSettings["SUserName"].ToString();
        string Spass = ConfigurationManager.AppSettings["SPassword"].ToString();
        string BusyAppPath = ConfigurationManager.AppSettings["BusyAppPath"].ToString();

        public dynamic ValidateUser(string UName, string Pass, string CompCode, int FY)
        {
            UserValidate USData = new UserValidate();
            try
            {
                string DBName = $"Busy{CompCode}_db1{FY}";
                string ConnectionString = $"Data Source={servername}; Initial catalog={DBName}; Uid={Suser}; Pwd={Spass}";
                SQLHELPER obj = new SQLHELPER(ConnectionString); 

                string sql = $"Select Top 1 A.[Name], A.Code From Master1 A inner Join MasterAddressInfo B On A.Code = B.MasterCode Where A.MasterType = 2 And B.OF1 = '{UName}' And B.OF2 = '{Pass}' ";
                DataTable DT = obj.getTable(sql);

                if (DT != null && DT.Rows.Count > 0)
                {
                    USData.Validate = true;
                    USData.Code = clsMain.MyInt(DT.Rows[0]["Code"]);
                    USData.Name = clsMain.MyString(DT.Rows[0]["Name"]).Trim();
                }
                else
                {
                    return new { Status = 0, Msg = "Invalid User" };
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString()};
            }
            return new { Status = 1, Msg = "Success", Data = USData };
        }

        //public dynamic ValidateUser(string UName, string Pass, string CompCode, int FY)
        //{

        //    BusyHelper helper = new BusyHelper();

        //    Boolean SuperUser = false;
        //    string err = "";
        //    bool Validate = helper.ValidateUser(UName, Pass, BusyAppPath, servername, Suser, Spass, FY, CompCode, ref SuperUser, ref err);
        //    bool branchYN = true;
        //    return new { validate = Validate, SuperUser = SuperUser, branchYN = branchYN };
        //}
        public List<AccList> GetBusyMaster(int MasterType, int VchType, string CompCode, string FY)
        {
            string DBName = $"Busy{CompCode}_db1{FY}";
            string ConnectionString = $"Data Source={servername}; Initial catalog={DBName}; Uid={Suser}; Pwd={Spass}";

            SQLHELPER obj = new SQLHELPER(ConnectionString);

            string sql = (MasterType == 21)
                ? $"SELECT Code, SUBSTRING(Name, 3, 25) AS Name, 0 AS Price FROM Master1 WHERE MasterType = {MasterType} AND I1 = {VchType} ORDER BY Name"
                : $"SELECT Code, Name, ISNULL(D3, 0) AS Price FROM Master1 WHERE Mastertype = {MasterType} ORDER BY Name";

            DataTable dt1 = obj.getTable(sql);

            List<AccList> lst = new List<AccList>();

            if (dt1 != null && dt1.Rows.Count > 0)
            {
                foreach (DataRow Drr in dt1.Rows)
                {
                    AccList lstObj = new AccList
                    {
                        Code = clsMain.MyInt(Drr["Code"]),
                        Name = clsMain.MyString(Drr["Name"]),
                        Price = clsMain.MyDouble(Drr["Price"])
                    };

                    lst.Add(lstObj);
                }
            }
            return lst;
        }
        public List<AccList> GetPartyItems(string CompCode, string FY, int AccCode)
        {
            string DBName = "Busy" + CompCode + "_db1" + FY;
            string ConnectionString = "Data Source = " + servername + "; Initial catalog = " + DBName + "; Uid = " + Suser + "; Pwd =" + Spass + "";

            SQLHELPER obj = new SQLHELPER(ConnectionString);
            string sql = "";
            List<AccList> lst = new List<AccList>();

            sql = "Select ItemCode,Name from MAPartyItemConfig MA left join Master1 M on MA.itemcode = M.code where AccCode = " + AccCode + "";
            DataTable dt1 = obj.getTable(sql);

            if (dt1 != null && dt1.Rows.Count > 0)
            {
                foreach (DataRow Drr in dt1.Rows)
                {
                    AccList lstObj = new AccList();

                    lstObj.Code = clsMain.MyInt(Drr["ItemCode"]);
                    lstObj.Name = clsMain.MyString(Drr["Name"]);
                    lst.Add(lstObj);
                }
            }
            return lst;
        }
        public GetProductionOrder GetPendingProductionDetails(string CompCode, string FY, int SAccCode, string PONo)
        {
            try
            {
                string DBName = $"Busy{CompCode}_db1{FY}";
                string constr = $"Data Source={servername}; Initial catalog={DBName}; Uid={Suser}; Pwd={Spass}";
                SQLHELPER obj = new SQLHELPER(constr);
                string sql = "";

                if (SAccCode == 0)
                {
                    sql = "Select A.RefNo as PONo, A.AccCode as CustCode, ISNULL(B.[Name], '') AS CustName, IsNull(A.PlanNo,'') AS PlanNo, 0 AS FGItemCode, IsNull(A.FgItem, 0) AS SFGItemCode, A.ItemCode, ISNULL(C.[Name], '') AS ItemName, IsNull(Sum(D.Qty),0) AS RQty,0 AS IQty, 0 AS Price, 0 AS Amount FROM ESReftran A Left join ESRefTran D On A.Rectype = D.Rectype And A.AccCode = D.AccCode And A.RefNo = D.RefNo And A.PlanNo = D.PlanNo And A.FGItem = D.FGItem And A.ItemCode = D.ItemCode Left Join Master1 B ON A.AccCode = B.Code AND B.MasterType = 2 Left Join Master1 C ON A.ItemCode = C.Code Where A.Rectype = 3 AND A.RefNo = '" + PONo + "' AND A.SuplierCode  = -101 And A.Method = 1 GROUP BY A.RefNo, A.PlanNo, A.AccCode, B.[Name], A.FgItem, A.ItemCode, C.[Name] Order By C.[Name]";
                }
                else
                {
                    sql = "Select A.RefNo as PONo, A.AccCode as CustCode, ISNULL(B.[Name], '') AS CustName, IsNull(A.PlanNo,'') AS PlanNo, 0 AS FGItemCode, IsNull(A.FgItem, 0) AS SFGItemCode, A.ItemCode, ISNULL(C.[Name], '') AS ItemName, IsNull(Sum(D.Qty),0) AS RQty,0 AS IQty, 0 AS Price, 0 AS Amount FROM ESReftran A Left join ESRefTran D On A.Rectype = D.Rectype And A.AccCode = D.AccCode And A.RefNo = D.RefNo And A.PlanNo = D.PlanNo And A.FGItem = D.FGItem And A.ItemCode = D.ItemCode Left Join Master1 B ON A.AccCode = B.Code AND B.MasterType = 2 Left Join Master1 C ON A.ItemCode = C.Code Where A.Rectype = 3 AND A.RefNo = '" + PONo + "' AND A.SuplierCode IN (" + SAccCode + ", -101) And A.Method = 1 GROUP BY A.RefNo, A.PlanNo, A.AccCode, B.[Name], A.FgItem, A.ItemCode, C.[Name] Order By C.[Name]";
                }
                System.Data.DataTable table = obj.getTable(sql);

                GetProductionOrder productionOrder = new GetProductionOrder();

                if (table != null && table.Rows.Count > 0)
                {
                    productionOrder.PONo = table.Rows[0]["PONo"].ToString();
                    productionOrder.CustCode = Convert.ToInt32(table.Rows[0]["CustCode"]);
                    productionOrder.CustName = table.Rows[0]["CustName"].ToString();

                    productionOrder.ProductionOrderDetails = new List<GetProductionOrderDetails>();

                    foreach (DataRow row in table.Rows)
                    {
                        productionOrder.ProductionOrderDetails.Add(new GetProductionOrderDetails
                        {
                            PlanNo = row["PlanNo"].ToString(),
                            FGItemCode = Convert.ToInt32(row["FGItemCode"]),
                            SFGItemCode = Convert.ToInt32(row["SFGItemCode"]),
                            ItemCode = Convert.ToInt32(row["ItemCode"]),
                            ItemName = row["ItemName"].ToString(),
                            RQty = Convert.ToDouble(row["RQty"]),
                            IQTy = Convert.ToDouble(row["IQty"]),
                            Price = Convert.ToDouble(row["Price"]),
                            Amount = Convert.ToDouble(row["Amount"])
                        });
                    }
                }
                return productionOrder;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public AlertOrder SaveAutoProductionOrderInv(PostProductionOrder obj, string CompCode, string FY)
        {
            AlertOrder objResult = new AlertOrder();

            try
            {
                object VchCode = 0;
                int MRBusyCode, MIBusyCode, Results;
                string DBName = $"Busy{CompCode}_db1{FY}";
                string constr = $"Data Source={servername}; Initial catalog={DBName}; Uid={Suser}; Pwd={Spass}";
                SQLHELPER ConObj = new SQLHELPER(constr);

                BusyVoucher BusyVch = new BusyVoucher();
                CFixedInterface FI = new CFixedInterface();
                string XMLStr = ""; double InvAmount = 0;
                PostProductionOrder NewInv = obj;
                XMLStr = GetMRInvoiceXML(4, NewInv, ref InvAmount, constr);
                bool Connect = false; FI.CloseDB();

                Connect = FI.OpenCSDBForYear(BusyAppPath, servername, Suser, Spass, CompCode, Convert.ToInt16(FY));

                if (!Connect)
                {
                    objResult.Status = 0; objResult.Msg = "Unable To Connect To Company"; objResult.OrderId = 0;
                    return objResult;
                }

                if (!SaveVoucherFromXML(4, XMLStr, ref VchCode, FI, out string errMsg))
                {
                    objResult.Status = 0; objResult.Msg = errMsg; objResult.OrderId = 0;
                    return objResult;
                }

                MRBusyCode = clsMain.MyInt(VchCode);

                XMLStr = GetMIInvoiceXML(11, NewInv, ref InvAmount, constr);
                if (!SaveVoucherFromXML(11, XMLStr, ref VchCode, FI, out errMsg))
                {
                    objResult.Status = 0; objResult.Msg = errMsg; objResult.OrderId = 0;
                    return objResult;
                }

                MIBusyCode = clsMain.MyInt(VchCode);

                if (MRBusyCode <= 0 || MIBusyCode <= 0)
                {
                    objResult.Status = 0; objResult.Msg = "Posting not done ......."; objResult.OrderId = 0;
                    return objResult;
                }

                int SeriesCode = GetSeriesCode(MIBusyCode, 11, constr);
                var ResultsObj1 = SaveToDbProductionDetails(NewInv, MRBusyCode, MIBusyCode, constr);

                if (ResultsObj1.Status == 1)
                {
                    var ResultsObj = AutoRefGeneratedInProductionOrder(NewInv, MRBusyCode, MIBusyCode, SeriesCode, constr);

                    if (ResultsObj.Status == 1)
                    {
                        objResult.Status = 1; objResult.Msg = "Success"; objResult.OrderId = clsMain.MyInt(VchCode);
                    }
                    else
                    {
                        objResult.Status = 0; objResult.Msg = "Posting not done1 ......."; objResult.OrderId = 0;
                    }
                }
                else 
                {
                    //objResult.Status = 0; objResult.Msg = "Posting not done2 ......."; objResult.OrderId = 0;
                    objResult.Status = 0; objResult.Msg = ResultsObj1.Msg; objResult.OrderId = 0;
                }
            }
            catch (Exception ex)
            {
                objResult.Status = 0; objResult.Msg = ex.Message.ToString(); objResult.OrderId = 0;
            }

            return objResult;
        }

        //public AlertOrder SaveAutoProductionOrderInv(PostProductionOrder obj, string CompCode, string FY)
        //{
        //    AlertOrder objResult = new AlertOrder();
        //    string Msg = ""; int Status = 0; object VchCode = 0;

        //    try
        //    {
        //        int MRBusyCode, MIBusyCode, Results;
        //        string DBName = $"Busy{CompCode}_db1{FY}";
        //        string constr = $"Data Source={servername}; Initial catalog={DBName}; Uid={Suser}; Pwd={Spass}";
        //        SQLHELPER ConObj = new SQLHELPER(constr); int VchType = 4;

        //        BusyVoucher BusyVch = new BusyVoucher();
        //        CFixedInterface FI = new CFixedInterface();
        //        string XMLStr = ""; double InvAmount = 0;
        //        PostProductionOrder NewInv = obj;

        //        XMLStr = GetMRInvoiceXML(VchType, NewInv, ref InvAmount, constr);

        //        bool Connect = false;
        //        FI.CloseDB();

        //        Connect = FI.OpenCSDBForYear(BusyAppPath, servername, Suser, Spass, CompCode, Convert.ToInt16(FY));

        //        if (Connect == true)
        //        {
        //            object Err = "";
        //            bool Return = FI.SaveVchFromXML(VchType, XMLStr, ref Err, false, 0, ref VchCode);

        //            if (Return == true)
        //            {
        //                MRBusyCode = clsMain.MyInt(VchCode); VchType = 11;
        //                if (MRBusyCode > 0)
        //                {
        //                    XMLStr = GetMIInvoiceXML(VchType, NewInv, ref InvAmount, constr);
        //                    bool Return1 = FI.SaveVchFromXML(VchType, XMLStr, ref Err, false, 0, ref VchCode);
        //                    if (Return1 == true)
        //                    {
        //                        MIBusyCode = clsMain.MyInt(VchCode);

        //                        if (MRBusyCode > 0 && MIBusyCode > 0)
        //                        {
        //                            int SeriesCode = GetSeriesCode(MIBusyCode, VchType, constr);
        //                            Results = SaveToDbProductionDetails(NewInv, MRBusyCode, MIBusyCode, constr);
        //                            if (Results == 1)
        //                            {
        //                                var ResultsObj = AutoRefGeneratedInProductionOrder(NewInv, MRBusyCode, MIBusyCode, SeriesCode, constr);

        //                                if (ResultsObj.Status == 1)
        //                                {
        //                                    objResult.Status = 1; objResult.Msg = "Success"; objResult.OrderId = clsMain.MyInt(VchCode);
        //                                }
        //                                else
        //                                {
        //                                    objResult.Status = 0; objResult.Msg = "Posting not done ......."; objResult.OrderId = 0;
        //                                }
        //                            }
        //                            else
        //                            {
        //                                objResult.Status = 0; objResult.Msg = "Posting not done ......."; objResult.OrderId = 0;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            Status = 0; Msg = "Posting not done .......";
        //                        }
        //                    }
        //                    else
        //                    {
        //                        Status = 0; Msg = Err.ToString();
        //                    }
        //                }
        //                else
        //                {
        //                    Status = 0; Msg = "Posting not done .......";
        //                }
        //            }
        //            else
        //            {
        //                Status = 0; Msg = Err.ToString();
        //            }

        //            //if (Return == true)
        //            //{
        //            //    int AccCode = BusyVch.GetMasterNameToCode(constr, NewInv.AccName, 2);
        //            //    Status = 0; Msg = "Success";
        //            //}
        //            //else
        //            //{
        //            //    Status = 0; Msg = Err.ToString();
        //            //}
        //        }
        //        else
        //        {
        //            Status = 0; Msg = "Unable To Connect To Company";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objResult.Status = 0; objResult.Msg = ex.Message.ToString(); objResult.OrderId = 0;
        //        return objResult;
        //    }
        //    objResult.Status = Status; objResult.Msg = Msg; objResult.OrderId = clsMain.MyInt(VchCode);
        //    return objResult;
        //}
        private bool SaveVoucherFromXML(int vchType, string xmlStr, ref object vchCode, CFixedInterface fi, out string errMsg)
        {
            object err = "";
            bool result = fi.SaveVchFromXML(vchType, xmlStr, ref err, false, 0, ref vchCode);
            errMsg = err.ToString();
            if (errMsg == "") 
            {
                return result;
            }
            else
            {
                return false;
            }
        }

        public string GetMRInvoiceXML(int VchType, PostProductionOrder Inv, ref double InvAmount, string ConnectionString)
        {
            string XMLStr = ""; double TaxableAmount = 0;
            try
            {
                string VchSeriesName = "Main";
                BusyVoucher BVch = new BusyVoucher();
                int MCCode = BVch.GetMasterNameToCode(ConnectionString, "Main Store", 11);

                BusyVoucher.MaterialReceipt ORD = new BusyVoucher.MaterialReceipt();
                ORD.VchSeriesName = VchSeriesName; //Inv.SeriesName; //BVch.GetMasterCodeToName(ConnStr, SeriesCode).Replace("12", "");
                ORD.Date = DateTime.UtcNow.ToString("dd-MM-yyyy");
                ORD.VchNo = "";
                ORD.VchType = VchType;
                ORD.StockUpdationDate = ORD.Date;
                ORD.MasterName1 = Inv.SAccName; //BVch.GetMasterCodeToName(ConnStr, clsMain.MyInt(PartyId));
                ORD.MasterName2 = "Main Store"; //Inv.MCName; //BVch.GetMasterCodeToName(ConnStr, MCCode);
                ORD.InputType = 1;
                ORD.TranCurName = "";
                ORD.TmpVchCode = 0;
                ORD.TmpVchSeriesCode = 253;

                ORD.ItemEntries = new List<BusyVoucher.ItemDetail>();
                //ORD.BillSundries = new List<BusyVoucher.BSDetail>();
                //ORD.VchOtherInfoDetails = new BusyVoucher.VchOtherInfoDetails();

                //ORD.VchOtherInfoDetails.Narration1 = clsMain.MyString(Inv.Remarks);
                //ORD.VchOtherInfoDetails.Transport = clsMain.MyString(Inv.Transport);
                //ORD.VchOtherInfoDetails.Station = clsMain.MyString(Inv.Station);

                int SrNo = 0;
                //int ISSrNo = 0;
                foreach (var item in Inv.ItemInvDetails)
                {
                    BusyVoucher.ItemDetail ID = new BusyVoucher.ItemDetail();
                    SrNo = SrNo + 1;
                    ID.SrNo = SrNo;
                    ID.VchType = ORD.VchType;
                    ID.Date = ORD.Date;
                    ID.VchNo = ORD.VchNo;
                    ID.ItemName = item.ItemName; //BVch.GetMasterCodeToName(ConnectionString, clsMain.MyInt(ItemId[i]));
                    ID.UnitName = BVch.GetItemMainUnitName(ConnectionString, clsMain.MyInt(item.ItemCode));
                    ID.ConFactor = 1;
                    ID.Qty = clsMain.MyDouble(item.Qty);
                    ID.QtyMainUnit = clsMain.MyDouble(ID.Qty);
                    ID.QtyAltUnit = 0;
                    ID.ConFactor = 0;
                    ID.ListPrice = 0;
                    ID.Price = clsMain.MyDouble(item.Price);
                    ID.Amt = clsMain.MyDouble(ID.Price * ID.Qty);
                    //ID.DiscountPercent = clsMain.MyDouble(item.DiscPerent);
                    //ID.Discount = clsMain.MyDouble(((ID.Amt * ID.DiscountPercent) / 100).ToString("0.00"));
                    //ID.Amt = clsMain.MyDouble(ID.Amt - ID.Discount);
                    ID.PriceAltUnit = 0;
                    if (ID.QtyAltUnit != 0) { ID.PriceAltUnit = clsMain.MyDouble((clsMain.MyDouble(ID.Amt) / clsMain.MyDouble(ID.QtyAltUnit)).ToString("0.00")); }
                    InvAmount = InvAmount + clsMain.MyDouble(ID.Amt);
                    ID.TmpVchCode = 0;
                    ID.MC = ORD.MasterName2;
                    ID.AF = Inv.PONo;
                    ID.ItemDescInfo = new BusyVoucher.ItemDescInfo();
                    //ID.ItemDescInfo.Description1 = item.PlanNo;
                    //ID.ItemDescInfo.Description2 = item.IDescription2;
                    //ID.ItemDescInfo.Description3 = item.IDescription3;
                    //ID.ItemDescInfo.Description4 = item.IDescription4;
                    //ID.ItemDescInfo.tmpSrNo = SrNo;
                    //ID.JobID = item.PlanNo;
                    //ID.Date = ORD.Date;

                    //ID.ItemSerialNoEntries = new List<BusyVoucher.ItemSerialNoDetail>();
                    //ISSrNo = 0;
                    //foreach (var SerialItem in item.ItemSerailDT)
                    //{
                    //    ISSrNo = ISSrNo + 1;
                    //    BusyVoucher.ItemSerialNoDetail serialNoDetail = new BusyVoucher.ItemSerialNoDetail();
                    //    serialNoDetail.ItemCode = item.ItemCode;
                    //    serialNoDetail.MCCode = MCCode;
                    //    serialNoDetail.VchItemSN = ID.SrNo;
                    //    serialNoDetail.GridSN = ISSrNo;
                    //    serialNoDetail.SerialNo = SerialItem.SerailNo;
                    //    serialNoDetail.MainQty = -1;
                    //    serialNoDetail.AltQty = -1;
                    //    serialNoDetail.MainUnitPrice = ID.Price;
                    //    serialNoDetail.AltUnitPrice = ID.Price;
                    //    ID.ItemSerialNoEntries.Add(serialNoDetail);
                    //}
                    ORD.ItemEntries.Add(ID);
                }
                XMLStr = CreateXML(ORD).Replace("<?xml version=\"1.0\"?>", "").Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
            }
            catch
            {
                return "";
            }

            return XMLStr;
        }

        public string GetMIInvoiceXML(int VchType, PostProductionOrder Inv, ref double InvAmount, string ConnectionString)
        {
            string XMLStr = ""; double TaxableAmount = 0;
            try
            {
                string VchSeriesName = "Main";
                BusyVoucher BVch = new BusyVoucher();
                int MCCode = BVch.GetMasterNameToCode(ConnectionString, "Main Store", 11);

                BusyVoucher.MaterialIssue ORD = new BusyVoucher.MaterialIssue();
                ORD.VchSeriesName = VchSeriesName; //Inv.SeriesName; //BVch.GetMasterCodeToName(ConnStr, SeriesCode).Replace("12", "");
                ORD.Date = DateTime.UtcNow.ToString("dd-MM-yyyy");
                ORD.VchNo = "";
                ORD.VchType = VchType;
                ORD.StockUpdationDate = ORD.Date;
                ORD.MasterName1 = Inv.AccName; //BVch.GetMasterCodeToName(ConnStr, clsMain.MyInt(PartyId));
                ORD.MasterName2 = "Main Store"; //Inv.MCName; //BVch.GetMasterCodeToName(ConnStr, MCCode);
                ORD.InputType = 1;
                ORD.TranCurName = "";
                ORD.TmpVchCode = 0;
                ORD.TmpVchSeriesCode = 253;
                ORD.TranType = 8;
                ORD.ItemEntries = new List<BusyVoucher.ItemDetail>();
                //ORD.BillSundries = new List<BusyVoucher.BSDetail>();
                ORD.VchOtherInfoDetails = new BusyVoucher.VchOtherInfoDetails();

                //ORD.VchOtherInfoDetails.Narration1 = clsMain.MyString(Inv.Remarks);
                //ORD.VchOtherInfoDetails.Transport = clsMain.MyString(Inv.Transport);
                //ORD.VchOtherInfoDetails.Station = clsMain.MyString(Inv.Station);

                int SrNo = 0;
                //int ISSrNo = 0;
                foreach (var item in Inv.ItemInvDetails)
                {
                    BusyVoucher.ItemDetail ID = new BusyVoucher.ItemDetail();
                    ID.ItemDescInfo = new ItemDescInfo();
                    SrNo = SrNo + 1;
                    ID.SrNo = SrNo;
                    ID.VchType = ORD.VchType;
                    ID.Date = ORD.Date;
                    ID.VchNo = ORD.VchNo;
                    ID.ItemName = item.ItemName; //BVch.GetMasterCodeToName(ConnectionString, clsMain.MyInt(ItemId[i]));
                    ID.UnitName = BVch.GetItemMainUnitName(ConnectionString, clsMain.MyInt(item.ItemCode));
                    ID.ConFactor = 1;
                    ID.Qty = clsMain.MyDouble(item.Qty);
                    ID.QtyMainUnit = clsMain.MyDouble(ID.Qty);
                    ID.QtyAltUnit = 0;
                    ID.ConFactor = 0;
                    ID.ListPrice = 0;
                    ID.Price = clsMain.MyDouble(item.Price);
                    ID.Amt = clsMain.MyDouble(ID.Price * ID.Qty);
                    //ID.DiscountPercent = clsMain.MyDouble(item.DiscPerent);
                    //ID.Discount = clsMain.MyDouble(((ID.Amt * ID.DiscountPercent) / 100).ToString("0.00"));
                    //ID.Amt = clsMain.MyDouble(ID.Amt - ID.Discount);
                    ID.PriceAltUnit = 0;
                    if (ID.QtyAltUnit != 0) { ID.PriceAltUnit = clsMain.MyDouble((clsMain.MyDouble(ID.Amt) / clsMain.MyDouble(ID.QtyAltUnit)).ToString("0.00")); }
                    InvAmount = InvAmount + clsMain.MyDouble(ID.Amt);
                    ID.TmpVchCode = 0;
                    ID.MC = ORD.MasterName2;
                    ID.JobID = Inv.PONo;
                    ID.JobDate = ORD.Date;
                    ID.AF = Inv.PONo;
                    ID.ItemDescInfo = new BusyVoucher.ItemDescInfo();
                    //ID.ItemDescInfo.Description1 = item.PlanNo;
                    //ID.ItemDescInfo.Description2 = item.IDescription2;
                    //ID.ItemDescInfo.Description3 = item.IDescription3;
                    //ID.ItemDescInfo.Description4 = item.IDescription4;
                    //ID.ItemDescInfo.tmpSrNo = SrNo;

                    //ID.ItemSerialNoEntries = new List<BusyVoucher.ItemSerialNoDetail>();
                    //ISSrNo = 0;
                    //foreach (var SerialItem in item.ItemSerailDT)
                    //{
                    //    ISSrNo = ISSrNo + 1;
                    //    BusyVoucher.ItemSerialNoDetail serialNoDetail = new BusyVoucher.ItemSerialNoDetail();
                    //    serialNoDetail.ItemCode = item.ItemCode;
                    //    serialNoDetail.MCCode = MCCode;
                    //    serialNoDetail.VchItemSN = ID.SrNo;
                    //    serialNoDetail.GridSN = ISSrNo;
                    //    serialNoDetail.SerialNo = SerialItem.SerailNo;
                    //    serialNoDetail.MainQty = -1;
                    //    serialNoDetail.AltQty = -1;
                    //    serialNoDetail.MainUnitPrice = ID.Price;
                    //    serialNoDetail.AltUnitPrice = ID.Price;
                    //    ID.ItemSerialNoEntries.Add(serialNoDetail);
                    //}
                    ORD.ItemEntries.Add(ID);
                }
                XMLStr = CreateXML(ORD).Replace("<?xml version=\"1.0\"?>", "").Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
            }
            catch
            {
                return "";
            }

            return XMLStr;
        }

        public dynamic SaveToDbProductionDetails(PostProductionOrder inv, int MRVchCode, int MIVchCode, string ConnectionString)
        {
            try
            {
                SQLHELPER ObjCon = new SQLHELPER(ConnectionString);
                DateTime CurrDate = DateTime.Today; string sql = "";

                foreach (var item in inv.ItemInvDetails)
                {
                    ObjCon = new SQLHELPER(ConnectionString);

                    sql = $"insert into ESRefTran([VchCode],[Vchtype],[Rectype],[Method],[AccCode],[RefNo],[Dtdate],[Merchant],[ItemCode],[Qty],[FgItem],[PlanNo],[SuplierCode],[Approval],[BusyCode]) Values (0, 4, 3, 2, {inv.AccCode}, '{inv.PONo}',CONVERT(Date,Getdate(),103) , 0, {item.ItemCode}, {item.Qty * -1}, {item.SFGItemCode}, '', {inv.SAccCode}, 0, {MRVchCode}) ";
                    int result = ObjCon.ExecuteSQL(sql);

                    if (result > 0)
                    {
                        ObjCon = new SQLHELPER(ConnectionString);

                        sql = $"insert into ESRefTran([VchCode],[Vchtype],[Rectype],[Method],[AccCode],[RefNo],[Dtdate],[Merchant],[ItemCode],[Qty],[FgItem],[PlanNo],[SuplierCode],[Approval],[BusyCode]) Values (0, 4, 4, 1, {inv.AccCode}, '{inv.PONo}', CONVERT(Date,Getdate(),103), 0, {item.ItemCode}, {item.Qty}, {item.SFGItemCode}, '', {inv.SAccCode}, 0, {MRVchCode}) ";
                        result = ObjCon.ExecuteSQL(sql);

                        sql = $"insert into ESRefTran([VchCode],[Vchtype],[Rectype],[Method],[AccCode],[RefNo],[Dtdate],[Merchant],[ItemCode],[Qty],[FgItem],[PlanNo],[SuplierCode],[Approval],[BusyCode]) Values (0, 4, 4, 2, {inv.AccCode}, '{inv.PONo}', CONVERT(Date,Getdate(),103), 0, {item.ItemCode}, {item.Qty * -1}, {item.SFGItemCode}, '', {inv.SAccCode}, 0, {MIVchCode}) ";
                        result = ObjCon.ExecuteSQL(sql);
                    }
                    else
                    {
                        return new { Status = 1, Msg = "Unable To Connect To Company" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
            return new { Status = 1, Msg ="Success" };
        }

        public dynamic AutoRefGeneratedInProductionOrder(PostProductionOrder Inv, int MRVchCode, int MIVchCode,int SeriesCode, string ConStr)
        {
            try
            {
                SQLHELPER ConObj = new SQLHELPER(ConStr);
                string sql = ""; double RQty = 0;
                DateTime CurrDate = DateTime.Today;
                int Result = 0; double Qty = 0;

                ConObj = new SQLHELPER(ConStr); 

                //sql = $"select A.RefNo as ProdNo,A.FgItem as SFGItem,(Select Sum(Qty) as RQty From ESRefTran B Where AccCode = {Inv.AccCode} And RefNo = A.RefNo And PlanNo = A.PlanNo And ItemCode = A.FgItem And rectype = 5) as RQty From ESRefTran A where A.AccCode = {Inv.AccCode} And A.RefNo = '{Inv.PONo}' And rectype = 4 And BusyCode = {MRVchCode} group By A.RefNo,A.FgItem Order By A.RefNo";
                sql = $"select A.RefNo as ProdNo,A.FgItem as SFGItem,(Select Sum(Qty) as RQty From ESRefTran B Where AccCode = {Inv.AccCode} And RefNo = A.RefNo And ItemCode = A.FgItem And rectype = 5) as RQty From ESRefTran A where A.AccCode = {Inv.AccCode} And A.RefNo = '{Inv.PONo}' And rectype = 4 And BusyCode = {MRVchCode} group By A.RefNo,A.FgItem Order By A.RefNo";
                DataTable DT1 = ConObj.getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item1 in DT1.Rows)
                    {
                        ConObj = new SQLHELPER(ConStr);
                        Qty = clsMain.MyDouble(item1["RQty"]);

                        //sql = $"select A.FgItem,Sum(A.Qty) as AdjQty From ESRefTran A where AccCode = {Inv.AccCode} And RefNo = '{item1["ProdNo"]}' And PlanNo = '{item1["PlanNo"]}' And ItemCode = {item1["SFGItem"]} And rectype = 5 Group By A.FgItem";
                        sql = $"select A.FgItem,Sum(A.Qty) as AdjQty From ESRefTran A where AccCode = {Inv.AccCode} And RefNo = '{item1["ProdNo"]}' And ItemCode = {item1["SFGItem"]} And rectype = 5 Group By A.FgItem";
                        DataTable DT2 = ConObj.getTable(sql);

                        if (DT2 != null && DT2.Rows.Count > 0)
                        {
                            foreach(DataRow item2 in DT2.Rows)
                            {
                                ConObj = new SQLHELPER(ConStr);
                                RQty = clsMain.MyDouble(item2["AdjQty"]);
                                if (Qty >= RQty)
                                {
                                    sql = $"insert into ESRefTran([VchCode],[Vchtype],[Rectype],[Method],[AccCode],[RefNo],[Dtdate],[Merchant],[ItemCode],[Qty],[FgItem],[PlanNo],[SuplierCode],[Approval],[BusyCode]) Values (0, 11, 5, 2, {Inv.AccCode}, '{item1["ProdNo"]}', CONVERT(Date,Getdate(),103), 0, {item1["SFGItem"]}, {RQty * -1}, {item2["FgItem"]}, '', {Inv.SAccCode}, 0, {MIVchCode}) ";
                                    Result = ConObj.ExecuteSQL(sql);

                                    sql = $"insert into ESRefTran([VchCode],[Vchtype],[Rectype],[Method],[AccCode],[RefNo],[Dtdate],[Merchant],[ItemCode],[Qty],[FgItem],[PlanNo],[SuplierCode],[Approval],[BusyCode]) Values (0, 11, 6, 1, {Inv.AccCode}, '{item1["ProdNo"]}', CONVERT(Date,Getdate(),103), 0, {item1["SFGItem"]}, {RQty}, {item2["FgItem"]}, '', {Inv.SAccCode}, 0, {MIVchCode}) ";
                                    Result = ConObj.ExecuteSQL(sql);
                                }
                                else
                                {
                                    sql = $"insert into ESRefTran([VchCode],[Vchtype],[Rectype],[Method],[AccCode],[RefNo],[Dtdate],[Merchant],[ItemCode],[Qty],[FgItem],[PlanNo],[SuplierCode],[Approval],[BusyCode]) Values (0, 11, 5, 2, {Inv.AccCode}, '{item1["ProdNo"]}', CONVERT(Date,Getdate(),103), 0, {item1["SFGItem"]}, {Qty * -1}, {item2["FgItem"]}, '', {Inv.SAccCode}, 0, {MIVchCode}) ";
                                    Result = ConObj.ExecuteSQL(sql);

                                    sql = $"insert into ESRefTran([VchCode],[Vchtype],[Rectype],[Method],[AccCode],[RefNo],[Dtdate],[Merchant],[ItemCode],[Qty],[FgItem],[PlanNo],[SuplierCode],[Approval],[BusyCode]) Values (0, 11, 6, 1, {Inv.AccCode}, '{item1["ProdNo"]}', CONVERT(Date,Getdate(),103), 0, {item1["SFGItem"]}, {Qty}, {item2["FgItem"]}, '', {Inv.SAccCode}, 0, {MIVchCode}) ";
                                    Result = ConObj.ExecuteSQL(sql);
                                }
                                if (Qty == 0) { break; }
                            }
                        }
                    }
                }

                sql = $"Delete From JobFinishedRefs Where [VchCode] = {MIVchCode} ";
                Result = ConObj.ExecuteSQL(sql);

                ConObj = new SQLHELPER(ConStr);

                sql = $"select A.[RefNo] as ProdNo,[FgItem] as SFGItem,(Select Sum(Qty) as RQty From ESRefTran B Where AccCode = {Inv.AccCode} And RefNo = A.RefNo And ItemCode = A.FGItem And rectype = 5) as RQty From ESRefTran A where A.AccCode = {Inv.AccCode} And A.RefNo = '{Inv.PONo}' And rectype = 4 And BusyCode = {MRVchCode} group By A.RefNo,FgItem Order By A.RefNo";
                DataTable DT = ConObj.getTable(sql);

                if (DT != null && DT.Rows.Count > 0)
                {
                    foreach (DataRow itemDT in DT.Rows)
                    {
                        ConObj = new SQLHELPER(ConStr);
                        RQty = clsMain.MyDouble(itemDT["RQty"]);
                        if (RQty > 0)
                        {
                            sql = $"Insert Into JobFinishedRefs ([JobId], [TranType], [VchCode], [MasterCode1], [MasterCode2], [SrNo], [VchType], [Date], [VchNo], [VchSeriesCode], [CM1], [Value1], [Value2], [C1], [C2], [C3], [Date1], [Rectype]) Values ('{itemDT["ProdNo"]}', 8, {MIVchCode}, {itemDT["SFGItem"]}, {Inv.AccCode}, 1, 11, CONVERT(Date,Getdate(),103), '{Inv.PONo}', {SeriesCode}, 0, {RQty * -1}, {RQty * -1}, '','', '', CONVERT(Date,Getdate(),103) ,2)";
                            Result = ConObj.ExecuteSQL(sql);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
            return new { Status = 1, Msg = "Success" };
        }

        private static string CreateXML(Object YourClassObject)
        {
            XmlDocument xmlDoc = new XmlDocument();   //Represents an XML document, 
            // Initializes a new instance of the XmlDocument class.          
            XmlSerializer xmlSerializer = new XmlSerializer(YourClassObject.GetType());
            // Creates a stream whose backing store is memory. 
            using (MemoryStream xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, YourClassObject);
                xmlStream.Position = 0;
                //Loads the XML document from the specified string.
                xmlDoc.Load(xmlStream);
                return xmlDoc.InnerXml;
            }
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

        public int GetSeriesCode(int VchCode, int VchType, string ConStr)
        {
            int SeriesCode = 0;
            try
            {
                SQLHELPER ConObj = new SQLHELPER(ConStr);
                string sql = $"select distinct A.[VchSeriesCode] as SeriesCode, B.[Name] from Tran1 A Inner Join Master1 B On A.VchSeriesCode = B.Code And B.Mastertype = 21 Where A.VchCode = {VchCode} And A.Vchtype = {VchType} Order By A.VchSeriesCode";
                DataTable DT = ConObj.getTable(sql);
                if (DT != null && DT.Rows.Count > 0)
                {
                    SeriesCode = clsMain.MyInt(DT.Rows[0]["SeriesCode"]);
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            return SeriesCode;
        }
    }
}
