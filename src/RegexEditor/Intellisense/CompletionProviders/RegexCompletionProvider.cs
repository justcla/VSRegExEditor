using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;

namespace Microsoft.VisualStudio.RegularExpression.Intellisense.CompletionProviders
{
    /// <summary>
    /// Provides a list of completions.
    /// </summary>
    internal abstract class RegexCompletionProvider
    {
        internal static string CompletionProviderSessionKey = "completionProvider";

        /// <summary>
        /// Returns a list of completions.
        /// </summary>
        /// <param name="session">The completion session.</param>
        /// <returns>A list of completions.</returns>
        internal abstract List<Completion> GetCompletions(ICompletionSession session);
    }
}
