using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using Newtonsoft.Json;
using System.Net;

using ITNSBOCustomization.Lib.Utils;
using Newtonsoft.Json;
using ITNSBOCustomization.Lib.Core;
//using System.Net.Http.Headers;
//using System.Net.Http;
//using RestSharp;

namespace ITNSBOCustomization.Lib.Localization
{

    class CBMSIntegration
    {
      
        const string OBJCODE_AR_INV = "13";
        const string OBJCODE_AR_CREDIT = "14";
        private static string configLocation = "assets/IRDInfo.json";
        private static CBMSConfiguration cbmsConfig;
        private static SAPbobsCOM.Recordset apiConfig;
        private static string message = null;
        public static CBMSParsedReponse cbmsResponse;
        static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static bool BORequiresCompliance(String BusinessObjectType) {
            if (cbmsConfig == null) {
                try {
                    logger.Debug("Fetching Api details");
                    cbmsConfig = new CBMSConfiguration();//JsonConvert.DeserializeObject<List<CBMSConfiguration>>(AssemblyHelper.GetEmbeddedResource(configLocation));
                    apiConfig = DBUtil.callStoredProc("ITN_GETAPIDETAILS");
                    cbmsConfig.billApiUrl = (string)apiConfig.Fields.Item("U_BillApiUrl").Value;
                    cbmsConfig.billReturnApiUrl = (string)apiConfig.Fields.Item("U_BillReturnApiUrl").Value;
                    cbmsConfig.isEnabled = (string)apiConfig.Fields.Item("U_Enabled").Value;
                    logger.Debug("Api details"+cbmsConfig.billApiUrl+cbmsConfig.billReturnApiUrl);

                }
                catch (Exception ex)
                {
                    logger.Debug("Exception on BORequires Compliance while fetchin API details");
                    Addon.showStatusBarMessage(ex);
                }
                
            }

            if (((BusinessObjectType == OBJCODE_AR_INV) || (BusinessObjectType == OBJCODE_AR_CREDIT)) && cbmsConfig.isEnabled.ToLower() == "true") {
                return true;
            }

            return false;
        }

        public static CBMSParsedReponse uploadSalesBill(SalesDataObject billObj)
        {
            logger.Debug("Uploading salesInvocice Bill" + billObj.invoice_number +" " +billObj.invoice_date);
            string responseTxt = sendCBMSRequest(cbmsConfig.billApiUrl, billObj);
            parseCBMSResponse(responseTxt, billObj);
            return cbmsResponse;
        }

        public static CBMSParsedReponse uploadSalesReturn(SalesReturnDataObject returnBillObj)
        {
            logger.Debug("Uploading SalesReturn Bill" + returnBillObj.credit_note_number +" " +returnBillObj.credit_note_date);

            string responseTxt =  sendCBMSRequest(cbmsConfig.billReturnApiUrl, returnBillObj);
            parseCBMSResponse(responseTxt, returnBillObj);
            return cbmsResponse;       
        }



        public static string sendCBMSRequest(string apiServer, object billObject)
        {
            string jsonObject = JsonConvert.SerializeObject(billObject);

            //var client = new RestClient("http://103.1.92.174:9050/api/bill");
            //client.Timeout = -1;
            //var request = new RestRequest(Method.POST);
            //request.AddHeader("Content-Type", "application/json");
            //request.AddParameter("application/json", "{\r\n    \"username\": \"Test_CBMS\",\r\n    \"password\": \"test@321\",\r\n    \"seller_pan\": \"999999999\",\r\n    \"buyer_pan\": \"602831264\",\r\n    \"fiscal_year\": \"FY1920\",\r\n    \"buyer_name\": \"Akarti Enterprises\",\r\n    \"invoice_number\": \"1000016\",\r\n    \"invoice_date\": \"2077.03.30\",\r\n    \"total_sales\": 53.1,\r\n    \"taxable_sales_vat\": 53.1,\r\n    \"vat\": 6.9,\r\n    \"excisable_amount\": 0.0,\r\n    \"excise\": 0.0,\r\n    \"taxable_sales_hst\": 0.0,\r\n    \"hst\": 0.0,\r\n    \"amount_for_esf\": 0.0,\r\n    \"esf\": 0.0,\r\n    \"export_sales\": 0.0,\r\n    \"tax_exempted_sales\": 0.0,\r\n    \"isrealtime\": false,\r\n    \"datetimeClient\": \"2020-07-14T10:52:06.9366929+05:45\"\r\n}", ParameterType.RequestBody);
            //IRestResponse response = client.Execute(request);
            //return response.Content;
            logger.Debug("Send CMBS REQUEST");
            //using (var client1 = new HttpClient())
            //{
            //    Uri address = new Uri(apiServer);
            //    client1.DefaultRequestHeaders.Accept.Clear();
            //    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    var response = client1.PostAsJsonAsync(address, jsonObject).Result;
            //    return response.IsSuccessStatusCode.ToString();
            //}
            using (WebClient webClient = new WebClient())
            {
                //webClient.UseDefaultCredentials = true;
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                //webClient.Headers.Add("Accept:application/json");
                Uri address = new Uri(apiServer);
                string str = webClient.UploadString(address, "POST", jsonObject);
                return str;
            }
        }

        public static void parseCBMSResponse(string str, object billObject)
        {
            string msg = null;
            bool isSuccess;

            if (str == "101" && billObject.GetType() == typeof(SalesDataObject))
            {
                logger.Debug("Bill already exists." + 101);
                isSuccess = true;
                message = "Bill already exists.";
            }
            else if (str == "101" && billObject.GetType() == typeof(SalesReturnDataObject))
            {
                logger.Debug("Bill does not exists." + 101);
                isSuccess = true;
                message = "Bill does not exist.";
            }
            else if (str == "104")
            {
                logger.Debug("Model Invalid" + 101);

                isSuccess = false;
                message = "Model invalid.";
            }
            else if (str == "200")
            {
                logger.Debug("Success" + 101);
                isSuccess = true;
                message = "Success.";
            }
            else if (str == "102")
            {
                logger.Debug("Exception while saving bill details. Please check model fields and values." + 102);
                isSuccess = false;
                message = "Exception while saving bill details. Please check model fields and values.";
            }
            else if (str == "100")
            {
                logger.Debug("API credentials do not match" + 100);

                isSuccess = false;
                message = "API credentials do not match";
            }
            else if (str == "103")
            {
                logger.Debug("Unknown exceptions. Please check API URL and model fields and values." + 103);
                isSuccess = false;
                message = "Unknown exceptions. Please check API URL and model fields and values.";
            }
            else
            {
                logger.Debug(str);
                isSuccess = false;
                message = "Bill does not exist";
            }
            cbmsResponse = new CBMSParsedReponse();
            cbmsResponse.isSuccess = isSuccess;
            cbmsResponse.responseMsg = message;
            logger.Debug(message + isSuccess);
        }

    }

    class CBMSConfiguration {
        //[JsonProperty("enabledObjects")]
        //public List<String> enabledObjects;

        //[JsonProperty("billApiUrl")]
        public string billApiUrl;

        //[JsonProperty("billReturnApiUrl")]
        public string billReturnApiUrl;

        public string isEnabled;
    }

    class CBMSParsedReponse {
        public bool isSuccess;
        public string responseMsg;
    }
}
