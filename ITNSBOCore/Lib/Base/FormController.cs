using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ITNSBOCustomization.Lib.Localization;
using Newtonsoft.Json;

//using ITNSBOCustomization.Controllers;

using SAPbouiCOM.Framework;
using NLog;
using ITNSBOCustomization.Lib.Utils;
using ITNSBOCustomization.Lib.Core;
namespace ITNSBOCustomization.Lib.Base
{
    public abstract class FormController
    {
        public String FormUID;
        public int FormTypeCount;
        public string FormCode;

        public SAPbouiCOM.Form oForm = default(SAPbouiCOM.Form);

        public Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /*dictionary of all controllers registered in the addon. 
          each form is identified with formuid(string), and each instances of the form are identified 
          with formtypecount(an integer). The registry may contain multiple controllers for each instances,
          eg a localization controller and  some other addon allowing same base to work with multiple addons.
        */
        public static Dictionary<String, Dictionary<int, List<FormController>>> ActiveFormControllers = new Dictionary<String, Dictionary<int, List<FormController>>>();

        public FormController(String formUID, int formTypeCount)
        {
            FormUID = formUID;
            FormTypeCount = formTypeCount;
            oForm = Application.SBO_Application.Forms.GetForm(FormUID, FormTypeCount);
            FormCode = GetPrimaryDataSource();
        }


        public static void onFormLoad()
        {
        }



        public String GetPrimaryDataSource()
        {
            if (this.FormCode != null)
            {
                return this.FormCode;
            }

            //Console.WriteLine(JsonConvert.SerializeObject(this.oForm.DataSources));
            if (this.oForm.DataSources.DBDataSources == null || this.oForm.DataSources.DBDataSources.Count == 0)
            {
                return null;
            }

            this.FormCode = this.oForm.DataSources.DBDataSources.Item(0).TableName;
            return this.FormCode;
        }

        //ensures the controller registry is populated with a list to avoid errors.
        private static void EnsureFormRegistry(String FormUID, int FormInstanceCount)
        {
            if (!ActiveFormControllers.ContainsKey(FormUID))
            {
                ActiveFormControllers[FormUID] = new Dictionary<int, List<FormController>>();
            }

            if (!ActiveFormControllers[FormUID].ContainsKey(FormInstanceCount) || ActiveFormControllers[FormUID][FormInstanceCount] == null)
            {
                ActiveFormControllers[FormUID][FormInstanceCount] = new List<FormController>();
            }

        }

        //register a new controller to listen to events raised in its form.
        public static void RegisterController(FormController ControllerToRegister)
        {
            Console.WriteLine("Form registeres : {0} {1} {2}", ControllerToRegister.FormCode, ControllerToRegister.FormTypeCount, ControllerToRegister.FormCode);
            EnsureFormRegistry(ControllerToRegister.FormUID, ControllerToRegister.FormTypeCount);
            ActiveFormControllers[ControllerToRegister.FormUID][ControllerToRegister.FormTypeCount].Add(ControllerToRegister);
        }

        //obtains all controllers listening to a form's events.
        public static List<FormController> GetListeningControllers(String FormUID, int FormTypeCount)
        {
            if (ActiveFormControllers.ContainsKey(FormUID) && ActiveFormControllers[FormUID].ContainsKey(FormTypeCount))
            {
                return ActiveFormControllers[FormUID][FormTypeCount];
            }

            return null;
        }

        //remove an controller from receiving events about a form. 
        //should be called when an controller is done listening to events.
        public static void UnregisterController(FormController ControllerToUnregister)
        {
            try
            {
                if (ActiveFormControllers[ControllerToUnregister.FormUID][ControllerToUnregister.FormTypeCount].Contains(ControllerToUnregister))
                {
                    ActiveFormControllers[ControllerToUnregister.FormUID][ControllerToUnregister.FormTypeCount].Remove(ControllerToUnregister);
                }
            }
            catch (Exception ex)
            {
                Addon.showStatusBarMessage(ex);
            }


        }

        //unregister all controllers linked to a form instance.
        //should be called when a form is closed.
        public static void UnregisterAllControllers(String FormUID, int FormTypeCount)
        {
            EnsureFormRegistry(FormUID, FormTypeCount);
            Console.WriteLine("Cleared all listeners for {0} {1}", FormUID, FormTypeCount);
            ActiveFormControllers[FormUID][FormTypeCount] = null;
        }



        public void AddItem()
        {
        }


