using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalResources
{
    public class WordSense
    {
        public WordSense(int senseNr, string lemma, string definition, string partOfSpeech)
        {
            this.SenseNr = senseNr;
            this.Lemma = lemma;
            this.Definition = definition;
            this.PartOfSpeech = partOfSpeech;
            TermRelations = new List<Relation>();
        }

        public int SenseNr;

        public string Lemma;

        public string Definition;
        
        public string Label;

        public string PartOfSpeech;

        public List<Relation> TermRelations;

        public int Synonymcount
        {
            get
            {
                return GetSynonyms().Count();
            }
        }

      
        public HashSet<string> GetSynonyms()
        {
            HashSet<string> synonyms = new HashSet<string>();
            foreach(Relation rel in TermRelations)
            {
                foreach(string synonym in rel.GetSynonyms())
                {
                    synonyms.Add(synonym);
                }
            }
            return synonyms;
        }

        public override string ToString()
        {
            string synonymString = string.Join("\t", TermRelations.Where(t => t.Type == RelationType.Synonym).Select(t => t.ToString()));
            string hyponymString = string.Join("\n\t", TermRelations.Where(t => t.Type == RelationType.Hyponym).Select(t => t.ToString()));
            string hyperonymString = string.Join("\n\t", TermRelations.Where(t => t.Type == RelationType.Hyperonym).Select(t => t.ToString())); 
            string antonymString = string.Join("\n\t", TermRelations.Where(t => t.Type == RelationType.Antonym).Select(t => t.ToString()));
            string associativeString = string.Join("\n\t", TermRelations.Where(t => t.Type == RelationType.Associative).Select(t => t.ToString()));
            string kapsString = string.Join("\n\t", TermRelations.Where(t => t.Type == RelationType.Kaps).Select(t => t.ToString()));
            StringBuilder sb = new StringBuilder();
            sb.Append(SenseNr+"\t");
            if(!string.IsNullOrEmpty(PartOfSpeech)) {sb.AppendFormat("[{0}] ", PartOfSpeech); }
            sb.AppendFormat("{0}", string.IsNullOrEmpty(synonymString) ? "[geen synoniemen]":synonymString);
            if (!string.IsNullOrEmpty(kapsString)) { sb.AppendFormat("; ", kapsString); }
            sb.AppendLine();
            if(!string.IsNullOrEmpty(associativeString)) { sb.AppendFormat("&#8776;\t{0}\n", associativeString);}
            if (!string.IsNullOrEmpty(antonymString)) { sb.AppendFormat("&#9668;&#9658;\t{0}\n", antonymString); }
            if (!string.IsNullOrEmpty(hyponymString)) { sb.AppendFormat("&#9660;\t{0}\n", hyponymString); }
            if (!string.IsNullOrEmpty(hyperonymString)) { sb.AppendFormat("&#9650;\t{0}\n", hyperonymString); }
            return sb.ToString();
        }
    }

}
