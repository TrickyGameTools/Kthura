using NSKthura;
using System.Collections.Generic;

namespace KthuraExport {
    abstract class ExportBasis {

        static Dictionary<string, ExportBasis> Drivers = new Dictionary<string, ExportBasis>();
        static public void Register(ExportBasis Driver, string name) => Drivers[name] = Driver;
        static public void Register(ExportBasis Driver) => Register(Driver, Driver.name);
        static bool HaveDriver(string name) => Drivers.ContainsKey(name);

        public string name { get; protected set; } = "";
        abstract public string DoExport(Kthura Map);
    }

}

