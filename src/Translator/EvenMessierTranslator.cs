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

                    var s = SubString(line, 0, i - 2);
                    if (s == "*/") compiler.WriteLine(" " + s);
                    else {
                        var str = SubString(line, i);
                        ParseCS_String(str, str.Trim(), str.TrimStart(), compiler);
                    }

                    compiler.CurrentLine = CurrentLine;
                    break;
                } else {
                    compiler.WriteLine("  " + line);
                }
            }
        }

        void HandleBraces(Cache.Cache compiler) {
            string line = compiler.GetCurrentLine();
            if (compiler.offsetIndex != 0)
                line = SubString(line, compiler.offsetIndex);

            line = line.TrimStart();

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

                if (!string.IsNullOrWhiteSpace(prepend)) {
                    string[] lines = line.Split(';');
                    int      len = lines.Length - 1;
                    int      i = 0;

                    string method = "";

                    if (len >= 1) {
                        foreach (string x in lines) {
                            if (i == len) {
                                // This is our required line.
                                method = x;
                            } else {
                                ParseCS_String(x, x.Trim(), x.TrimStart(), compiler);
                            } i++;
                        }
                    } else {
                        method = line;
                    }

                    compiler.WriteLine(method.TrimEnd().Replace('{', ':'));
                } else {
                    // Append ":" after the method/statement
                    // - Get the list of lines of the output
                    // - Remove the last line (A line just with a brace)
                    // - Append ":" to the last item
                    // - Remove the last item (the statement/method)
                    var nl = compiler.Output.Split('\n');
                        nl = nl.Take(nl.Count() - 1).ToArray();
                    var cnt = nl.Length - 1;
                    var lin = nl[cnt].Trim() + ":\n";
                        nl = nl.Take(nl.Count() - 1).ToArray();

                    // Rebuild the output
                    // - Set the output to the current array joined by a new line 
                    //      and add a new line for the next statement
                    // - Increment the currentIndent
                    // - Append the current statement/method etc.
                    compiler.Output = string.Join("\n", nl) + "\n";
                    compiler.WriteLine(lin, true);
                }

                compiler.currentIndent++;
                if (!string.IsNullOrWhiteSpace(append))
                    ParseCS_String(append, append.Trim(), append.TrimStart(), compiler);

                return;
            }

            if(endFirst) {
                int index = line.IndexOf('}', 0);
                string prepend = "",
                       append  = "";

                if (index != 0) {
                    prepend = SubString(line, 0, index - 1);
                    append  = SubString(line, index + 1);
                }

                if (!string.IsNullOrWhiteSpace(prepend)) 
                    ParseCS_String(prepend, prepend.Trim(), prepend.TrimStart(), compiler);
                
                compiler.currentIndent--;
                if (!string.IsNullOrWhiteSpace(append))
                    ParseCS_String(append, append.Trim(), append.TrimStart(), compiler);
            }   
        }

        private void ParseCS_String(string line, string trim, string trims, Cache.Cache compiler) {
            compiler.Logger.Info.WriteLine(this, compiler, "Parsing string: " + trims);

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
            if (trims.Contains("{") || trims.Contains("}")) {
                HandleBraces(compiler);
                return;
            }

            // Check if the line does not contain a
            // semi-colon If it does not exist bitch about it
            // because there is no reason it should not be there
            //     mainly for debuggin and syntax checking?
            //if (!trims.Contains(";")) 
            //    throw new Exception("U WOT M8, U GOT NO ;... DO YOU THINK THIS A GAME????");


            // Check if the line contains more than one semi-colons
            string[] lines = trims.Split(';');
            if       (lines.Length -1 >= 1) {
                foreach (string x in lines)
                    ParseCS_String(x, x.Trim(), x.TrimStart(), compiler);
                return;
            }

            // Now we print our code
            compiler.WriteLine(trims);
        }


        public override string ToPySyncsFromString(string Code, Logger.Logger logger) {
            var compiler = new Cache.EvenMessierCache(_tSet);
                compiler.Logger = logger;
                compiler.LoadString(Code);

            for (compiler.CurrentLine = 0;
                 compiler.StillProcessing();
                 compiler.CurrentLine++) {

                logger.Debug.WriteLine(this, compiler, "Starting new line");

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
