using Frog;
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
    public class Woordenboek: IFrequentTerms
    {
        public Dictionary<string, int> lexicon;
        internal  Dictionary<string, HashSet<string>> lexMapping;

        public Woordenboek()
        {
            lexicon = new Dictionary<string, int>();
            lexMapping = new Dictionary<string, HashSet<string>>();
        }

        public bool IsFrequent(FrogToken token)
        {
            if(lexicon.ContainsKey(token.Lemma))
            {
                return true;
            }
            foreach(string part in token.MorphologicalSegmentation.Split(new char[]{'[', ']'},StringSplitOptions.RemoveEmptyEntries))
            {
                if(part.StartsWith("m") && lexicon.ContainsKey(part))
                {
                    return true;
                };
            }
            return false;
        }

        public bool IsFrequent(string lemma)
        {
            return lexicon.ContainsKey(lemma);
        }

        public bool IsFrequent(Token token)
        {
            HashSet<string> lemmas;
            if (lexMapping.TryGetValue(token.Stem, out lemmas))
            {
                foreach (string l in lemmas)
                {
                    if (IsFrequent(l))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
