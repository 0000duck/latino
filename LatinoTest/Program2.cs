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
            LanguageDetector langDet = new LanguageDetector();
            langDet.ReadCorpus(@"C:\Users\mIHA\Desktop\langdet\MultextEast");
            LanguageProfile p = langDet.FindMatchingLanguage("To je slovenski stavek. Čeprav ga naš detektor ne zazna pravilno. Mogoče šumniki pomagajo...");
            Console.WriteLine(p.Language);
            p = langDet.FindMatchingLanguage("Funny thing... This is a very short English sentence...");
            Console.WriteLine(p.Language);
            //Console.WriteLine(langDet.GetLanguageProfile("et"));
            //StreamWriter w = new StreamWriter("c:\\krneki\\langSim.txt");
            //foreach (LanguageProfile p in langDet.LanguageProfiles)
            //{
            //    w.Write("{0}\t", p.Code);
            //}
            //w.WriteLine();
            //foreach (LanguageProfile p in langDet.LanguageProfiles)
            //{
            //    foreach (LanguageProfile p2 in langDet.LanguageProfiles)
            //    {
            //        //w.Write("{0}\t", Math.Max(p.CalcSpearman(p2), p2.CalcSpearman(p)));
            //    }
            //    w.WriteLine();
            //}
            //w.Close();
        }
    }
}
