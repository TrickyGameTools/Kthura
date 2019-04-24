// Lic:
// Export Kthura
// Lua
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



using System.Text;
using NSKthura;

namespace KthuraExport_NS {
    class xport_lua:ExportBasis {

        public xport_lua() {
            name = "lua";
            Register(this);
        }

        public override string DoExport(Kthura Map) {
            var ret = new StringBuilder("local ret = {\n\n");
            bool comma = false;
            bool comma2 = false;
            // Meta
            ret.Append("\tMetaData = {\n\t\t");
            foreach (string key in Map.MetaData.Keys) {
                if (comma)
                    ret.Append(",\n\t\t");
                comma = true;
                ret.Append($"[\"{key}\"] = \"{Map.MetaData[key]}\"");
            }
            ret.Append("\n\t},\n\n");
            // Layers
            comma = false;
            ret.Append("\tLayers = {\n");
            foreach (string key in Map.Layers.Keys) {
                var lay = Map.Layers[key];
                comma2 = false;
                if (comma)
                    ret.Append(",\n");
                comma = true;
                ret.Append($"\t\t[\"{key}\"] = {'{'}\n\t\t\tGrid = {'{'}{lay.GridX}, {lay.GridY}{'}'},\n\t\t\tObjects = {'{'}\n");
                foreach (KthuraObject obj in lay.Objects) {
                    if (comma2)
                        ret.Append(",\n"); comma2 = true;
                    ret.Append("\t\t\t\t{\n");
                    ret.Append($"\t\t\t\t\tKind = '{obj.kind}',\n");
                    ret.Append($"\t\t\t\t\tX = {obj.x},  Y = {obj.y},\n");
                    ret.Append($"\t\t\t\t\tWidth = {obj.w}, Height = {obj.h},\n");
                    ret.Append($"\t\t\t\t\tInsertX = {obj.insertx}, InsertY = {obj.inserty},\n");
                    ret.Append($"\t\t\t\t\tTexture = '{obj.Texture}',\n");
                    ret.Append($"\t\t\t\t\tVisible = {obj.Visible.ToString().ToLower()},\n");
                    ret.Append($"\t\t\t\t\tScaleX = {obj.ScaleX}, ScaleY = {obj.ScaleY},\n");
                    ret.Append($"\t\t\t\t\tRed = {obj.R}, Green = {obj.G}, Blue = {obj.B},\n");
                    ret.Append($"\t\t\t\t\tAnimSpeed = {obj.AnimSpeed}, Frame = {obj.AnimFrame},\n");
                    ret.Append($"\t\t\t\t\tDominance = {obj.Dominance},\n");
                    ret.Append($"\t\t\t\t\tImpassible = {obj.Impassible.ToString().ToLower()}, ForcePassible = {obj.ForcePassible.ToString().ToLower()},\n");
                    ret.Append($"\t\t\t\t\tTag = '{obj.Tag}',\n");
                    ret.Append("\t\t\t\t\tData = {\n");
                    var comma3 = false;
                    foreach(string dkey in obj.MetaData.Keys) {
                        if (comma3)
                            ret.Append(",\n"); comma3 = true;
                        ret.Append($"\t\t\t\t\t\t['{dkey}'] = '{obj.MetaData[dkey]}'");
                    }
                    ret.Append("\t\t\t\t\t}\n");
                    ret.Append("\t\t\t\t}");
                }
                ret.Append("\n\t\t\t}");
                ret.Append("\n\t\t}");
            }            
            ret.Append("\n\t}\n");
            ret.Append("}\n\nreturn ret");
            return ret.ToString();
        }

        public override string ExportedFile(string inputfile) => inputfile + ".lua";

    }
    
}



