using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.RegularExpression.BraceMatching
{
    /// <summary>
    /// Provides an implementation of <see cref="IBraceMatcher"/>
    /// </summary>
    internal interface IBraceMatcherProvider
    {
        IBraceMatcher GetBraceMatcher(ITextSnapshot snapshot);
    }
}