using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;

namespace Microsoft.VisualStudio.RegularExpression.Intellisense.CompletionProviders
{
    /// <summary>
    /// Provides the completions for the bracket '{' character.
    /// </summary>
    internal class BracketCompletions : RegexCompletionProvider
    {
        internal override List<Completion> GetCompletions(Microsoft.VisualStudio.Language.Intellisense.ICompletionSession session)
        {
            return new List<Completion>()
                {
                    new RegexCompletion("{N} : Exactly N", "{}", 1),
                    new RegexCompletion("{N,M} : From N to M", "{,}", 2)
                };
        }
    }
}
