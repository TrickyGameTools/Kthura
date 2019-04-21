// Lic:
// Class/KthuraDraw.cs
// Draw Kthura for C#
// version: 19.04.21
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
using TrickyUnits;

namespace NSKthura{

    delegate void DelDrawZone(KthuraObject obj, int ix = 0, int iy = 0, int scrollx = 0, int scrolly = 0);
    delegate void DelDrawPoint(KthuraObject obj, int ix = 0, int iy = 0, int scrollx = 0, int scrolly = 0);

    abstract class KthuraDraw {
        #region The Abstract part every Draw Driver must have!
        abstract public void DrawTiledArea(KthuraObject obj, int ix = 0, int iy = 0, int scrollx = 0, int scrolly = 0);
        abstract public void DrawObstacle(KthuraObject obj, int ix = 0, int iy = 0, int scrollx = 0, int scrolly = 0);
        abstract public int ObjectWidth(KthuraObject obj);
        abstract public int ObjectHeight(KthuraObject obj);
        abstract public bool HasTexture(KthuraObject obj);
        #endregion

        #region Some static functions for Kthura's functionality in general
        static public KthuraDraw DrawDriver = null;
        static public DelDrawZone DrawZone = delegate { }; // Normally zones should be ignored! Only editors should use this, and thus this delegate!
        static public DelDrawPoint DrawPivot = null; // Only needed in editors
        static public DelDrawPoint DrawExit = null;
        static public DelDrawPoint DrawCSpot = null;
        static public bool IgnoreVisibility = false; // If set to true, invisible objects will still be shown.

        static public void DrawMap(Kthura map, string layer, int scrollx = 0, int scrolly = 0, int x = 0, int y = 0) => DrawMap(map.Layers[layer], scrollx, scrolly, x, y);
        static public void DrawMap(KthuraLayer layer, int scrollx = 0, int scrolly = 0, int x = 0, int y = 0) {
            if (layer.ObjectDrawOrder == null) layer.RemapDominance();
            foreach(KthuraObject obj in layer.ObjectDrawOrder) {
                if (obj.Visible || IgnoreVisibility) {
                    if (true) { // This looks useless now, but this routine will be used later in optimalisation to see if an object is actually on screen, and if not, ignore it.
                        switch (obj.kind) {
                            case "TiledArea":
                                if (DrawDriver != null) DrawDriver.DrawTiledArea(obj, x, y, scrollx, scrolly);
                                break;
                            case "Obstacle":
                                if (DrawDriver != null) DrawDriver.DrawObstacle(obj, x, y, scrollx, scrolly);
                                break;
                            case "Zone": DrawZone(obj, x, y, scrollx, scrolly); break;
                            case "Pivot":
                                DrawPivot?.Invoke(obj, x, y, scrollx, scrolly); break;
                            case "Exit":
                                DrawExit?.Invoke(obj, x, y, scrollx, scrolly); break;
                            default:
                                if (qstr.Prefixed(obj.kind,"$")) {
                                    DrawCSpot?.Invoke(obj, x, y, scrollx, scrolly); break;
                                } else {
                                    throw new Exception($"Unknown drawing object kind: {obj.kind}");
                                }
                                break;
                        }
                    }
                }
            }
        }
        #endregion
    }
}










