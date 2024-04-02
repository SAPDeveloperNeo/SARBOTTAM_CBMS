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

namespace ITNSBOCustomization.NPLocalization.Controllers
{

    class LocalizationController : FormController
    {
        private LocalizationConfiguration LocalizationData;

        public LocalizationController(String formUID, int formTypeCount) : base(formUID, formTypeCount)
        {
            //this.logger.Debug("Activated form : {0}", this.GetPrimaryDataSource());
            LocalizationData = LocalizationManagement.getLocalizationConfig(this.FormCode);
            if (this.oForm.Title == "A/P Invoice")
            {
                ((SAPbouiCOM.EditText)(this.oForm.Items.Item("10").Specific)).Value = DateTime.Now.Date.ToString("yyyyMMdd");
            }

            if (LocalizationData != null)
            {
                this.addDateFields();
            }

            RegisterController(this);
            //this.logger.Debug(oForm.GetAsXML());
        }


        //specific events inside the form
        public override void FormEventTrigger(ref SAPbouiCOM.ItemEvent EventData)
        {

            //check if form is supposed to be localized and item that just lost focus is the defined english date field.
            if (LocalizationData != null
                && EventData.EventType == SAPbouiCOM.BoEventTypes.et_LOST_FOCUS
                && EventData.ItemUID == LocalizationData.DateChangeTriggeringItem)
            {
                this.setItemEnabledMode("txtNPDate", false);
                this.onDateChange();
            }

            //if (ReferenceFieldsExist())
            //{
            //    if (LocalizationData != null
            //        && EventData.EventType == SAPbouiCOM.BoEventTypes.et_FORM_ACTIVATE
            //        && this.oForm.Items.Item("txtNPDate").Enabled == true)
            //    {
            //        this.setItemEnabledMode("txtNPDate", false);
            //    }
            //}
        }

        public static new void onMenuEvent(ref SAPbouiCOM.MenuEvent eventData)
        { 
            //  SAPbouiCOM.Form oForm2 = Application.SBO_Application.Forms.ActiveForm.UDFFormUID;

            var Forms = GetListeningControllers((Application.SBO_Application.Forms.ActiveForm.TypeEx), (Application.SBO_Application.Forms.ActiveForm.TypeCount));
            if (Forms == null)
            {
                return;
            }
            foreach (FormController form in Forms)
            {
                form.handleMenuEvent(ref eventData);
            }
        }

        public override void handleMenuEvent(ref SAPbouiCOM.MenuEvent eventData)
        {
            switch (eventData.MenuUID)
            {
                case "1290":
                    this.setItemEnabledMode("txtNPDate", false);
                    break;
                case "1289":
                    this.setItemEnabledMode("txtNPDate", false);
                    break;
                case "1288":
                    this.setItemEnabledMode("txtNPDate", false);
                    break;
                case "1291":
                    this.setItemEnabledMode("txtNPDate", false);
                    break;
                case "1282":
                    this.setItemEnabledMode("txtNPDate", false);
                    if (this.oForm.Title == "A/P Invoice")
                    {
                        ((SAPbouiCOM.EditText)(this.oForm.Items.Item("10").Specific)).Value = DateTime.Now.Date.ToString("yyyyMMdd");
                    }
                    this.onDateChange();
                    break;
                case "1281":
                    this.setItemEnabledMode("txtNPDate", false);
                    break;
                case "1304":
                    this.setItemEnabledMode("txtNPDate", false);
                    break;
                case "1287":
                    this.setItemEnabledMode("txtNPDate", false);
                    this.onDateChange();
                    break;
                default:
                    return;
            }

        }

        public static new void HandleFormItemEvent(ref SAPbouiCOM.ItemEvent EventData)
        {

            //form load event. all relevant controllers should be created at this stage
            //TODO : use reflection to identify which all controllers to load at this point.

        }



