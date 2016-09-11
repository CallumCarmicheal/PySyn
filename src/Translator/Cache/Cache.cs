using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PySynCS.Translator.Cache {
    public abstract class Cache {
        /////////////// Constructor
        public Cache(Settings.TranslatorSettings settings) {
            _set = settings;
        }

        /////////////// Private Variables
        private Settings.TranslatorSettings _set;

        /////////////// Variables
        public int      CurrentLine = 0;
        public int      LineCount = 0;
        public string   Source = "";
        public string   Output = "";
        public bool     commentML = false;
        public int      offsetIndex = 0;
        public int      currentIndent = 0;
        public string[] Lines;


        /////////////// WRITING TO THE OUTPUT
        public void WriteLine(string text, bool indent = true) {
            var txt = oWrite(text, true, indent);
            Output += txt;
        } public void Write(string text, bool newline = false, bool indent = true) {
            Output += oWrite(text, newline, indent);
        } public string oWrite(string text, bool newline = false, bool indent = true) {
            var nl = (newline ? "\n" : "");

            if (indent)
                return GetTheIndent() + text + nl;
            else return text + nl;
        } public string GetTheIndent() {
            if (_set.SyntaxType == ESyntaxType.Python_Tabs)
                return "".PadRight(currentIndent, '\t');
            else if (_set.SyntaxType == ESyntaxType.Python_Spaces)
                return "".PadRight(currentIndent * _set.Syntax.Spaces, ' ');
            else return "".PadRight(currentIndent, '\t');
        }


        /////////////// Events
        public void LoadString(string src) {
            Lines = src.Split('\n');
            LineCount = Lines.Length - 1;
            CurrentLine = 0;
            Source = src;
        }

        public bool StillProcessing() {
            return (LineCount >= CurrentLine);
        }

        public string GetCurrentLine() {
            return this.Lines[CurrentLine];
        }

        public bool GetLine(int index, ref string @result) {
            @result = "";

            if (index > LineCount) return false;
            if (index < 0) return false;
            
            try {
                var res = Lines[index];
                @result = res;
                return true;
            } catch { }
            
            return false;
        }

        [Obsolete("This method of translating is not recommended, if you intend to use it please do with caution!")]
        public string GetMultiLineComment_Native(int startingChar = 0) {
            commentML = false;
            offsetIndex = 0;

            int     start = CurrentLine,
                    x = 0;
            bool    containCode = false,
                    bstart = true;
            string  line = "",
                    trim = "",
                    comment = "";

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
                    var str = line.Substring(index + 2);
                    var strt = str.Trim();

                    if (strt.Length > 0) {
                        // We have CODE <3
                        commentML = true;
                        offsetIndex = index + 2;

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

        /////////////// Abstract methods/properties.
        [Obsolete("This method of translating is not recommended, if you intend to use it please do with caution!")]
        public abstract string GetMultiLineComment(int startingChar = 0);

    }
}
