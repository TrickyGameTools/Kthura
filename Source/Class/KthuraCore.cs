using System.Collections.Generic;
using System.Diagnostics;
using System;
using UseJCR6;

namespace NSKthura { 

    class KthuraObject {
        readonly public string kind;
        readonly public KthuraLayer Parent;
        public int x,y;
        public int w, h;
        public int insertx = 0, inserty = 0;
        public int R=255, G=255, B=255;
        public int ScaleX, ScaleY;
        public int Dominance = 20;        
        public float TrueScaleX => (float)ScaleX / 1000;
        public float TrueScaleY => (float)ScaleY / 1000;
        public string Labels = "";
        string _Tag = "";
        public string Tag { get => _Tag; set {
                _Tag = value;
                if (Kthura.automap) Parent.RemapTags();
            }
        }

        #region recalc values
        public int Alpha255=255;
        public int Alpha1000 {
            // Due to BlitzMax settings I had to use a 1=max scale, as I needed Kthura not to suffer from rouding errors, the 1000 scale has been set up.
            // Now MonoGame uses the (actual) 0-255 scale, but as Kthura maps do not cover that properly, this measure was taken.
            get {
                float A = Alpha255;
                return (int)((A / 255) * 1000);
            }
            set {
                Alpha255 = (int)((value / 1000) * 255);
                if (Alpha255 < 0) Alpha255 = 0;
                if (Alpha255 > 255) Alpha255 = 0;
            }
        }

        double _rotrad = 0;
        int _rotdeg = 0;
        double RotationRadians {
            set {
                _rotrad = value;
                _rotdeg = (int)(value * (180 / Math.PI));
            }
            get => _rotrad;
        }
        int RotationDegrees {
            set {
                _rotdeg = value;
                _rotrad = ((double)value * (180 / Math.PI));
            }
        }
        

        #endregion

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
            Parent = hufter ?? throw new Exception("What the....... do you think you're doing???");
        }

        #region Remap
        public void RemapTags() {
            TagMap.Clear();
            foreach(KthuraObject o in Objects) {
                if (o.Tag != "") {
                    var ok = true;
                    if (o.Tag != o.Tag.Trim()) { ok = false; Kthura.Log($"RemapTags: \"{o.Tag}\": invalid tag!"); }
                    if (TagMap.ContainsKey(o.Tag) { ok = false; Kthura.Log($"RemapTags: \"{o.Tag}\": duplicate tag!"); }
                    if (ok) TagMap[o.Tag] = o;
                }
            }
        }
        #endregion
    }

        delegate void KthuraLog(string l);

    /// <summary>
    /// The class containing Kthura core features and some specific data about a Kthura map.
    /// </summary>
    class Kthura {
        #region debug
       static public KthuraLog Log = delegate (string m) { Debug.Print(m); };
        #endregion

        #region Data specific to the map!
        /// <summary>
        /// When set to true, values requiring remapping (such as Tags, Lables, Dominance, BlockMapValues and (believe it or not) the y coordinate) will immediately cause that to happen.
        /// Please note that that if you have a lot of modifications to do, this will take away performance, so then you can best turn it off, do your modifications and remap everything later.
        /// </summary>
        static public bool automap = true;
        public TJCRDIR TextureJCR;
        public SortedDictionary<string, KthuraLayer> Layers = new SortedDictionary<string, KthuraLayer>();
        #endregion

        #region Control Methods
        public void CreateLayer(string name) {
            if (Layers.ContainsKey(name)) {
                Debug.Print($"Kthura map already has a layer named {name}. Please pick a different name!");
                return;
            }
            Layers[name] = new KthuraLayer(this);
        }
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
