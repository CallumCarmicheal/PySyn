using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PySynCS.Translator {
    class GenericCompiler {
        private Settings.TranslatorSettings _set;

        public GenericCompiler(Translator.Settings.TranslatorSettings settings) {
            _enc = new class_Enclosure(this);
            _set = settings;
        }


        #region Enclosure
        private class_Enclosure _enc;
        public class_Enclosure Enclosure { get { return this._enc; } }
        public class class_Enclosure {
            private GenericCompiler parent;
            public class_Enclosure(GenericCompiler p) { this.parent = p; }
            
            public int
                    LStart = 0, // Line starts on
                    LEnd   = 0, // Line ends on

                    CStart = 0, // Character starts on
                    CEnd   = 0; // Character ends on

            public char CharStart = ' ';
            public char CharEnd   = ' ';

            // Examples are inside parameters etc.
            //      object sender, Event args
            //      "Hello", "World", obj
            //      a, b, c, x, y, z

            //    string Hello, string World,
            //          string Hi;
            public string FullString    = "";
            public string WorkingString = "";
            public string Output        = "";

            public bool ProcessString(string code, int startLine, int startCharacter) {
                var status = true;
                
                // This wont be required unless we are translating to 
                //   PYSYNCS With pip8

                if(this.parent._set.TranslationType == ETranslationType.toPySynCS &&
                    this.parent._set.Syntax.PIP8_ForceParametersIndentation) {

                    // TODO: Follow the PIP8 Crap

                    return false;
                }

                return status;
            }
        }
        #endregion
        
        public int      CurrentLine   = 0;
        public int      LineCount     = 0;
        public string   Source        = "";
        public string   Output        = "";
        public bool     commentML     = false;
        public int      commentIndex  = 0; 
        public int      currentIndent = 0;
        public string[] Lines;


        // WRITING TO THE OUTPUT
        public void WriteLine(string text, bool indent = true) {
            var txt = oWrite(text, true, indent);
            Output += txt;
        }

        public void Write(string text, bool newline = false, bool indent = true) {
            Output += oWrite(text, newline, indent);
        }

        public string oWrite(string text, bool newline = false, bool indent = true) {
            var nl = (newline ? "\n" : "");

            if (indent)
                 return GetTheIndent() + text + nl;
            else return text + nl;
        }

        public string GetTheIndent() {
            if (_set.SyntaxType == ESyntaxType.Python_Tabs)
                 return "".PadRight(currentIndent, '\t');
            else if (_set.SyntaxType == ESyntaxType.Python_Spaces)
                 return "".PadRight(currentIndent * _set.Syntax.Spaces, ' ');
            else return "".PadRight(currentIndent, '\t');
        }

        public void LoadString(string src) {
            Lines = src.Split('\n');
            LineCount = Lines.Length-1;

            _enc = new class_Enclosure(this);
            CurrentLine = 0;
            Source = src;
        }

        public bool StillProcessing() {
            return (LineCount >= CurrentLine);
        }

        public string GetCurrentLine() {
            return this.Lines[CurrentLine];
        }

        public string GetMultiLineComment(int startingChar = 0) {
            commentML    = false;
            commentIndex = 0;

            int     start       = CurrentLine,
                    x           = 0;
            bool    containCode = false,
                    bstart      = true;
            string  line        = "",
                    trim        = "",
                    comment     = "";

            // Find the end of the multi line comment
            for (x = start; x > LineCount; x++) {

                line = Lines[x]; if (bstart) {
                    line = line.Substring(startingChar);
                    bstart = false;
                } trim = line.Trim();
                
                if (trim.Contains("*/")) {
                    CurrentLine = x;

                    var index = line.IndexOf("*/");
                                //      |
                                //    */Console.WriteLine ("Hi ;O")
                    var str   = line.Substring(index+2);
                    var strt  = str.Trim();

                    if (strt.Length > 0) {
                        // We have CODE <3
                        commentML = true;
                        commentIndex = index + 2;

                        comment += line;

                        // Stay on the current line
                        CurrentLine--;
                    } else {
                        // Goto the next line 
                        CurrentLine = x; // 1 will be added automatically
                        comment += line.Substring(0, index + 1);
                    }
                }
            }


            commentML = containCode;
            return comment;
        }
    }
}
