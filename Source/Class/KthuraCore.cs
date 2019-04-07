using System.Collections.Generic;
using UseJCR6;

namespace NSKthura { 

    class KthuraObject {
        readonly public string kind;
        readonly public KthuraLayer Parent;
        public int x,y;
        public int w, h;
        public int insertx, inserty;
        public int R, G, B;
        public int ScaleX, ScaleY;
        public float TrueScaleX => (float)ScaleX / 1000;
        public float TrueScaleY => (float)ScaleY / 1000;
        public string Labels = "";
        public string Tag = "";
        public KthuraObject(string objectkind, KthuraLayer prnt) {
            kind = objectkind;
            Parent = prnt;
        }
    }

    class KthuraLayer {
        List<KthuraObject> Objects = new List<KthuraObject>(); // Really this is basically the true core of Kthura!
        Dictionary<string, KthuraObject> TagMap = new Dictionary<string, KthuraObject>();
        Dictionary<string, List<KthuraObject>> LabelMap = new Dictionary<string, List<KthuraObject>>();
        SortedDictionary<string, KthuraObject> DominanceMap = new SortedDictionary<string, KthuraObject>();
        Kthura Parent;
        public KthuraLayer(Kthura hufter) {
            Parent = hufter;
        }
    }

    class Kthura {
        public Dictionary<string, KthuraLayer> Layers = new Dictionary<string, KthuraLayer>();
        #region Data specific to the map!
        public TJCRDIR TextureJCR;
        #endregion

        #region New Map constructors
        private Kthura() { } // I won't allow "New". A bit dangerous. So some static functions act as "constructors".
        public static Kthura Create() => Create(DefaultTextureJCR);
        public static Kthura Create(TJCRDIR TexJCR) {
            var ret = new Kthura();
            ret.TextureJCR = TexJCR;
            // Please note! Single layer maps, were already officially deprecated a few years before this project began in C#
            // And if BlitzMax didn't get into the state it's now, this C# version may actually never have been created (at least not by me)
            // It's only logical I will not support it any more in the C# version :P
            ret.Layers["_BASE"] = new KthuraLayer(ret);

            // All done
            return ret;
        }

        public static Kthura Load(TJCRDIR sourcedir,string prefix,TJCRDIR TexJcr)  {
            // This is the TRUE load routine. All overloads eventually lead to this one! ;-P
            throw new System.Exception("Kthura.Load: This routine is not yet available!");
        }

        public static Kthura Load(string sourcefile,string prefix, TJCRDIR TexJCR) {
            var sf = JCR6.Dir(sourcefile);
            return Load(sf, prefix, TexJCR);
        }

        public static Kthura Load(TJCRDIR sourcedir, string prefix="") {
            var tj = DefaultTextureJCR;
            if (tj == null) tj = sourcedir;
            return Load(sourcedir, prefix, tj);
        }

        public static Kthura Load(string sourcefile, string prefix = "") {
            var sf = JCR6.Dir(sourcefile);
            return Load(sf, prefix);
        }
        #endregion

        #region Core functions as statics (since C# requires classes even when you don't need them).
        static TJCRDIR DefaultTextureJCR = null;
        static public void SetDefaultTextureJCR(TJCRDIR j) => DefaultTextureJCR = j;
        #endregion
    }

}
