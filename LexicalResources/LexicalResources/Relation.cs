using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalResources
{
    public class Relation
    {
        public Relation() 
        {
            Type = RelationType.Unknown;
            Terms = new List<RelatedTerm>();
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public RelationType Type;

        public string Nuance;

        public List<RelatedTerm> Terms;

        public IEnumerable<string> GetSynonyms()
        {
            return Terms.ConvertAll(t => t.Term);
        }
        public override string ToString()
        {
            return (Nuance == null ? "" :string.Format("/{0}/:",Nuance)) + string.Join(separator, Terms.ConvertAll(t => t.ToString()));
        }

        private string separator { get { return Type == RelationType.Hyperonym ? " > " : ", "; } }
    }
}
