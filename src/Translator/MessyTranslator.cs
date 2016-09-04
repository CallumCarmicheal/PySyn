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


        // TODO: Later uniform both methods into one
        public string RemoveNL(string text) {
            return text.Replace("\r\n", "").Replace("\n", "");
        }

        public string TO_PYSYNCS_TranslateViaString(string Code, GUI.MainGUI gui) {
            var compiler = new CompilerStorage(_tSet);
            compiler.LoadString(Code);

            for (compiler.CurrentLine = 0;
                 compiler.StillProcessing();
                 compiler.CurrentLine++) {
                gui.WriteDebug("Parsing Line: " + compiler.CurrentLine);


                string line = compiler.GetCurrentLine(),
                       trim = line.Trim();

                if (string.IsNullOrWhiteSpace(trim)) {
                    compiler.WriteLine("", true);
                    continue;
                }

                /* 0. Check for starting comment such as // or /* */ { 
                    if (trim.StartsWith("//")) {
                        // Just add the line to the file
                        compiler.WriteLine(trim, true);
                        continue;
                    } else if (trim.StartsWith("/*")) {
                        compiler.GetMultiLineComment();
                        continue;
                    } else if(trim.Contains("/*") ) {
                        // This means there is code or a character before the comment
                        // Display code
                        var code = trim.Substring(0, line.IndexOf("/*")).Replace(";", "");
                            compiler.Write(code, false, true);
                            compiler.GetMultiLineComment();

                        continue;
                    }
                }


                /* 1. Check for semi-colons */ {
                    var colcnt = countOcc(";", line);
                    if (colcnt >= 1) {

                        if (colcnt == 1) {
                            string res1 = trim.Replace(";", ""),
                                   res2 = RemoveNL(res1);

                            compiler.WriteLine(res2, true);
                        } else {
                            // Multiple instructions
                            string[] instructions = line.Split(';');

                            foreach(string inst in instructions) 
                                compiler.WriteLine(RemoveNL(inst.Trim()), true);
                        }

                    } else {
                        // Maybe a method or a namespace etc.
                        if(trim == "{") {

                            var nl  = compiler.Output.Split('\n');
                                nl  = nl.Take(nl.Count() - 1).ToArray();
                            var cnt = nl.Length - 1;
                            var lin = nl[cnt].Trim() + ":\n";
                                nl[cnt] = lin;

                            compiler.Output = string.Join("\n", nl);
                            compiler.currentIndent++; continue;
                        } else if(trim == "}") {
                            compiler.currentIndent--; continue;
                        } 

                        // ELSE IF HAS {} BUT CODE
                        else if(trim.EndsWith("{")) {
                            var flag = (trim.EndsWith(" {"));

                            if (flag)
                                 compiler.WriteLine(trim.Substring(0, trim.Length - 2) + ":");
                            else compiler.WriteLine(trim.Substring(0, trim.Length - 1) + ":");

                            compiler.currentIndent++; continue;
                        } else if (trim.EndsWith("}")) {
                            compiler.currentIndent--; continue;
                        } 
                        
                        else {
                            // Cant figure out what it is but hell lets just add it in anyway for debugging
                            compiler.WriteLine(trim, true);
                        }

                    }
                }
            }




            return compiler.Output;
        }
    }
}
