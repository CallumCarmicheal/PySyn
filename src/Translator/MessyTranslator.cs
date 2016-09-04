using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PySynCS.Translator {
    public class MessyTranslator {

        Settings.TranslatorSettings _tSet;
        Settings.TranslatorSettings TranslatorSettings { get { return this._tSet; } }
        
        public MessyTranslator(Settings.TranslatorSettings tSettings) {
            _tSet = tSettings;
        }

        private int countOcc(string needle, string haystack) {
            if (needle.Length < 1) return -1;
            return (haystack.Length - haystack.Replace(needle, "").Length) / needle.Length;
        }

        private string SubString(string txt, int start, int end) {
            if (string.IsNullOrWhiteSpace(txt)) return "";
            if (txt.Length <= start)            return "";
            if (txt.Length <= end)              return txt.Substring(start, txt.Length - start + (start == 0 ? 0 : 1));
            if (start > end)                    return "";
            if (start == end)                   return txt.Substring(start, 1);
            return txt.Substring(start, end - start + 1);
        }


        // TODO: Later uniform both methods into one
        public string RemoveNL(string text) {
            return text.Replace("\r\n", "").Replace("\n", "");
        }
        
        bool s_ContainsNewBrace(string code) { return (code.Contains("{")); }
        bool s_ContainsEndBrace(string code) { return (code.Contains("}")); }

        // Returns if to continue
        bool s_ProcessBraces(string trim, string trims, GenericCompiler compiler) {
            
            // Method namespace etc,
            //  Something with braces.
            //  THIS COULD BE A FAKE BRACE
            //  SUCH AS /*COMMENT*/ { CODE }
            //      DETECT?
            if(trim == "{") {
                                
                // Append ":" after the method/statement
                // - Get the list of lines of the output
                // - Remove the last line (A line just with a brace)
                // - Append ":" to the last item
                // - Remove the last item (the statement/method)
                var nl      = compiler.Output.Split('\n');
                    nl      = nl.Take(nl.Count() - 1).ToArray();
                var cnt     = nl.Length - 1;
                var lin     = nl[cnt].Trim() + ":\n";
                    nl      = nl.Take(nl.Count() - 1).ToArray();
                                
                // Rebuild the output
                // - Set the output to the current array joined by a new line 
                //      and add a new line for the next statement
                // - Increment the currentIndent
                // - Append the current statement/method etc.
                compiler.Output = string.Join("\n", nl) + "\n";
                compiler.WriteLine(lin, true);
                compiler.currentIndent++; return true;
            } else if(trim == "}") {
                // Closing brace
                // - Decrement the current indent
                compiler.currentIndent--; return true;
            }

            // Braces with code 

            // Example: { // Blah
            // Example: { /**/ {} } 
            else if(trim.StartsWith("{")) {
                // TODO: Support for multi braces.

                // Increment the currentIndent
                compiler.currentIndent++;
                // Remove the brace and writeline
                compiler.WriteLine(SubString(trim, 0, 1));
            }

            // Example: /* test */ { // test2
            else if(trim.Contains("{")) {
                bool sw = trim.StartsWith("{"),
                        ew = trim.EndsWith("{");

                if (sw) {
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
                    compiler.currentIndent++; return true;
                } 
                            
                else if (ew) trim = SubString(trim, 0, trim.Length - 2);
                else {
                    // The brace is somewhere between 2 statements
                    var index  = trim.IndexOf("{");
                    string s1  = SubString(trim, 0, index - 1),
                            s2 = SubString(trim, index + 1, trim.Length-1);

                    compiler.WriteLine(s1 + ":", true);
                    compiler.currentIndent++;
                    compiler.WriteLine(s2, true);
                    return true;
                }

                compiler.WriteLine(trim, true);
                compiler.currentIndent++;
                return true;
            }
                        
            // Example: } // End !getUses
            else if(trim.StartsWith("}")) {
                // We start with the brace, lets just remove it and append the line
                trim = SubString(trim, 0, 1);
                compiler.currentIndent--;
                compiler.WriteLine(trim, true);
                return true;
            }

            // Example: /**/ }
            else if(trim.EndsWith("}")) {
                // We start with the brace, lets just remove it and append the line
                trim = SubString(trim, 0, trim.Length-2);
                compiler.WriteLine(trim, true);
            }


            else if (trim.Contains("}")) {
                bool sw = trim.StartsWith("{"),
                     ew = trim.EndsWith("{");

                if (sw) {
                    // Append ":" after the method/statement
                    // - Get the list of lines of the output
                    // - Remove the last line (A line just with a brace)
                    // - Append ":" to the last item
                    // - Remove the last item (the statement/method)
                    var nl = compiler.Output.Split('\n');
                        nl = nl.Take(nl.Count() - 1).ToArray();
                    var cnt = nl.Length - 1;
                    var lin = nl[cnt].Trim() + "\n";
                        nl = nl.Take(nl.Count() - 1).ToArray();

                    // Rebuild the output
                    // - Set the output to the current array joined by a new line 
                    //      and add a new line for the next statement
                    // - Increment the currentIndent
                    // - Append the current statement/method etc.
                    compiler.Output = string.Join("\n", nl) + "\n";
                    compiler.WriteLine(lin, true);
                    compiler.currentIndent++; return true;
                } 
                            
                else if (ew) trim = SubString(trim, 0, trim.Length - 1);
                else {
                    // The brace is somewhere between 2 statements
                    var index = trim.IndexOf("}");
                    string s1 = SubString(trim, 0, index - 1),
                           s2 = SubString(trim, index + 1, trim.Length);

                    compiler.WriteLine(s1, true);
                    compiler.currentIndent++;
                    compiler.WriteLine(s2, true);
                    return true;
                }

                compiler.WriteLine(trim, true);
                compiler.currentIndent++;
                return true;
            }

            // Example: /* */ {
            else if(trim.EndsWith("{")) {
                // Check if the line ends with a space and brace
                var flag = (trim.EndsWith(" {"));

                // IF the line ends with " {"
                //      - Remove the last 2 characters and append ":"
                // ELSE - Remove the last character (ENDS WITH "}") and append ":"
                if (flag)
                        compiler.WriteLine(SubString(trim, 0, trim.Length - 2) + ":");
                else compiler.WriteLine(SubString(trim, 0, trim.Length - 1) + ":");

                compiler.currentIndent++; return true;
            } else if (trim.EndsWith("}")) {
                compiler.currentIndent--; return true;
            } 
                        
            // OTHER, PROB TODO STUFF
            else {
                // Cant figure out what it is but hell lets just add it in anyway for debugging
                // 
                // IF (Settings -> IgnoreTrailingWhitespace)
                //      - Add trims to the compiler instead of trims to allow whitespace for comments.
                // ELSE - Add trim to compiler to remove extra trailing whitespace
                if(this._tSet.Syntax.IgnoreTrailingWhitespace) 
                     compiler.WriteLine(trims, true);
                else compiler.WriteLine(trim,  true);
                return true;
            }

            return false;
        } 

        public string TO_PYSYNCS_TranslateViaString(string Code, GUI.MainGUI gui) {
            var compiler = new GenericCompiler(_tSet);
            compiler.LoadString(Code);

            for (compiler.CurrentLine = 0;
                 compiler.StillProcessing();
                 compiler.CurrentLine++) {
                gui.WriteDebug("Parsing Line: " + compiler.CurrentLine);

                bool dbgBreak = false;


                if(compiler.CurrentLine == 21) 
                    dbgBreak = true;
                // Parse our current line of code etc into a 
                // more usable format. 
                // - line  = The untouched code
                // - trim  = The code that has whitespace removed.
                // - trims = The code that has only the starting whitespace removed
                string line  = compiler.GetCurrentLine(),
                       trim  = "",
                       trims = "";

                // Support for a multiline comment 
                if(compiler.commentML) 
                    line = SubString(line, compiler.commentIndex, line.Length);

                // Trim the code
                trim  = line.Trim();
                trims = line.TrimStart();

                if (string.IsNullOrWhiteSpace(trim)) {
                    // Whitespaced line, append new line with correct tabulation/whitespace.
                    compiler.WriteLine("", true);
                    continue;
                }

                /* 0. Check for starting comment such as // or /* */ { 
                    if (trim.StartsWith("//")) {
                        // Just add the line to the file
                        // - Add the comment to the file with indentation
                        compiler.WriteLine(trim, true);
                        continue;
                    } 
                    
                    else if (trim.StartsWith("/*") && trim.Contains("*/")) {
                        // The line contains a multi line comment but it stops on the 
                        // same line. TODO: CHECK FOR CODE.

                        int comStart = trim.IndexOf("/*"),
                            comEnd   = trim.IndexOf("*/") + 2;


                        string s1    = SubString(trim, 0, comStart),
                               com   = SubString(trim, comStart, comEnd),
                               s2    = SubString(trim, comEnd, trim.Length-2); // What?

                        // Process s1, write the comment, Process s2.
                        if (!string.IsNullOrWhiteSpace(s1)) s_ProcessBraces(s1, s1, compiler);
                            compiler.WriteLine(com, true);
                        if (!string.IsNullOrWhiteSpace(s2)) s_ProcessBraces(s2, s2, compiler);
                    }
                    
                    else if (trim.StartsWith("/*")) {
                        // This is when the line consists of just a multi line comment
                        // - Let the compiler process the multi line comment automatically.
                        compiler.WriteLine(compiler.GetMultiLineComment().Trim(), true);
                        continue;
                    } 

                    else if(trim.Contains("/*") ) {
                        // This means there is code or a character before the comment
                        // Display code
                        //   - Trim the text to get the code before a multiline comment
                        //   - Write the code
                        //   - Parse multi line comments
                        var code = SubString(trim, 0, line.IndexOf("/*")).Replace(";", "");
                            compiler.Write(code, false, true);
                            compiler.GetMultiLineComment();

                        continue;
                    }
                }


                /* 1. Check for semi-colons */ {
                    // Count the amount of colons there are on the line
                    var colcnt = countOcc(";", line);

                    // If there is more than one colon
                    if (colcnt >= 1) {

                        // If there is 1 colon
                        if (colcnt == 1) {
                            string res1 = trim.Replace(";", ""),
                                   res2 = RemoveNL(res1);

                            compiler.WriteLine(res2, true);
                        } 
                        
                        // More than one
                        else {
                            // Multiple instructions
                            // - Split them into a array
                            // - Add each instruction on a new line
                            string[] instructions = line.Split(';');
                            foreach(string inst in instructions) 
                                compiler.WriteLine(RemoveNL(inst.Trim()), true);
                        }

                    } else {
                        if (s_ProcessBraces(trim, trims, compiler))
                            continue;
                    }
                }
            }




            return compiler.Output;
        }
    }
}
