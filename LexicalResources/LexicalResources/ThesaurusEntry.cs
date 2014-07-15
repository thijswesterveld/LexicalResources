using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalResources
{
    public class ThesaurusEntry
    {
        public ThesaurusEntry(string term)
        {
            Term = term;
            Senses = new List<WordSense>();
        }

        public string Term;
        
        public List<WordSense> Senses;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Term);
            foreach(WordSense sense in Senses)
            {
                sb.Append(sense.ToString());
            }
            return sb.ToString();
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public List<HashSet<string>> GetSynonyms()
        {
            List<HashSet<string>> synonymSets = new List<HashSet<string>>();
            foreach(WordSense sense in  Senses)
            {
                synonymSets.Add(sense.GetSynonyms());
            }
            return synonymSets;
        }


        public void Merge(ThesaurusEntry entry)
        {

            throw new NotImplementedException();
        }

        public void AddSense(WordSense sense)
        {
            Senses.Add(sense);
        }
    }
}
