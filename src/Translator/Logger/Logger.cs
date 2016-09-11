using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PySynCS.Translator.Logger {
    public class Logger {
        private BaseLogger _dbg;
        private BaseLogger _inf;
        private BaseLogger _err;

        public BaseLogger Debug { get { return this._dbg; } }
        public BaseLogger Info  { get { return this._dbg; } }
        public BaseLogger Error { get { return this._dbg; } }
        
        public Logger() {
            _dbg = new BaseLogger(); _dbg.Name = "Debug";
            _inf = new BaseLogger(); _dbg.Name = "Info";
            _err = new BaseLogger(); _dbg.Name = "Error";
        }
    }
}
