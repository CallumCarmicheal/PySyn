using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PySynCS.Translator.Settings;

namespace PySynCS.Translator.Cache {
    class EvenMessierCache : Cache {
        public EvenMessierCache(TranslatorSettings settings) : base(settings) { }

        [Obsolete("This method of translating is not recommended, if you intend to use it please do with caution!")]
        public override string GetMultiLineComment(int startingChar = 0) {
            // Return native
            return GetMultiLineComment_Native(startingChar);
        }
    }
}
