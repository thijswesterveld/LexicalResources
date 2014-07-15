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
    public class ThesaurusVanDale : Thesaurus
    {
        ThesaurusEntry entry = null;
        WordSense sense = null;
        Relation relatedTerms = null;
        RelatedTerm rt = null;

        public ThesaurusVanDale(string fileName)
            : base()
        {
            TextReader tr = new StreamReader(fileName);
            var text = tr.ReadToEnd();
            var doc = XElement.Parse(text);
            foreach (var lemma in doc.XPathSelectElements("//lemma"))
            {
                string term = lemma.XPathSelectElement(".//trefw").Value;
                StartEntry(term);
                foreach(var betekenis in lemma.XPathSelectElements(".//bet"))
                {
                    string betNr = betekenis.XPathSelectElement("./betnr").Value;
                    string pos = null ;
                    var gramt = betekenis.XPathSelectElement("./gramt");
                    if(gramt != null)
                    {
                        pos = gramt.Value;
                    }
                    StartWordSense(int.Parse(betNr), term, "", pos);

                    foreach(var element in betekenis.XPathSelectElements("./*"))
                    {
                        switch(element.Name.LocalName)
                        {
                            case "centr":
                            case "padnt":
                            case "assoc":
                            case "hypon":
                            case "anto":
                                StartRelation(element.Name.LocalName);
                                StartTerm(element);
                                break;
                            case "nuanc":
                                StartRelation(element.Name.LocalName);
                                relatedTerms.Nuance = GetMainEntry(element);
                                break;
                            case "kaps":
                                foreach (var kaps in element.XPathSelectElements("./*"))
                                {
                                    switch(kaps.Name.LocalName)
                                    {
                                        case "kapst":
                                            StartRelation(kaps.Name.LocalName);
                                            relatedTerms.Nuance = kaps.Value;
                                            break;
                                        case "kaper":
                                            StartTerm(kaps);
                                            break;
                                        case "label":
                                            rt.Label = kaps.Value;
                                            break;
                                        default:
                                            Console.WriteLine("Unknown kaps element: {0}", kaps.Name.LocalName);
                                            break;
                                    }
                                }
                                break;
                            case "resum":
                                sense.Definition = GetMainEntry(element);
                                break;
                            case "perif":
                            case "finit":
                                StartTerm(element);
                                break;
                            case "label":
                                if (rt != null)
                                {
                                    rt.Label = GetMainEntry(element);
                                }
                                else 
                                {
                                    // no related term yet, label belongs to wordsense
                                    sense.Label = GetMainEntry(element);
                                }
                                break;
                            case "gramt":
                            case "betnr":
                                break;
                            default:
                                Console.WriteLine("Unknown element: {0}", element.Name.LocalName);
                                break;
                        }
                    }
                }
            }
            EndEntry();
        }

        private void StartEntry(string term)
        {
            EndEntry();
            entry = new ThesaurusEntry(term) ;   
        }

        private void EndEntry()
        {
            EndWordSense();
            if (entry != null)
            {
                thesaurus.Add(entry.Term, entry);
                entry = null;
            }
        }

        private void StartRelation(string elementName)
        {
            RelationType newType = GetType(elementName);
            EndRelation(newType);
            if(relatedTerms == null) 
            {
                relatedTerms = new Relation();
            }
            relatedTerms.Type = GetType(elementName);            
        }

        private void EndRelation(RelationType newType)
        {
            EndTerm();
            if (relatedTerms != null && relatedTerms.Type != RelationType.Unknown && relatedTerms.Type != newType)
            {
                sense.TermRelations.Add(relatedTerms);
                relatedTerms = null;
            }
        }

        private void StartTerm(XElement element)
        {
            EndTerm();
            rt = new RelatedTerm(GetMainEntry(element),null,element.Name.LocalName == "centr");
        }

        private void EndTerm()
        {
            if (rt != null)
            {
                if (relatedTerms == null)
                {
                    relatedTerms = new Relation();
                    relatedTerms.Type = RelationType.Synonym;
                }
                relatedTerms.Terms.Add(rt);
            }
            rt = null;
        }

        private void StartWordSense(int senseNr, string lemma, string definition, string pos)
        {
            EndWordSense();
            sense = new WordSense(senseNr, lemma, definition, pos);
        }

        private void EndWordSense()
        {
            EndRelation(RelationType.Unknown);
            if (sense != null)
            {
                entry.Senses.Add(sense);
                sense = null;
            }
        }

        private RelationType GetType(string localName)
        {
            switch (localName)
            {
                
                case "padnt":
                    return RelationType.Hyperonym;
                case "assoc":
                    return RelationType.Associative;
                case "hypon":
                    return RelationType.Hyponym;
                case "anto":
                    return RelationType.Antonym;
                case "centr":
                    return RelationType.Synonym;
                case "kapst":
                    return RelationType.Kaps;
                default: 
                    return RelationType.Unknown;    
            }
        }

        private string GetMainEntry(XElement element)
        {
            XText textNode = element.FirstNode as XText;
            if (textNode != null)
            {
                return textNode.Value;
            }
            return element.Value;
        }

    }
}
