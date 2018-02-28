using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text.Adornments;

namespace Microsoft.VisualStudio.RegularExpression.BraceMatching
{
    internal class BraceMatchingPresenter
    {
        private IWpfTextView textView;
        //private IAdornmentLayer adornmentLayer;
        private IEnumerable<IBraceMatcher> braceMatchers;
        private ITextMarkerProviderFactory tagger;

        internal BraceMatchingPresenter(IWpfTextView textView, IList<IBraceMatcherProvider> braceMatcherProviders, ITextMarkerProviderFactory tagger)
        {
            this.braceMatchers = braceMatcherProviders.Select<IBraceMatcherProvider, IBraceMatcher>(prov => prov.GetBraceMatcher(textView.TextSnapshot));
            this.textView = textView;
            this.textView.Caret.PositionChanged += new EventHandler<CaretPositionChangedEventArgs>(Caret_PositionChanged);
            this.tagger = tagger;
        }

        private void Caret_PositionChanged(object source, CaretPositionChangedEventArgs e)
        {
            RemoveAllAdornments(e.TextView.TextBuffer);
            if (e.TextView.TextViewLines != null)
            {
                foreach (IBraceMatcher braceMatcher in braceMatchers)
                {
                    foreach (Tuple<SnapshotSpan, SnapshotSpan> spans in braceMatcher.GetBraceMatchingSpans(e.NewPosition.BufferPosition))
                    {
                        HighlightBounds(e.TextView.TextBuffer, spans.Item1);
                        HighlightBounds(e.TextView.TextBuffer, spans.Item2);
                    }
                }
            }
        }

        private void RemoveAllAdornments(ITextBuffer buffer)
        {
            tagger.GetTextMarkerTagger(buffer).RemoveTagSpans(span => true);
        }

        private void HighlightBounds(ITextBuffer buffer, SnapshotSpan span)
        {
               tagger.GetTextMarkerTagger(buffer).CreateTagSpan(span.Snapshot.CreateTrackingSpan(span.Span, SpanTrackingMode.EdgeExclusive), new TextMarkerTag("bracehighlight"));        
        }
    }
}