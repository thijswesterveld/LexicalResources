using NLP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace LexicalResources
{
    public class WoordenboekVanDale : Woordenboek
    {
        public WoordenboekVanDale(string fileName) : base()
        {
            TextReader tr = new StreamReader(fileName);
            var text = tr.ReadToEnd();
            var doc = XElement.Parse(text);
            foreach (var lemma in doc.XPathSelectElements("//artikel"))
            {
                string term = lemma.Attribute("zoekwoord").Value;
                if (!lexicon.ContainsKey(term))
                {
                    lexicon.Add(term, 1);
                }
                string stem = Stemmer.InstanceNL.Stem(term);
                if (!lexMapping.ContainsKey(stem))
                {
                    lexMapping.Add(stem, new HashSet<string>());
                }
                lexMapping[stem].Add(term);
            }

        }

    }
}
