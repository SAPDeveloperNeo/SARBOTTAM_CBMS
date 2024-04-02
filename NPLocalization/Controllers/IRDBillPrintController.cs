using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ITNSBOCustomization.Lib.Base;
using ITNSBOCustomization.Lib.Localization;


using System.Globalization;

using ITNSBOCustomization.Lib.Utils;

using Newtonsoft.Json;
using SAPbouiCOM.Framework;
using ITNSBOCustomization.Lib.Core;
using System.Xml;

namespace NPLocalization.Controllers
{
    
    class IRDBillPrintController : FormController
    {
  
        public IRDBillPrintController(string formUID, int formTypeCount) : base(formUID, formTypeCount)
        {
            if (this.oForm.Title == "A/R Invoice")
            {
                RegisterController(this);
            }
        }

        public override void FormEventTrigger(ref SAPbouiCOM.ItemEvent EventData)
        {

        }

        //public override void SBO_Application_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        //{
        //    BubbleEvent = true;
        //    try
        //    {

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        //all form level events
        public static void onFormEvent(ref SAPbouiCOM.ItemEvent EventData)
        {
            //CHECK FOR AR INVOICE
            if ((EventData.BeforeAction == false) && (EventData.EventType == SAPbouiCOM.BoEventTypes.et_FORM_LOAD))
            {
                //Console.WriteLine("creating new ctrl");
                var newController = new IRDBillPrintController(EventData.FormTypeEx, EventData.FormTypeCount);
                newController.logger.Debug("New controller of type {0} registered.", newController.FormCode);
            }

            //form close event, controllers will be unregistered.
            if ((EventData.BeforeAction == false) && (EventData.EventType == SAPbouiCOM.BoEventTypes.et_FORM_CLOSE))
            {
                UnregisterAllControllers(EventData.FormTypeEx, EventData.FormTypeCount);
            }
        }

        public static new void onPrintEvent(ref SAPbouiCOM.PrintEventInfo eventInfo)
        {
            var oForm2 = SAPbouiCOM.Framework.Application.SBO_Application.Forms.ActiveForm;

            var Forms = GetListeningControllers(oForm2.TypeEx, oForm2.TypeCount);
            if (Forms == null)
            {
                return;
            }
            foreach (FormController form in Forms)
            {
                if ((form.GetType() == typeof(IRDBillPrintController)) && eventInfo.BeforeAction == true)
                {
                    form.handlePrintEvent(ref eventInfo);
                }
            }
        }


        public override void handlePrintEvent(ref SAPbouiCOM.PrintEventInfo EventData)
        {
            //try
            //{
            //    var oForm2 = SAPbouiCOM.Framework.Application.SBO_Application.Forms.ActiveForm;

            //    XmlDocument xmlDoc = new XmlDocument();
            //    xmlDoc.LoadXml(oForm2.BusinessObject.Key);

            //    String docEntry = xmlDoc.DocumentElement.ChildNodes[0].InnerText;               
            //    if (Helpers.ApplicationHandlers.PrintMenuUId != "519" && Helpers.ApplicationHandlers.PrintMenuUId!="521")
            //    {

            //        DBUtil.callStoredProc("ITN_UPDATEPRINTCOUNT", docEntry);


            //    }
            //    Helpers.ApplicationHandlers.PrintMenuUId = "";
            try
            {
                var oForm2 = SAPbouiCOM.Framework.Application.SBO_Application.Forms.ActiveForm;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(oForm2.BusinessObject.Key);

                String docEntry = xmlDoc.DocumentElement.ChildNodes[0].InnerText;

                // Check if it's the first click
                //if (Helpers.ApplicationHandlers.PrintMenuUId != "520") /*&& Helpers.ApplicationHandlers.PrintMenuUId == "519" && Helpers.ApplicationHandlers.PrintMenuUId == "521"*/
                //{
                //    // Update PrintMenuUId for the first click
                //    Helpers.ApplicationHandlers.PrintMenuUId = oForm2.UniqueID;
                //}
                //else if (Helpers.ApplicationHandlers.PrintMenuUId != oForm2.UniqueID)
                //{
                //    // Check if it's the second click with a different form
                //    if (Helpers.ApplicationHandlers.PrintMenuUId != "519" && Helpers.ApplicationHandlers.PrintMenuUId != "521")
                //    {
                //        // Update the print count on the second click
                //        DBUtil.callStoredProc("ITN_UPDATEPRINTCOUNT1", docEntry);
                //    }

                //    // Reset PrintMenuUId for the next click
                //    //Helpers.ApplicationHandlers.PrintMenuUId = "";
                //}
                // Check if it's the first click
                // Check if it's the first click
                // Check if it's the first click with "520"
                if (Helpers.ApplicationHandlers.PrintMenuUId != "520")
                {
                    // Update PrintMenuUId for the first click
                    Helpers.ApplicationHandlers.PrintMenuUId = oForm2.UniqueID;
                }
                else
                {
                    // Check if it's the second click with the same form "520"
                    if (Helpers.ApplicationHandlers.PrintMenuUId == "520" && Helpers.ApplicationHandlers.PrintMenuUId == oForm2.UniqueID)
                    {
                        // Update the print count on the second click
                        DBUtil.callStoredProc("ITN_UPDATEPRINTCOUNT1", docEntry);
                    }

                    // Reset PrintMenuUId for the next click
                    Helpers.ApplicationHandlers.PrintMenuUId = "";
                }



                // }
                //catch (Exception ex)
                //{
                //    // Handle exceptions here
                //}




            }
            catch (Exception ex)
            {
                Addon.showStatusBarMessage(ex);
            }
        }



    }
}
