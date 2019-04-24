// Lic:
// Kthura Exporter
// Basis class
// 
// 
// 
// (c) Jeroen P. Broks, 
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
// Please note that some references to data like pictures or audio, do not automatically
// fall under this licenses. Mostly this is noted in the respective files.
// 
// Version: 19.04.24
// EndLic




#undef EXPORT_ALPHA

using NSKthura;
using System.Diagnostics;
using System.Collections.Generic;

namespace KthuraExport_NS {
    abstract class ExportBasis {

        static Dictionary<string, ExportBasis> Drivers = new Dictionary<string, ExportBasis>();
        static public void Register(ExportBasis Driver, string name) {
            Drivers[name] = Driver;
            Debug.WriteLine($"Registered export driver '{name}'");
#if EXPORT_ALPHA
            System.Console.WriteLine($"ALPHA> Registered export driver '{name}'");
#endif
        }
        static public void Register(ExportBasis Driver) => Register(Driver, Driver.name);
        static public bool HaveDriver(string name) => Drivers.ContainsKey(name.ToLower());
        static public ExportBasis Get(string name) {
            if (HaveDriver(name))
                return Drivers[name.ToLower()];
            else
                return null;
        }

        public string name { get; protected set; } = "";
        abstract public string ExportedFile(string inputfile);
        abstract public string DoExport(Kthura Map);

        static public void Init() {
            new xport_xml();
            new xport_lua();
            new javascript();
            new python();
        }
    }

}