        //all form level events
        public static void onFormEvent(ref SAPbouiCOM.ItemEvent EventData)
        {
            if ((EventData.BeforeAction == false) && (EventData.EventType == SAPbouiCOM.BoEventTypes.et_FORM_LOAD))
            {
                //Console.WriteLine("creating new ctrl");
                var newController = new LocalizationController(EventData.FormTypeEx, EventData.FormTypeCount);
                newController.logger.Debug("New controller of type {0} registered.", newController.FormCode);
            }

            //all events occuring in the form lifetime
            var Forms = GetListeningControllers(EventData.FormTypeEx, EventData.FormTypeCount); //ActiveFormControllers[EventData.FormTypeEx][EventData.FormTypeCount];
            if (Forms != null)
            {
                foreach (FormController FormCtrl in Forms)
                {
                    FormCtrl.FormEventTrigger(ref EventData);
                }
            }

            if (EventData.FormTypeEx == "426" && EventData.EventType == SAPbouiCOM.BoEventTypes.et_VALIDATE  && EventData.BeforeAction == true  && EventData.ItemUID == "5")
            {
                try {
                     SAPbouiCOM.Form oForm = Application.SBO_Application.Forms.GetForm(EventData.FormTypeEx, EventData.FormTypeCount );
                    String adString = ((SAPbouiCOM.EditText)(oForm.Items.Item("10").Specific)).Value; //oForm.DataSources.DBDataSources.Item(0).GetValue("DocDate", 0);
                    String bsString = DateUtil.ConvertToBS(adString);
                    ((SAPbouiCOM.EditText)(oForm.Items.Item("txtNPDate").Specific)).Value = bsString;
                }
                catch (Exception Ex)
                { }

            }

            if (EventData.FormTypeEx == "170" && EventData.EventType == SAPbouiCOM.BoEventTypes.et_VALIDATE && EventData.BeforeAction == true && EventData.ItemUID == "5")
            {
                try
                {
                    SAPbouiCOM.Form oForm = Application.SBO_Application.Forms.GetForm(EventData.FormTypeEx, EventData.FormTypeCount);
                    String adString = ((SAPbouiCOM.EditText)(oForm.Items.Item("10").Specific)).Value; //oForm.DataSources.DBDataSources.Item(0).GetValue("DocDate", 0);
                    String bsString = DateUtil.ConvertToBS(adString);
                    ((SAPbouiCOM.EditText)(oForm.Items.Item("txtNPDate").Specific)).Value = bsString;
                }
                catch (Exception Ex)
                { }

            }

            //form close event, controllers will be unregistered.
            if ((EventData.BeforeAction == false) && (EventData.EventType == SAPbouiCOM.BoEventTypes.et_FORM_CLOSE))
            {
                UnregisterAllControllers(EventData.FormTypeEx, EventData.FormTypeCount);
            }
        }

        //public static void onEvent(ref SAPbouiCOM.BusinessObjectInfo boInf, ref bool BubbleEvent)
        //{
        //    var oCompany = AddonUtils.getCompany();

        //    if ((boInf.FormTypeEx == "133") 
        //        && (boInf.EventType == SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD)
        //        && (boInf.BeforeAction == true))
        //    {
        //        oCompany.StartTransaction();
        //        BubbleEvent = false;
        //    }

        //    if ((boInf.FormTypeEx == "133") && (boInf.EventType == SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD))
        //    {
        //        if((boInf.BeforeAction == false) && (boInf.ActionSuccess))
        //        {

        //        }
        //    }
        //}

        public bool ReferenceFieldsExist()
        {
            try
            {
                var labelReference = oForm.Items.Item(LocalizationData.DateEditItemReference);
                var editTextReference = oForm.Items.Item(LocalizationData.DateEditItemReference);
                if (labelReference != null && editTextReference != null)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                //Addon.showStatusBarMessage(ex);
            }


            return false;
        }

