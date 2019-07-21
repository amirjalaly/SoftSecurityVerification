using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;

namespace TruSyFire.Auxiliary
{
    class Console
    {
        public static void Clear()
        {
            System.Console.Clear();
            File.WriteAllText(FilePath, "");
        }

        public static void Beep()
        {
            System.Console.Beep();
            //SystemSounds.Beep.Play();
        }

        static string _FilePath = "out.txt";
        public static string FilePath {get{return _FilePath;}set{_FilePath=value;}}
        public static void WriteMiniSeparator()
        {
            string sep =
                "--------------------------------------------------";
            WriteLine(sep);
        }
        public static void WriteFullSeparator()
        {
            string sep =
                "--------------------------------------------------";
            string Minsep =
                "                 -------------                    ";
            //WriteLine("");
            WriteLine(Minsep);
            WriteLine(sep);
            WriteLine(Minsep);
            //WriteLine("");

        }
        public static void WriteLine(string str, params object[] param)
        {
            System.Console.WriteLine(str, param);
            File.AppendAllText(FilePath, string.Format(str, param)+"\r\n");
        }
        public static void ReadLine()
        {
            System.Console.ReadLine();
        }
        public static void InplaceWrite(string str, params object[] p)
        {
            System.Console.WriteLine(str, p);
            System.Console.SetCursorPosition(0, System.Console.CursorTop - 1);
        }
    }
}
