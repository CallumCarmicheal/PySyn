using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PySynCS.Translator.Settings {

    public class Syntax {
        #region Booleans 
        /// <summary>
        /// Allows the programmer to end each instruction line with a semicolon ";"
        /// -- DOES NOT ALLOW MULTIPLE INSTRUCTIONS PER LINE
        /// </summary>
        public bool AllowSemiColon = false;

        /// <summary>
        /// Will throw a error if there is whitespace like extra space, tab between functions
        /// ++ This will ignore whitespace between instructions
        /// -- This will throw a error if there is a extra space/tab between 2 instructions on a empty line
        /// </summary>
        public bool IgnoreBetweenWhitespace = true; 

        /// <summary>
        /// Will throw a error if there is any trailing whitespace after a instruction on the same line
        /// -- Setting this to false would cause issues where you would be required to have 1 space after the instruction
        /// -- then the comment if this is set to false. 
        /// ++ Allow as much whitespace as required after a instruction for example aligning a set of comments
        /// </summary>
        public bool RetainEmptyLineWhitespace = true;

        /// <summary>
        /// Enforces the PIP8 Parameter syntax rules.
        /// 
        /// </summary>
        public bool PIP8_ForceParametersIndentation = false;

        #endregion

        #region Integers

        /// <summary>
        /// The amount of spaces per indent
        /// </summary>
        public int Spaces = 4;

        #endregion
    }


    public class TranslatorSettings {
        private Syntax          _syntax;

        public ESyntaxType      SyntaxType { get; set; }
        public ETranslationType TranslationType { get; set; }

        public Syntax           Syntax { get { return this._syntax; } }

        public TranslatorSettings() {
            SyntaxType      = ESyntaxType.Python_Tabs;
            TranslationType = ETranslationType.toCS;

            _syntax         = new Syntax();
        }
    }
}
