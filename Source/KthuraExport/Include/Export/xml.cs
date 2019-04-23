// Lic:
// XML exporter for Kthura
// ---
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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSKthura;
using TrickyUnits;

namespace KthuraExport_NS {
    class xport_xml : ExportBasis {

        public xport_xml() {
            name = "xml";
            Register(this);
        }

        public override string DoExport(Kthura Map) {
            var ret = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF - 8\"?>\n");
            ret.Append($"<!-- Generated by the Kthura Exporter (build:{BuildDate.sBuildDate}) -->\n\n");
            ret.Append("<Kthura>\n");
            foreach (string k in Map.MetaData.Keys) ret.Append($"\t<MetaData key=\"{k}\" value=\"{Map.MetaData[k]}\" />\n");
            ret.Append("\t<LayerList>\n");
            foreach (string k in Map.Layers.Keys) ret.Append($"\t\t<lay>{k}</lay>\n");
            ret.Append("\t</LayerList>\n");
            foreach (string k in Map.Layers.Keys) {
                var lay = Map.Layers[k];
                ret.Append($"\n\t<Layer id=\"{k}\" grid=\"{lay.GridX}x{lay.GridY}\">\n");
                foreach (KthuraObject obj in lay.Objects) {
                    ret.Append("\t\t<Object\n");
                    ret.Append($"\t\t\tKind='{obj.kind}'\n");
                    ret.Append($"\t\t\tX='{obj.x}' Y='{obj.y}'\n");
                    ret.Append($"\t\t\tWidth='{obj.w}' Height='{obj.h}'\n");
                    ret.Append($"\t\t\tInsertX='{obj.insertx}' InsertY='{obj.inserty}'\n");
                    ret.Append($"\t\t\tTexture='{obj.Texture}'\n");
                    ret.Append($"\t\t\tVisible='{obj.Visible}'\n");
                    ret.Append($"\t\t\tScaleX='{obj.ScaleX}' ScaleY='{obj.ScaleY}'\n");
                    ret.Append($"\t\t\tRed='{obj.R}' Green='{obj.G}' Blue='{obj.B}'\n");
                    ret.Append($"\t\t\tAnimSpeed='{obj.AnimSpeed}' Frame='{obj.AnimFrame}'\n");
                    ret.Append($"\t\t\tDominance='{obj.Dominance}'\n");
                    ret.Append($"\t\t\tImpassible='{obj.Impassible}' ForcePassible='{obj.ForcePassible}'\n");
                    ret.Append($"\t\t\tTag='{obj.Tag}'>\n");
                    //ret.Append("\t\t>\n");
                    foreach (string mk in obj.MetaData.Keys) ret.Append($"\t\t\t<data key='{mk}' value='{obj.MetaData[mk]}' />\n");
                    ret.Append("\t\t</Object>\n");
                }
                ret.Append("$\t</Layer>");
            }
            ret.Append("</Kthura>");
            return ret.ToString();
        }

        public override string ExportedFile(string inputfile) => inputfile + ".xml";
        
    }
}

