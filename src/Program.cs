using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PySynCS {
    static class Program {
        static string SubString(string txt, int start, int end) {
            if (string.IsNullOrWhiteSpace(txt)) return "";
            if (txt.Length - 1 <= start)        return txt.Substring(start, 1);

            if (txt.Length - 1 <= end) { 
                int v1 = txt.Length - 1,
                    v2 = v1 - start,
                    v3 = v2 + (start == 0 ? 0 : 1);
                string
                    ret = txt.Substring(start, v3);
                return ret;
            }

            if (start > end)  return "";
            if (start == end) return txt.Substring(start, 1);

            return txt.Substring(start, end - start + 1);
        }

        static void PerformSubStringTests() {
            string str = "0123456789ABCDEF";

            Console.WriteLine("Sub String test:\n");
            Console.WriteLine("String:");
            Console.WriteLine("\t       0123456789");
            Console.WriteLine("\t                 10      = 9");
            Console.WriteLine("\t                 |11     = 10");
            Console.WriteLine("\t                 ||12    = 11");
            Console.WriteLine("\t                 |||13   = 12");
            Console.WriteLine("\t                 ||||14  = 13");
            Console.WriteLine("\t                 |||||15 = 14");
            Console.WriteLine("\t       0123456789ABCDEF");
            Console.WriteLine("\t       |--|                 \t   00-03  Test 1");
            Console.WriteLine("\t           |--|             \t   04-07  ");
            Console.WriteLine("\t               |--|         \t   08-11  ");
            Console.WriteLine("\t                   |--|     \t   12-15  ");
            Console.WriteLine("\t       |------|             \t   00-07  Test 2");
            Console.WriteLine("\t            |------|        \t   05-13  ");
            Console.WriteLine("\t            |<<<<<<|        \t   13-05  "); // Should not work
            Console.WriteLine("\t        |------------|      \t   01-14  ");
            Console.WriteLine("\t           |<<|             \t   04-01  "); 
            Console.WriteLine("\t       |-----------------|  \t   00-17  Test 3");
            Console.WriteLine("\t                     |-|    \t   14-16  ");


            // Tests 1
            Console.WriteLine("\n\nTest 1:");
            Console.WriteLine($"\t 00-03  {SubString(str, 00, 03)}");
            Console.WriteLine($"\t 04-07  {SubString(str, 04, 07)}");
            Console.WriteLine($"\t 08-11  {SubString(str, 08, 11)}");
            Console.WriteLine($"\t 12-15  {SubString(str, 12, 15)}"); //*/

            // Tests 2
            Console.WriteLine("\nTest 2:");
            Console.WriteLine($"\t 00-07  {SubString(str, 00, 07)}");
            Console.WriteLine($"\t 05-13  {SubString(str, 05, 13)}");
            Console.WriteLine($"\t 13-05  {SubString(str, 13, 05)}");
            Console.WriteLine($"\t 01-14  {SubString(str, 01, 14)}");
            Console.WriteLine($"\t 04-01  {SubString(str, 04, 01)}"); //*/

            // Tests 3
            Console.WriteLine("\nTest 3:");
            Console.WriteLine($"\t 00-19  {SubString(str, 00, 19)}");
            Console.WriteLine($"\t 14-16  {SubString(str, 14, 16)}"); //*/

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nDone"); Console.ReadKey();
        }



        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GUI.MainGUI());

            //PerformSubStringTests();
        }
    }
}
