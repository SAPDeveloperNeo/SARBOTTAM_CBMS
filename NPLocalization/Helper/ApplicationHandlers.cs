using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM.Framework;
using System.Threading.Tasks;
using ITNSBOCustomization.SAPB1;
//using NPLocalization.Forms;
using ITNSBOCustomization.Utilities;
//using ITNepal.Addon.SystemForms;
using GlobalVariable;
using System.Net.Mail;
using System.Threading;
using ITNSBOCustomization.Lib.Base;
using NLog;
using Newtonsoft.Json;
using NPLocalization.Forms;

namespace NPLocalization.Helpers
{
    public class ApplicationHandlers
    {
        #region Members
        public static List<B1FormBase> FrmInstances = new List<B1FormBase>();
        private List<string> UDForms = new List<string> { "frm_FILL", "frm_FILL" };
        private static string sCredentials, sSenderEmail, Body, sSubject, sdocentry, squeryS, squeryE, CC;
        static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static string PrintMenuUId = "";

        #endregion

        #region Constructor
        public ApplicationHandlers()
        {
            try
            {
                Application.SBO_Application.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(HandleAppEvents);
                Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(HandleItemEvents);
                Application.SBO_Application.FormDataEvent += new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(FormDataEvent);
                Application.SBO_Application.MenuEvent += new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(MenuEvent);
                Application.SBO_Application.PrintEvent += new SAPbouiCOM._IApplicationEvents_PrintEventEventHandler(PrintEvent);
            }
            catch (Exception ex)
            {
                Utility.LogException(ex);
                //Log.LogException(LogLevel.Error, ex);
            }
        }
        #endregion

        static void HandleAppEvents(SAPbouiCOM.BoAppEventTypes EventType)
        {
            switch (EventType)
            {
                case SAPbouiCOM.BoAppEventTypes.aet_ShutDown:
                    //Exit Add-On
                    System.Windows.Forms.Application.Exit();
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged:
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_FontChanged:
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_LanguageChanged:
                    break;
                case SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition:
                    break;
                default:
                    break;
            }
        }


        static void HandleItemEvents(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.FormType > 0)
            {
                try
                {
                    //Console.WriteLine("HELLO ITEM EVENT");
                    FormController.HandleFormItemEvent(ref pVal);
                }
                catch (Exception ex)
                {
                    // throw ex;
                    showStatusBarMessage(ex);
                }


                //Call function to add UDF when form load event is fire

                /* if ((pVal.BeforeAction == false) && (pVal.EventType == SAPbouiCOM.BoEventTypes.et_FORM_LOAD))
                 {

                     if (pVal.FormType > 0) {
                         new LocalizationController(pVal.FormTypeEx, pVal.FormTypeCount);
                     }


                     //g_funAddUDFs(pVal.FormTypeEx, pVal.FormTypeCount);
                 }*/

            }
        }

        static void FormDataEvent(ref SAPbouiCOM.BusinessObjectInfo BusinessObjectInfo, out bool BubbleEvent)
        {

            BubbleEvent = true;
            try
            {
                BOController.OnDataEvent(ref BusinessObjectInfo);
            }
            catch (Exception ex)
            {
                //throw ex;
                showStatusBarMessage(ex);
            }

            /*if (BusinessObjectInfo.EventType == SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD && BusinessObjectInfo.ActionSuccess && BusinessObjectInfo.Type.Equals("15"))
            {

                SAPbobsCOM.Documents d = (SAPbobsCOM.Documents)B1Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDeliveryNotes);

                string k = BusinessObjectInfo.ObjectKey;

                if (d.Browser.GetByKeys(k))
                {
                    // Fa qualcosa con il documento...
                }
            }*/
        }


        static void MenuEvent(ref SAPbouiCOM.MenuEvent menuEvent, out bool BubbleEvent)
        {

            BubbleEvent = true;
            try
            {
                if (menuEvent.BeforeAction)
                {
                    SBO_Application_MenuEventBefore(ref menuEvent, out BubbleEvent);
                }

                FormController.onMenuEvent(ref menuEvent);
            }
            catch (Exception ex)
            {
                //throw ex;
                showStatusBarMessage(ex);
            }
        }

