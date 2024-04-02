using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


using ITNSBOCustomization.Lib.Utils;

namespace ITNSBOCustomization.Lib.Localization

{
    class LocalizationConfiguration
    {
        [JsonProperty("formName")]
        public String formName;

        [JsonProperty("labelItemReference")]
        public String DateLabelItemReference;


        [JsonProperty("editItemReference")]
        public String DateEditItemReference;

        [JsonProperty("triggerredOn")]
        public String DateChangeTriggeringItem;
    }

    class LocalizationManagement {
        public static LocalizationConfiguration getLocalizationConfig(String formName)
        {
            Console.WriteLine("Obtaining config for {0}",formName);
            List<LocalizationConfiguration> formReferences = JsonConvert.DeserializeObject<List<LocalizationConfiguration>>( AssemblyHelper.GetEmbeddedResource("assets/FormReference.json"));

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
