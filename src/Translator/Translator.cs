using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PySynCS.Translator {
    public abstract class Translator {
        protected Settings.TranslatorSettings _tSet;
        public Settings.TranslatorSettings TranslatorSettings { get { return this._tSet; } }

        public Translator(Settings.TranslatorSettings tSettings) {
            _tSet = tSettings;
        }
        

        public abstract string ToPySyncsFromString(string Code, Logger.Logger logger);
        public abstract void   handleMultilineComment(Cache.Cache compiler, int startIndex = 0);


        protected string SubString(string txt, int start, int end = -1) {
            if (string.IsNullOrWhiteSpace(txt)) return "";
            if (txt.Length - 1 <= start)        return txt.Substring(start, 1);
            if (end >= txt.Length - 1)          return txt.Substring(start);
            if (start == end)                   return txt.Substring(start, 1);
            if (start > end)                    return "";
            return txt.Substring(start, end - start + 1);
        }
        protected int countOcc(string needle, string haystack) {
            if (needle.Length < 1) return -1;
            return (haystack.Length - haystack.Replace(needle, "").Length) / needle.Length;
        }
        // TODO: Later uniform both methods into one
        protected string RemoveNL(string text) {
            return text.Replace("\r\n", "").Replace("\n", "");
        }

        protected bool s_ContainsNewBrace(string code) { return (code.Contains("{")); }
        protected bool s_ContainsEndBrace(string code) { return (code.Contains("}")); }

        
    }
}
