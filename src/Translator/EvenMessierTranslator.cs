using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PySynCS.GUI;
using PySynCS.Translator.Cache;
using PySynCS.Translator.Settings;

namespace PySynCS.Translator {
    public class EvenMessierTranslator : Translator {
        public EvenMessierTranslator(TranslatorSettings tSettings) : base(tSettings) {}

        // TrimStart is ignored!
        public override void handleMultilineComment(Cache.Cache compiler, int startIndex = 0) {
            string 
                line = "";
            int CurrentLine = 0;
            
            for (CurrentLine = compiler.CurrentLine;
                 compiler.LineCount >= CurrentLine;
                 CurrentLine++) {

                if(!compiler.GetLine(CurrentLine, ref line)) 
                    throw new IndexOutOfRangeException("Line is out of expected code range in EvenMessierTranslator.handleMultilineComment");
                
                if (CurrentLine == compiler.CurrentLine)
                    if (startIndex != 0)
                        line = SubString(line, startIndex);

                line = line.TrimStart();

                if(line.Contains("*/")) {
                    var i = line.IndexOf("*/") + 2;
                    compiler.CurrentLine = CurrentLine - 1;
                    compiler.offsetIndex = i;

                    var s = SubString(line, 0, i - 1);
                    if (s == "*/") compiler.WriteLine(" " + s);
                    else           compiler.WriteLine("  " + line);

                    compiler.CurrentLine = CurrentLine;
                    break;
                } else {
                    compiler.WriteLine("  " + line);
                }
            }
        }

        public void HandleBraces(Cache.Cache compiler) {
            string line = compiler.GetCurrentLine();
            if (compiler.offsetIndex != 0)
                line = SubString(line, compiler.offsetIndex);

            // Check for prepending code
            // Increment/Decrement indent
            // Check of appending code

            bool containsStart = false,
                 containsEnd   = false,
                 startFirst    = false,
                 endFirst      = false;

            containsStart = line.Contains("{");
            containsEnd   = line.Contains("}");

            // Find out whats first
            foreach(char c in line) {
                if (c == '{') {
                    startFirst = true;
                    break;
                } else if (c == '}') {
                    endFirst = true;
                    break;
                }
            }

            if(startFirst) {
                int    index   = line.IndexOf('{', 0);
                string prepend = "",
                       append  = "";

                if (index != 0) {
                    prepend = SubString(line, 0, index-1);
                    append  = SubString(line, index + 1);
                } 



            }
            
        }


        private void ParseCS_String(string line, string trim, string trims, Cache.EvenMessierCache compiler) {
            // Check if line is empty of whitespaced
            if (string.IsNullOrWhiteSpace(trim)) {
                var lastLine = "";

                if (compiler.GetLine(compiler.CurrentLine - 1, ref lastLine))
                    if (!string.IsNullOrWhiteSpace(lastLine) || _tSet.Syntax.IgnoreBetweenWhitespace) {
                        // Whitespaced line, append new line with correct tabulation/whitespace.
                        compiler.WriteLine("", true);
                        return;
                    }

                return;
            }

            // Check if string starts with a single line comment
            if (trims.StartsWith("//")) {
                compiler.WriteLine(trims, true);
                return;
            }

            // Check if line contains a multi-line comment
            if (trims.Contains("/*")) {
                if (!trims.StartsWith("/*")) {
                    // Get index of the first occurence of /*
                    int i = trims.IndexOf("/*");
                    string code = SubString(trims, 0, i);

                    ParseCS_String(code, code.Trim(), code.TrimStart(), compiler);

                    this.handleMultilineComment(compiler, i);
                } else {
                    this.handleMultilineComment(compiler, 0);
                } return;
            }

            // Check if line contains braces 
            if (line.Contains("{") || line.Contains("}")) {
                HandleBraces(compiler);
                return;
            }
        }


        public override string ToPySyncsFromString(string Code, Logger.Logger logger) {
            var compiler = new Cache.EvenMessierCache(_tSet);
                compiler.LoadString(Code);

            for (compiler.CurrentLine = 0;
                 compiler.StillProcessing();
                 compiler.CurrentLine++) {

                // Parse our current line of code etc into a 
                // more usable format. 
                // - line  = The untouched code
                // - trim  = The code that has whitespace removed.
                // - trims = The code that has only the starting whitespace removed
                string line  = compiler.GetCurrentLine(),
                       trim  = "",
                       trims = "";

                // Support for a multiline comment 
                if (compiler.commentML)
                    line = SubString(line, compiler.offsetIndex, line.Length);

                // Trim the code
                trim = line.Trim();
                trims = line.TrimStart();

                ParseCS_String(line, trim, trims, compiler);

                // Reset comment index
                compiler.offsetIndex = 0;
            } return compiler.Output;
        }
    }
}
