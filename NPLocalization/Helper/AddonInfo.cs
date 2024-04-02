using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAPbouiCOM.Framework;
using ITNSBOCustomization.SAPB1;
//using GlobalVariable;
using SAPbobsCOM;
using NPLocalization;

namespace NPLocalization.Helper
{
    public class AddonInfo
    {
        #region Members
        public int Index { get; set; }
        public bool isHana { get; set; }
        private static int RetCode = 0;
        private static string ErrMsg = null;
        #endregion

        #region Constructor

        public AddonInfo()
        {
        }

        #endregion

        #region Methods

        public static bool InstallUDOs()
        {
            try
            {
                bool UDOAdded = true;

                string[] ChildTable = new string[0];
                string[] FindColumn = new string[0];
                string[] FormColumn = new string[0];

                #region System Tables Fields

                //B1Helper.DiCompany.StartTransaction();

                Application.SBO_Application.StatusBar.SetText("Database structure is modifying...", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success);

                #region CBMS Table

                B1Helper.AddTable("CBMS_CONFIG", "CBMS Configuration", BoUTBTableType.bott_NoObjectAutoIncrement);

                B1Helper.AddField("Username", "Username", "CBMS_CONFIG", BoFieldTypes.db_Alpha, 50, BoYesNoEnum.tNO, BoFldSubTypes.st_None, false, "");
                B1Helper.AddField("Enabled", "Is Enable", "CBMS_CONFIG", BoFieldTypes.db_Alpha, 10, BoYesNoEnum.tNO, BoFldSubTypes.st_None, true, "false", new string[,] { { "false", "false" }, { "true", "true" } }, "false");
                B1Helper.AddField("Password", "Password", "CBMS_CONFIG", BoFieldTypes.db_Alpha, 50, BoYesNoEnum.tNO, BoFldSubTypes.st_None, false, "");
                B1Helper.AddField("BillApiUrl", "Bill API URL", "CBMS_CONFIG", BoFieldTypes.db_Alpha, 50, BoYesNoEnum.tNO, BoFldSubTypes.st_None, false, "");
                B1Helper.AddField("BillReturnApiUrl", "Bill Return API URL", "CBMS_CONFIG", BoFieldTypes.db_Alpha, 50, BoYesNoEnum.tNO, BoFldSubTypes.st_None, false, "");
                B1Helper.AddField("CompanyCode", "CompanyCode", "CBMS_CONFIG", BoFieldTypes.db_Alpha, 50, BoYesNoEnum.tNO, BoFldSubTypes.st_None, false, "");

                #endregion

                #region UDFs

                // Marketing Documents
                //B1Helper.AddField("ITN_NPDate", "Nepali Date", "OPOR", BoFieldTypes.db_Alpha, 10, BoYesNoEnum.tNO, BoFldSubTypes.st_None, false, "");
                B1Helper.AddField("ITN_Is_RealTime", "Is RealTime", "OPOR", BoFieldTypes.db_Alpha, 10, BoYesNoEnum.tNO, BoFldSubTypes.st_None, false, "");
                B1Helper.AddField("ITN_Is_Synced", "Is Synced", "OPOR", BoFieldTypes.db_Alpha, 10, BoYesNoEnum.tNO, BoFldSubTypes.st_None, false, "");
                B1Helper.AddField("ITN_Sync_Date", "Sync Date", "OPOR", BoFieldTypes.db_Date, BoYesNoEnum.tNO, BoFldSubTypes.st_None, false);
                B1Helper.AddField("ITN_Print_Count", "Print Count", "OPOR", BoFieldTypes.db_Numeric, 10, BoYesNoEnum.tNO, BoFldSubTypes.st_None, false, "");

                //// Journal Entry
                //B1Helper.AddField("ITN_NPDate", "Nepali Date", "OJDT", BoFieldTypes.db_Alpha, 10, BoYesNoEnum.tNO, BoFldSubTypes.st_None, false, "");

                //// Deposit
                //B1Helper.AddField("ITN_NPDate", "Nepali Date", "ODPS", BoFieldTypes.db_Alpha, 10, BoYesNoEnum.tNO, BoFldSubTypes.st_None, false, "");

                //// Payment
                //B1Helper.AddField("ITN_NPDate", "Nepali Date", "OVPM", BoFieldTypes.db_Alpha, 10, BoYesNoEnum.tNO, BoFldSubTypes.st_None, false, "");

                //// Landed Costs
                //B1Helper.AddField("ITN_NPDate", "Nepali Date", "OIPF", BoFieldTypes.db_Alpha, 10, BoYesNoEnum.tNO, BoFldSubTypes.st_None, false, "");

                //// Opportunity
                //B1Helper.AddField("ITN_NPDate", "Nepali Date", "OOPR", BoFieldTypes.db_Alpha, 10, BoYesNoEnum.tNO, BoFldSubTypes.st_None, false, "");

                #endregion

                //B1Helper.DiCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                #endregion

                //Utility.LogException("Ending Transaction: UDOs Creation Process");
                //B1Helper.DiCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                //#endregion

                return UDOAdded;
            }
            catch (Exception ex)
            {
                //Utility.LogException(ex);
                //B1Helper.DiCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                return false;
            }
        }

