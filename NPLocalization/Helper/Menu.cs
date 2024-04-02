using ITNSBOCustomization.SAPB1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPLocalization.Helpers
{
    public class Menu
    {
        public void AddMenuItems()
        {
            try
            {
                B1Helper.addMenuItem("2048", "NPLocalization.Forms.UploadBillsToCBMS", "Pending IRD Sync");

                //B1Helper.AddSubMenu(MenuID_UD.MODULE, "Gate Pass master", "Gate Pass", -1, string.Concat(System.Windows.Forms.Application.StartupPath, @"\Images\Icon.png"));
                //B1Helper.addMenuItem("Gate Pass master", "Gate Pass", "Gate Pass");
                //B1Helper.addMenuItem("2304", "Shipment Tracking", "Shipment Tracking");
                //B1Helper.addMenuItem("2304", "Provisional Cost", "Provisional Cost");
                //B1Helper.addMenuItem("43537", "Letter of Credit", "Letter of Credit");
                //B1Helper.addMenuItem("43534", "Landed Cost Details Report", "Landed Cost Details Report");
                //B1Helper.addMenuItem(MenuID_UD.ReportMID, MenuID_UD.ServiceEarningDataDistributorsMID, MenuID_UD.ServiceEarningDataDistributors);
                //B1Helper.addMenuItem(MenuID_UD.ReportMID, MenuID_UD.ServiceCallPendingForInvociesMID, MenuID_UD.ServiceCallPendingForInvocies);
                ////B1Helper.addMenuItem(MenuID_UD.SERVICEMODULEMASTERS, MenuID_UD.MACHINEPRICINGMASTER, MenuID_UD.MACHINEPRICINGMASTER);
                //B1Helper.addMenuItem(MenuID_UD.ReportMID, MenuID_UD.AdministrativeDataFieldServiceCallMID, MenuID_UD.AdministrativeDataFieldServiceCall);

            }
            catch (Exception ex)
            {
            }
            
        }
    }
}
