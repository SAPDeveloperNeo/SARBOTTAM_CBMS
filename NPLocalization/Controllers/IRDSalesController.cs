using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ITNSBOCustomization.Lib.Base;
using SAPbouiCOM;
using Newtonsoft.Json;
using ITNSBOCustomization.Lib.Localization;
using System.Collections.ObjectModel;
using System.Xml;

using System.Net;
using ITNSBOCustomization.Lib.Utils;
using ITNSBOCustomization.Lib.Core;
using NLog;

namespace ITNSBOCustomization.NPLocalization.Controllers
{
    class IRDSalesController : BOController
    {
        const string OBJCODE_AR_INV = "13";
        const string OBJCODE_AR_CREDIT = "14";
        static Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public IRDSalesController(String BOCode) : base(BOCode)
        {
            BusinessObjectCode = BOCode;
        }


        //fired whenever an business object event is received by the addon. 
        public static new void onEvent(ref BusinessObjectInfo boInf)
        {
            if (CBMSIntegration.BORequiresCompliance(boInf.Type))
            {
                //route the data event to appropriate functions in the controllers.
                routeDataEvent(ref boInf);

            }

            //SAPbobsCOM.Company oCompany = AddonUtils.getCompany();

            //if ((boInf.EventType == SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD)
            //&& (boInf.BeforeAction == true))
            //{
            //    if (!oCompany.InTransaction)
            //        oCompany.StartTransaction();
            //    //BubbleEvent = false;

            //}

            //if ((boInf.EventType == SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD))
            //{
            //    if ((boInf.BeforeAction == false) && (boInf.ActionSuccess))
            //    {
            //        bool success = true;
            //        //var localCtrl = new LocalizationController()
            //        //LocalizationController.updateJournalEntry(oCompany, ref success, boInf);
            //        try
            //        {
            //            if (success)
            //                oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
            //            else
            //                oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
            //        }
            //        catch (Exception ex)
            //        {
            //            Addon.showStatusBarMessage(ex);
            //        }
            //    }
            //}

        }

        //handles data events to cater to the customization. For IRD compliance, it's getting fired 
        //once the sales invoice and return documents are added in the SAP application.
        private static void routeDataEvent(ref BusinessObjectInfo boInf)
        {
            Console.WriteLine("Routed data event to action on custom component");
            switch (boInf.EventType)
            {
                case BoEventTypes.et_FORM_DATA_ADD:
                    if (boInf.ActionSuccess)
                    {
                        var boCtrl = new IRDSalesController(boInf.Type);
                        boCtrl.onDataAddEvent(ref boInf);
                    }
                    break;
                case BoEventTypes.et_FORM_DATA_UPDATE:
                    if (boInf.ActionSuccess)
                    {
                        var boCtrl = new IRDSalesController(boInf.Type);
                        boCtrl.onDataAddEvent(ref boInf);
                    }
                    break;

                default:
                    break;
            }
        }
        //Lokesh
        public void onDataAddEvent(ref BusinessObjectInfo boInf)
        {
            try
            {
                switch (boInf.Type)
                {
                    case OBJCODE_AR_INV:
                        string docEntry = GetDocEntryFromXML(boInf.ObjectKey);
                        var response = sendBillToCBMS(docEntry);
                        setSyncStatusOINV(response.isSuccess, docEntry);
                        //SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(response.responseMsg);
                        break;

                    case OBJCODE_AR_CREDIT:
                        string returnDocEntry = GetDocEntryFromXML(boInf.ObjectKey);
                        var returnResponse = sendBillReturnToCBMS(returnDocEntry);
                        setSyncStatusORIN(returnResponse.isSuccess, returnDocEntry);
                        SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(returnResponse.responseMsg);
                        break;
                }
            }
            catch (Exception ex)
            { SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(ex.Message); }
        }



        /*public override void DataEventTrigger(ref SAPbouiCOM.BusinessObjectInfo BusinessObjectInfo)
        {
            Console.WriteLine("Event received at IRD sales ctrl: {0}", BusinessObjectInfo.EventType);
            if (BusinessObjectInfo.EventType == SAPbouiCOM.BoEventTypes.et_FORM_DATA_ADD && BusinessObjectInfo.ActionSuccess )
            {
                Console.WriteLine("Sales Invoice Add Success Event Captured");
            }
        }*/

