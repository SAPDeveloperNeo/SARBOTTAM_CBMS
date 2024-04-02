using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM.Framework;
using ITNSBOCustomization.Lib.Core;
using ITNSBOCustomization.Lib.Utils;
using ITNSBOCustomization.NPLocalization.Controllers;
using ITNSBOCustomization.Lib.Localization;

namespace NPLocalization.Forms
{
    [FormAttribute("NPLocalization.Forms.UploadBillsToCBMS", "Forms/UploadBillsToCBMS.b1f")]
    class UploadBillsToCBMS : UserFormBase
    {
        static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private SAPbouiCOM.Button btnUpload;
        //public SAPbouiCOM.Form oForm = default(SAPbouiCOM.Form);
        private SAPbouiCOM.Form oForm;
        private string[] docs;
        const string OBJCODE_AR_INV = "13";
        const string OBJCODE_AR_CREDIT = "14";

        private string FormId;
        private static int initCtr = 1;
        public UploadBillsToCBMS(string FormID)
        {
            this.FormId = FormID;

            oForm = Application.SBO_Application.Forms.GetForm(FormID, this.UIAPIRawForm.TypeCount);

            this.OnCustomInitialize();
        }

        public UploadBillsToCBMS()
        {
            this.OnCustomInitialize();
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.btnUpload = ((SAPbouiCOM.Button)(this.GetItem("btnUpload").Specific));
            this.Mat_Bills = ((SAPbouiCOM.Matrix)(this.GetItem("MAT_BILL").Specific));
            this.oForm = ((SAPbouiCOM.Form)(this.UIAPIRawForm));
        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
        }

        private void OnCustomInitialize()
        {
            //oForm = Application.SBO_Application.Forms.GetForm("NPLocalization.Forms.UploadBillsToCBMS", 1);
            //bindDataSource();
            this.AddCheckBox("cbInvoice", 15, 15, "A/R Invoice", true, "Invoice");
            this.AddCheckBox("cbReturn", 98, 15, "Return", true, "Return");
            bindDataSource();
            try
            {
                ((SAPbouiCOM.CheckBox)oForm.Items.Item("cbInvoice").Specific).Checked = true;
            }
            catch (Exception ex)
            {
            }

        }

        private void AddCheckBox(string newID, int left, int top, string caption, bool isEnabled, string dbString)
        {
            try
            {
                //SAPbouiCOM.Item oItem = (Item)oForm.Items.Item("2");  /// Existing Item on the form
                SAPbouiCOM.Item oItem1 = oForm.Items.Add(newID, SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX);
                SAPbouiCOM.CheckBox oCheckBox = (SAPbouiCOM.CheckBox)oItem1.Specific;
                oItem1.Top = top;
                oItem1.Left = left;
                oItem1.Enabled = isEnabled;
                oCheckBox.Caption = caption;
                oForm.DataSources.UserDataSources.Add(dbString, SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1);
                oCheckBox.DataBind.SetBound(true, "", dbString);
                //oCheckBox.ValOn = "Y";
                //oCheckBox.ValOff = "N";
            }

            catch (Exception ex)
            {
                //throw;
                Addon.showStatusBarMessage(ex);
            }
        }

        private SAPbouiCOM.Matrix Mat_Bills;

