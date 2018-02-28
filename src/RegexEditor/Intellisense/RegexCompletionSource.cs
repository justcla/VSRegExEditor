using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.RegularExpression.Intellisense.CompletionProviders;

namespace Microsoft.VisualStudio.RegularExpression.Intellisense
{
    /// <summary>
    /// Implementation of <see cref="ICompletionSource"/>. Provides the completion sets for the regex editor. 
    /// </summary>
    internal class RegexCompletionSource : ICompletionSource
    {
        internal static string CompletionSetName = "Regex";

        internal RegexCompletionSource()
        { }

        void ICompletionSource.AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            int triggerPointPosition = session.GetTriggerPoint(session.TextView.TextBuffer).GetPosition(session.TextView.TextSnapshot);
            ITrackingSpan trackingSpan = session.TextView.TextSnapshot.CreateTrackingSpan(
                triggerPointPosition, 0, SpanTrackingMode.EdgeInclusive);

            RegexCompletionProvider completionProvider = session.Properties[RegexCompletionProvider.CompletionProviderSessionKey] as RegexCompletionProvider;

            if (completionProvider != null)
            {
                // TODO: RC1 Investigate what the moniker parameter is
                completionSets.Add(new CompletionSet("regex", CompletionSetName, trackingSpan, completionProvider.GetCompletions(session), null));
            }
        }

        public void Dispose()
        {            
        }
    }
}