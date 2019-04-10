using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KthuraEdit.Stages;
using NSKthura;

namespace KthuraEdit
{

    // This class will not really been used by the Kthura editor itself
    // But merely serve in order to 
    class Lua_API {

        // Should Replace Lua's own print command
        public void KthuraPrint(string content) => DBG.Log(content);

        // When creating new CSpots, the "ME" object should contain the Kthura object in question.
        public KthuraObject ME;
    }
}
