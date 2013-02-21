using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Marine.Database
{
    public class TextObj
    {
        public TextObj()
        {

        }

        public static void WriteTextLine(string fileName, string textValue)
        {
            StreamWriter textWriter = null;
            try
            {
                textWriter = GetTextStreamWriter(fileName);
                textWriter.WriteLine(textValue);
            }
            catch (Exception err)
            {
                System.Diagnostics.Trace.WriteLine(err.Message);
            }
        }

        public static void WriteText(string fileName, string textValue)
        {
            StreamWriter textWriter = null;
            try
            {
                textWriter = GetTextStreamWriter(fileName);
                textWriter.Write(textValue);
            }
            catch (Exception err)
            {
                System.Diagnostics.Trace.WriteLine(err.Message);
            }
        }

        public static string[] ReadTextLines(string fileName)
        {
            string[] textLines = null;
            if (File.Exists(fileName))
            {
                textLines = File.ReadAllLines(fileName);
            }
            return textLines;
        }

        private static StreamWriter GetTextStreamWriter(string fileName)
        {
            StreamWriter textWriter = null;
            if (!File.Exists(fileName))
            {
                textWriter = File.CreateText(fileName);
            }
            else
            {
                textWriter = File.AppendText(fileName);
            }
            return textWriter;
        }
    }
}