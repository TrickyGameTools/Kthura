using NSKthura;
using System.Collections.Generic;

namespace KthuraExport_NS {
    abstract class ExportBasis {

        static Dictionary<string, ExportBasis> Drivers = new Dictionary<string, ExportBasis>();
        static public void Register(ExportBasis Driver, string name) => Drivers[name] = Driver;
        static public void Register(ExportBasis Driver) => Register(Driver, Driver.name);
        static public bool HaveDriver(string name) => Drivers.ContainsKey(name.ToLower());

        public string name { get; protected set; } = "";
        abstract public string ExportedFile(string inputfile);
        abstract public string DoExport(Kthura Map);
    }

}

