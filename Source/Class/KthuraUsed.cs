// Lic:
// Class/KthuraUsed.cs
// Kthura Used
// version: 19.11.23
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
using UseJCR6;
using TrickyUnits;

namespace NSKthura {


    /// <summary>
    /// This class is solely created for editors, and I don't deem it very useful for games. 
    /// It will just check which textures have been used in a Kthura map 
    /// (case insensitively, due to its reliance on JCR6 which voids all case differences... yes, even in Linux).
    /// It was designed for my own Kthura Map Editor, but if you have some use for it in your own editor, hey, go ahead and use it!
    /// </summary>
    class KthuraUsed {

        Kthura Map;
        DateTime LastUpdate;
        TJCRDIR JCR;
        TMap<string, bool> Data = new TMap<string, bool>();

        public KthuraUsed(Kthura Map,TJCRDIR TexMap) {
            this.Map = Map;
            JCR = TexMap;
            Update();
        }

        void Update() {
            Data.Clear();
            foreach(KthuraLayer Layer in Map.Layers.Values) {
                foreach(KthuraObject Obj in Layer.Objects) {
                    var Tex = Obj.Texture.ToUpper();
                    Data[Tex] = true;
                }
            }
            LastUpdate = DateTime.Now;
        }

        void AutoUpdate() {
            var Nu = DateTime.Now;
            var Diff = Nu.Subtract(LastUpdate);
            if (Diff.Minutes >= 5) Update();
        }

        /// <summary>
        /// Contains "true" if the requested texture has actually been used. Please keep in mind that in order not to spook up the performance, the system only scans which textures have been used once per five minutes. So when you used a texture not used before it can take up to five minutes max until this class sees it... Same goes of course for removing them all.
        /// </summary>
        /// <param name="Tex">Texture</param>
        /// <returns></returns>
        public bool this[string Tex] {
            get {
                AutoUpdate();
                return Data[Tex.ToUpper()];
            }
        }


    }
}

