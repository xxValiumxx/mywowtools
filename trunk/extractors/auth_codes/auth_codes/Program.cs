﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using WoWReader;

namespace auth_codes
{
    class Program
    {
        static string wow_exe = "WoW.exe";

        static FileStream fs = null;
        static GenericReader gr = null;
        static TextReader tr = null;

        static string stream_string = String.Empty;

        static void Main(string[] args)
        {
            if (!File.Exists(wow_exe))
            {
                Console.WriteLine("File {0} not found!", wow_exe);
                return;
            }

            fs = new FileStream(wow_exe, FileMode.Open);
            gr = new GenericReader(fs);
            tr = new StreamReader(fs, Encoding.ASCII);

            stream_string = tr.ReadToEnd();

            gr.BaseStream.Position = FindStartPos();

            string name = String.Empty;

            List<string> names = new List<string>();

            do
            {
                name = gr.ReadStringNull();
                names.Add(name);
                FindNextPos();
            } while (!name.Equals("RESPONSE_SUCCESS"));

            tr.Close();
            gr.Close();
            fs.Close();

            names.Reverse();

            StreamWriter sw = new StreamWriter("authcodes.h");

            sw.WriteLine("enum AuthCodes");
            sw.WriteLine("{");

            foreach (string str in names)
            {
                string temp = str;

                int spaces = 41 - temp.Length;

                for (int i = 0; i < spaces; i++)
                    temp += " ";

                sw.WriteLine("    {0} = 0x{1},", temp, names.IndexOf(str).ToString("X2"));
            }

            names.Clear();

            sw.WriteLine("};");

            sw.Flush();
            sw.Close();
        }

        static int FindStartPos()
        {
            return stream_string.IndexOf("CHAR_NAME_SUCCESS");
        }

        static int FindNextPos()
        {
            while (gr.PeekChar() == 0x00)
            {
                gr.BaseStream.Position++;
            }

            return (int)gr.BaseStream.Position;
        }
    }
}
