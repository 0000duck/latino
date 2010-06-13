using System;
using System.Collections.Generic;
using System.Text;
using Latino.TextMining;
using System.IO;

namespace LatinoTest
{
    class Program2
    {
        static void Main(string[] args)
        {
            LanguageDetector langDet = new LanguageDetector(2);
            langDet.ReadCorpus(@"C:\Users\Miha\Desktop\LangDetectCorpus");
            //LanguageProfile p = langDet.FindMatchingLanguage("To je slovenski stavek. Čeprav ga naš detektor ne zazna pravilno. Mogoče šumniki pomagajo...");
            //Console.WriteLine(p.Code);
            //Console.WriteLine(langDet.GetLanguageProfile("et"));
            StreamWriter w = new StreamWriter("c:\\krneki\\langSim.txt");
            foreach (LanguageProfile p in langDet.LanguageProfiles)
            {
                w.Write("{0}\t", p.Code);
            }
            w.WriteLine();
            foreach (LanguageProfile p in langDet.LanguageProfiles)
            {
                foreach (LanguageProfile p2 in langDet.LanguageProfiles)
                {
                    //w.Write("{0}\t", Math.Max(p.CalcSpearman(p2), p2.CalcSpearman(p)));
                }
                w.WriteLine();
            }
            w.Close();
        }
    }
}
