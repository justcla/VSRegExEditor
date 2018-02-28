using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.VisualStudio.RegularExpression.BraceMatching
{
    [Export(typeof(IBraceMatcherProvider))]
    [ContentType(RegexContentType.ContentTypeName)]
    internal sealed class RegexBraceMatcherProvider : IBraceMatcherProvider
    {
        public IBraceMatcher GetBraceMatcher(ITextSnapshot snapshot)
        {
            return new RegexBraceMatcher(snapshot);
        }
    }
}