        public static CBMSParsedReponse sendBillToCBMS(string docEntry)
        {
            try
            {
                SalesDataObject salesData = null;

                SAPbobsCOM.Recordset recordSet = DBUtil.callStoredProc("ITN_GET_INVOICE_DATA_DETAILS", docEntry);
                if (recordSet.RecordCount > 0)
                {
                    salesData = getSalesDataObject(recordSet);
                    var jsonSalesData = JsonConvert.SerializeObject(salesData);
                    string objectMsg = "Sales Invoice Data: " + jsonSalesData;
                    logger.Debug(objectMsg);
                }
                var response = CBMSIntegration.uploadSalesBill(salesData);
                string responseMsg = "Response from the server: " + response.responseMsg;
                logger.Debug(responseMsg);

                return response;
                //Addon.showStatusBarMessage(response.responseMsg, response.isSuccess);
            }
            catch (Exception ex)
            {
                Addon.showStatusBarMessage(ex);
            }
            Console.WriteLine("sending bill to cbms : {0} ", docEntry);
            return null;
        }


        public static CBMSParsedReponse sendBillReturnToCBMS(string docEntry)
        {
            try
            {
                SalesReturnDataObject returnData = null;

                SAPbobsCOM.Recordset recordSet = DBUtil.callStoredProc("ITN_POSTING_CREDITMEMO_DATA", docEntry);
                if (recordSet.RecordCount > 0)
                {
                    returnData = getSalesReturnDataObject(recordSet);
                    var jsonReturnData = JsonConvert.SerializeObject(returnData);
                    string objectMsg = "A/R Credit Memo Data: " + jsonReturnData;
                    logger.Debug(objectMsg);
                }
                var response = CBMSIntegration.uploadSalesReturn(returnData);
                string responseMsg = "Response from the server: " + response.responseMsg;
                logger.Debug(responseMsg);

                //Addon.showStatusBarMessage(response.responseMsg, response.isSuccess);
                return response;
            }
            catch (Exception ex)
            {
                Addon.showStatusBarMessage(ex);
            }
            Console.WriteLine("sending bill return to cbms : {0} ", docEntry);
            return null;
        }

        //public static SalesDataObject getSalesDataObject(string DocEntry) {
        //    return new SalesDataObject();
        //}

        //public static SalesReturnDataObject getSalesReturnDataObject(string DocEntry){
        //    return new SalesReturnDataObject();
        //}


        ~IRDSalesController()
        {
            UnregisterBOEventListener(this);
        }

        public static SalesDataObject getSalesDataObject(SAPbobsCOM.Recordset recordSet)
        {
            SalesDataObject billData = new SalesDataObject();
            billData.username = (string)recordSet.Fields.Item("username").Value;//"Test_CBMS";//(string)recordSet.Fields.Item("username").Value;
            billData.password = (string)recordSet.Fields.Item("password").Value;//"test@321";// (string)recordSet.Fields.Item("password").Value;
            billData.seller_pan = (string)recordSet.Fields.Item("seller_pan").Value;//"999999999";
            billData.buyer_pan = (string)recordSet.Fields.Item("buyer_pan").Value;
            billData.fiscal_year = ((string)recordSet.Fields.Item("fiscal_year").Value).Replace("-", ".");
            billData.buyer_name = (string)recordSet.Fields.Item("buyer_name").Value;
            billData.invoice_number = (string)recordSet.Fields.Item("invoice_number").Value;
            billData.invoice_date = DateUtil.ConvertToBSIRD(((string)recordSet.Fields.Item("invoice_date").Value).Remove(10).Replace("-", ""));
            billData.total_sales = (double)recordSet.Fields.Item("total_sales").Value;
            billData.taxable_sales_vat = (double)recordSet.Fields.Item("taxable_sales_vat").Value;
            billData.vat = (double)recordSet.Fields.Item("vat").Value;
            billData.excisable_amount = (double)recordSet.Fields.Item("excisable_amount").Value;
            billData.excise = (double)recordSet.Fields.Item("excise").Value;
            billData.taxable_sales_hst = (double)recordSet.Fields.Item("taxable_sales_hst").Value;
            billData.hst = (double)recordSet.Fields.Item("hst").Value;
            billData.amount_for_esf = (double)recordSet.Fields.Item("amount_for_esf").Value;
            billData.esf = (double)recordSet.Fields.Item("esf").Value;
            billData.export_sales = (double)recordSet.Fields.Item("export_sales").Value;
            billData.tax_exempted_sales = (double)recordSet.Fields.Item("tax_exempted_sales").Value;
            billData.isrealtime = true;
            billData.datetimeClient = DateTime.Now;
            return billData;
        }

