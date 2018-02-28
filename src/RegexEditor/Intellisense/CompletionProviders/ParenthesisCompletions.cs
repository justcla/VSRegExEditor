using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;

namespace Microsoft.VisualStudio.RegularExpression.Intellisense.CompletionProviders
{
    /// <summary>
    /// Provides the completions for the parenthesis '(' character.
    /// </summary>
    internal class ParenthesisCompletions : RegexCompletionProvider
    {
        internal override List<Completion> GetCompletions(Microsoft.VisualStudio.Language.Intellisense.ICompletionSession session)
        {
            return new List<Completion>()
                {
                    new RegexCompletion("(<group>) : Capture group", "()", 1),
                    new RegexCompletion("(<group>|<group>) : Choice", "(|)", 2),
                    new RegexCompletion("(?<group>) : Named Capture", "(?<>)", 2),
                    new RegexCompletion("(?:<group>) : Passive group", "(?:)", 1),
                    new RegexCompletion("(?i:<group>) : Case insensitive group", "(?i:)", 1),
                    new RegexCompletion("(?m:<group>) : Multiline group", "(?m:)", 1),
                    new RegexCompletion("(?n:<group>) : Explicit capture group", "(?n:)", 1),
                    new RegexCompletion("(?s:<group>) : Single line group", "(?s:)", 1),
                    new RegexCompletion("(?x:<group>) : Allow whitespace in group", "(?x:)", 1),
                    new RegexCompletion("(?-i:<group>) : Case sensitive group", "(?-i:)", 1),
                    new RegexCompletion("(?-m:<group>) : Non-multiline group", "(?-m:)", 1),
                    new RegexCompletion("(?-n:<group>) : Non-explicit capture group", "(?-n:)", 1),
                    new RegexCompletion("(?-s:<group>) : Non-single line group", "(?-s:)", 1),
                    new RegexCompletion("(?-x:<group>) : Disallow whitespace in group", "(?-x:)", 1)
                };
        }
    }
}
