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

        public List<AccList> GetBusyMaster(int MasterType, int VchType, string CompCode, string FY)
        {
            string DBName = "Busy" + CompCode + "_db1" + FY;
            string ConnectionString = "Data Source = " + servername + "; Initial catalog = " + DBName + "; Uid = " + Suser + "; Pwd =" + Spass + "";

            SQLHELPER obj = new SQLHELPER(ConnectionString);
            string sql = "";
            List<AccList> lst = new List<AccList>();

            if (MasterType == 21)
            {
                sql = "SElect Code,substring(Name,3,25) as Name,0 as Price From Master1 Where MasterType = " + MasterType + " And I1 = " + VchType + " Order By Name";
            }
            else
            {
                sql = "Select Code,Name,IsNull(D3,0) as Price from Master1 where Mastertype = " + MasterType + " Order by Name";
            }

            DataTable dt1 = obj.getTable(sql);

            if (dt1 != null && dt1.Rows.Count > 0)
            {
                foreach (DataRow Drr in dt1.Rows)
                {
                    AccList lstObj = new AccList();

                    lstObj.Code = clsMain.MyInt(Drr["Code"]);
                    lstObj.Name = clsMain.MyString(Drr["Name"]);
                    lstObj.Price = clsMain.MyDouble(Drr["Price"]);

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

    }
}
