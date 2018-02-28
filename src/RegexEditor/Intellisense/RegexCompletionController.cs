using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.RegularExpression.Intellisense.CompletionProviders;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System.ComponentModel;
using System.Windows;

namespace Microsoft.VisualStudio.RegularExpression.Intellisense
{
    /// <summary>
    /// Triggers the intellisense completion for the regex editor.
    /// </summary>
    internal class RegexCompletionController : IIntellisenseController
    {
        private ITextView subjectTextView;
        private IList<ITextBuffer> subjectBuffers;
        private ICompletionBroker completionBroker;
        private ICompletionSession activeSession;
        private KeyTypeConverter keyTypeConverter;
        private RegexCompletion selectedCompletionBeforeCommit;
 
        /// <summary>
        /// The predicate specifies when the completion provider should be used.
        /// </summary>
        private Dictionary<Predicate<KeyEventArgs>, RegexCompletionProvider> completionProviders;

        internal RegexCompletionController(IList<ITextBuffer> subjectBuffers, ITextView subjectTextView, ICompletionBroker completionBroker)
        {
            this.subjectBuffers = subjectBuffers;
            this.subjectTextView = subjectTextView;
            this.completionBroker = completionBroker;
            this.keyTypeConverter = new KeyTypeConverter();

            this.BuildCompletionProviders();
            this.AttachToKeyEvents();
        }

        private void BuildCompletionProviders()
        {
            this.completionProviders = new Dictionary<Predicate<KeyEventArgs>, RegexCompletionProvider>();
            this.completionProviders.Add(e => keyTypeConverter.ConvertToChar(e.Key) == '{', new BracketCompletions());
            this.completionProviders.Add(e => keyTypeConverter.ConvertToChar(e.Key) == '[', new SquareBracketCompletions());
            this.completionProviders.Add(e => keyTypeConverter.ConvertToChar(e.Key) == '(', new ParenthesisCompletions());
            this.completionProviders.Add(e => keyTypeConverter.ConvertToChar(e.Key) == '\\', new SlashCompletions());
        }

        public void ConnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }

