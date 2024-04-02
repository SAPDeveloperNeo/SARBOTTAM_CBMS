using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SAPbouiCOM.Framework;

using ITNSBOCustomization.Lib.Core;

namespace ITNSBOCustomization
{
    class Program
    {
        [STAThread]
        static void Main(string[] args) { 
            new Addon(args);
        }

    }
}
