using Newtonsoft.Json;
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
    public class Thesaurus
    {
        internal Dictionary<string, ThesaurusEntry> thesaurus = new Dictionary<string, ThesaurusEntry>();

        public static JsonSerializerSettings JsonSettings = new JsonSerializerSettings()
        { 
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
        };

        public Thesaurus()
        {
            thesaurus = new Dictionary<string, ThesaurusEntry>();
        }

        public void Stem()
        {
            Dictionary<string,ThesaurusEntry> stemmedThesaurus = new Dictionary<string,ThesaurusEntry>();
            foreach(KeyValuePair<string,ThesaurusEntry> entry in thesaurus)
            {
                string stem = Stemmer.InstanceNL.Stem(entry.Key);
                ThesaurusEntry stemEntry;
                if(!stemmedThesaurus.TryGetValue(stem, out stemEntry))
                {
                    stemEntry = new ThesaurusEntry(stem);
                    stemmedThesaurus.Add(stem, stemEntry);
                }
                foreach(WordSense sense in entry.Value.Senses)
                {
                    stemmedThesaurus[stem].AddSense(sense);
                }
            }
            thesaurus = stemmedThesaurus;
        }

        public ThesaurusEntry LookUp(string term)
        {
            ThesaurusEntry entry;
            if(!thesaurus.TryGetValue(term,out entry))
            {
                entry = new ThesaurusEntry(term);
            }
            return entry;
        }

        public string ToJson()
        {
            
            return JsonConvert.SerializeObject(thesaurus.Values,JsonSettings);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach(ThesaurusEntry entry in thesaurus.Values)
            {
                sb.AppendLine(entry.ToString());
            }
            return sb.ToString();
        }
    }
}
