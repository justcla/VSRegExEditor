using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.RegularExpression.Parser;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace Microsoft.VisualStudio.RegularExpression.Coloring
{
    /// <summary>
    /// Implements the coloring classification.
    /// </summary>
    internal sealed class RegexClassifier : IClassifier
    {
        private ITextBuffer buffer;
        private IClassificationTypeRegistryService classificationTypeRegistry;

        internal RegexClassifier(ITextBuffer bufferToClassify, IClassificationTypeRegistryService classificationTypeRegistry)
        {
            this.buffer = bufferToClassify;
            this.classificationTypeRegistry = classificationTypeRegistry;
        }

        //Coloring some expressions may affect other lines. The classifier is called for a specific line, so we
        //need to instruct it to be called again for each line that may be affected. This code calls the colorizer
        //for any span that will be affected.
        private void RecolorizeAffectedLines(Span span, ITextSnapshot snapshot)
        {
            if (span.End > span.Start)
            {
                OnClassificationChanged(new SnapshotSpan(snapshot, span.Start, span.End - span.Start));
            }
        }

        #region IClassifier Members

        // Use this event if a text change causes classifications on a line other the one on which the line occurred.
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        internal void OnClassificationChanged(SnapshotSpan span)
        {
            if (ClassificationChanged != null)
            {
                ClassificationChanged(this, new ClassificationChangedEventArgs(span));
            }
        }

        //This is the main method of the classifier. It should return one ClassificationSpan per group that needs coloring.
        //It will be called with a span that spans a single line where the edit has been made (or multiple times in paste operations).
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            List<ClassificationSpan> classificationSpans = new List<ClassificationSpan>();

            //Create a parser to parse the regular expression, and return the classification spans defined by it.
            foreach (Token token in span.Snapshot.TextBuffer.Properties.GetProperty<ParserRunner>(typeof(ParserRunner)).Parser.Tokens)
            {
                if (token.Kind == TokenKind.Capture)
                {
                    classificationSpans.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, token.Start, 1), classificationTypeRegistry.GetClassificationType(ClassificationTypes.Capture)));
                    classificationSpans.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, token.End - 1, 1), classificationTypeRegistry.GetClassificationType(ClassificationTypes.Capture)));
                }
                else
                {
                    classificationSpans.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, token.Start, token.End - token.Start), classificationTypeRegistry.GetClassificationType(Token.TranslateEnumToString(token.Kind))));
                }

                if (span.IntersectsWith(new Span(token.Start, token.End)) && !(span.Start.Position <= token.Start && span.End.Position >= token.End)  &&
                    (token.Kind == TokenKind.Capture || token.Kind == TokenKind.CharGroup || token.Kind == TokenKind.Multiplier))
                {
                    RecolorizeAffectedLines(new Span(span.End, token.End - span.End), span.Snapshot);
                }
            }

            return classificationSpans;
        }

        #endregion
    }
}