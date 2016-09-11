using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PySynCS.Translator.Logger {
    public class BaseLogger {
        public delegate void __evtWrite     (BaseLogger log, Translator t, string str);
        public delegate void __evtWriteLine (BaseLogger log, Translator t, string str);
        public delegate void __evtClear     (BaseLogger log, Translator t);

        public event         __evtWrite     evtWrite;
        public event         __evtWriteLine evtWriteLine;
        public event         __evtClear     evtClear;

        public string        Name;
        
        public void Write(Translator t, string str)     { evtWrite(this, t, str); }
        public void WriteLine(Translator t, string str) { evtWriteLine(this, t, str); }
        public void Clear(Translator t)                 { evtClear(this, t); }
    }
}