        //trigerred whenever an item event is fired in a form.
        public static void HandleFormItemEvent(ref SAPbouiCOM.ItemEvent EventData)
        {

            //form load event. all relevant controllers should be created at this stage
            //TODO : use reflection to identify which all controllers to load at this point.
            foreach (Type x in AssemblyHelper.GetEnumerableOfType<FormController>())
            {

                object[] paramts = { EventData };
                x.GetMethod("onFormEvent").Invoke(null, paramts);
            };
        }

        public static void onMenuEvent(ref SAPbouiCOM.MenuEvent EventData)
        {

            //form load event. all relevant controllers should be created at this stage
            //TODO : use reflection to identify which all controllers to load at this point.


            foreach (Type x in AssemblyHelper.GetEnumerableOfType<FormController>())
            {
                if (x.GetMethod("onMenuEvent") != null)
                {
                    object[] paramts = { EventData };
                    x.GetMethod("onMenuEvent").Invoke(null, paramts);
                }
            };
        }

        public static void onPrintEvent(ref SAPbouiCOM.PrintEventInfo eventInfo)
        {

            //form load event. all relevant controllers should be created at this stage
            //TODO : use reflection to identify which all controllers to load at this point.


            foreach (Type x in AssemblyHelper.GetEnumerableOfType<FormController>())
            {
                if (x.GetMethod("onPrintEvent") != null)
                {
                    object[] paramts = { eventInfo };
                    x.GetMethod("onPrintEvent").Invoke(null, paramts);
                }
            };
        }

        public virtual void handleMenuEvent(ref SAPbouiCOM.MenuEvent EventData) { }

        public abstract void FormEventTrigger(ref SAPbouiCOM.ItemEvent EventData);

        public virtual void handlePrintEvent(ref SAPbouiCOM.PrintEventInfo EventData) { }


        public void FreezeForm()
        {
            this.oForm.Freeze(true);
        }

        public void UnfreezeForm()
        {
            this.oForm.Freeze(false);
        }

        public void AddStaticText(string newID, int left, int width, int height, int top, string caption)
        {
            try
            {
                SAPbouiCOM.Item obj = oForm.Items.Add(newID, SAPbouiCOM.BoFormItemTypes.it_STATIC);
                SAPbouiCOM.StaticText oStaticText = (SAPbouiCOM.StaticText)obj.Specific;
                obj.Left = left;
                obj.Width = width;
                obj.Height = height;
                obj.Top = top + height + 2;
                obj.Visible = true;
                oStaticText.Caption = caption;
            }
            catch (Exception ex)
            {
                //throw;
                Addon.showStatusBarMessage(ex);
            }


        }

        public void AddEditText(string newID, int left, int width, int height, int top, string linkID, string table, string alias, bool isEnabled)
        {
            try
            {
                SAPbouiCOM.Item obj = oForm.Items.Add(newID, SAPbouiCOM.BoFormItemTypes.it_EDIT);
                obj.Left = left;
                obj.Width = width;
                obj.Height = height;
                obj.Top = top + height + 2;
                obj.Visible = true;
                obj.Enabled = isEnabled;
                obj.LinkTo = linkID;
                obj.AffectsFormMode = true;
                SAPbouiCOM.EditText oEditText = ((SAPbouiCOM.EditText)(obj.Specific));
                oEditText.DataBind.SetBound(true, table, alias);
            }

            catch (Exception ex)
            {
                //throw;
                Addon.showStatusBarMessage(ex);
            }
        }

        public void AddButton(string newID, int left, int width, int height, int top, string caption, bool isEnabled)
        {
            try
            {
                //SAPbouiCOM.Item oItem = (Item)oForm.Items.Item("2");  /// Existing Item on the form
                SAPbouiCOM.Item oItem1 = (SAPbouiCOM.Item)oForm.Items.Add(newID, SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                SAPbouiCOM.Button oButton = (SAPbouiCOM.Button)oItem1.Specific;
                oItem1.Top = top;
                oItem1.Left = left;
                oItem1.Width = width;
                oItem1.Height = height;
                oItem1.Enabled = isEnabled;
                oButton.Caption = caption;
            }

            catch (Exception ex)
            {
                //throw;
                Addon.showStatusBarMessage(ex);
            }
        }

        public void setItemEnabledMode(string ctrlID, bool isEnabled)
        {
            try
            {
                this.oForm.Items.Item(ctrlID).Enabled = isEnabled;
            }
            catch { }

        }

    }
}


