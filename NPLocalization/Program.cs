using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ITNSBOCustomization.Lib.Core;
using System.Reflection;
using SAPbouiCOM.Framework;
using NPLocalization.Forms;
using System.Configuration;
using NPLocalization.Helpers;
using NPLocalization.Helper;
using System.IO;

namespace NPLocalization
{
    class Program
    {
        #region Variables

        public static SAPbobsCOM.Company oCompany;
        public static SAPbouiCOM.Application SBO_Application;
        public static SAPbouiCOM.Form oForm { get; set; }

        #endregion

        [STAThread]
        static void Main(string[] args)
        {
            Application oApp = null;
            if (args.Length < 1)
                oApp = new Application();
            else
                oApp = new Application(args[0]);

            SBO_Application = SAPbouiCOM.Framework.Application.SBO_Application;
            oCompany = (SAPbobsCOM.Company)SBO_Application.Company.GetDICompany();

            Menu MyMenu = new Menu();
            MyMenu.AddMenuItems();
            if (ConfigurationManager.AppSettings["UDF"].ToString()  == "N")
            {
               // AddonInfo.InstallUDOs();
            }

            ApplicationHandlers applicationHandler = new ApplicationHandlers();
            Application.SBO_Application.StatusBar.SetSystemMessage("CBMS Add-ons installed successfully.", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            oApp.Run();
        }

    }
}
