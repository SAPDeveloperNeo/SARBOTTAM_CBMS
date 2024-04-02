using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ITNSBOCustomization.Lib.Utils;

namespace HACBatchManagement.Lib
{
    class BatchConfiguration
    {
        [JsonProperty("formName")]
        public String formName;

        [JsonProperty("isEnabled")]
        public bool isEnabled;
    }

    class BatchManagement
    {
        public static BatchConfiguration getLocalizationConfig(String formName)
        {
            Console.WriteLine("Obtaining config for {0}", formName);
            List<BatchConfiguration> formReferences = JsonConvert.DeserializeObject<List<BatchConfiguration>>(AssemblyHelper.GetEmbeddedResource("assets/FormName.json"));

            for (int i = 0; i < formReferences.Count; i++)
            {
                Console.WriteLine("{0} {1} {2}", formName, formReferences[i].formName, formName == formReferences[i].formName);
                if (formReferences[i].formName == formName)
                    return formReferences[i];
            }

            return null;
        }
    }
}
