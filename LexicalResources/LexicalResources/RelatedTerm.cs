using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalResources
{
    public class RelatedTerm
    {
        public RelatedTerm(string term,string label, bool isMain)
        {
            this.Term = term;
            this.Label = label;
            this.IsMain = IsMain;
        }

        public string Term;

        public string Label;

        public bool IsMain;

        public override string ToString()
        {

            return Term + (string.IsNullOrEmpty(Label) ? "" : string.Format(" ({0})",Label));
        }
    }
}