        private void addDateFields()
        {
            if (!this.ReferenceFieldsExist())
            {
                return;
            }

            try
            {
                this.logger.Debug("Adding fields for the object: {0}", this.FormCode);
                SAPbouiCOM.Item oItem = default(SAPbouiCOM.Item);
                SAPbouiCOM.EditText oEditText = default(SAPbouiCOM.EditText);
                SAPbouiCOM.StaticText oStaticText = default(SAPbouiCOM.StaticText);

                this.FreezeForm();

                this.AddStaticText("lblNPDate", oForm.Items.Item(LocalizationData.DateLabelItemReference).Left,
                                    oForm.Items.Item(LocalizationData.DateLabelItemReference).Width,
                                    oForm.Items.Item(LocalizationData.DateLabelItemReference).Height,
                                    oForm.Items.Item(LocalizationData.DateLabelItemReference).Top,
                                    "Nepali Date");

                this.AddEditText("txtNPDate", oForm.Items.Item(LocalizationData.DateEditItemReference).Left,
                                    oForm.Items.Item(LocalizationData.DateEditItemReference).Width,
                                    oForm.Items.Item(LocalizationData.DateEditItemReference).Height,
                                    oForm.Items.Item(LocalizationData.DateEditItemReference).Top,
                                    "lblNPDate", this.FormCode, "U_ITN_NPDate", false);



                this.onDateChange();

                this.UnfreezeForm();
            }
            catch (Exception ex)
            {
                this.logger.Debug("Error while Adding fields for the object: {0}; exception info : {1}", this.FormCode, JsonConvert.SerializeObject(ex));
            }

        }

        private void onDateChange()
        {
            if (!this.ReferenceFieldsExist())
            {
                return;
            }
            String adString = ((SAPbouiCOM.EditText)(oForm.Items.Item(LocalizationData.DateChangeTriggeringItem).Specific)).Value; //oForm.DataSources.DBDataSources.Item(0).GetValue("DocDate", 0);

            String bsString = DateUtil.ConvertToBS(adString);

            ((SAPbouiCOM.EditText)(oForm.Items.Item("txtNPDate").Specific)).Value = bsString;

        }

        ~LocalizationController()
        {
                UnregisterController(this);
        }

        public static void updateJournalEntry(SAPbobsCOM.Company oCompany, ref bool success, SAPbouiCOM.BusinessObjectInfo boInf)
        {
            try
            {
                string objectKey = GetDocEntryFromXML(boInf.ObjectKey);

                SAPbobsCOM.Documents oDocuments;
                SAPbobsCOM.BoObjectTypes objType = (SAPbobsCOM.BoObjectTypes)Enum.Parse(typeof(SAPbobsCOM.BoObjectTypes), boInf.Type);

                oDocuments = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(objType);

                if (oDocuments.GetByKey(Convert.ToInt32(objectKey)))
                {
                    string udfValueInDoc = null;
                    for (int i = 0; i < oDocuments.UserFields.Fields.Count - 1; i++)
                    {
                        if (oDocuments.UserFields.Fields.Item(i).Name == "U_ITN_NPDate")
                            udfValueInDoc = oDocuments.UserFields.Fields.Item("U_ITN_NPDate").Value.ToString();

                    }
                    if (udfValueInDoc == null)
                        return;

                    SAPbobsCOM.JournalEntries oJE = (SAPbobsCOM.JournalEntries)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);
                    if (oJE.GetByKey(Convert.ToInt32(oDocuments.TransNum)))
                    {
                        oJE.UserFields.Fields.Item("U_ITN_NPDate").Value = udfValueInDoc;
                        oJE.Update();

                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Addon.showStatusBarMessage(ex);
            }

        }

        private static string GetDocEntryFromXML(string sXML)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(sXML);
            var docNode = xDoc.SelectSingleNode("DocumentParams");
            if (docNode != null)
            {
                string sDocEntry = docNode.ChildNodes[0].InnerText;
                if (sDocEntry != "")
                {
                    return sDocEntry;
                }
                else
                {
                    return "-1";
                }
            }
            else
                return "";

        }

    }
}