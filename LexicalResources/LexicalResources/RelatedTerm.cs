using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalResources
{
    public class RelatedTerm
    {
        public RelatedTerm(string term, int senseNr, string label, bool isMain)
        {
            this.Term = term;
            this.Label = label;
            this.SenseNr = senseNr;
            this.IsMain = isMain;
        }

        public string Term;

        public int SenseNr;

        public string Label;

        public bool IsMain;

        public override string ToString()
        {

            return Term + (string.IsNullOrEmpty(Label) ? "" : string.Format(" ({0})",Label));
        }
    }
}
