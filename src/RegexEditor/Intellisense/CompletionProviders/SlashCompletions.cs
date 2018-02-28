using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;

namespace Microsoft.VisualStudio.RegularExpression.Intellisense.CompletionProviders
{
    /// <summary>
    /// Provides the completions for the slash ('\') character.
    /// </summary>
    internal class SlashCompletions : RegexCompletionProvider
    {
        internal override List<Completion> GetCompletions(Microsoft.VisualStudio.Language.Intellisense.ICompletionSession session)
        {
            return new List<Completion>()
                {
                    new RegexCompletion("\\d : Digits", "\\d", 0),
                    new RegexCompletion("\\D : Non-digit characters", "\\D", 0),
                    new RegexCompletion("\\w : Letters", "\\w", 0),
                    new RegexCompletion("\\W : Non-letter characters", "\\W", 0),
                    new RegexCompletion("\\s : Spaces", "\\s", 0),
                    new RegexCompletion("\\S : Non-space characters", "\\S", 0),
                    new RegexCompletion("\\A : Start of string", "\\A", 0),
                    new RegexCompletion("\\Z : End of string", "\\Z", 0),
                    new RegexCompletion("\\B : Word boundary", "\\B", 0),
                    new RegexCompletion("\\b : Non-word boundary", "\\B", 0),
                    new RegexCompletion("\\t : Tab", "\\t", 0),
                    new RegexCompletion("\\v : Vertical Tab", "\\v", 0),
                    new RegexCompletion("\\r : Carriage return", "\\r", 0),
                    new RegexCompletion("\\f : Form feed", "\\f", 0),
                    new RegexCompletion("\\n : New line", "\\n", 0)
                };
        }
    }
}
