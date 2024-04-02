using ITNSBOCustomization.Lib.Core;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITNSBOCustomization.Lib.Utils
{
    public class DBUtil
    {
        static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static SAPbobsCOM.Recordset callStoredProc(string spName)
        {
            SAPbobsCOM.Recordset _recordSet = null;
            string _sql;
            try
            {
                if (AddonUtils.getCompany().DbServerType == BoDataServerTypes.dst_HANADB)
                {
                    _sql = "CALL \"" + spName + "\"";
                    _recordSet = (SAPbobsCOM.Recordset)AddonUtils.getCompany().GetBusinessObject(BoObjectTypes.BoRecordset);
                    _recordSet.DoQuery(_sql);
                }
                else
                {
                    _sql = "EXEC " + spName + "";
                    _recordSet = (SAPbobsCOM.Recordset)AddonUtils.getCompany().GetBusinessObject(BoObjectTypes.BoRecordset);
                    _recordSet.DoQuery(_sql);
                }
            }
            catch (Exception ex)
            {
                Addon.showStatusBarMessage(ex);
            }
            return _recordSet;
        }

        public static SAPbobsCOM.Recordset callStoredProc(string spName, string pOne) {
            SAPbobsCOM.Recordset _recordSet = null;
            string _sql;
            try
            {
                if (AddonUtils.getCompany().DbServerType == BoDataServerTypes.dst_HANADB)
                {
                    _sql = "CALL \"" + spName + "\" ('" + pOne + "')";
                    _recordSet = (SAPbobsCOM.Recordset)AddonUtils.getCompany().GetBusinessObject(BoObjectTypes.BoRecordset);
                    _recordSet.DoQuery(_sql);
                    logger.Debug("Call" + spName +"value:"+pOne);
                }
                else
                {

                    _sql = "EXEC " + spName + " " + pOne + "";
                    _recordSet = (SAPbobsCOM.Recordset)AddonUtils.getCompany().GetBusinessObject(BoObjectTypes.BoRecordset);
                    _recordSet.DoQuery(_sql);
                    logger.Debug("Execute" + spName + "value:" + pOne);

                }
            }
            catch (Exception ex)
            {
                Addon.showStatusBarMessage(ex);
            }
            return _recordSet;
        }

        public static SAPbobsCOM.Recordset callStoredProc(string spName, string pOne, string pTwo, string pThree, string pFour)
        {
            SAPbobsCOM.Recordset _recordSet = null;
            string _sql;
            try
            {
                if(AddonUtils.getCompany().DbServerType == BoDataServerTypes.dst_HANADB)
                {

                    _sql = "CALL \"" + spName + "\" ('" + pOne + "','" + pTwo + "', '" + pThree + "', '" + pFour + "')";
                    _recordSet = (SAPbobsCOM.Recordset)AddonUtils.getCompany().GetBusinessObject(BoObjectTypes.BoRecordset);
                    _recordSet.DoQuery(_sql);
                }
                else
                {
                    _sql = "EXEC " + spName + " '" + pOne + "', '" + pTwo + "', '" + pThree + "', '" + pFour + "'";
                    _recordSet = (SAPbobsCOM.Recordset)AddonUtils.getCompany().GetBusinessObject(BoObjectTypes.BoRecordset);
                    _recordSet.DoQuery(_sql);
                }             
            }
            catch  (Exception ex)
            {
                Addon.showStatusBarMessage(ex);
            }
            return _recordSet;
        }

        public static SAPbobsCOM.Recordset callStoredProc(string spName, string pOne, string pTwo, string pThree)
        {
            SAPbobsCOM.Recordset _recordSet = null;
            string _sql;
            try
            {
                if (AddonUtils.getCompany().DbServerType == BoDataServerTypes.dst_HANADB)
                {

                    _sql = "CALL \"" + spName + "\" ('" + pOne + "','" + pTwo + "', '" + pThree + "')";
                    _recordSet = (SAPbobsCOM.Recordset)AddonUtils.getCompany().GetBusinessObject(BoObjectTypes.BoRecordset);
                    _recordSet.DoQuery(_sql);
                }
                else
                {
                    _sql = "EXEC " + spName + " '" + pOne + "', '" + pTwo + "', '" + pThree + "'";
                    _recordSet = (SAPbobsCOM.Recordset)AddonUtils.getCompany().GetBusinessObject(BoObjectTypes.BoRecordset);
                    _recordSet.DoQuery(_sql);
                }
            }
            catch (Exception ex)
            {
                Addon.showStatusBarMessage(ex);
            }
            return _recordSet;
        }

        public static SAPbobsCOM.Recordset callStoredProc(string spName, string pOne, string pTwo, string pThree, string pFour, string pFive, string pSix)
        {
            SAPbobsCOM.Recordset _recordSet = null;
            string _sql;
            try
            {
                if (AddonUtils.getCompany().DbServerType == BoDataServerTypes.dst_HANADB)
                {
                    _sql = "CALL \"" + spName + "\" ('" + pOne + "','" + pTwo + "', '" + pThree + "', '" + pFour + "', '" + pFive + "', '" + pSix + "')";
                    _recordSet = (SAPbobsCOM.Recordset)AddonUtils.getCompany().GetBusinessObject(BoObjectTypes.BoRecordset);
                    _recordSet.DoQuery(_sql);
                }
                else
                {
                    _sql = "EXEC " + spName + " '" + pOne + "', '" + pTwo + "', '" + pThree + "', '" + pFour + "', '" + pFive + "', '" + pSix + "'";
                    _recordSet = (SAPbobsCOM.Recordset)AddonUtils.getCompany().GetBusinessObject(BoObjectTypes.BoRecordset);
                    _recordSet.DoQuery(_sql);
                }
            }
            catch (Exception ex)
            {
                Addon.showStatusBarMessage(ex);
            }
            return _recordSet;
        }
    }
}
