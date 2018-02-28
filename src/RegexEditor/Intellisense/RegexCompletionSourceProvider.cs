using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.VisualStudio.RegularExpression.Intellisense
{
    [Export(typeof(ICompletionSourceProvider))]
    [Name("Regex Completion Source Provider")]
    [Order(Before = "default")]
    [ContentType(RegexContentType.ContentTypeName)]
    internal class RegexCompletionSourceProvider : ICompletionSourceProvider
    {
        #region ICompletionSourceProvider Members
        
        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new RegexCompletionSource();
        }

        #endregion
    }
}