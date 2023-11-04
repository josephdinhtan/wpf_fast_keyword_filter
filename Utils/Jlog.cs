using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WpfFastFilter.Utils
{
    public static class Jlog
    {
        public static void d(String message) {
            Trace.WriteLine("Debug: " + message);
        }
        public static void e(String message)
        {
            Trace.WriteLine("Error: " + message);
        }
    }
}
