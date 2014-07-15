using NLP;
using ReadabilityLevels.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexicalResources
{
    class Program
    {
        static void Main(string[] args)
        {
            InitDictionaries();
            
            InitThesauri();

            ExportThesaurus(vDaleThesaurus, @"C:\Users\ThijsWizeNoze\Documents\Data\VanDale\VanDaleSynonymList.txt");

        //    FrogTest();

            //TestStemmer();

           // CompareDictionaries(@"C:\Users\ThijsWizeNoze\Documents\DictionaryCompare_M.tsv"); ;
            
            //Compare(@"C:\Users\ThijsWizeNoze\Documents\SynsetCompare.tsv");

            //TextReader tr = new StreamReader(@"C:\Users\ThijsWizeNoze\Documents\kinderteksten.txt",Encoding.UTF8);
            //var text = tr.ReadToEnd();
            //var annotatedText  = Annotate(text, vDaleThesaurus,MWO);
        }

        private static void ExportThesaurus(Thesaurus thesaurus, string outputFile)
        {
            TextWriter stemResult = new StreamWriter(outputFile);
         
            foreach (var lemma in thesaurus.thesaurus)
            {
                foreach (var sense in lemma.Value.Senses)
                {
                    string synonyms = string.Join("\t", sense.GetSynonyms());
                    stemResult.WriteLine("{0}\t{1}", lemma.Key, synonyms);
                }
            }
            stemResult.Flush();
            stemResult.Close();
        }

        private static void TestStemmer()
        {
            TextWriter stemResult = new StreamWriter(@"C:\Users\ThijsWizeNoze\Documents\FrogStem.tsv");
             Dictionary<FrogToken, int> terms = new Dictionary<FrogToken, int>();
             foreach (FrogToken token in ReadFrog())
             {
                 if (!terms.ContainsKey(token))
                 {
                     terms.Add(token, 0);
                 }
                 terms[token]++;
             }
            Dictionary<string,SortedSet<string>> kpStem2FullForm = new Dictionary<string,SortedSet<string>>();
            Dictionary<string,SortedSet<string>> porterStem2FullForm = new Dictionary<string,SortedSet<string>>();
            Dictionary<string, SortedSet<string>> frogLemma2FullForm = new Dictionary<string, SortedSet<string>>();

            Dictionary<string, Tuple<string,string,string>> fullForm2Stems = new Dictionary<string,Tuple<string,string,string>>();
            Dictionary<string, int> fullFormFreq = new Dictionary<string, int>(); 
            foreach(var token in terms.OrderByDescending(t => t.Value))
            {
                string fullForm = RemoveDiacritics(token.Key.Term.ToLower());
                string kpStem = Stemmer.InstanceKP.Stem(fullForm);
                if(!kpStem2FullForm.ContainsKey(kpStem))
                {
                    kpStem2FullForm.Add(kpStem,new SortedSet<string>());
                }
                kpStem2FullForm[kpStem].Add(fullForm);

                string porterStem = Stemmer.InstancePorter.Stem(fullForm);
                if(!porterStem2FullForm.ContainsKey(porterStem))
                {
                    porterStem2FullForm.Add(porterStem,new SortedSet<string>());
                }
                porterStem2FullForm[porterStem].Add(fullForm);

                string lemma = token.Key.Lemma.ToLower();
                if(!frogLemma2FullForm.ContainsKey(lemma))
                {
                    frogLemma2FullForm.Add(lemma, new SortedSet<string>());
                }
                frogLemma2FullForm[lemma].Add(fullForm);

                if (!fullForm2Stems.ContainsKey(fullForm))
                {
                    fullForm2Stems.Add(fullForm, new Tuple<string, string, string>(lemma, kpStem, porterStem));
                    fullFormFreq.Add(fullForm,token.Value);
                }
            }
            foreach(var ff in fullForm2Stems)
            {
                string frogLemma = ff.Value.Item1;
                string kpStem = ff.Value.Item2;
                string porterStem = ff.Value.Item3;
                int freq = fullFormFreq[ff.Key];

                string frogSet = string.Join(", ", frogLemma2FullForm[frogLemma]);
                string kpSet = string.Join(", ", kpStem2FullForm[kpStem]);
                string porterSet = string.Join(", ", porterStem2FullForm[porterStem]);
                stemResult.WriteLine(string.Join("\t", ff.Key, freq,frogLemma, kpStem, porterStem,frogSet,kpSet,porterSet));
            }
            stemResult.Flush();
            stemResult.Close();
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        private static void FrogTest()
        {
            TextWriter dictComp = new StreamWriter(@"C:\Users\ThijsWizeNoze\Documents\FrogDictionaryCompare_M.tsv");
            TextWriter synonymComp = new StreamWriter(@"C:\Users\ThijsWizeNoze\Documents\FrogSynonymCompare_E.tsv");

            Dictionary<FrogToken, int> eTerms = new Dictionary<FrogToken, int>();
            Dictionary<FrogToken, int> mTerms = new Dictionary<FrogToken, int>();
            foreach (FrogToken token in ReadFrog())
            {
                if (token.Lemma.StartsWith("m"))
                {
                    if(!mTerms.ContainsKey(token))
                    {
                        mTerms.Add(token, 0);
                    }
                    mTerms[token]++;    
                }
                if(token.Lemma.StartsWith("e"))
                {
                    if(!eTerms.ContainsKey(token))
                    {
                        eTerms.Add(token,0); 
                    }
                    eTerms[token]++;
                }
            }
            foreach(var term in mTerms)
            {
                var lemma = term.Key.Lemma;
                dictComp.WriteLine(string.Join("\t", term.Key.Term, lemma, term.Key.MorphologicalSegmentation, term.Value, 
                    vDaleBasis.IsFrequent(lemma), vDaleBasis.IsFrequent(term.Key), 
                    vDaleJunior.IsFrequent(lemma),vDaleJunior.IsFrequent(term.Key), 
                    corporaFreq.IsFrequent(lemma),corporaFreq.IsFrequent(term.Key),
                    schrootenVermeer.IsFrequent(lemma),schrootenVermeer.IsFrequent(term.Key)
                    ));
            }

            foreach(var term in eTerms)
            {
                var vDaleSynonyms = Flatten(vDaleThesaurus.LookUp(term.Key.Lemma));
                 var MWOSynonyms = Flatten(MWO.LookUp(term.Key.Lemma));
                Compare(term.Key.Lemma, term.Value, vDaleSynonyms, MWOSynonyms, synonymComp);
            }
            dictComp.Flush();
            dictComp.Close();
            synonymComp.Flush();
            synonymComp.Close();
        }

        private static IEnumerable<FrogToken> ReadFrog()
        {
            foreach(string file in Directory.EnumerateFiles(@"C:\Users\ThijsWizeNoze\Documents\Data\frogOutput"))
            {
                TextReader tr = new StreamReader(file);
                string line = tr.ReadLine();
                while(line != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        yield return new FrogToken(line);
                    }
                    line = tr.ReadLine();
                }
            }
        }


        private static WoordenboekVanDale vDaleJunior;
        private static WoordenboekVanDale vDaleBasis;
        private static WoordenboekFreqList schrootenVermeer;
        private static WoordenboekFreqList corporaFreq;
        private static ThesaurusVanDale vDaleThesaurus;
        private static Thesaurus MWO; 
        
        private static void InitDictionaries()
        {
            vDaleJunior = new WoordenboekVanDale(@"C:\Users\ThijsWizeNoze\Documents\Data\VanDale\20140623\Sample_Junior_Van_Dale\nlnljunior-sample-m.xml");
            vDaleBasis = new WoordenboekVanDale(@"C:\Users\ThijsWizeNoze\Documents\Data\VanDale\20140623\Sample_Basis_Van_Dale\nlnlbasis-sample-m.xml");
            schrootenVermeer = new WoordenboekFreqList(@"C:\Users\ThijsWizeNoze\Documents\Data\SchrootenVermeerBasisOnderwijs\SV_GE.txt");
            corporaFreq = new WoordenboekFreqList(@"C:\Users\ThijsWizeNoze\Google Drive\Wizenoze public\Input classificatiesysteem\Frequentielijsten_Corpora_v3.0\AllLemmas.txt");
        }


        private static void InitThesauri()
        {
            vDaleThesaurus = new ThesaurusVanDale(@"C:\Users\ThijsWizeNoze\Documents\Data\VanDale\20140623\Sample_Thesaurus\thesaurus_E.xml");
            vDaleThesaurus.AddMainEntryHyponyms();
            //vDale.Stem();
            MWO = new ThesaurusMijnWoordenboekOnline(@"C:\Users\ThijsWizeNoze\Documents\Data\SynoniemenMijnWoordenboekTermOnly.txt");
            //MWO.Stem();
        }

        private static void CompareDictionaries(string outputFile)
        {
            TextWriter tw = new StreamWriter(outputFile);
            
            var vDbasis = vDaleBasis.lexicon.Keys;
            var vDjunior = vDaleJunior.lexicon.Keys;
            var bgKids = BackgroundCorpus.InstanceKidsNL.lexicon.Where(term => term.Value > 30 && term.Key.StartsWith("m")).Select(term => term.Key);
            var bgCorpora = BackgroundCorpus.InstanceNL.lexicon.Where(term => term.Value > 30 && term.Key.StartsWith("m")).Select(term => term.Key);
            var union = bgCorpora.Union(bgKids).Union(vDjunior).Union(vDbasis).OrderBy(s => s);
            foreach(string s in union)
            {
                tw.WriteLine(string.Join("\t", s, vDbasis.Contains(s), vDjunior.Contains(s), bgCorpora.Contains(s), bgKids.Contains(s)));
            }
            tw.Flush();
            tw.Close();
        }

        private static void CompareThesauri(string outputFile)
        {
            TextWriter tw = new StreamWriter(outputFile);
            List<string> MWOSynonyms;
            List<string> vDaleSynonyms;
            foreach (var lemma in vDaleThesaurus.thesaurus)
            {
                string term = lemma.Value.Term;
                vDaleSynonyms = Flatten(lemma.Value);
                ThesaurusEntry MWOentry;
                if (MWO.thesaurus.TryGetValue(lemma.Key, out MWOentry))
                {
                    // both thesauri have this term
                    MWOSynonyms = Flatten(MWOentry);
                }
                else
                {
                    // only van Dale has synonyms for this term
                    MWOSynonyms = new List<string>();
                }
                Compare(term,1,vDaleSynonyms, MWOSynonyms, tw);
                MWO.thesaurus.Remove(lemma.Key);
            }
            foreach (var lemma in MWO.thesaurus)
            {
                string term = lemma.Value.Term;
                if (term.StartsWith("e"))
                {
                    // only MWO has this term
                    MWOSynonyms = Flatten(lemma.Value);
                    vDaleSynonyms = new List<string>();
                    Compare(term, 1,vDaleSynonyms, MWOSynonyms, tw);
                }
            }
            tw.Flush();
            tw.Close();
        }
        
        private static void Compare(string term, int termCount, List<string> vDaleSynonyms,List<string> MWOSynonyms,TextWriter tw)
        {
            List<string> intersect = new List<string>();
            List<string> vDaleOnly = new List<string>();
            List<string> MWOOnly = new List<string>();
            
            foreach (string synonym in vDaleSynonyms)
            {
                string[] parts = synonym.Split(' ');
                if (parts.Length > 1)
                {
                    var synonymTerm = parts[1];
                    if(MWOSynonyms.Count(s=> s.EndsWith(synonymTerm)) > 0)
                    {
                        intersect.Add(synonym);
                        MWOSynonyms.RemoveAll(s => s.EndsWith(synonymTerm));
                    }
                    else
                    {
                        vDaleOnly.Add(synonym);
                    }
                }
            }
            MWOOnly = MWOSynonyms;
            
            tw.WriteLine(string.Join("\t", term, termCount, escape(intersect), escape(vDaleOnly),escape(MWOOnly)));
        }

        private static string escape(List<string> synonyms)
        {
            if(synonyms.Count > 0 && synonyms[0].StartsWith("="))
            {
                synonyms[0] = "'"+synonyms[0]; 
            }
            return "\"" + string.Join("\n", synonyms) + "\"";
        }
        

        private static List<string> Flatten(ThesaurusEntry entry)
        {
            IEnumerable<string> result = new List<string>();
            foreach(WordSense sense in entry.Senses)
            {
                result = result.Union(sense.GetSynonyms());
            }
            return new List<string>(result);
        }

        private static  string Annotate(string text, params Thesaurus[] args)
        {
            IEnumerable<StandOffAnnotation> issues = GetIssues(text,args);
            return new Preview().Annotate(text,issues);
        }

        private static IEnumerable<StandOffAnnotation> GetIssues(string text, params  Thesaurus[] args)
        {
            foreach (Token token in PreProcessor.InstanceNL.GetTokens(text))
            {
                if (token.Term.StartsWith("e") && BackgroundCorpus.InstanceKidsNL.TF(token) < 100)
                {
                    List<string> synonyms = new List<string>();
                    foreach (Thesaurus thesaurus in args)
                    {
                        yield return new StandOffAnnotation()
                        {
                            OriginalText = token.Term,
                            IssueDescription = string.Format("{1} term: {0}", token.Term, thesaurus.GetType().Name),
                            StartOffset = token.StartOffset,
                            EndOffset = token.EndOffset,
                            Alternatives = Flatten(thesaurus.LookUp(token.Term)).ToArray(),
                        };
                    }
                }
            }
        }

        
    }
}
