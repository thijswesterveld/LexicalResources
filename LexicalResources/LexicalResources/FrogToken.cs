using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalResources
{
    public class FrogToken :  IEquatable<FrogToken>
    {
        public FrogToken(string frogLine)
        {
            string[] parts = frogLine.Split('\t');
            if (parts.Length > 5)
            {
                TokenNumber = int.Parse(parts[0]);
                Term = parts[1];
                Lemma = parts[2];
                MorphologicalSegmentation = parts[3];
                POS = parts[4];
                POSconfidence = float.Parse(parts[5]);
            }
                    
        }

        public int TokenNumber;
        public string Term;
        public string Lemma;
        public string MorphologicalSegmentation;
        public string POS;
        public float POSconfidence;

        public bool Equals(FrogToken other)
        {
            return (this.Term.Equals(other.Term) && this.Lemma.Equals(other.Lemma) && this.MorphologicalSegmentation.Equals(other.MorphologicalSegmentation) && this.POS.Equals(other.POS));
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as FrogToken);
        }

        public override int GetHashCode()
        {
            return (this.Term+this.Lemma+this.MorphologicalSegmentation+this.POS).GetHashCode();
        }
    }
}