        private static void SBO_Application_MenuEventBefore(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                //UserFormBase activeForm = null;
                //B1FormBase activeForm;
                PrintMenuUId = "";
                switch (pVal.MenuUID)
                {
                    case "NPLocalization.Forms.UploadBillsToCBMS":
                        UploadBillsToCBMS activeForm = new UploadBillsToCBMS();
                        activeForm.Show();
                        break;
                    case "519":
                        PrintMenuUId = "519";
                        break;
                    case "521":
                        PrintMenuUId = "521";
                        break;


                }
                //if (activeForm != null)
                //{
                //    activeForm.Show();
                //}
            }
            catch (Exception ex)
            {

            }

        }


        static void PrintEvent(ref SAPbouiCOM.PrintEventInfo eventInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                FormController.onPrintEvent(ref eventInfo);
            }
            catch (Exception ex)
            {
                //throw ex;
                showStatusBarMessage(ex);
            }
        }

        /*static FormController getFormController(String formTypeEx, String formTypeCount) {
            return new FormController(0, 0);
        }*/

        static void disposeForm()
        {

        }

        //public static void displayErrorMessage(string msg)
        //{
        //    Application.SBO_Application.StatusBar.SetText(msg, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
        //}

        public static void showStatusBarMessage(Exception ex)
        {
            //Application.SBO_Application.StatusBar.SetText(ex.Message.ToString(), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            logger.Debug(JsonConvert.SerializeObject(ex));
        }

        public static void showStatusBarMessage(string msg, bool isError)
        {
            /*if (isError)
                Application.SBO_Application.StatusBar.SetText(msg, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            else
                Application.SBO_Application.StatusBar.SetText(msg, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            logger.Debug(msg);*/
        }

        #region Methods
        private void Notification_IVT(string docEntry, string docnum, string Notystatus, string Emailstatus)
        {
            string[] FWH;
            string[] TWH;
            string exception = string.Empty;

            SAPbobsCOM.Recordset rs = (SAPbobsCOM.Recordset)B1Helper.DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            string query = "\"max(to_int(ifnull(T0.\"Code\",0))) \"Code\" FROM \"@Z_NOTIF\"  T0";
            rs.DoQuery(query);
            FWH = Globals.Fromwarehouse.Split(',');
            TWH = Globals.Towarehouse.Split(',');
            var code = Convert.ToInt32(rs.Fields.Item("Code").Value.ToString()) + 1;
            query = "insert into \"@Z_NOTIF\" values(" + Convert.ToInt32(code) + " ,'" + code + "','3','940','Inventory Transfer', " +
            " '[ ' || '" + FWH[0].Trim() + "' || '] Transferred materials for your stock request [' || '" + docnum + "' || '] to ' || '" + TWH[0].Trim() + "' ,'/stock-transfer?display=' || '" + docnum + "' ,'" + FWH[0].Trim() + "','" + TWH[0].Trim() + "','N','N','N', now(),'')";

            if (!string.IsNullOrEmpty(Notystatus))
            {
                if (Utility.Left(Notystatus, 7) != "Success")
                {
                    try
                    {
                        rs.DoQuery(query);
                        exception = String.Empty;
                    }
                    catch (Exception ex)
                    {
                        exception = ex.Message;
                    }
                }
                else { exception = String.Empty; }
            }
            else
            {
                try
                {
                    rs.DoQuery(query);
                    exception = String.Empty;
                }
                catch (Exception ex)
                {
                    exception = ex.Message;
                }
            }//T0."U_Email"
            if (exception.Length > 0)
            { query = "update OWTR set \"U_Notification\" = 'Fail : ' || '" + exception + "' where \"DocEntry\" = '" + docEntry + "'"; }
            else { query = "update OWTR set \"U_Notification\" = 'Success' where \"DocEntry\" = '" + docEntry + "'"; }
            rs.DoQuery(query);

        }
        private void Notification_IVTR(string docEntry, string docnum, string Notystatus, string Emailstatus, string Cancel)
        {
            string[] FWH;
            string[] TWH;
            string exception = string.Empty;

            SAPbobsCOM.Recordset rs = (SAPbobsCOM.Recordset)B1Helper.DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            string query = "SELECT max(to_int(ifnull(T0.\"Code\",0))) \"Code\" FROM \"@Z_NOTIF\"  T0";
            rs.DoQuery(query);

            FWH = Globals.Fromwarehouse.Split(',');
            TWH = Globals.Towarehouse.Split(',');
            var code = Convert.ToInt32(rs.Fields.Item("Code").Value.ToString()) + 1;
            query = "insert into \"@Z_NOTIF\" values(" + Convert.ToInt32(code) + " ,'" + code + "','2','1250000940','Inventory Transfer Request', " +
            "  '" + FWH[0].Trim() + "' || ' Cancelled Inventory Transfer Request [' || '" + docnum + "' || '] to ' || '" + TWH[0].Trim() + "' ,'/stock-transfer?display=' || '" + docnum + "' ,'" + FWH[0].Trim() + "','" + TWH[0].Trim() + "','N','N','N',  now(),'')";

            if (!string.IsNullOrEmpty(Notystatus))
            {
                if (Utility.Left(Notystatus, 7) != "Success")
                {
                    try
                    {
                        rs.DoQuery(query);
                        exception = String.Empty;
                    }
                    catch (Exception ex)
                    {
                        exception = ex.Message;
                    }
                }
                else { exception = String.Empty; }
            }
            else
            {
                try
                {
                    rs.DoQuery(query);
                    exception = String.Empty;
                }
                catch (Exception ex)
                {
                    exception = ex.Message;
                }
            }//T0."U_Email"
            if (exception.Length > 0)
            { query = "update OWTQ set \"U_Notification\" = 'Fail : ' || '" + exception + "' where \"DocEntry\" = '" + docEntry + "'"; }
            else { query = "update OWTQ set \"U_Notification\" = 'Success' where \"DocEntry\" = '" + docEntry + "'"; }
            rs.DoQuery(query);

        }
        private void Notification_IVTA(string docEntry, string docnum, string Notystatus, string Emailstatus)
        {
            string FWH;
            string[] TWH;
            string exception = string.Empty;

            SAPbobsCOM.Recordset rs = (SAPbobsCOM.Recordset)B1Helper.DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            string query = "SELECT max(to_int(ifnull(T0.\"Code\",0))) \"Code\" FROM \"@Z_NOTIF\"  T0";
            rs.DoQuery(query);

            FWH = Globals.Fromwarehouse;
            TWH = Globals.Towarehouse.Split(',');
            var code = Convert.ToInt32(rs.Fields.Item("Code").Value.ToString()) + 1;
            query = "insert into \"@Z_NOTIF\" values(" + Convert.ToInt32(code) + " ,'" + code + "','4','651','New Activity Creation', " +
            "  '" + FWH + "' || ' New Activity is Created for Service Call [' || '" + docnum + "' || ']' ,'/call/details/' || '" + docnum + "','" + FWH + "','" + TWH[0].Trim() + "','N','N','N',  now(),'')";

            if (!string.IsNullOrEmpty(Notystatus))
            {
                if (Utility.Left(Notystatus, 7) != "Success")
                {
                    try
                    {
                        rs.DoQuery(query);
                        exception = String.Empty;
                    }
                    catch (Exception ex)
                    {
                        exception = ex.Message;
                    }
                }
                else { exception = String.Empty; }
            }
            else
            {
                try
                {
                    rs.DoQuery(query);
                    exception = String.Empty;
                }
                catch (Exception ex)
                {
                    exception = ex.Message;
                }
            }//T0."U_Email"
            if (exception.Length > 0)
            { query = "update OCLG set \"U_Notification\" = 'Fail : ' || '" + exception + "' where \"ClgCode\" = '" + docEntry + "'"; }
            else { query = "update OCLG set \"U_Notification\" = 'Success' where \"ClgCode\" = '" + docEntry + "'"; }
            rs.DoQuery(query);

        }
        private void Notification_IVTS(string docEntry, string docnum, string Notystatus, string Emailstatus, ref string exception)
        {
            string FWH;
            string[] TWH;


            SAPbobsCOM.Recordset rs = (SAPbobsCOM.Recordset)B1Helper.DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            string query = "SELECT max(to_int(ifnull(T0.\"Code\",0))) \"Code\" FROM \"@Z_NOTIF\"  T0";
            rs.DoQuery(query);

            FWH = Globals.Fromwarehouse;
            TWH = Globals.Towarehouse.Split(',');
            var code = Convert.ToInt32(rs.Fields.Item("Code").Value.ToString()) + 1;
            query = "insert into \"@Z_NOTIF\" values(" + Convert.ToInt32(code) + " ,'" + code + "','5','60110','Service Call ', " +
            "  '" + FWH + "' || ' Activity is Assigned for Service Call [' || '" + docnum + "' || ']' ,'/call/details/' || '" + docnum + "','" + FWH + "','" + TWH[0].Trim() + "','N','N','N',  now(),'')";

            if (!string.IsNullOrEmpty(Notystatus))
            {
                if (Utility.Left(Notystatus, 7) != "Success")
                {
                    try
                    {
                        rs.DoQuery(query);
                        exception = String.Empty;
                    }
                    catch (Exception ex)
                    {
                        exception = ex.Message;
                    }
                }
                else { exception = String.Empty; }
            }
            else
            {
                try
                {
                    rs.DoQuery(query);
                    exception = String.Empty;
                }
                catch (Exception ex)
                {
                    exception = ex.Message;
                }
            }//T0."U_Email"


        }

        // public long SendEmailNotification(string sCredentials, string sSenderEmail, string sBody, string sSubject, ref string sErrDesc)
        public static void SendEmailNotification()
        {
            string sFuncName = String.Empty;
            SmtpClient oSmtpServer = new SmtpClient();
            MailMessage oMail = new MailMessage();
            string[] split;
            string[] Email;
            string p_SyncDateTime = string.Empty;
            Email = CC.Split(',');
            SAPbobsCOM.Recordset rs = (SAPbobsCOM.Recordset)B1Helper.DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            split = sCredentials.Split(';');  // 0-port 1-server 2-username 3-password 4-emailform 5- ssl
            try
            {

                oSmtpServer.Credentials = new System.Net.NetworkCredential(split[2], split[3]);
                oSmtpServer.Port = Convert.ToInt32(split[0]);
                oSmtpServer.Host = split[1];

                if (split[5] == "Y")
                { oSmtpServer.EnableSsl = true; }
                else { oSmtpServer.EnableSsl = false; }

                oMail.From = new MailAddress(split[4]);
                if (Email.Length > 0)
                { sSenderEmail += "," + CC; }

                oMail.To.Add(sSenderEmail);

                oMail.Subject = sSubject;
                // oMail.Body = Mail_Body(sBody)
                oMail.Body = Body;
                oMail.IsBodyHtml = true;

                oSmtpServer.Send(oMail);
                oMail.Dispose();
                rs.DoQuery(string.Format(squeryS, sdocentry));

                // return 1;
            }
            catch (Exception ex)
            {
                rs.DoQuery(string.Format(squeryE, sdocentry, ex.Message));
                //sErrDesc = ("Please check the email Address." + ("Fail to send email : "
                //            + (sSenderEmail + (":: " + ex.Message))));

                // return 0;
            }

        }


        public static void CallToChildThread(string sCredentials, string sSenderEmail, string sBody, string sSubject, ref string sErrDesc)
        {
            Console.WriteLine("Child thread starts");

            // the thread is paused for 5000 milliseconds
            int sleepfor = 5000;

            Console.WriteLine("Child Thread Paused for {0} seconds", sleepfor / 1000);
            Thread.Sleep(sleepfor);
            Console.WriteLine("Child thread resumes");
        }

        #endregion

    }
}
