// Lic:
// Class/KthuraDrawFake.cs
// Kthura - Fake Draw Driver
// version: 22.03.21
// Copyright (C) 2022 Jeroen P. Broks
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


// Merely used to fake a drawing environment. This is only used for CLI tools I use for generation purposes


namespace NSKthura {

    class KthuraDrawFake : KthuraDraw {
        public override int ObjectHeight(KthuraObject obj) => 0;
        public override int ObjectWidth(KthuraObject obj) => 0;

        public override void AnimReset(KthuraObject obj) {}
        public override void DrawActor(KthuraActor obj, int ix = 0, int iy = 0, int scrollx = 0, int scrolly = 0) {}
        public override void DrawObstacle(KthuraObject obj, int ix = 0, int iy = 0, int scrollx = 0, int scrolly = 0) {}
        public override void DrawPic(KthuraObject obj, int ix = 0, int iy = 0, int scrollx = 0, int scrolly = 0) {}
        public override void DrawStretchedArea(KthuraObject obj, int ix = 0, int iy = 0, int scrollx = 0, int scrolly = 0) {}
        public override void DrawTiledArea(KthuraObject obj, int ix = 0, int iy = 0, int scrollx = 0, int scrolly = 0) {}
        public override bool HasTexture(KthuraObject obj) => false;

        public KthuraDrawFake() { DrawDriver = this; }
    }
}