        private void SBO_Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.Before_Action == false)
            {
                if ((pVal.EventType == SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED) && (pVal.ItemUID == "cbInvoice" || pVal.ItemUID == "cbReturn"))
                {
                    if (pVal.FormUID != oForm.UniqueID)
                        return;
                    //oForm = Application.SBO_Application.Forms.ActiveForm;
                    if (((SAPbouiCOM.CheckBox)oForm.Items.Item(pVal.ItemUID).Specific).Checked)
                    {
                        switch (pVal.ItemUID)
                        {
                            case "cbInvoice":
                                ((SAPbouiCOM.CheckBox)oForm.Items.Item("cbReturn").Specific).Checked = false;
                                SAPbobsCOM.Recordset pendingBills = DBUtil.callStoredProc("ITN_GETUNSYNCEDOINV");
                                if (pendingBills.RecordCount > 0)
                                {
                                    PopulateGrid(((SAPbouiCOM.Matrix)oForm.Items.Item("MAT_BILL").Specific), pendingBills);
                                    ((SAPbouiCOM.Matrix)oForm.Items.Item("MAT_BILL").Specific).AutoResizeColumns();
                                }
                                else
                                {
                                    ((SAPbouiCOM.Matrix)oForm.Items.Item("MAT_BILL").Specific).Clear();
                                }
                                break;
                            case "cbReturn":
                                ((SAPbouiCOM.CheckBox)oForm.Items.Item("cbInvoice").Specific).Checked = false;
                                SAPbobsCOM.Recordset pendingReturnBills = DBUtil.callStoredProc("ITN_GETUNSYNCEDORIN");
                                if (pendingReturnBills.RecordCount > 0)
                                {
                                    PopulateGrid(((SAPbouiCOM.Matrix)oForm.Items.Item("MAT_BILL").Specific), pendingReturnBills);
                                    ((SAPbouiCOM.Matrix)oForm.Items.Item("MAT_BILL").Specific).AutoResizeColumns();
                                }
                                else
                                {
                                    ((SAPbouiCOM.Matrix)oForm.Items.Item("MAT_BILL").Specific).Clear();
                                }
                                break;
                        }
                    }

                }
                else if ((pVal.EventType == SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED) && pVal.ItemUID == "btnUpload")
                {
                    try
                    {
                        if (((SAPbouiCOM.CheckBox)oForm.Items.Item("cbInvoice").Specific).Checked)
                        {
                            if (CBMSIntegration.BORequiresCompliance(OBJCODE_AR_INV))
                            {
                                int billSuccess = 0;

                                for (int i = 0; i < docs.Length; i++)
                                {
                                    var response = IRDSalesController.sendBillToCBMS(docs[i]);
                                    setSyncStatusOINV(response.isSuccess, docs[i]);
                                    if (response.isSuccess)
                                        billSuccess++;
                                    //Mat_Bills.GetLineData(i + 1);
                                    //oForm.DataSources.UserDataSources.Item("Msg").ValueEx = response.responseMsg;
                                }
                                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Total Bills: " + (docs.Length) + Environment.NewLine + "Total bills successfully uploaded: " + billSuccess + Environment.NewLine + "Total bills failed to upload: " + ((docs.Length) - billSuccess));

                                //if (billSuccess > 0)
                                //    ((SAPbouiCOM.CheckBox)oForm.Items.Item("cbInvoice").Specific).Checked = true;
                            }
                            else
                                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Upload to CBMS is disabled.");
                        }
                        else if (((SAPbouiCOM.CheckBox)oForm.Items.Item("cbReturn").Specific).Checked)
                        {
                            //Lokesh
                            if (CBMSIntegration.BORequiresCompliance(OBJCODE_AR_CREDIT))


                            {
                                int billSuccess = 0;
                                for (int i = 0; i < docs.Length; i++)
                                {
                                    var response = IRDSalesController.sendBillReturnToCBMS(docs[i]);
                                    setSyncStatusORIN(response.isSuccess, docs[i]);
                                    if (response.isSuccess)
                                        billSuccess++;
                                    //Mat_Bills.GetLineData(i + 1);
                                    //oForm.DataSources.UserDataSources.Item("Msg").Value = response.responseMsg;
                                }
                                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Total Bills: " + (docs.Length) + Environment.NewLine + "Total bills successfully uploaded: " + billSuccess + Environment.NewLine + "Total bills failed to upload: " + ((docs.Length) - billSuccess));

                                //if (billSuccess > 0)
                                //    ((SAPbouiCOM.CheckBox)oForm.Items.Item("cbReturn").Specific).Checked = true;
                            }
                            else
                                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Upload to CBMS is disabled.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Addon.showStatusBarMessage(ex);
                    }
                }
            }
        }

        private void PopulateGrid(SAPbouiCOM.Matrix formGrid, SAPbobsCOM.Recordset pendingBills)
        {
            try
            {
                clearGrid();
                pendingBills.MoveFirst();
                docs = new string[pendingBills.RecordCount];
                //formGrid.AddRow();
                for (int i = 1; i <= pendingBills.RecordCount; i++)
                {
                    //formGrid.AddRow();
                    oForm.Freeze(true);
                    oForm.DataSources.UserDataSources.Item("DocNum").Value = pendingBills.Fields.Item("DocNum").Value.ToString();
                    var data = pendingBills.Fields.Item("DocDate").Value.ToString();
                    oForm.DataSources.UserDataSources.Item("DocDate").Value = pendingBills.Fields.Item("DocDate").Value.ToString();
                    oForm.DataSources.UserDataSources.Item("CardName").Value = pendingBills.Fields.Item("CardName").Value.ToString();
                    docs[i - 1] = pendingBills.Fields.Item("DocEntry").Value.ToString();
                    formGrid.AddRow(1, -1);
                    oForm.Freeze(false);
                    pendingBills.MoveNext();
                }
            }
            catch (Exception ex)
            {
                Addon.showStatusBarMessage(ex);
            }


        }

