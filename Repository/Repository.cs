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
using System.Transactions;

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

        public dynamic GetProductionOrder(string CompCode, string FY, int SAccCode, string PONo)
        {
            GetProductionOrder productionOrder = new GetProductionOrder();
            try
            {
                string DBName = $"Busy{CompCode}_db1{FY}";
                string constr = $"Data Source={servername}; Initial catalog={DBName}; Uid={Suser}; Pwd={Spass}";
                SQLHELPER obj = new SQLHELPER(constr);
                string sql = "";

                if (SAccCode == 0)
                {
                    sql = $"Select A.RefNo as PONo, A.AccCode as CustCode, ISNULL(B.[Name], '') AS CustName, '' as PlanNo,A.MainFGItem,A.SRawCode,A.Lotno,0 AS FGItemCode, IsNull(A.FgItem, 0) AS SFGItemCode,A.ItemCode as RItemCode,A.ItemCode, ISNULL(C.[Name], '') AS ItemName, C.ParentGrp as IGrp, IsNull(Sum(D.Qty),0) AS RQty,0 AS IQty, 0 AS Price, 0 AS Amount FROM ESReftran A Left join ESRefTran D On A.Rectype = D.Rectype And A.AccCode = D.AccCode And A.RefNo = D.RefNo And A.FGItem = D.FGItem And A.ItemCode = D.ItemCode Left Join Master1 B ON A.AccCode = B.Code AND B.MasterType = 2 Left Join Master1 C ON A.ItemCode = C.Code Where A.Rectype = 3 AND A.RefNo = '{PONo}' AND A.SuplierCode = -101 And A.Method = 1 GROUP BY A.RefNo, A.AccCode, B.[Name], A.FgItem, A.ItemCode, C.[Name], C.[ParentGrp],A.MainFGItem,A.SRawCode,A.Lotno Order By C.[Name]";
                    //sql = "Select A.RefNo as PONo, A.AccCode as CustCode, ISNULL(B.[Name], '') AS CustName, '' as PlanNo, 0 AS FGItemCode, IsNull(A.FgItem, 0) AS SFGItemCode, A.ItemCode, ISNULL(C.[Name], '') AS ItemName, C.ParentGrp as IGrp,IsNull(Sum(D.Qty),0) AS RQty,0 AS IQty, 0 AS Price, 0 AS Amount FROM ESReftran A Left join ESRefTran D On A.Rectype = D.Rectype And A.AccCode = D.AccCode And A.RefNo = D.RefNo And A.FGItem = D.FGItem And A.ItemCode = D.ItemCode Left Join Master1 B ON A.AccCode = B.Code AND B.MasterType = 2 Left Join Master1 C ON A.ItemCode = C.Code Where A.Rectype = 3 AND A.RefNo = '" + PONo + "' AND A.SuplierCode  = -101 And A.Method = 1 GROUP BY A.RefNo, A.AccCode, B.[Name], A.FgItem, A.ItemCode, C.[Name], C.[ParentGrp] Order By C.[Name]";
                }
                else
                {
                    sql = $"Select A.RefNo as PONo, A.AccCode as CustCode, ISNULL(B.[Name], '') AS CustName, '' as PlanNo,A.MainFGItem,A.SRawCode,A.Lotno,0 AS FGItemCode, IsNull(A.FgItem, 0) AS SFGItemCode,A.ItemCode as RItemCode,A.ItemCode, ISNULL(C.[Name], '') AS ItemName, C.ParentGrp as IGrp, IsNull(Sum(D.Qty),0) AS RQty,0 AS IQty, 0 AS Price, 0 AS Amount FROM ESReftran A Left join ESRefTran D On A.Rectype = D.Rectype And A.AccCode = D.AccCode And A.RefNo = D.RefNo And A.FGItem = D.FGItem And A.ItemCode = D.ItemCode Left Join Master1 B ON A.AccCode = B.Code AND B.MasterType = 2 Left Join Master1 C ON A.ItemCode = C.Code Where A.Rectype = 3 AND A.RefNo = '{PONo}' AND A.SuplierCode IN {(SAccCode, -101)} And A.Method = 1 GROUP BY A.RefNo, A.AccCode, B.[Name], A.FgItem, A.ItemCode, C.[Name], C.[ParentGrp],A.MainFGItem,A.SRawCode,A.Lotno Order By C.[Name]";
                    //sql = "Select A.RefNo as PONo, A.AccCode as CustCode, ISNULL(B.[Name], '') AS CustName, '' as PlanNo, 0 AS FGItemCode, IsNull(A.FgItem, 0) AS SFGItemCode, A.ItemCode, ISNULL(C.[Name], '') AS ItemName, C.ParentGrp as IGrp, IsNull(Sum(D.Qty),0) AS RQty,0 AS IQty, 0 AS Price, 0 AS Amount FROM ESReftran A Left join ESRefTran D On A.Rectype = D.Rectype And A.AccCode = D.AccCode And A.RefNo = D.RefNo And A.FGItem = D.FGItem And A.ItemCode = D.ItemCode Left Join Master1 B ON A.AccCode = B.Code AND B.MasterType = 2 Left Join Master1 C ON A.ItemCode = C.Code Where A.Rectype = 3 AND A.RefNo = '" + PONo + "' AND A.SuplierCode IN (" + SAccCode + ", -101) And A.Method = 1 GROUP BY A.RefNo, A.AccCode, B.[Name], A.FgItem, A.ItemCode, C.[Name], C.[ParentGrp] Order By C.[Name]";
                }
                System.Data.DataTable table = obj.getTable(sql);

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
                            PlanNo = Convert.ToString(row["PlanNo"]),
                            MainFGItem = clsMain.MyInt(row["MainFGItem"]),
                            SRawCode = clsMain.MyInt(row["SRawCode"]),
                            LotNo = clsMain.MyString(row["LotNo"]),
                            FGItemCode = Convert.ToInt32(row["FGItemCode"]),
                            SFGItemCode = Convert.ToInt32(row["SFGItemCode"]),
                            IGrp = clsMain.MyInt(row["Igrp"]),
                            RItemCode = clsMain.MyInt(row["RItemCode"]),
                            ItemCode = Convert.ToInt32(row["ItemCode"]),
                            ItemName = row["ItemName"].ToString(),
                            RQty = clsMain.MyDouble(row["RQty"]),
                            IQty = clsMain.MyDouble(row["IQty"]),
                            Price = clsMain.MyDouble(row["Price"]),
                            Amount = clsMain.MyDouble(row["Amount"])
                        });
                    }
                }
                else
                {
                    return new { Status = 0, Msg = "Data Not Found ....." };
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
            return new { Status = 1, Msg = "Success", Data = productionOrder };
        }

        //public GetProductionOrder GetPendingProductionDetails(string CompCode, string FY, int SAccCode, string PONo)
        //{
        //    try
        //    {
        //        string DBName = $"Busy{CompCode}_db1{FY}";
        //        string constr = $"Data Source={servername}; Initial catalog={DBName}; Uid={Suser}; Pwd={Spass}";
        //        SQLHELPER obj = new SQLHELPER(constr);
        //        string sql = "";

        //        if (SAccCode == 0)
        //        {
        //            sql = "Select A.RefNo as PONo, A.AccCode as CustCode, ISNULL(B.[Name], '') AS CustName, '' as PlanNo, 0 AS FGItemCode, IsNull(A.FgItem, 0) AS SFGItemCode, A.ItemCode, ISNULL(C.[Name], '') AS ItemName, C.ParentGrp as IGrp,IsNull(Sum(D.Qty),0) AS RQty,0 AS IQty, 0 AS Price, 0 AS Amount FROM ESReftran A Left join ESRefTran D On A.Rectype = D.Rectype And A.AccCode = D.AccCode And A.RefNo = D.RefNo And A.FGItem = D.FGItem And A.ItemCode = D.ItemCode Left Join Master1 B ON A.AccCode = B.Code AND B.MasterType = 2 Left Join Master1 C ON A.ItemCode = C.Code Where A.Rectype = 3 AND A.RefNo = '" + PONo + "' AND A.SuplierCode  = -101 And A.Method = 1 GROUP BY A.RefNo, A.AccCode, B.[Name], A.FgItem, A.ItemCode, C.[Name], C.[ParentGrp] Order By C.[Name]";
        //        }
        //        else
        //        {
        //            sql = "Select A.RefNo as PONo, A.AccCode as CustCode, ISNULL(B.[Name], '') AS CustName, '' as PlanNo, 0 AS FGItemCode, IsNull(A.FgItem, 0) AS SFGItemCode, A.ItemCode, ISNULL(C.[Name], '') AS ItemName, C.ParentGrp as IGrp, IsNull(Sum(D.Qty),0) AS RQty,0 AS IQty, 0 AS Price, 0 AS Amount FROM ESReftran A Left join ESRefTran D On A.Rectype = D.Rectype And A.AccCode = D.AccCode And A.RefNo = D.RefNo And A.FGItem = D.FGItem And A.ItemCode = D.ItemCode Left Join Master1 B ON A.AccCode = B.Code AND B.MasterType = 2 Left Join Master1 C ON A.ItemCode = C.Code Where A.Rectype = 3 AND A.RefNo = '" + PONo + "' AND A.SuplierCode IN (" + SAccCode + ", -101) And A.Method = 1 GROUP BY A.RefNo, A.AccCode, B.[Name], A.FgItem, A.ItemCode, C.[Name], C.[ParentGrp] Order By C.[Name]";
        //        }

        //        //With Plan Wise
        //        //if (SAccCode == 0)
        //        //{
        //        //    sql = "Select A.RefNo as PONo, A.AccCode as CustCode, ISNULL(B.[Name], '') AS CustName, IsNull(A.PlanNo,'') AS PlanNo, 0 AS FGItemCode, IsNull(A.FgItem, 0) AS SFGItemCode, A.ItemCode, ISNULL(C.[Name], '') AS ItemName, IsNull(Sum(D.Qty),0) AS RQty,0 AS IQty, 0 AS Price, 0 AS Amount FROM ESReftran A Left join ESRefTran D On A.Rectype = D.Rectype And A.AccCode = D.AccCode And A.RefNo = D.RefNo And A.PlanNo = D.PlanNo And A.FGItem = D.FGItem And A.ItemCode = D.ItemCode Left Join Master1 B ON A.AccCode = B.Code AND B.MasterType = 2 Left Join Master1 C ON A.ItemCode = C.Code Where A.Rectype = 3 AND A.RefNo = '" + PONo + "' AND A.SuplierCode  = -101 And A.Method = 1 GROUP BY A.RefNo, A.PlanNo, A.AccCode, B.[Name], A.FgItem, A.ItemCode, C.[Name] Order By C.[Name]";
        //        //}
        //        //else
        //        //{
        //        //    sql = "Select A.RefNo as PONo, A.AccCode as CustCode, ISNULL(B.[Name], '') AS CustName, IsNull(A.PlanNo,'') AS PlanNo,0 AS FGItemCode, IsNull(A.FgItem, 0) AS SFGItemCode, A.ItemCode, ISNULL(C.[Name], '') AS ItemName, IsNull(Sum(D.Qty),0) AS RQty,0 AS IQty, 0 AS Price, 0 AS Amount FROM ESReftran A Left join ESRefTran D On A.Rectype = D.Rectype And A.AccCode = D.AccCode And A.RefNo = D.RefNo And A.PlanNo = D.PlanNo And A.FGItem = D.FGItem And A.ItemCode = D.ItemCode Left Join Master1 B ON A.AccCode = B.Code AND B.MasterType = 2 Left Join Master1 C ON A.ItemCode = C.Code Where A.Rectype = 3 AND A.RefNo = '" + PONo + "' AND A.SuplierCode IN (" + SAccCode + ", -101) And A.Method = 1 GROUP BY A.RefNo, A.PlanNo, A.AccCode, B.[Name], A.FgItem, A.ItemCode, C.[Name] Order By C.[Name]";
        //        //}
        //        System.Data.DataTable table = obj.getTable(sql);

        //        GetProductionOrder productionOrder = new GetProductionOrder();

        //        if (table != null && table.Rows.Count > 0)
        //        {
        //            productionOrder.PONo = table.Rows[0]["PONo"].ToString();
        //            productionOrder.CustCode = Convert.ToInt32(table.Rows[0]["CustCode"]);
        //            productionOrder.CustName = table.Rows[0]["CustName"].ToString();

        //            productionOrder.ProductionOrderDetails = new List<GetProductionOrderDetails>();

        //            foreach (DataRow row in table.Rows)
        //            {
        //                productionOrder.ProductionOrderDetails.Add(new GetProductionOrderDetails
        //                {
        //                    PlanNo = Convert.ToString(row["PlanNo"]),
        //                    FGItemCode = Convert.ToInt32(row["FGItemCode"]),
        //                    SFGItemCode = Convert.ToInt32(row["SFGItemCode"]),
        //                    IGrp = clsMain.MyInt(row["Igrp"]),
        //                    ItemCode = Convert.ToInt32(row["ItemCode"]),
        //                    ItemName = row["ItemName"].ToString(),
        //                    RQty = Convert.ToDouble(row["RQty"]),
        //                    IQty = Convert.ToDouble(row["IQty"]),
        //                    Price = Convert.ToDouble(row["Price"]),
        //                    Amount = Convert.ToDouble(row["Amount"])
        //                });
        //            }
        //        }
        //        return productionOrder;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
        public AlertOrder SaveAutoProductionOrderInv(PostProductionOrder obj, string CompCode, string FY)
        {
            AlertOrder objResult = new AlertOrder();

            try
            {
                object VchCode = 0;
                int MRBusyCode = 0, MIBusyCode = 0, Results = 0;
                string DBName = $"Busy{CompCode}_db1{FY}";
                string constr = $"Data Source={servername}; Initial catalog={DBName}; Uid={Suser}; Pwd={Spass}";

                // Create database helper objects
                SQLHELPER ConObj = new SQLHELPER(constr);
                BusyVoucher BusyVch = new BusyVoucher();
                CFixedInterface FI = new CFixedInterface();

                string XMLStr = ""; double InvAmount = 0; double InvQty = 0;
                PostProductionOrder NewInv = obj;

                // 1. Get STPTName code | 2. Get MR Series Name |  3. Get MI Series Name |  4. Get Material Center
                string STPTName = GetSTPTName(clsMain.MyInt(NewInv.SAccCode), constr);
                string MRSeriesName = GetSeriesConfig(4, constr);
                string MISeriesName = GetSeriesConfig(11, constr);
                string MCName = GetMCConfig(constr);

                // Generate MR invoice XML
                XMLStr = GetMRInvoiceXML(4, NewInv, clsMain.MyString(MRSeriesName), clsMain.MyString(MCName), clsMain.MyString(STPTName), ref InvAmount, constr);

                // Connect to the database
                bool Connect = false; FI.CloseDB();
                Connect = FI.OpenCSDBForYear(BusyAppPath, servername, Suser, Spass, CompCode, Convert.ToInt16(FY));

                if (!Connect)
                {
                    objResult.Status = 0; objResult.Msg = "Unable To Connect To Company"; objResult.OrderId = 0;
                    return objResult;
                }

                // Save MR invoice
                if (!SaveVoucherFromXML(4, XMLStr, ref VchCode, FI, out string errMsg))
                {
                    objResult.Status = 0; objResult.Msg = errMsg; objResult.OrderId = 0;
                    return objResult;
                }

                MRBusyCode = clsMain.MyInt(VchCode);

                // Generate MI invoice XML
                XMLStr = GetMIInvoiceXML(11, NewInv, clsMain.MyString(MISeriesName), clsMain.MyString(MCName), ref InvAmount, ref InvQty, constr);

                // Save MR invoice
                if (!SaveVoucherFromXML(11, XMLStr, ref VchCode, FI, out errMsg))
                {
                    DeleteVoucher(FI, MRBusyCode, out errMsg);
                    objResult.Status = 0; objResult.Msg = errMsg; objResult.OrderId = 0;
                    return objResult;
                }

                MIBusyCode = clsMain.MyInt(VchCode);

                //Check for valid Busy codes
                if (MRBusyCode <= 0 || MIBusyCode <= 0)
                {
                    if (MRBusyCode > 0) { DeleteVoucher(FI, MRBusyCode, out errMsg); }
                    if (MIBusyCode > 0) { DeleteVoucher(FI, MIBusyCode, out errMsg); }

                    objResult.Status = 0; objResult.Msg = "Posting not done ......."; objResult.OrderId = 0;
                    return objResult;
                }

                // Get Series code
                int SeriesCode = GetSeriesCode(MIBusyCode, 11, constr);

                // Call SaveToDbProductionDetails method
                var ResultsObj1 = SaveToDbProductionDetails(NewInv, MRBusyCode, MIBusyCode, InvQty, constr);

                if (ResultsObj1.Status != 1)
                {
                    DeleteVoucher(FI, MRBusyCode, out errMsg);
                    DeleteVoucher(FI, MIBusyCode, out errMsg);
                    objResult.Status = ResultsObj1.Status;
                    objResult.Msg = ResultsObj1.Msg;
                    objResult.OrderId = 0;
                    return objResult;
                }
                // If save succeeds, proceed with AutoRefGeneratedInProductionOrder
                var ResultsObj = AutoRefGeneratedInProductionOrder(NewInv, MRBusyCode, MIBusyCode, SeriesCode, constr);

                if (ResultsObj.Status != 1)
                {
                    DeleteVoucher(FI, MRBusyCode, out errMsg);
                    DeleteVoucher(FI, MIBusyCode, out errMsg);

                    objResult.Status = ResultsObj.Status;
                    objResult.Msg = ResultsObj.Msg;
                    objResult.OrderId = 0;
                    return objResult;
                }

                objResult.Status = 1;
                objResult.Msg = "Success";
                objResult.OrderId = clsMain.MyInt(VchCode);
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
        private bool SaveVoucherFromXML(int VchType, string xmlStr, ref object VchCode, CFixedInterface fi, out string errMsg)
        {
            object err = "";
            bool result = fi.SaveVchFromXML(VchType, xmlStr, ref err, false, 0, ref VchCode);
            errMsg = err?.ToString();
            return errMsg == "" ? result : false ;
        }

        private void DeleteVoucher(CFixedInterface Fi, int VchCode, out string ErrMsg)
        {
            ErrMsg = "";
            object err = null;
            object BusyVchCode = (object)VchCode;

            try
            {
                Fi.DeleteVchByCode(BusyVchCode, ref err);
                if (err != null)
                {
                    ErrMsg = err?.ToString();
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message.ToString();
            }
        }

        //private bool DeleteVoucher(CFixedInterface FI, int VchCode, out string errMsg)
        //{
        //    errMsg = "";
        //    object err = null;
        //    object busyVchCode = (object)VchCode;

        //    try
        //    {
        //        bool success = FI.DeleteVchByCode(busyVchCode, ref err);

        //        if (!success)
        //        {
        //            errMsg = err?.ToString() ?? "Unknown error";
        //        }

        //        return success;
        //    }
        //    catch (Exception ex)
        //    {
        //        errMsg = ex.Message;
        //        return false;
        //    }
        //}

        public string GetMRInvoiceXML(int VchType, PostProductionOrder Inv, string VchSeriesName, string MCName, string STPTName, ref double InvAmount, string ConnectionString)
        {
            string XMLStr = string.Empty;
            try
            {
                BusyVoucher BVch = new BusyVoucher();
                BusyVoucher.MaterialReceipt ORD = new BusyVoucher.MaterialReceipt();
                ORD.VchSeriesName = VchSeriesName; //Inv.SeriesName; //BVch.GetMasterCodeToName(ConnStr, SeriesCode).Replace("12", "");
                ORD.Date = DateTime.UtcNow.ToString("dd-MM-yyyy");
                //ORD.Date = DateTime.UtcNow.ToString("29-04-2023");
                ORD.VchNo = "";
                ORD.VchType = VchType;
                ORD.TranType = 3;
                ORD.STPTName = STPTName;
                ORD.StockUpdationDate = ORD.Date;
                ORD.MasterName1 = Inv.SAccName; //BVch.GetMasterCodeToName(ConnStr, clsMain.MyInt(PartyId));
                ORD.MasterName2 = MCName;
                ORD.InputType = 1;
                ORD.TranCurName = "";
                ORD.TmpVchCode = 0;
                //ORD.TmpVchSeriesCode = 253;
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
                    ID.Price = ID.Price = clsMain.MyDouble(GetBusyItemMasterPrice(ConnectionString, clsMain.MyInt(item.ItemCode)));
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
                ORD.TmpTotalAmt = InvAmount;
                XMLStr = CreateXML(ORD).Replace("<?xml version=\"1.0\"?>", "").Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
            }
            catch
            {
                return "";
            }
            return XMLStr;
        }

        public string GetMIInvoiceXML(int VchType, PostProductionOrder Inv, string VchSeriesName, string MCName, ref double InvAmount, ref double InvQty, string ConnectionString)
        {
            string XMLStr = string.Empty;
            try
            {
                BusyVoucher BVch = new BusyVoucher();
                BusyVoucher.MaterialIssue ORD = new BusyVoucher.MaterialIssue();
                ORD.VchSeriesName = VchSeriesName; //Inv.SeriesName; //BVch.GetMasterCodeToName(ConnStr, SeriesCode).Replace("12", "");
                ORD.Date = DateTime.UtcNow.ToString("dd-MM-yyyy");
                //ORD.Date = DateTime.UtcNow.ToString("29-04-2023");
                ORD.VchNo = "";
                ORD.VchType = VchType;
                ORD.TranType = 8;
                ORD.StockUpdationDate = ORD.Date;
                ORD.MasterName1 = Inv.AccName; //BVch.GetMasterCodeToName(ConnStr, clsMain.MyInt(PartyId));
                ORD.MasterName2 = MCName;
                ORD.InputType = 1;
                ORD.TranCurName = "";
                ORD.TmpVchCode = 0;
                //ORD.TmpVchSeriesCode = 253;
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
                    InvQty = InvQty + clsMain.MyDouble(item.Qty);
                    ID.QtyMainUnit = clsMain.MyDouble(ID.Qty);
                    ID.QtyAltUnit = 0;
                    ID.ConFactor = 0;
                    ID.ListPrice = 0;
                    ID.Price = clsMain.MyDouble(GetBusyItemMasterPrice(ConnectionString, clsMain.MyInt(item.ItemCode)));
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

        public dynamic SaveToDbProductionDetails(PostProductionOrder inv, int MRVchCode, int MIVchCode, double InvQty, string ConStr)
        {
            SQLHELPER ObjCon = new SQLHELPER(ConStr);
            try
            {
                var CurrDate = DateTime.Today.ToString("dd/MMM/yyyy"); string sql = ""; int SNo = 0;

                sql = $"Update Tran1 Set [AutoSync] = 1, [AccCode] = {inv.AccCode} Where VchCode = {MRVchCode} And VchType = 4";
                int res = ObjCon.ExecuteSQL(sql);

                foreach (var item in inv.ItemInvDetails)
                {
                    SNo = SNo + 1;

                    sql = $"Update Tran2 Set SFGItem = {item.SFGItemCode}, FgItem = {item.MainFGItem} where VchCode = {MIVchCode} and Rectype = 2 and SrNo = {SNo} ";
                    int res1 = ObjCon.ExecuteSQL(sql);

                    sql = $"INSERT INTO ESRefTran ([VchCode], [Vchtype], [Rectype], [Method], [AccCode], [RefNo], [Dtdate], [Merchant], [ItemCode], [Qty], [FgItem], [PlanNo], [SuplierCode], [Approval], [MainFGItem], [SRawCode], [LotNo], [BusyCode]) " +
                          $"VALUES (0, 4, 3, 2, {inv.AccCode}, '{inv.PONo}', '{CurrDate}', 0, {item.RItemCode}, {item.Qty * (-1)}, {item.SFGItemCode},' ', {inv.SAccCode}, 0, {item.MainFGItem}, {item.SRawCode}, '{item.LotNo}', {MRVchCode}) ";
                    int result = ObjCon.ExecuteSQL(sql);

                    if (result == 1)
                    {
                        sql = $"INSERT INTO ESRefTran ([VchCode], [Vchtype], [Rectype], [Method], [AccCode], [RefNo], [Dtdate], [Merchant], [ItemCode], [Qty], [FgItem], [PlanNo], [SuplierCode], [Approval], [MainFGItem], [SRawCode], [LotNo], [BusyCode]) " + 
                              $"Values (0, 4, 4, 1, {inv.AccCode}, '{inv.PONo}', '{CurrDate}', 0, {item.ItemCode}, {item.Qty}, {item.SFGItemCode},' ', {inv.SAccCode}, 0, {item.MainFGItem}, {item.SRawCode}, '{item.LotNo}', {MRVchCode}) ";
                        result = ObjCon.ExecuteSQL(sql);

                        sql = $"INSERT INTO ESRefTran ([VchCode], [Vchtype], [Rectype], [Method], [AccCode], [RefNo], [Dtdate], [Merchant], [ItemCode], [Qty], [FgItem], [PlanNo], [SuplierCode], [Approval], [MainFGItem], [SRawCode], [LotNo], [BusyCode]) " +
                              $"Values (0, 4, 4, 2, {inv.AccCode}, '{inv.PONo}', '{CurrDate}', 0, {item.ItemCode}, {item.Qty * (-1)}, {item.SFGItemCode},' ', {inv.SAccCode}, 0, {item.MainFGItem}, {item.SRawCode}, '{item.LotNo}', {MIVchCode}) ";
                        result = ObjCon.ExecuteSQL(sql);

                        sql = $"Insert into ESRawReftran ([VchCode], [DtDate], [Vchtype], [Rectype], [Method], [RefNo], [FGItem], [SfgItem], [SourceRawCode], [RawItemcode], [AccCode], [RawQty], [LotNo]) " +
                              $"Values ({MIVchCode}, '{CurrDate}', 11, 1, 1, '{inv.PONo}', {item.MainFGItem}, {item.SFGItemCode}, {item.SRawCode}, {item.ItemCode}, {inv.AccCode}, {InvQty}, '{item.LotNo}')";
                        result = ObjCon.ExecuteSQL(sql);
                    }
                    else
                    {
                        return new { Status = 0, Msg = "Unable To Connect To Company" };
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
            SQLHELPER ObjExe = new SQLHELPER(ConStr);
            try
            {
                SQLHELPER ConObj = new SQLHELPER(ConStr);
                string sql = ""; int Result = 0;
                DateTime CurrDate = DateTime.Today;


                sql = $"select A.RefNo as ProdNo, A.FgItem as SFGItem, A.MainFgItem, A.LotNo, (Select Sum(Qty) as RQty From ESRefTran B Where AccCode = {Inv.AccCode} And RefNo = A.RefNo And ItemCode = A.FgItem And FgItem = A.MainFgItem And rectype = 5) as RQty From ESRefTran A where A.AccCode = {Inv.AccCode} And A.RefNo = '{Inv.PONo}' And rectype = 4 And BusyCode = {MRVchCode} group By A.RefNo,A.FgItem,A.MainFgItem,A.LotNo Order By A.RefNo" ;
                DataTable DT1 = ConObj.getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow item1 in DT1.Rows)
                    {
                        int ItemCode = clsMain.MyInt(item1["SFGItem"]);
                        int FGItem = clsMain.MyInt(item1["MainFGItem"]);
                        double Qty = clsMain.MyDouble(item1["RQty"]);
                 
                        if (Qty > 0)
                        {
                            sql = $"insert into ESRefTran([VchCode], [Vchtype], [Rectype], [Method], [AccCode], [RefNo], [Dtdate], [Merchant], [ItemCode], [Qty], [FgItem], [PlanNo], [SuplierCode], [Approval], [BusyCode], [LotNo]) Values (0, 11, 5, 2, {Inv.AccCode}, '{item1["ProdNo"]}', CONVERT(Date,Getdate(),103), 0, {ItemCode}, {Qty * -1}, {FGItem}, '', {Inv.SAccCode}, 0, {MIVchCode}, '{clsMain.MyString(item1["LotNo"])}') ";
                            Result = ObjExe.ExecuteSQL(sql);

                            sql = $"insert into ESRefTran([VchCode], [Vchtype], [Rectype], [Method], [AccCode], [RefNo], [Dtdate], [Merchant], [ItemCode], [Qty], [FgItem], [PlanNo], [SuplierCode], [Approval], [BusyCode], [LotNo]) Values (0, 11, 6, 1, {Inv.AccCode}, '{item1["ProdNo"]}', CONVERT(Date,Getdate(),103), 0, {ItemCode}, {Qty}, {FGItem}, '', {Inv.SAccCode}, 0, {MIVchCode}, '{clsMain.MyString(item1["LotNo"])}') ";
                            Result = ObjExe.ExecuteSQL(sql);
                        }
                    }
                }

                sql = $"Delete From JobFinishedRefs Where [VchCode] = {MIVchCode} ";
                Result = ObjExe.ExecuteSQL(sql);

                SQLHELPER ConObj2 = new SQLHELPER(ConStr);
                sql = $"Select A.[RefNo] as ProdNo, ItemCode as SFGItem, [FgItem], Sum(Qty) as RQty From ESRefTran A where A.AccCode = {Inv.AccCode} And A.RefNo = '{Inv.PONo}' And rectype = 6 And BusyCode = {MIVchCode} group By A.RefNo,ItemCode,FgItem Order By A.RefNo ";
                DataTable DT = ConObj2.getTable(sql);

                if (DT != null && DT.Rows.Count > 0)
                {
                    foreach (DataRow row in DT.Rows)
                    {
                        double RQty = clsMain.MyDouble(row["RQty"]);
                        if (RQty > 0)
                        {
                            if (CheckIfExistsInFGItem(clsMain.MyInt(row["SFGItem"]), clsMain.MyInt(row["FgItem"]), ConStr) == 1)
                            {
                                sql = $"Insert Into JobFinishedRefs ([JobId], [TranType], [VchCode], [MasterCode1], [MasterCode2], [SrNo], [VchType], [Date], [VchNo], [VchSeriesCode], [CM1], [Value1], [Value2], [C1], [C2], [C3], [Date1], [Rectype]) Values ('{row["ProdNo"]}', 8, {MIVchCode}, {row["FgItem"]}, {Inv.AccCode}, 1, 11, CONVERT(Date,Getdate(),103), '{Inv.PONo}', {SeriesCode}, 0, {RQty * (-1)}, {RQty * (-1)}, '','', '', CONVERT(Date,Getdate(),103) ,2)";
                            }
                            else
                            {
                                sql = $"Insert Into JobFinishedRefs ([JobId], [TranType], [VchCode], [MasterCode1], [MasterCode2], [SrNo], [VchType], [Date], [VchNo], [VchSeriesCode], [CM1], [Value1], [Value2], [C1], [C2], [C3], [Date1], [Rectype]) Values ('{row["ProdNo"]}', 8, {MIVchCode}, {row["SFGItem"]}, {Inv.AccCode}, 1, 11, CONVERT(Date,Getdate(),103), '{Inv.PONo}', {SeriesCode}, 0, {RQty * (-1)}, {RQty * (-1)}, '','', '', CONVERT(Date,Getdate(),103) ,2)";
                            }
                            Result = ObjExe.ExecuteSQL(sql);
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

        private int CheckIfExistsInFGItem(int Process,int FGItemCode, string ConStr)
        {
            try
            {
                SQLHELPER ConObj = new SQLHELPER(ConStr);
                string sql = $"Select Top 1 * From ESBomConfig Where FGitemCode = {FGItemCode} And ProcessCode = {Process} And FG = 1";
                DataTable DT1 = ConObj.getTable(sql);

                return (DT1 != null && DT1.Rows.Count > 0) ? 1 : 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
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
            try
            {
                SQLHELPER ConObj = new SQLHELPER(ConStr);
                string sql = $"select distinct A.[VchSeriesCode] as SeriesCode, B.[Name] from Tran1 A Inner Join Master1 B On A.VchSeriesCode = B.Code And B.Mastertype = 21 Where A.VchCode = {VchCode} And A.Vchtype = {VchType} Order By A.VchSeriesCode";
                DataTable DT = ConObj.getTable(sql);

                return (DT != null && DT?.Rows.Count > 0) ? clsMain.MyInt(DT.Rows[0]["SeriesCode"]) : 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static string GetSTPTName(int AccCode, string ConnectionString)
        {
            try
            {
                SQLHELPER ConObj = new SQLHELPER(ConnectionString);

                string sql = $"Select A.CM7,B.Name from Master1 A Inner Join Master1 B On A.CM7 = B.Code And B.MasterType = 14 Where A.Code = {AccCode} ";
                DataTable DT = ConObj.getTable(sql);

                return (DT != null && DT?.Rows.Count > 0) ? clsMain.MyString(DT.Rows[0]["Name"]) : ""; 

            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public static string GetSeriesConfig(int VchType, string ConnectionString)
        {
            try
            {
                CFixedInterface F1 = new CFixedInterface();
                SQLHELPER ConObj = new SQLHELPER(ConnectionString);
                string sql = VchType == 4 ? $"select MRSeries,B.[Name] as SeriesName from ESSeriesconfig A Inner Join Master1 B On A.MRSeries = B.Code And B.MasterType = 21" : $"select MISeries,B.[Name] as SeriesName from ESSeriesconfig A Inner Join Master1 B On A.MISeries = B.Code And B.MasterType = 21" ;

                DataTable DT = ConObj.getTable(sql);

                return (DT != null && DT?.Rows.Count > 0) ? F1.MasterName2Series(clsMain.MyString(DT.Rows[0]["SeriesName"])) : "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string GetMCConfig(string ConnStr)
        {
            try
            {
                SQLHELPER ConObj = new SQLHELPER(ConnStr);
                string sql = $"select MCCode,B.[Name] as MCName from ESSeriesconfig A Inner Join Master1 B On A.MCCode = B.Code And B.MasterType = 11";
                DataTable DT = ConObj.getTable(sql);

                return (DT != null && DT?.Rows.Count > 0) ? clsMain.MyString(DT.Rows[0]["MCName"]) : "";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public dynamic GetMRAutoSyncDetails(string CompCode, string FY, int SAccCode, string FDate, string TDate)
        {
            List<GetMRAutoSyncDet> InvAutoSyncDet = new List<GetMRAutoSyncDet>();
            try
            {
                string DBName = $"Busy{CompCode}_db1{FY}";
                string constr = $"Data Source = {servername}; Initial Catalog = {DBName}; Uid = {Suser}; PWd = {Spass}";
                SQLHELPER ConObj = new SQLHELPER(constr);

                string sql = $"Select VchCode,IsNull(A.AccCode,0) as SCode, IsNull(B.[Name],'') as SName,A.[Date] as VchDate,LTrim(A.VchNo) as VchNo,IsNull(A.[VchAmtBaseCur],0) as GrandTot From Tran1 A Left Join Master1 B On A.AccCode = B.Code And B.MasterType = 2 Where A.VchType = 4 And A.MasterCode1 = {SAccCode} And A.Date >= '{FDate}' And A.Date <= '{TDate}' And AutoSync = 1 Order By A.VchCode";
                DataTable DT = ConObj.getTable(sql);

                if (DT != null && DT.Rows.Count > 0)
                {
                    foreach (DataRow row in DT.Rows)
                    {
                        GetMRAutoSyncDet ListObj = new GetMRAutoSyncDet();
                        ListObj.VchCode = clsMain.MyInt(row["VchCode"]);
                        ListObj.SName = clsMain.MyString(row["SName"]);
                        ListObj.VchDate = Convert.ToDateTime(row["VchDate"]).ToString("dd/MMM/yyyy");
                        ListObj.VchNo = clsMain.MyString(row["VchNo"]);
                        ListObj.GrandTot = clsMain.MyDouble(row["GrandTot"]);
                        InvAutoSyncDet.Add(ListObj);
                    }
                }
                else
                {
                    return new { Status = 0, Msg = "Data Not Found ....." };
                }
            }
            catch(Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
            return new { Status = 1, Msg = "Success", Data = InvAutoSyncDet };
        }

        public dynamic GetMRAutoSyncItemsDetails(string CompCode, string FY, int VchCode)
        {
            List<GetMRAutoSyncItemDet> IList = new List<GetMRAutoSyncItemDet>();
            try
            {
                string DBName = $"Busy{CompCode}_db1{FY}";
                string constr = $"Data Source = {servername}; Initial Catalog = {DBName}; Uid = {Suser}; PWd = {Spass}";
                SQLHELPER ConObj = new SQLHELPER(constr);

                string sql = $"select A.SrNo,A.MasterCode1 as ItemCode, B.[Name] as ItemName, IsNull(A.[CM2],0) as UnitCode, C.[Name] as UnitName, ISNULL(A.Value1,0) as Qty,ISNULL(A.D2,0) as Price,ISNULL(A.Value3,0) as Amount From Tran2 A Left Join Master1 B On A.MasterCode1 = B.Code And B.MasterType = 6 Left Join Master1 C On A.CM2 = C.Code And C.MasterType = 8 Where A.VchCode = {VchCode} And A.Rectype = 2 And A.VchType = 4 Order By B.[Name]";
                DataTable DT1 = ConObj.getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach ( DataRow row in DT1.Rows )
                    {
                        GetMRAutoSyncItemDet ListObj = new GetMRAutoSyncItemDet();

                        ListObj.SrNo = clsMain.MyInt(row["SrNo"]);
                        ListObj.ItemName = clsMain.MyString(row["ItemName"]);
                        ListObj.UnitName = clsMain.MyString(row["UnitName"]);
                        ListObj.Qty = clsMain.MyDouble(row["Qty"]);
                        ListObj.Price = clsMain.MyDouble(row["Price"]);
                        ListObj.Amount = clsMain.MyDouble(row["Amount"]);
                        IList.Add(ListObj);
                    }
                }
                else
                {
                    return new { Status = 0, Msg = "Data Not Found ....." };
                }
            }
            catch(Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
            return new { Status = 1, Msg = "Success", Data = IList };
        }

        public dynamic GetBusyItemMasterDt(string CompCode, string FY, int GrpCode)
        {
            List<ItemList> IList = new List<ItemList>();
            try
            {
                string DBName = $"Busy{CompCode}_db1{FY}";
                string constr = $"Data Source = {servername}; Initial Catalog = {DBName}; Uid = {Suser}; PWd = {Spass}";
                SQLHELPER ConObj = new SQLHELPER(constr);

                string sql = $"Select Code,[Name] From Master1 Where ParentGrp = {GrpCode} Group By Code,[Name] Order By [Name]";
                DataTable DT1 = ConObj.getTable(sql);

                if (DT1 != null && DT1.Rows.Count > 0)
                {
                    foreach (DataRow row in DT1.Rows)
                    {
                        ItemList ListObj = new ItemList();
                        ListObj.Code = clsMain.MyInt(row["Code"]);
                        ListObj.Name = clsMain.MyString(row["Name"]);
                        IList.Add(ListObj);
                    }
                }
                else
                {
                    return new { Status = 0, Msg = "Data Not Found ....." };
                }
            }
            catch (Exception ex)
            {
                return new { Status = 0, Msg = ex.Message.ToString() };
            }
            return new { Status = 1, Msg = "Success", Data = IList };
        }

        public double GetBusyItemMasterPrice(string constr,  int ItemCode)
        {
            try
            {
                SQLHELPER ConObj = new SQLHELPER(constr);

                string sql = $"Select Top 1 IsNull(D4, 0) as Price From [Master1] Where [Code] = {ItemCode} And [MasterType] = 6";
                DataTable DT1 = ConObj.getTable(sql);

                return (DT1 != null && DT1?.Rows.Count > 0) ? clsMain.MyDouble(DT1.Rows[0]["Price"]) : 0;
            }
            catch (Exception)
            {
                return 0;
            }

        }
    }
}