        public static SalesReturnDataObject getSalesReturnDataObject(SAPbobsCOM.Recordset recordSet)
        {
            SalesReturnDataObject billData = new SalesReturnDataObject();
            billData.username = (string)recordSet.Fields.Item("username").Value;//"Test_CBMS";//(string)recordSet.Fields.Item("username").Value;
            billData.password = (string)recordSet.Fields.Item("password").Value;//"test@321";// (string)recordSet.Fields.Item("password").Value;
            billData.seller_pan = (string)recordSet.Fields.Item("seller_pan").Value;//"999999999";
            billData.buyer_pan = (string)recordSet.Fields.Item("buyer_pan").Value;
            billData.fiscal_year = ((string)recordSet.Fields.Item("fiscal_year").Value).Replace("-", ".");
            billData.buyer_name = (string)recordSet.Fields.Item("buyer_name").Value;
            billData.ref_invoice_number = recordSet.Fields.Item("ref_invoice_number").Value.ToString();
            billData.credit_note_number = (string)recordSet.Fields.Item("credit_note_number").Value;
            billData.credit_note_date = DateUtil.ConvertToBSIRD(((string)recordSet.Fields.Item("return_date").Value).Remove(10).Replace("-", ""));
            billData.reason_for_return = (string)recordSet.Fields.Item("reason_for_return").Value;
            billData.total_sales = (double)recordSet.Fields.Item("total_sales").Value;
            billData.taxable_sales_vat = (double)recordSet.Fields.Item("taxable_sales_vat").Value;
            billData.vat = (double)recordSet.Fields.Item("vat").Value;
            billData.excisable_amount = (double)recordSet.Fields.Item("excisable_amount").Value;
            billData.excise = (double)recordSet.Fields.Item("excise").Value;
            billData.taxable_sales_hst = (double)recordSet.Fields.Item("taxable_sales_hst").Value;
            billData.hst = (double)recordSet.Fields.Item("hst").Value;
            billData.amount_for_esf = (double)recordSet.Fields.Item("amount_for_esf").Value;
            billData.esf = (double)recordSet.Fields.Item("esf").Value;
            billData.export_sales = (double)recordSet.Fields.Item("export_sales").Value;
            billData.tax_exempted_sales = (double)recordSet.Fields.Item("tax_exempted_sales").Value;
            billData.isrealtime = true;
            billData.datetimeClient = DateTime.Now;
            return billData;
        }
        
        public static void setSyncStatusOINV(bool isSuccess, string docEntry)
        {
            //string date = DateTime.Now.ToString("yyMMdd");
            string date = DateTime.Now.ToString("yyyyMMdd");
            if (isSuccess)
            {
                
                DBUtil.callStoredProc("ITN_OINVSETSYNCSTATUS", docEntry, "true", "true",date);
            }
            else
            {
                DBUtil.callStoredProc("ITN_OINVSETSYNCSTATUS", docEntry, "false", "false",null);
            }
        }

        public static void setSyncStatusORIN(bool isSuccess, string docEntry)
        {
            //string date = DateTime.Now.ToString("yyMMdd");
            string date = DateTime.Now.ToString("yyyyMMdd");
            if (isSuccess)
            {
                DBUtil.callStoredProc("ITN_ORINSETSYNCSTATUS", docEntry, "true", "true", date);
            }
            else
            {
                DBUtil.callStoredProc("ITN_ORINSETSYNCSTATUS", docEntry, "false", "false",null);
            }
        }

        private static string GetDocEntryFromXML(string sXML)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(sXML);

            String docEntry = xmlDoc.DocumentElement.ChildNodes[0].InnerText;
            return docEntry;

        }
    }
}