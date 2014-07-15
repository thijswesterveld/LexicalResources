using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LexicalResources
{
    public class ThesaurusMijnWoordenboekOnline : Thesaurus
    {

        public ThesaurusMijnWoordenboekOnline(string fileName)
            : base()
        {
            TextReader tr = new StreamReader(fileName);
            string line = tr.ReadLine();
            while (line != null)
            {
                string[] fields = line.Split(';');
                string term = fields[0].Trim();
                
                ThesaurusEntry entry;
                if(!thesaurus.TryGetValue(term,out entry))
                {
                    entry = new ThesaurusEntry(term);
                    thesaurus.Add(term, entry);
                }
                WordSense sense = new WordSense(term);
                Relation synonyms = new Relation();
                synonyms.Type = RelationType.Synonym;
                for (int i = 1; i < fields.Length;++i)
                {
                    string syn = fields[i].Trim();
                    if (!string.IsNullOrEmpty(syn))
                    {
                        synonyms.Terms.Add(new RelatedTerm(fields[i], 0, null, false));
                    }
                }
                sense.TermRelations.Add(synonyms);
                entry.Senses.Add(sense);
                line = tr.ReadLine();
            }
        }
    }
}
