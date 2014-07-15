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
    public class WoordenboekFreqList: Woordenboek
    {
        public WoordenboekFreqList(string fileName)
            : base()
        {
            TextReader tr = new StreamReader(fileName);
            var line = tr.ReadLine();
            while(line != null)
            {
                string[] parts = line.Split('\t');
                if (parts.Length > 1)
                {
                    string[] terms = parts[0].Split(';');
                    int freq = int.Parse(parts[1]);

                    if (terms.Length > 0 && freq > 45)
                    {
                        if (!lexicon.ContainsKey(terms[0]))
                        {
                            lexicon.Add(terms[0], freq);
                        }
                        else
                        {
                            lexicon[terms[0]]+=freq;
                        }
                        foreach (string term in terms)
                        {
                            string stem = Stemmer.InstanceNL.Stem(term);
                            if (!lexMapping.ContainsKey(stem))
                            {
                                lexMapping.Add(stem, new HashSet<string>());
                            }
                            lexMapping[stem].Add(terms[0]);
                        }
                    }
                }
                line = tr.ReadLine();
            }
        }

    }
}
