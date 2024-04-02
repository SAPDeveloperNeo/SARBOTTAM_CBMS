using HACBatchManagement.Lib;
using ITNSBOCustomization.Lib.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbouiCOM;
using Newtonsoft.Json;
using ITNSBOCustomization.Lib.Utils;
using ITNSBOCustomization.Lib.Core;

namespace HACBatchManagement.Controllers
{
    class BatchManagementController : FormController
    {
        private static bool fromReceipt = false;

        public BatchManagementController(String formUID, int formTypeCount) : base(formUID, formTypeCount)
        {
            if(this.FormCode == "OIGN")
            {
                fromReceipt = true;
            }
            else if(this.FormCode == "OPDN")
            {
                fromReceipt = false;
            }

            if (this.FormCode == "SBDR" && fromReceipt)
            {
                this.FreezeForm();
                addButton();
                this.UnfreezeForm();

            }
            RegisterController(this);
        }

        public override void FormEventTrigger(ref ItemEvent EventData)
        {
            try
            {
                if (EventData.EventType == SAPbouiCOM.BoEventTypes.et_CLICK
                    && EventData.ItemUID == "btnSearch"
                    && EventData.BeforeAction == false)
                {
                    string woNum = ((SAPbouiCOM.EditText)oForm.Items.Item("txtRefNum").Specific).Value; 
                    if(string.IsNullOrEmpty(woNum))
                    {
                        SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Please enter the reference number.");
                        return;
                    }
                                      
                    var grid = ((SAPbouiCOM.Matrix)(oForm.Items.Item(2).Specific));
                    var gridDocs = ((SAPbouiCOM.Matrix)(oForm.Items.Item(7).Specific));
                    string quanNeeded = ((SAPbouiCOM.EditText)gridDocs.Columns.Item(9).Cells.Item(1).Specific).Value.ToString().Replace(".000000","");

                    grid.Clear();
                    grid.AutoResizeColumns();
                    grid.AddRow();

                    SAPbobsCOM.Recordset batchDetails = DBUtil.callStoredProc("ITN_GETBATCHDETAILS", woNum);

                    if(batchDetails.RecordCount.ToString() != quanNeeded)
                    {
                        SAPbouiCOM.Framework.Application.SBO_Application.MessageBox("Irregularity in quantity. Please do the needful.");
                        return;
                    }

                    PopulateGrid(grid, batchDetails);                  
                }
            }
            catch(Exception ex)
            {
                Addon.showStatusBarMessage(ex);
            }
        }

        public static void onFormEvent(ref SAPbouiCOM.ItemEvent EventData)
        {
            if ((EventData.BeforeAction == false) && (EventData.EventType == SAPbouiCOM.BoEventTypes.et_FORM_LOAD))
            {
                //Console.WriteLine("creating new ctrl");
                var formName = new BatchManagementController(EventData.FormTypeEx, EventData.FormTypeCount);
                formName.logger.Debug("New controller of type {0} registered.", formName.FormCode);

            }

            var Forms = GetListeningControllers(EventData.FormTypeEx, EventData.FormTypeCount); //ActiveFormControllers[EventData.FormTypeEx][EventData.FormTypeCount];
            if (Forms != null)
            {
                foreach (FormController FormCtrl in Forms)
                {
                    FormCtrl.FormEventTrigger(ref EventData);
                }
            }

            //form close event, controllers will be unregistered.
            if ((EventData.BeforeAction == false) && (EventData.EventType == SAPbouiCOM.BoEventTypes.et_FORM_CLOSE))
            {
                UnregisterAllControllers(EventData.FormTypeEx, EventData.FormTypeCount);
            }

        }

        private void addButton()
        {
            this.AddButton("btnSearch", 480, 60, 15, 155, "Search", true);
            
            this.AddStaticText("lblRefNum", 300,
                                    100,
                                    20,
                                    130,
                                    "Reference No:");

            this.AddEditText("txtRefNum", 380,
                                80,
                                15,
                                138,
                                "lblWRefum", this.FormCode, "", true);
        }

        private void PopulateGrid(SAPbouiCOM.Matrix formGrid, SAPbobsCOM.Recordset batchDetails)
        {
            while (batchDetails.EoF == false)
            {
                ((SAPbouiCOM.EditText)formGrid.Columns.Item(3).Cells.Item(formGrid.RowCount).Specific).Value = batchDetails.Fields.Item("LotNumber").Value.ToString();
                batchDetails.MoveNext();
            }
            batchDetails.MoveFirst();
            for (int i = 1; i <= formGrid.RowCount; i++)
            {
                ((SAPbouiCOM.EditText)formGrid.Columns.Item(1).Cells.Item(i).Specific).Value = batchDetails.Fields.Item("MnfSerial").Value.ToString();
                ((SAPbouiCOM.EditText)formGrid.Columns.Item(2).Cells.Item(i).Specific).Value = batchDetails.Fields.Item("DistNumber").Value.ToString();
                batchDetails.MoveNext();
            }
        }
    }
}
