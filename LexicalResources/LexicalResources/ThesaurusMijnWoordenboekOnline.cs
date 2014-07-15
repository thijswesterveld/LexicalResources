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
                string term = fields[0];
                ThesaurusEntry entry = new ThesaurusEntry(term);
                WordSense sense = new WordSense(1, term, null, null);
                Relation synonyms = new Relation();
                synonyms.Type = RelationType.Synonym;
                for (int i = 1; i < fields.Length;++i)
                {
                    synonyms.Terms.Add(new RelatedTerm(fields[i],null,false));
                }
                sense.TermRelations.Add(synonyms);
                entry.Senses.Add(sense);
                line = tr.ReadLine();
            }
        }
    }
}