        private void bindDataSource()
        {
            SAPbouiCOM.Column oColumn;

            oForm.DataSources.UserDataSources.Add("DocNum", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 20);
            oColumn = Mat_Bills.Columns.Item("Col_Num");
            oColumn.DataBind.SetBound(true, "", "DocNum");

            oForm.DataSources.UserDataSources.Add("DocDate", SAPbouiCOM.BoDataType.dt_DATE, 20);
            oColumn = Mat_Bills.Columns.Item("Col_Date");
            oColumn.DataBind.SetBound(true, "", "DocDate");

            oForm.DataSources.UserDataSources.Add("CardName", SAPbouiCOM.BoDataType.dt_LONG_TEXT, 500);
            oColumn = Mat_Bills.Columns.Item("Col_Name");
            oColumn.DataBind.SetBound(true, "", "CardName");

            //oForm.DataSources.UserDataSources.Add("Msg", SAPbouiCOM.BoDataType.dt_LONG_TEXT, 500);
            //oColumn = Mat_Bills.Columns.Item("Col_Msg");
            //oColumn.DataBind.SetBound(true, "", "Msg");
        }

        private void clearGrid()
        {
            ((SAPbouiCOM.Matrix)oForm.Items.Item("MAT_BILL").Specific).Clear();// Mat_Bills.Clear();
            //for(int i =0; i< oForm.DataSources.UserDataSources.Count; i++)
            //{
            //    oForm.DataSources.UserDataSources.Item("DocNum").Value = "";
            //    oForm.DataSources.UserDataSources.Item("DocDate").Value = "";
            //    oForm.DataSources.UserDataSources.Item("CardName").Value = "";
            //}        
        }

        public static void setSyncStatusOINV(bool isSuccess, string docEntry)
        {
            string date = DateTime.Now.ToString("yyyyMMdd");
            if (isSuccess)
            {
                // static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Debug("Updating sync status to success for" + docEntry);
                //DBUtil.callStoredProc("ITN_OINVSETSYNCSTATUS", docEntry, "false", "true");
                DBUtil.callStoredProc("ITN_OINVSETSYNCSTATUS", docEntry, "true", "true", date);
                //DBUtil.callStoredProc("ITN_OINVSETSYNCSTATUS", docEntry, "false", "true", DateTime.Now.Date.ToShortDateString());
            }
            else
            {
                logger.Debug("Updating sync status to false for" + docEntry);
                //DBUtil.callStoredProc("ITN_OINVSETSYNCSTATUS", docEntry, "false", "false");
                DBUtil.callStoredProc("ITN_OINVSETSYNCSTATUS", docEntry, "false", "false", null);
                //DBUtil.callStoredProc("ITN_OINVSETSYNCSTATUS", docEntry, "false", "false", null);
            }
        }

        public static void setSyncStatusORIN(bool isSuccess, string docEntry)
        {
            string date = DateTime.Now.ToString("yyyyMMdd");
            if (isSuccess)
            {
                //DBUtil.callStoredProc("ITN_ORINSETSYNCSTATUS", docEntry, "false", "true", DateTime.Now.Date.ToShortDateString());
                //DBUtil.callStoredProc("ITN_ORINSETSYNCSTATUS", docEntry, "false", "true");
                DBUtil.callStoredProc("ITN_ORINSETSYNCSTATUS", docEntry, "true", "true", date);
            }
            else
            {
                //DBUtil.callStoredProc("ITN_ORINSETSYNCSTATUS", docEntry, "false", "false", null);
                //DBUtil.callStoredProc("ITN_ORINSETSYNCSTATUS", docEntry, "false", "false");
                DBUtil.callStoredProc("ITN_ORINSETSYNCSTATUS", docEntry, "false", "false", null);
            }
        }
    }
}
