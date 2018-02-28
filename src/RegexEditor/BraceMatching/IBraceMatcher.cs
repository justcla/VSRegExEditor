using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.RegularExpression.BraceMatching
{
    /// <summary>
    /// Provides the brace macthing spans.
    /// </summary>
    internal interface IBraceMatcher
    {
        /// <summary>
        /// Returns the list of brace matching spans. Each tuple defines from/to the brace matching should be applied.
        /// It's possible to highlight more than one caracter.
        /// </summary>
        /// <param name="caretLocation"></param>
        /// <returns></returns>
        IList<Tuple<SnapshotSpan, SnapshotSpan>> GetBraceMatchingSpans(SnapshotPoint caretLocation);
    }
}