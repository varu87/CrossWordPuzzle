using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Dictionary
{
    public class InputText
    {
        private static Random r = new Random();

        /// <summary>
        /// Read the dictionary file and get the list of words and meanings
        /// </summary>
        /// <param name="inputFile"></param>
        /// <returns>Dictionary</returns>
        public static Dictionary<string,string> InputWords(string inputFile)
        {
            List<string> wordList = new List<string>();
            string fileExtension = Path.GetExtension(inputFile);

            if (!fileExtension.Equals(".xml") && fileExtension.Equals(".txt"))
                inputFile = ConvertTxtToXml(inputFile);


            return ConvertXmlToDictionary(inputFile);
        }

        /// <summary>
        /// Convert text file to xml
        /// </summary>
        /// <param name="textFile"></param>
        /// <returns>string</returns>
        static string ConvertTxtToXml(string textFile)
        {
            string xmlFile = Path.GetDirectoryName(textFile) + @"\Dictionary.xml";
            string[] words = File.ReadAllLines(textFile, Encoding.GetEncoding("iso-8859-1"));

            XElement dictionary = new XElement("dictionary",
                from word in words
                select new XElement("word", 
                    word.Split(' ')[0], 
                    new XAttribute("meaning", word.Substring(word.IndexOf(" ") + 1, word.Length - word.Split(' ')[0].Length - 1))));

            dictionary.Save(xmlFile);

            return xmlFile;
        }

        /// <summary>
        /// Convert xml to dictionary
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <returns>Dictionary</returns>
        static Dictionary<string, string> ConvertXmlToDictionary(string xmlPath)
        {
            XDocument xmlFile = XDocument.Load(xmlPath);
            return xmlFile.Descendants("word").GroupBy(w=>(string)w).ToDictionary(w => (string)w.First(), w => (string)w.First().Attribute("meaning"));
        }
    }
}
