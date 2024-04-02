using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ITNSBOCustomization.Lib.Core;
using System.Reflection;

namespace HACBatchManagement
{
    class Init
    {
        [STAThread]
        static void Main(string[] args)
        {
            new Addon(args);
        }

    }
}
