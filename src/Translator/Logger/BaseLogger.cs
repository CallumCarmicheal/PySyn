using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PySynCS.Translator.Logger {
    public class BaseLogger {
        public delegate void __evtWrite     (BaseLogger log, Translator t, Cache.Cache cache, string str);
        public delegate void __evtWriteLine (BaseLogger log, Translator t, Cache.Cache cache, string str);
        public delegate void __evtClear     (BaseLogger log, Translator t, Cache.Cache cache);

        public event         __evtWrite     evtWrite;
        public event         __evtWriteLine evtWriteLine;
        public event         __evtClear     evtClear;

        public string        Name;
        
        public void Write       (Translator t, Cache.Cache cache,   string str)     { evtWrite      (this, t, cache, str); }
        public void WriteLine   (Translator t, Cache.Cache cache, string str)       { evtWriteLine  (this, t, cache, str); }
        public void Clear       (Translator t, Cache.Cache cache)                   { evtClear      (this, t, cache); }
    }
}
