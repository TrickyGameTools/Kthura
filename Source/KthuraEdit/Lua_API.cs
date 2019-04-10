using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KthuraEdit.Stages;

namespace KthuraEdit
{

    // This class will not really been used by the Kthura editor itself
    // But merely serve in order to 
    class Lua_API {
        public void KthuraPrint(string content) => DBG.Log(content);
    }
}
