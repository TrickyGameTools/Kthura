// Lic:
// KthuraBubble/KthuraBubbleBlockMapScreen.cs
// KBMS
// version: 19.11.24
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
using TrickyUnits;
using Bubble;
using NSKthura;
using System;
using Microsoft.Xna.Framework;


namespace KthuraBubble {

    class BubBlockMap : HardFlowClass {

        static Kthura Map;
        static KthuraLayer MapLayer;
        static int ScrollX;
        static int ScrollY;
        static int time;
#if DEBUG
        static bool first = false;
#endif

        static HardFlowClass ReturnFlow;
        readonly static public BubBlockMap Flow=new BubBlockMap();

        

        static Color BlCol(bool a) {
            if (a)
                return new Color(0, 180, 255);
            else
                return Color.Black;
        }

        public override void Draw(GameTime gameTime) {

            // Blocked, or not?
            for (int y = 0; y < TQMG.ScrHeight; y++) for (int x = 0; x < TQMG.ScrWidth; x++) {
#if DEBUG
                    if (first)
                        BubConsole.WriteLine($"Pure({x},{y}); Scroll({ScrollX},{ScrollY}); Combined({x + ScrollX},{ y + ScrollY});  Block({MapLayer.Block(x + ScrollX, y + ScrollY)})");
#endif
                    TQMG.Color(
                        BlCol(
                            MapLayer.Block(x + ScrollX, y + ScrollY)
                            )
                        );
                    TQMG.Plot(x, y);
                }
#if DEBUG
            first = false;
#endif

            // Actors
            foreach (KthuraObject O in MapLayer.Objects) {
                TQMG.Color(255, 0, 0);
                if (O.kind == "Actor") TQMG.Plot(O.x-ScrollX, O.y-ScrollY);
            }
        }

        public override void Update(GameTime gameTime) {
            --time;
            if (time < 0) FlowManager.GoHardFlow(ReturnFlow);
        }

        public static void ComeToMe(HardFlowClass back, Kthura m, KthuraLayer l,int x, int y) {
            Map = m;
            MapLayer = l;
            ScrollX = x;
            ScrollY = y;
            ReturnFlow = back;
            FlowManager.GoHardFlow( Flow);
            time = 2500;
        }
        public static void ComeToMe(HardFlowClass back, Kthura m, string l, int x, int y) => ComeToMe(back, m, m.Layers[l], x, y);
    }

    class KthBlockAPI {

        public void ComeToMe(int ID,string Lay,int x, int y) {
            try {
                var m = KthuraBubble.GetMap(ID);
                BubBlockMap.ComeToMe(FlowManager.GetHardFlow,m, Lay, x, y);
            } catch (Exception Crap) {
                SBubble.MyError("Kthura Blockmap Debug Error", Crap.Message, $"{ID}/{Lay}/({x},{y})");
            }
        }

        public static void Init(string vm) {
            var api = new KthBlockAPI();
            var s = SBubble.State(vm).state;
            s["KTHURA_BLOCK"] = api;
            //*
            SBubble.DoNIL(vm, @"

                global void ShowBlockMap(map,string Layer,int x, int y)
                    switch type(map)
                        case 'int'
                            KTHURA_BLOCK:ComeToMe(map,Layer,x,y)
                        case 'table'
                            KTHURA_BLOCK:ComeToMe(map.ID,Layer,x,y)
                        default
                            error('Blockmap request faulty!')
                    end
                end

            ", "Console initizer");

        }

    }
}