        private static bool CreateUDO(string CodeID, string Name, string TableName, string[] FormColoums, SAPbobsCOM.BoUDOObjType ObjectType, string ManageSeries)
        {
            SAPbobsCOM.UserObjectsMD oUserObjectMD = default(SAPbobsCOM.UserObjectsMD);
            try
            {
                oUserObjectMD = ((SAPbobsCOM.UserObjectsMD)(Program.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD)));
                if (oUserObjectMD.GetByKey(CodeID) == true)
                {
                    return true;
                }
                oUserObjectMD.CanLog = SAPbobsCOM.BoYesNoEnum.tYES;
                oUserObjectMD.CanFind = SAPbobsCOM.BoYesNoEnum.tYES;
                oUserObjectMD.CanClose = SAPbobsCOM.BoYesNoEnum.tYES;
                oUserObjectMD.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES;
                oUserObjectMD.CanDelete = SAPbobsCOM.BoYesNoEnum.tYES;
                oUserObjectMD.ManageSeries = SAPbobsCOM.BoYesNoEnum.tNO;
                oUserObjectMD.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tNO;

                oUserObjectMD.Code = CodeID;
                oUserObjectMD.Name = Name;
                oUserObjectMD.TableName = TableName;
                oUserObjectMD.ObjectType = ObjectType;


                oUserObjectMD.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tYES;
                oUserObjectMD.EnableEnhancedForm = SAPbobsCOM.BoYesNoEnum.tNO;
                oUserObjectMD.MenuItem = SAPbobsCOM.BoYesNoEnum.tNO;
                oUserObjectMD.MenuCaption = Name;
                oUserObjectMD.FatherMenuID = 47616;
                oUserObjectMD.Position = 0;
                oUserObjectMD.MenuUID = CodeID;

                if (FormColoums != null)
                {
                    for (int i = 0; i <= FormColoums.Length - 1; i++)
                    {
                        if (FormColoums[i].Trim() != "U_RUNDB")
                        {
                            oUserObjectMD.FormColumns.FormColumnAlias = FormColoums[i];
                            oUserObjectMD.FormColumns.Editable = SAPbobsCOM.BoYesNoEnum.tNO;
                            oUserObjectMD.FormColumns.Add();
                        }
                        else
                        {
                            oUserObjectMD.FormColumns.FormColumnAlias = FormColoums[i];
                            oUserObjectMD.FormColumns.Editable = SAPbobsCOM.BoYesNoEnum.tYES;
                            oUserObjectMD.FormColumns.Add();
                        }
                    }
                }
                // check for errors in the process
                RetCode = oUserObjectMD.Add();

                if (RetCode != 0)
                {
                    if (RetCode != -1)
                    {
                        Program.oCompany.GetLastError(out RetCode, out ErrMsg);
                        Program.SBO_Application.StatusBar.SetText("Object Failed : " + ErrMsg + "", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    }
                }
                else
                {
                    Program.SBO_Application.StatusBar.SetText("Object Registered : " + Name + "", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oUserObjectMD);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }


        //public static bool GetCommonSettings()
        //{
        //    string query = "SELECT T0.\"U_A_Email\", T0.\"U_S_Email\", T0.\"U_J_Email\" , \"U_ExcessDay\" , \"U_N_Email\" FROM OADM T0";
        //    SAPbobsCOM.Recordset rsQry = (SAPbobsCOM.Recordset)B1Helper.DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
        //    rsQry.DoQuery(query);
        //    if (rsQry.RecordCount > 0)
        //    {
        //        Globals.SetsAEmail(rsQry.Fields.Item(0).Value.ToString());
        //        Globals.SetsSEmail(rsQry.Fields.Item(1).Value.ToString());
        //        Globals.SetsJournal(rsQry.Fields.Item(2).Value.ToString());
        //        Globals.SetsExcessDay(Convert.ToDouble(rsQry.Fields.Item(3).Value.ToString()));
        //        Globals.SetsNEmail(rsQry.Fields.Item(4).Value.ToString());
        //    }

        //    query = "SELECT T0.\"U_BillProcees\", T0.\"U_Account\" FROM \"@Z_SCGL\"  T0";
        //    rsQry = (SAPbobsCOM.Recordset)B1Helper.DiCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
        //    rsQry.DoQuery(query);
        //    if (rsQry.RecordCount > 0)
        //    {
        //        while (rsQry.EoF == false)
        //        {
        //            if (rsQry.Fields.Item(0).Value.ToString() == "A")
        //            { Globals.SetsSAdvance(rsQry.Fields.Item(1).Value.ToString()); }
        //            else if (rsQry.Fields.Item(0).Value.ToString() == "C") { Globals.SetsSCredit(rsQry.Fields.Item(1).Value.ToString()); }
        //            rsQry.MoveNext();
        //        }
        //    }
        //    rsQry = null;
        //    return true;

        //}
        public static void SetFormFilter()
        {
            try
            {
                //SAPbouiCOM.EventFilters objFilters = new SAPbouiCOM.EventFilters();
                //SAPbouiCOM.EventFilter objFilter;

                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_FORM_LOAD);
                //objFilter.AddEx("frm_TransferItems");


                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_FORM_CLOSE);
                //objFilter.AddEx("frm_TransferItems");



                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_MENU_CLICK);
                //objFilter.AddEx("frm_TransferItems");


                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED);
                //objFilter.AddEx("frm_TransferItems");



                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_COMBO_SELECT);
                //objFilter.AddEx("frm_TransferItems");

                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST);
                //objFilter.AddEx("frm_TransferItems");


                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_KEY_DOWN);
                //objFilter.AddEx("frm_TransferItems");


                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_LOST_FOCUS);
                //objFilter.AddEx("frm_TransferItems");


                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_VALIDATE);
                //objFilter.AddEx("frm_TransferItems");


                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD);
                //objFilter.AddEx("frm_TransferItems");



                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_CLICK);
                //objFilter.AddEx("frm_TransferItems");


                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_RIGHT_CLICK);
                //objFilter.AddEx("frm_TransferItems");


                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_DOUBLE_CLICK);
                //objFilter.AddEx("frm_TransferItems");

                //objFilter = objFilters.Add(SAPbouiCOM.BoEventTypes.et_PICKER_CLICKED);
                //objFilter.AddEx("frm_TransferItems");


                //SetFilter(objFilters);
            }
            catch (Exception ex)
            {
                //Utility.LogException(ex);
                // Log.LogException(LogLevel.Error, ex);
            }
        }
        public static void RemoveMenu(string menuId)
        {
            Application.SBO_Application.Menus.RemoveEx(menuId);
        }
        public static string GetNextEntryIndex(string tableName)
        {
            try
            {
                var result = B1Helper.GetNextEntryIndex(tableName);
                if (result.Equals(string.Empty))
                    result = "0";
                else
                    if (result.Equals("0"))
                    {
                        result = "1";
                    }

                return result;
            }
            catch (Exception ex)
            {
                //Utility.LogException(ex);
                // Log.LogException(LogLevel.Error, ex);
                return null;
            }

        }
        protected static void SetFilter(SAPbouiCOM.EventFilters Filters)
        {
            Application.SBO_Application.SetFilter(Filters);
        }
        #endregion
    }
}

