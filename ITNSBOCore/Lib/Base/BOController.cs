using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

//using ITNSBOCustomization.Lib.Localization;
//using ITNSBOCustomization.Controllers;
using ITNSBOCustomization.Lib.Utils;

namespace ITNSBOCustomization.Lib.Base
{
    public abstract class BOController
    {
        protected String BusinessObjectCode;
        public static Dictionary<String, List<BOController>> FormBOControllers = new Dictionary<String, List<BOController>>();

        public BOController(String businessObjectCode) {
            BusinessObjectCode = businessObjectCode;
        }

        //ensures the controller registry is populated with a list to avoid errors.
        private static void EnsureBOListenerRegistry(String BoCode)
        {
            if (!FormBOControllers.ContainsKey(BoCode))
            {
                FormBOControllers[BoCode] = new List<BOController>();
            }
        }

        public static void RegisterBOEventListener(BOController BoCtrl)
        {
            EnsureBOListenerRegistry(BoCtrl.BusinessObjectCode);
            FormBOControllers[BoCtrl.BusinessObjectCode].Add(BoCtrl);
        }


        public static List<BOController> GetBOListeners(String BoCode)
        {
            
            Console.WriteLine("Obtaining BOCtrls for {0}", BoCode);
            EnsureBOListenerRegistry(BoCode);
            
            return FormBOControllers[BoCode];
        }

        public static void UnregisterBOEventListener(BOController BoCtrl)
        {
            EnsureBOListenerRegistry(BoCtrl.BusinessObjectCode);
            FormBOControllers[BoCtrl.BusinessObjectCode].Remove(BoCtrl);
        }

        //triggerred when a data event is fired in a form
        public static void OnDataEvent(ref SAPbouiCOM.BusinessObjectInfo BusinessObjectInfo)
        {
            foreach (Type x in AssemblyHelper.GetEnumerableOfType<BOController>()) {
                Console.WriteLine("calling onevent of {0}", x);
                object[] paramts = {  BusinessObjectInfo};
                x.GetMethod("onEvent").Invoke(null,  paramts);
            };
        }



        public static void onEvent(ref SAPbouiCOM.BusinessObjectInfo BusinessObjectInfo) { }
    }
}
