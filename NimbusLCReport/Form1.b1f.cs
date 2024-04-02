using System;
using System.Collections.Generic;
using System.Xml;
using SAPbouiCOM.Framework;
using System.Globalization;
using ITNSBOCustomization.Lib.Core;
using ITNSBOCustomization.Lib.Utils;
using SAPbouiCOM;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace NimbusLCReport
{
    [FormAttribute("NimbusLCReport.Form1", "Form1.b1f")]
    class Form1 : UserFormBase
    {
        private string fromDate;
        private string toDate;
        private string itemCode;
        private string vendName;
        private string prodGrp;
        private string division;

        private SAPbouiCOM.StaticText StaticText0;
        private SAPbouiCOM.StaticText StaticText1;
        private SAPbouiCOM.StaticText StaticText2;
        private SAPbouiCOM.StaticText StaticText3;
        private SAPbouiCOM.StaticText StaticText4;
        private SAPbouiCOM.StaticText StaticText5;
        private SAPbouiCOM.EditText txtFromDate;
        private SAPbouiCOM.EditText txtToDate;
        private SAPbouiCOM.Button Button0;
        private SAPbouiCOM.EditText txtItemCode;
        private SAPbouiCOM.EditText txtVendName;
        private SAPbouiCOM.EditText txtProdGrp;
        private SAPbouiCOM.EditText txtDivision;

        
        public Form1()
        {
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_0").Specific));
            this.StaticText1 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_1").Specific));
            this.StaticText2 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_2").Specific));
            this.StaticText3 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_3").Specific));
            this.StaticText4 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_4").Specific));
            this.StaticText5 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_5").Specific));
            this.txtFromDate = ((SAPbouiCOM.EditText)(this.GetItem("FROM_DATE").Specific));
            this.txtToDate = ((SAPbouiCOM.EditText)(this.GetItem("TO_DATE").Specific));
            this.Button0 = ((SAPbouiCOM.Button)(this.GetItem("BTN_OK").Specific));
            this.Button0.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.Button0_ClickBefore);
            this.Button0.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.Button0_ClickAfter);
            this.txtItemCode = ((SAPbouiCOM.EditText)(this.GetItem("ITEM_CODE").Specific));
            this.txtVendName = ((SAPbouiCOM.EditText)(this.GetItem("VEND_NAME").Specific));
            this.txtProdGrp = ((SAPbouiCOM.EditText)(this.GetItem("PROD_GRP").Specific));
            this.txtDivision = ((SAPbouiCOM.EditText)(this.GetItem("DIVISION").Specific));
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.LoadAfter += new LoadAfterHandler(this.Form_LoadAfter);
            SAPbouiCOM.Framework.Application.SBO_Application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
        }

        

        private void OnCustomInitialize()
        {
            var oForm = SAPbouiCOM.Framework.Application.SBO_Application.Forms.ActiveForm;

            oForm.DataSources.UserDataSources.Add("FromDate", SAPbouiCOM.BoDataType.dt_DATE, 10);
            this.txtFromDate.DataBind.SetBound(true, "", "FromDate");

            oForm.DataSources.UserDataSources.Add("ToDate", SAPbouiCOM.BoDataType.dt_DATE, 10);
            this.txtToDate.DataBind.SetBound(true, "", "ToDate");

            oForm.DataSources.UserDataSources.Add("ItemCode", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 254);
            this.AddChooseFromList(this.txtItemCode, "4",  false);
            this.txtItemCode.DataBind.SetBound(true, "", "ItemCode");
            this.txtItemCode.ChooseFromListUID = this.txtItemCode.Item.UniqueID.ToString();
            this.txtItemCode.ChooseFromListAlias = "ItemCode";

            oForm.DataSources.UserDataSources.Add("VendorName", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 254);
            this.AddChooseFromList(this.txtVendName, "2",  false);
            this.txtVendName.DataBind.SetBound(true, "", "VendorName");
            this.txtVendName.ChooseFromListUID = this.txtVendName.Item.UniqueID.ToString();
            this.txtVendName.ChooseFromListAlias = "CardCode";

            oForm.DataSources.UserDataSources.Add("PrdGrp", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 254);
            this.AddChooseFromList(this.txtProdGrp, "52", false);
            this.txtProdGrp.DataBind.SetBound(true, "", "PrdGrp");
            this.txtProdGrp.ChooseFromListUID = this.txtProdGrp.Item.UniqueID.ToString();
            this.txtProdGrp.ChooseFromListAlias = "ItmsGrpCod";

            oForm.DataSources.UserDataSources.Add("Div", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 254);
            this.AddChooseFromList(this.txtDivision, "61", false, "DimCode", BoConditionOperation.co_EQUAL, "3");
            this.txtDivision.DataBind.SetBound(true, "", "Div");
            this.txtDivision.ChooseFromListUID = this.txtDivision.Item.UniqueID.ToString();
            this.txtDivision.ChooseFromListAlias = "PrcCode";
        }

        private void Form_LoadAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            //throw new System.NotImplementedException();

        }


        private void Button0_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {

            //SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Irregularity in quantity. Please do the needful.");
            //throw new System.NotImplementedException();
            SAPbobsCOM.Recordset recordSet = DBUtil.callStoredProc("ITN_LANDEDCOSTREPORT", fromDate, toDate, division, itemCode, vendName, prodGrp);
            if (recordSet.RecordCount > 0)
            {
                ExportDataSetToExcel(recordSet);
            }
        }

        private void Button0_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            //throw new System.NotImplementedException();
            if (txtFromDate.Value != "") 
                fromDate = DateTime.ParseExact(txtFromDate.Value, "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
            if(txtToDate.Value != "")
                toDate = DateTime.ParseExact(txtToDate.Value, "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString(); 
            itemCode = txtItemCode.Value;
            vendName = txtVendName.Value;
            prodGrp = txtProdGrp.Value;
            division = txtDivision.Value;
        }
        private void AddChooseFromList(SAPbouiCOM.EditText txtEdit, string objType, bool multiSelection, [Optional] string conAliasName, [Optional] SAPbouiCOM.BoConditionOperation conOperation, [Optional] string conVal)
        {
            try
            {
                var oForm = SAPbouiCOM.Framework.Application.SBO_Application.Forms.ActiveForm;
                SAPbouiCOM.ChooseFromListCollection oCFLs = null;
                SAPbouiCOM.Conditions oCons = null;
                SAPbouiCOM.Condition oCon = null;

                oCFLs = oForm.ChooseFromLists;

                SAPbouiCOM.ChooseFromList oCFL = null;
                SAPbouiCOM.ChooseFromListCreationParams oCFLCreationParams = null;
                oCFLCreationParams = ((SAPbouiCOM.ChooseFromListCreationParams)(SAPbouiCOM.Framework.Application.SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)));

                //  Adding 2 CFL, one for the button and one for the edit text.
                oCFLCreationParams.MultiSelection = multiSelection;
                oCFLCreationParams.ObjectType = objType;
                oCFLCreationParams.UniqueID = txtEdit.Item.UniqueID.ToString();

                oCFL = oCFLs.Add(oCFLCreationParams);

                //  Adding Conditions to CFL1
                if(conAliasName != null && conOperation != SAPbouiCOM.BoConditionOperation.co_NONE && conVal != null)
                {
                    oCons = oCFL.GetConditions();

                    oCon = oCons.Add();
                    oCon.Alias = conAliasName;
                    oCon.Operation = conOperation;
                    oCon.CondVal = conVal;
                    oCFL.SetConditions(oCons);
                }
                //txtEdit.ChooseFromListUID = aliasName;
                //txtEdit.ChooseFromListAlias = aliasName;
            }
            catch (Exception ex)
            {
                //SAPbouiCOM.Framework.Application.SBO_Application.MessageBox(ex.Message);
                Addon.showStatusBarMessage(ex);
                //Interaction.MsgBox(Information.Err().Description, (Microsoft.VisualBasic.MsgBoxStyle)(0), null);
            }
        }

        private void SBO_Application_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.EventType == SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST)
            {
                SAPbouiCOM.IChooseFromListEvent oCFLEvento = ((SAPbouiCOM.IChooseFromListEvent)(pVal));
                string sCFL_ID  = oCFLEvento.ChooseFromListUID;
                SAPbouiCOM.Form oForm = SAPbouiCOM.Framework.Application.SBO_Application.Forms.Item(FormUID);
                SAPbouiCOM.ChooseFromList oCFL = oForm.ChooseFromLists.Item(sCFL_ID);

                if (oCFLEvento.BeforeAction == false)
                {
                    SAPbouiCOM.DataTable oDataTable = null;
                    oDataTable = oCFLEvento.SelectedObjects;
                    string val = null;
                    try
                    {
                        val = System.Convert.ToString(oDataTable.GetValue(0, 0));
                        switch (pVal.ItemUID)
                        {
                            case "ITEM_CODE":
                                oForm.DataSources.UserDataSources.Item("ItemCode").ValueEx = val;
                                break;
                            case "VEND_NAME":
                                oForm.DataSources.UserDataSources.Item("VendorName").ValueEx = val;
                                break;
                            case "PROD_GRP":
                                oForm.DataSources.UserDataSources.Item("PrdGrp").ValueEx = val;
                                break;
                            case "DIVISION":
                                oForm.DataSources.UserDataSources.Item("Div").ValueEx = val;
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Addon.showStatusBarMessage(ex);
                    }
                }
            }
        }

        [STAThread]
        private void ExportDataSetToExcel(SAPbobsCOM.Recordset ds)
        {
            string strPath = "";
            System.Windows.Forms.Form form = new System.Windows.Forms.Form();
            form.TopMost = true;
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.InitialDirectory = @"C:\";
            saveDlg.Filter = "Excel files (*.xlsx)|*.xlsx";
            saveDlg.FilterIndex = 0;
            saveDlg.RestoreDirectory = true;
            saveDlg.Title = "Export Excel File To";
            DialogResult ret = STAShowDialog(saveDlg);

            if (ret != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            strPath = saveDlg.FileName;
            try
            {

                //Creae an Excel application instance
                Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();

                //Create an Excel workbook instance and open it from the predefined location
                Microsoft.Office.Interop.Excel.Workbook excelWorkBook = excelApp.Workbooks.Add(Type.Missing);

 
                //Add a new worksheet to workbook with the Datatable name
                Microsoft.Office.Interop.Excel.Worksheet excelWorkSheet = null;
                //excelWorkSheet.Name = table.TableName;

                excelWorkSheet = excelWorkBook.Sheets["Sheet1"];
                excelWorkSheet = excelWorkBook.ActiveSheet;

                for (int i = 1; i < ds.Fields.Count + 1; i++)
                {
                    excelWorkSheet.Cells[1, i] = ds.Fields.Item(i - 1).Description.ToString();
                }

                for (int j = 0; j < ds.RecordCount; j++)
                {
                    for (int k = 0; k < ds.Fields.Count; k++)
                    {
                        excelWorkSheet.Cells[j + 2, k + 1] = ds.Fields.Item(k).Value.ToString();
                    }
                    ds.MoveNext();
                }
                excelApp.ActiveWorkbook.SaveCopyAs(strPath);
                excelApp.ActiveWorkbook.Saved = true;
                excelApp.Quit();
                SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Excel file created successfully.");
                clearForm();
            }
            catch(Exception ex)
            {
                Addon.showStatusBarMessage(ex);
            }
        }

        private static DialogResult STAShowDialog(FileDialog dialog)

        {
            DialogState state = new DialogState();
            state.dialog = dialog;
            System.Threading.Thread t = new System.Threading.Thread(state.ThreadProcShowDialog);
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.Start();
            t.Join();
            return state.result;

        }

        public class DialogState

        {
            public DialogResult result;
            public FileDialog dialog;
            public void ThreadProcShowDialog()
            {
                result = dialog.ShowDialog();
            }
        }

        private void clearForm()
        {
            txtFromDate.Value = "";
            txtToDate.Value = "";
            txtItemCode.Value = "";
            txtProdGrp.Value = "";
            txtDivision.Value = "";
            txtVendName.Value = "";

            fromDate = "";
            toDate = "";
            itemCode = "";
            vendName = "";
            prodGrp = "";
            division = "";
        }
    }       
}           
            