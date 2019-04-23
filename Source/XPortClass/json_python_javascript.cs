// Lic:
// Export Kthura
// JSON, JavaScript, Python
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
// Version: 19.04.23
// EndLic


using System.Text;
using NSKthura;

namespace KthuraExport_NS {
    class json:ExportBasis {

        internal static void IIN() {
            if (!HaveDriver("json")) {
                var me = new json();
                me.name = "json";
                Register(me);
            }
        }

        public override string DoExport(Kthura Map) {
            var ret = new StringBuilder("{\n\n");
            bool comma = false;
            bool comma2 = false;
            // Meta
            ret.Append("\t\"MetaData\" : {\n\t\t");
            foreach (string key in Map.MetaData.Keys) {
                if (comma)
                    ret.Append(",\n\t\t");
                comma = true;
                ret.Append($"\"{key}\" : \"{Map.MetaData[key]}\"");
            }
            ret.Append("\t},\n\n");
            // Layers
            comma = false;
            ret.Append("\t\"Layers\" : {\n");
            foreach (string key in Map.Layers.Keys) {
                var lay = Map.Layers[key];
                comma2 = false;
                if (comma)
                    ret.Append(",\n");
                comma = true;
                ret.Append($"\t\t\"{key}\" : {'{'}\n\t\t\t\"Grid\" : [{lay.GridX}, {lay.GridY}],\n\t\t\t\"Objects\" : [\n");
                foreach (KthuraObject obj in lay.Objects) {
                    if (comma2)
                        ret.Append(",\n"); comma2 = true;
                    ret.Append("\t\t\t\t{\n");
                    ret.Append($"\t\t\t\t\t'Kind' : '{obj.kind}',\n");
                    ret.Append($"\t\t\t\t\t'X' : {obj.x},  'Y': {obj.y},\n");
                    ret.Append($"\t\t\t\t\t'Width' : {obj.w}, 'Height' : {obj.h},\n");
                    ret.Append($"\t\t\t\t\t'InsertX' : {obj.insertx}, 'InsertY' : {obj.inserty},\n");
                    ret.Append($"\t\t\t\t\t'Texture' : '{obj.Texture}',\n");
                    ret.Append($"\t\t\t\t\t'Visible' : '{obj.Visible}',\n");
                    ret.Append($"\t\t\t\t\t'ScaleX' : {obj.ScaleX}, 'ScaleY' : {obj.ScaleY},\n");
                    ret.Append($"\t\t\t\t\t'Red' : {obj.R}, 'Green' : {obj.G}, 'Blue' : {obj.B},\n");
                    ret.Append($"\t\t\t\t\t'AnimSpeed' : {obj.AnimSpeed}, 'Frame' : {obj.AnimFrame},\n");
                    ret.Append($"\t\t\t\t\t'Dominance' : {obj.Dominance},\n");
                    ret.Append($"\t\t\t\t\t'Impassible' : '{obj.Impassible}', 'ForcePassible' : '{obj.ForcePassible}',\n");
                    ret.Append($"\t\t\t\t\t'Tag' : '{obj.Tag}',\n");
                    ret.Append("\t\t\t\t\t'Data' : {\n");
                    var comma3 = false;
                    foreach(string dkey in obj.MetaData.Keys) {
                        if (comma3)
                            ret.Append(",\n"); comma3 = true;
                        ret.Append($"\t\t\t\t\t\t'{dkey}' : '{obj.MetaData[dkey]}'");
                    }
                    ret.Append("\t\t\t\t\t}\n");
                    ret.Append("\t\t\t\t}");
                }
                ret.Append("\n\t\t\t]");
                ret.Append("\n\t\t}");
            }            
            ret.Append("\n\t}\n");
            ret.Append("}");
            return ret.ToString();
        }

        public override string ExportedFile(string inputfile) => inputfile + ".json";

    }

    class python : ExportBasis {
        public override string DoExport(Kthura Map) => $"KthuraMap = {Get("json").DoExport(Map)}";
        public override string ExportedFile(string inputfile) => $"{inputfile}.py";
        public python() {
            json.IIN();
            name = "python";
            Register(this);
        }
    }

    class javascript : ExportBasis {
        public override string DoExport(Kthura Map) => $"let KthuraMap = {Get("json").DoExport(Map)}";
        public override string ExportedFile(string inputfile) => $"{inputfile}.js";
        public javascript() {
            json.IIN();
            name = "javascript";
            Register(this);
        }

    }
}


