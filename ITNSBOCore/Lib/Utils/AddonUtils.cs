using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ITNSBOCustomization.Lib.Core;
using SAPbouiCOM.Framework;
using ITNSBOCustomization.Lib.Core;

namespace ITNSBOCustomization.Lib.Utils
{
    public class AddonUtils
    {
        private static SAPbobsCOM.Company oCompany;
        private static string sCookie = null;
        private static string sConnectionContext = null;

        public static SAPbobsCOM.Company getCompany() {
            //return (SAPbobsCOM.Company)Application.SBO_Application.Company.GetDICompany() ;
            int ret = 1;
            if (oCompany == null)
            {
                oCompany = new SAPbobsCOM.Company();
                sCookie = oCompany.GetContextCookie();
                sConnectionContext = Application.SBO_Application.Company.GetConnectionContext(sCookie);
                if (oCompany.Connected == true)
                { 
                    oCompany.Disconnect();
                }
                ret = oCompany.SetSboLoginContext(sConnectionContext);
                if (ret == 0)
                {
                    oCompany.Connect();
                }
            }
            return oCompany;
        }
        
    }
}
