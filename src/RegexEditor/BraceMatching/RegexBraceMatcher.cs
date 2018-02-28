using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.RegularExpression.Parser;
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.RegularExpression.BraceMatching
{
    internal class RegexBraceMatcher : IBraceMatcher
    {
        private ITextSnapshot snapshot;

        internal RegexBraceMatcher(ITextSnapshot snapshot)
        {
            this.snapshot = snapshot;
        }

        public IList<Tuple<SnapshotSpan, SnapshotSpan>> GetBraceMatchingSpans(Microsoft.VisualStudio.Text.SnapshotPoint caretLocation)
        {
            string text = snapshot.GetText();
            List<Tuple<SnapshotSpan, SnapshotSpan>> braceMatchingSet = new List<Tuple<SnapshotSpan, SnapshotSpan>>();

            //Create a parser to parse the regular expression, and return the classification spans defined by it.
            foreach (Token token in snapshot.TextBuffer.Properties.GetProperty<ParserRunner>(typeof(ParserRunner)).Parser.Tokens)
            {
                if (token.MatchBraces && (token.Start == caretLocation.Position || token.End == caretLocation.Position))
                {
                    if (token.Kind == TokenKind.CaptureName)
                    {
                        braceMatchingSet.Add(new Tuple<SnapshotSpan, SnapshotSpan>(new SnapshotSpan(snapshot, token.Start + 1, 1), new SnapshotSpan(snapshot, token.End - 1, 1)));
                    }
                    else
                    {
                        braceMatchingSet.Add(new Tuple<SnapshotSpan, SnapshotSpan>(new SnapshotSpan(snapshot, token.Start, 1), new SnapshotSpan(snapshot, token.End - 1, 1)));
                    }
                }
            }

            return braceMatchingSet;
        }
    }
}