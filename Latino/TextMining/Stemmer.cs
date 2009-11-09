/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:          Stemmer.cs
 *  Version:       1.0
 *  Desc:		   Snowball word stemmer (LATINO wrapper)
 *  Author:        Miha Grcar
 *  Created on:    Dec-2008
 *  Last modified: Nov-2009
 *  Revision:      Nov-2009
 *
 ***************************************************************************/

using System;
using SF.Snowball.Ext;

namespace Latino.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Class Stemmer
       |
       '-----------------------------------------------------------------------
    */
    public class Stemmer : IStemmer, ISerializable
    {
        private Language m_language;
        private ISnowballStemmer m_stemmer;

        public Stemmer(Language language)
        {
            m_language = language;
            bool success = CreateStemmer();
            Utils.ThrowException(!success ? new ArgumentNotSupportedException("language") : null);
        }

        public Stemmer(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        private bool CreateStemmer()
        {
            switch (m_language)
            {
                case Language.English:
                    m_stemmer = new EnglishStemmer();
                    return true;
                case Language.German:
                    m_stemmer = new German2Stemmer();
                    return true;
                case Language.French:
                    m_stemmer = new FrenchStemmer();
                    return true;
                case Language.Spanish:
                    m_stemmer = new SpanishStemmer();
                    return true;
                case Language.Italian:
                    m_stemmer = new ItalianStemmer();
                    return true;
                case Language.Portuguese:
                    m_stemmer = new PortugueseStemmer();
                    return true;
                case Language.Danish:
                    m_stemmer = new DanishStemmer();
                    return true;
                case Language.Dutch:
                    m_stemmer = new DutchStemmer();
                    return true;
                case Language.Finnish:
                    m_stemmer = new FinnishStemmer();
                    return true;
                case Language.Norwegian:
                    m_stemmer = new NorwegianStemmer();
                    return true;
                case Language.Russian:
                    m_stemmer = new RussianStemmer();
                    return true;
                case Language.Swedish:
                    m_stemmer = new SwedishStemmer();
                    return true;
                default:
                    return false;
            }
        }

        // *** IStemmer interface implementation ***

        public string GetStem(string word)
        {
            Utils.ThrowException(word == null ? new ArgumentNullException("word") : null);
            m_stemmer.SetCurrent(word);
            m_stemmer.Stem();
            return m_stemmer.GetCurrent();
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions 
            writer.WriteInt((int)m_language);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions 
            m_language = (Language)reader.ReadInt();
            CreateStemmer();
        }
    }
}
