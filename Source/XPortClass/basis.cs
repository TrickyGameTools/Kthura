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
        }
    }

}

