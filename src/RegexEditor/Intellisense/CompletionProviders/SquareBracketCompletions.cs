using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;

namespace Microsoft.VisualStudio.RegularExpression.Intellisense.CompletionProviders
{
    /// <summary>
    /// Provides the completions for the square bracket '[' character.
    /// </summary>
    internal class SquareBracketCompletions : RegexCompletionProvider
    {
        internal override List<Completion> GetCompletions(Microsoft.VisualStudio.Language.Intellisense.ICompletionSession session)
        {
            return new List<Completion>()
                {
                    new RegexCompletion("[<chars>] : Group", "[]", 1),
                    new RegexCompletion("[^<chars>] : Negated Group", "[^]", 1)
                };
        }
    }
}