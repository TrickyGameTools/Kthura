// Lic:
// Class/KthuraSave.cs
// Save Kthura for C#
// version: 19.04.22
// Copyright (C)  Jeroen P. Broks
// This software is provided 'as-is', without any express or implied
// warranty.  In no event will the authors be held liable for any damages
// arising from the use of this software.
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
// 1. The origin of this software must not be misrepresented; you must not
// claim that you wrote the original software. If you use this software
// in a product, an acknowledgment in the product documentation would be
// appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be
// misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.
// EndLic



using System;
using System.Text;
using System.Collections.Generic;
using UseJCR6;

namespace NSKthura {

    class KthuraSave {

        static KthuraSave() {
            bl2bt[false] = 0;
            bl2bt[true] = 1;
        }

        static Dictionary<bool, byte> bl2bt = new Dictionary<bool, byte>();

        static string GenObjects(Kthura map) {
            StringBuilder ret = new StringBuilder(300000);
            // Header
            ret.Append($"-- Generated: {DateTime.Now.ToString()}, by Kthura for C#\n\n");

            // Layer list
            ret.Append("LAYERS\n");
            foreach (string lay in map.Layers.Keys)
                ret.Append($"\t{lay}\n");
            ret.Append("__END\n\n");

            // Layer head
            foreach (string lay in map.Layers.Keys) {
                var layer = map.Layers[lay];
                ret.Append($"LAYER = {lay}\n\tGRID={layer.GridX}x{layer.GridY}\n");
                foreach (KthuraObject obj in layer.Objects) {
                    var vis = 0;
                    if (obj.Visible) vis = 1;
                    ret.Append($"\tNEW\n\t\tKIND = {obj.kind}\n\t\tCOORD = {obj.x},{obj.y}\n\t\tINSERT = {obj.insertx * (-1)},{obj.inserty * (-1)}\n\t\tROTATION = {obj.RotationDegrees}\n\t\tSIZE = {obj.w}x{obj.h}\n\t\tTAG = {obj.Tag}\n\t\tLABELS = {obj.Labels}\n\t\tDOMINANCE = {obj.Dominance}\n\t\tTEXTURE = {obj.Texture}\n\t\tCURRENTFRAME = {obj.AnimFrame}\n\t\tFRAMESPEED = {obj.AnimSpeed}\n\t\tALPHA = {(float)obj.Alpha1000 / 1000}\n\t\tVISIBLE = {vis}\n\t\tCOLOR = {obj.R}, {obj.G},{obj.B}\n\t\tIMPASSIBLE = {bl2bt[obj.Impassible]}\n\t\tFORCEPASSIBLE = {bl2bt[obj.ForcePassible]}\n\t\tSCALE = {obj.ScaleX},{obj.ScaleY}\n\t\tBLEND = 0\n");
                    foreach (string k in obj.MetaData.Keys) ret.Append($"\t\t\tDATA.{k} = {obj.MetaData[k]}\n");
                }
            }
            return ret.ToString();
        }

        public static void Save(Kthura map, TJCRCreate j, string prefix="", string storage="Store",string Author="",string Notes="") {
            j.NewStringMap(map.MetaData, $"{prefix}Data", storage, Author, Notes);
            j.AddString(GenObjects(map), $"{prefix}Objects", storage, Author, Notes);
        }

        public static void PSave(Kthura map, string outfile, string prefix, string storage="Store", string Author="",string Notes="") {
            var j = new TJCRCreate(outfile, storage);
            Save(map, j, prefix, storage, Author, Notes);
            j.Close();
        }

        public static void Save(Kthura map, string outfile, string storage = "Store", string Author = "", string Notes = "") => PSave(map, outfile, "", storage, Author, Notes);

    }
}

