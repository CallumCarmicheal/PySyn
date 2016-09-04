using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PySynCS.Translator {

    public enum ETranslationType {
        toCS,
        // -----------
        toPySynCS
    }


    public enum ESyntaxType {
        Python_Tabs,
        Python_Spaces,

        // Mix of spaces and tabs 
        //      BEAWARE OF PIP8 THEY ARE ALLWAYS WATCHING!
        Lax_Mixed
    }
}
