/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    BoilerplateRemoverComponent.cs
 *  Desc:    Boilerplate remover component 
 *  Created: Apr-2011
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Latino.WebMining;

namespace Latino.Workflows.TextMining
{
    /* .-----------------------------------------------------------------------
       |
       |  Class FoobarComponent
       |
       '-----------------------------------------------------------------------
    */
    public class FoobarComponent : DocumentProcessor
    {
        public FoobarComponent() : base(typeof(FoobarComponent))
        {
        }

        protected override void ProcessDocument(Document document)
        {
            while (true) ;    
        }
    }
}