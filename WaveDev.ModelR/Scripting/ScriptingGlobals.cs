using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveDev.ModelR.Scripting
{
    public class ScriptingGlobals
    {
        public ScriptingGlobals(ScriptingContext adapter)
        {
            ModelR = adapter;
        }

        public ScriptingContext ModelR
        {
            get;

            // [RS] Scripts cannot access and manipulate the globals object. So we can use a public setter here for full flexibility. 
            set;
        }
    }
}
