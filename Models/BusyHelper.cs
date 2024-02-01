using System;
using Busy2184;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;
using ESCommon;
using System.Xml.Serialization;
using System.Xml;

namespace CRYSTAL_DRESSES_API.Models
{
    public class BusyHelper
    {
        CFixedInterface FI = new CFixedInterface();

        public BusyHelper()
        {

        }
        public bool ValidateUser(string UserName, string Password, string BusyAppPath, string ServerNm, string SuserNm, String Spass, int FY, string compcode, ref bool SuperUser, ref string Err)
        {
            bool Connect;
            string errstr = "";
            bool Uexist = false;

            Connect = ConnectToBusyN(ref errstr, BusyAppPath, compcode, FY, ServerNm, SuserNm, Spass);
            if (Connect == true)
            {
                Uexist = FI.ValidUserPassword(UserName, Password);
                SuperUser = FI.IfSuperUser(UserName);
            }
            else
            {
                Err = errstr;
            }
            return Uexist;
        }
        private bool ConnectToBusy(string ErrorStr, BusyVoucher.ServerInfo ServerInfo)
        {
            bool Connect = false;
            try
            {
                FI.CloseDB();
                //if (_PrjType == 1)
                //{
                //    Connect = FI.OpenDBForYear("", "", _CompCode, Convert.ToInt16(_FinYear));
                //}
                //else
                //{
                Connect = FI.OpenCSDBForYear(ServerInfo.BusyAppPath, ServerInfo.ServerName, ServerInfo.SUserName, ServerInfo.SPassword, ServerInfo.CompCode, Convert.ToInt16(ServerInfo.FinYear));
                //}

            }
            catch (Exception EX)
            {
                ErrorStr = EX.Message.ToString();
                return false;
            }

            if (Connect == false)
            {
                ErrorStr = "Unable to Connect to Company";
            }
            return Connect;
        }
        private bool ConnectToBusyN(ref string ErrorStr, string _BusyAppPath, string _CompCode, int _FinYear, string _SQLServerName, string _SQLUserName, string _SQLPassword)
        {
            bool Connect = false;
            try
            {
                FI.CloseDB();
                //if (_PrjType == 1)
                //{
                //    Connect = FI.OpenDBForYear("", "", _CompCode, Convert.ToInt16(_FinYear));
                //}
                //else
                //{
                Connect = FI.OpenCSDBForYear(_BusyAppPath, _SQLServerName, _SQLUserName, _SQLPassword, _CompCode, Convert.ToInt16(_FinYear));
                //}

            }
            catch (Exception EX)
            {
                ErrorStr = EX.Message.ToString();
                return false;
            }

            if (Connect == false)
            {
                ErrorStr = "Unable to Connect to Company";
            }
            return Connect;
        }
        public static string CreateXML(Object YourClassObject)
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
    }

}
