/*=====================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          Tutorial4_1.cs
 *  Version:       1.0
 *  Desc:          Tutorial 4.1: Text preprocessing
 *  Author:        Miha Grcar
 *  Created on:    Apr-2010
 *  Last modified: Apr-2010
 *  Revision:      Apr-2010
 *
 **********************************************************************/

using System;
using System.IO;
using Latino;
using Latino.TextMining;

namespace LatinoTutorials
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get the stop words and stemmer for English.

            IStemmer stemmer;
            Set<string>.ReadOnly stopWords;
            TextMiningUtils.GetLanguageTools(Language.English, 
                out stopWords, out stemmer);

            // Test the stemmer.

            Console.WriteLine(stemmer.GetStem("running"));
            // Output: run

            // Create a tokenizer.

            UnicodeTokenizer tokenizer = new UnicodeTokenizer();
            tokenizer.MinTokenLen = 2; // Each token must be at least 2 
                // characters long.
            tokenizer.Filter = TokenizerFilter.AlphaStrict; // Tokens
                // can consist of alphabetic characters only.

            // Test the tokenizer.

            tokenizer.Text = "one 1 two 2 three 3 one_1 two_2 three_3";
            foreach (string token in tokenizer)
            {
                Console.Write("\"{0}\" ", token);
            }
            Console.WriteLine();
            // Output: "one" "two" "three"

            // Load a document corpus from a file. Each line represents
            // a separate document.

            string[] docs = new string[] { "a", "b", "c" }; // !!!!!!!!!!!!!!!!

            // Create a bag-of-words space.

            BowSpace bowSpc = new BowSpace();
            bowSpc.Tokenizer = tokenizer; // Assign the tokenizer.
            bowSpc.StopWords = stopWords; // Assign the stop words.
            bowSpc.Stemmer = stemmer; // Assign the stemmer.
            bowSpc.MinWordFreq = 3; // A term must appear at least 3
                // times in the corpus for it to be part of the 
                // vocabulary.
            bowSpc.MaxNGramLen = 3; // Terms consisting of at most 3 
                // consecutive words will be considered.
            bowSpc.WordWeightType = WordWeightType.TfIdf; // Set the 
                // weighting scheme for the bag-of-words vectors to
                // TF-IDF.
            bowSpc.NormalizeVectors = true; // The TF-IDF vectors will 
                // be normalized.
            bowSpc.CutLowWeightsPerc = 0.2; // The terms with the lowest
                // weights representing the 20% of the overall weight 
                // sum will be removed from each TF-IDF vector.

            bowSpc.Initialize(docs); // Initialize the BOW space.

            // ...
        }
    }
}