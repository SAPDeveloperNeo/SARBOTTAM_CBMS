using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SAPbouiCOM.Framework;

//using ITNSBOCustomization.Controllers;
using Newtonsoft.Json;

using ITNSBOCustomization.Lib.Base;
using NLog;

namespace ITNSBOCustomization.Lib.Core
{
    public class Addon
    {
        public static Application oApp;
        static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public delegate void MenuItems();
        

        public Addon(string[] args) {
            InitializeApp(args);
            RunApp();
        }

        public Addon(string[] args, Type t, SAPbouiCOM._IApplicationEvents_MenuEventEventHandler menuEventHandler)
        {
            //t.addMenuItems();
            InitializeApp(args);
            t.GetMethod("addMenuItems").Invoke(null, null);
            oApp.RegisterMenuEventHandler(menuEventHandler);

            RunApp();
        }

        public void InitializeApp(string[] args) {
            Console.WriteLine("Addon speaking");
            Console.WriteLine(JsonConvert.SerializeObject(args));
            if (args.Length < 1)
            {
                oApp = new Application();
            }
            else
            {
                oApp = new Application(args[0]);
            }
        }

        static void RunApp() {
            Application.SBO_Application.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(HandleAppEvents);


            //form events
            Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(HandleItemEvents);

            //business object events
            Application.SBO_Application.FormDataEvent += new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(FormDataEvent);


            // Application.SBO_Application.MenuEvent += new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(FormDataEvent);
            Application.SBO_Application.MenuEvent += new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(MenuEvent);
            //adding print event
            Application.SBO_Application.PrintEvent += new SAPbouiCOM._IApplicationEvents_PrintEventEventHandler(PrintEvent);

            //MenuItems addMenuItems = new MenuItems(Menu.addMenuItems);
            oApp.Run();
            Console.WriteLine("Addon speaking");
        }

        static void HandleAppEvents(SAPbouiCOM.BoAppEventTypes EventType) {
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
            catch (Exception ex) {
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
                FormController.onMenuEvent(ref menuEvent);
            }
            catch (Exception ex)
            {
                //throw ex;
                showStatusBarMessage(ex);
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
    }
}