        public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }

        /// <summary>
        /// Detaches the events
        /// </summary>
        /// <param name="textView"></param>
        public void Detach(ITextView textView)
        {
            if (textView == null)
            {
                throw new InvalidOperationException("Already detached from text view");
            }
            if (this.subjectTextView != textView)
            {
                throw new ArgumentException("Not attached to specified text view", "textView");
            }

            RegexKeyProcessor.KeyDownEvent -= new System.Windows.Input.KeyEventHandler(OnKeyDown);
            RegexKeyProcessor.KeyUpEvent -= new System.Windows.Input.KeyEventHandler(OnKeyUp);
        }

        /// <summary>
        /// Handles the key up event.
        /// The intellisense window is dismissed when one presses ESC key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (activeSession != null)
            {
                if (e.Key == Key.Escape)
                {
                    activeSession.Dismiss();
                    e.Handled = true;
                }

                if (e.Key == Key.Enter)
                {
                    if (this.activeSession.SelectedCompletionSet.SelectionStatus != null && this.activeSession.SelectedCompletionSet.SelectionStatus.IsSelected)
                    {
                        selectedCompletionBeforeCommit = this.activeSession.SelectedCompletionSet.SelectionStatus.Completion as RegexCompletion;
                        activeSession.Commit();
                    }
                    else
                    {
                        activeSession.Dismiss();
                    }
                    e.Handled = true;
                }
            }
        }


        /// <summary>
        /// Returns the first completion provider that matches the event args.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private RegexCompletionProvider GetCompletionProvider(KeyEventArgs e)
        {
            foreach (KeyValuePair<Predicate<KeyEventArgs>, RegexCompletionProvider> pair in this.completionProviders)
            {
                if (pair.Key(e))
                {
                    return pair.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Triggers Statement completion when appropriate keys are pressed ('[', '{', '\', '(')
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Make sure that this event happened on the same text view to which we're attached.
            ITextView textView = sender as ITextView;
            if (this.subjectTextView != textView)
            {
                return;
            }

            // Find the completion provider associated with the pressed key
            RegexCompletionProvider completionProvider = GetCompletionProvider(e);

            if (completionProvider != null)
            {
                if (activeSession != null)
                {
                    activeSession.Dismiss();
                }

                // determine which subject buffer is affected by looking at the caret position
                SnapshotPoint? caretPoint = textView.Caret.Position.Point.GetPoint
                                                (textBuffer => (subjectBuffers.Contains(textBuffer)),
                                                 PositionAffinity.Predecessor);

                if (caretPoint.HasValue)
                {
                    // the invocation occurred in a subject buffer of interest to us
                    ICompletionBroker broker = completionBroker;
                    ITrackingPoint triggerPoint = caretPoint.Value.Snapshot.CreateTrackingPoint(caretPoint.Value.Position, PointTrackingMode.Positive);

                    // Create a completion session
                    activeSession = broker.CreateCompletionSession(textView, triggerPoint, true);
                    // Set the completion provider that will be used by the completion source
                    activeSession.Properties.AddProperty(RegexCompletionProvider.CompletionProviderSessionKey, completionProvider);
                    // Attach to the session events
                    activeSession.Dismissed += new System.EventHandler(OnActiveSessionDismissed);
                    activeSession.Committed += new System.EventHandler(OnActiveSessionCommitted);

                    this.subjectTextView.Properties.GetProperty<TextViewEventManager>(TextViewEventManager.Key).OnIntellisenseSessionStart(new EventArgs());
                    // Start the completion session. The intellisense will be triggered.
                    activeSession.Start();
                }
            }
            else if (e.Key == Key.Down && activeSession != null)
            {
                SelectNextCompletion();
                e.Handled = true;
            }
            else if (e.Key == Key.Up && activeSession != null)
            {
                SelectPreviousCompletion();
                e.Handled = true;
            }
        }

        private CompletionSet GetRegexCompletionSet()
        {
            return activeSession.CompletionSets.FirstOrDefault(set => set.DisplayName == RegexCompletionSource.CompletionSetName);
        }

        private void SelectNextCompletion()
        {
            int selectedCompletionIndex = GetRegexCompletionSet().Completions.IndexOf(activeSession.SelectedCompletionSet.SelectionStatus.Completion);
            if (++selectedCompletionIndex < GetRegexCompletionSet().Completions.Count)
            {
                activeSession.SelectedCompletionSet.SelectionStatus = new CompletionSelectionStatus(this.GetRegexCompletionSet().Completions[selectedCompletionIndex], true, true);
            }
        }

        private void SelectPreviousCompletion()
        {
            int selectedCompletionIndex = GetRegexCompletionSet().Completions.IndexOf(activeSession.SelectedCompletionSet.SelectionStatus.Completion);
            if (--selectedCompletionIndex >= 0)
            {
                activeSession.SelectedCompletionSet.SelectionStatus = new CompletionSelectionStatus(this.GetRegexCompletionSet().Completions[selectedCompletionIndex], true, true);
            }
        }

        void OnActiveSessionDismissed(object sender, System.EventArgs e)
        {
            this.subjectTextView.Properties.GetProperty<TextViewEventManager>(TextViewEventManager.Key).OnIntellisenseSessionEnd(new EventArgs());
            activeSession = null;
        }

        void OnActiveSessionCommitted(object sender, System.EventArgs e)
        {
            if (selectedCompletionBeforeCommit != null)
            {
                activeSession.TextView.Caret.MoveTo(new VirtualSnapshotPoint(activeSession.TextView.Caret.Position.BufferPosition.Subtract(selectedCompletionBeforeCommit.CommitPositionModifier)));
            }
        }

        /// <summary>
        /// Attaches to the key events.
        /// </summary>
        private void AttachToKeyEvents()
        {
            RegexKeyProcessor.KeyDownEvent += new System.Windows.Input.KeyEventHandler(OnKeyDown);
            RegexKeyProcessor.KeyUpEvent += new System.Windows.Input.KeyEventHandler(OnKeyUp);
        }

    }
}
