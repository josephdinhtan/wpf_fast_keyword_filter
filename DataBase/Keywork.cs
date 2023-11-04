using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WpfFastFilter.DataBase
{
    internal class Keywork
    {
        public Keywork(string _name, string? _tooltip, string _value) => (name, tooltip, value) = (_name, _tooltip, _value);
        public string name { get; set; }
        public string? tooltip { get; set; }
        public string value { get; set; }
    }
}
