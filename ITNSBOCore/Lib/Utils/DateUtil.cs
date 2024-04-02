using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NepaliDateConverter;
using System.Globalization;

namespace ITNSBOCustomization.Lib.Utils
{
    public class DateUtil
    {
        public static string ConvertToBS(String adString)
        {
            adString = adString.Trim();

            if (adString == null || adString == "")
            {
                return "";
            }

            DateTime dt = DateTime.ParseExact(adString, "yyyyMMdd", CultureInfo.InvariantCulture);

            DateConverter convertedDate = DateConverter.ConvertToNepali(dt.Year, dt.Month, dt.Day);
            String bsString = convertedDate.Day.ToString("00") + "/" + convertedDate.Month.ToString("00") + "/" + convertedDate.Year;

            return bsString;
        }

        public static string ConvertToAD()
        {
            return "";
        }

        public static string ConvertToBSIRD(String adString)
        {
            adString = adString.Trim();

            if (adString == null || adString == "")
            {
                return "";
            }

            DateTime dt = DateTime.ParseExact(adString, "yyyyMMdd", CultureInfo.InvariantCulture);

            DateConverter convertedDate = DateConverter.ConvertToNepali(dt.Year, dt.Month, dt.Day);
            String bsString = convertedDate.Year + "." + convertedDate.Month.ToString("00") + "." + convertedDate.Day.ToString("00");

            return bsString;
        }


    }
}
