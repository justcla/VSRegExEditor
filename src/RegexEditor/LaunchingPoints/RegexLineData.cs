using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace Microsoft.VisualStudio.RegularExpression.LaunchingPoints
{
    /// <summary>
    /// Provides information about the current regex line.
    /// Tipically the line will be similar to: Regex regex = new Regex("pattern", options);
    /// </summary>
    internal class RegexLineData
    {
        private const string Quote = "\"";

        private RegexLineData() { }

        /// <summary>
        /// Gets the regex snapshot line.
        /// </summary>
        internal ITextSnapshotLine Line { get; private set; }

        /// <summary>
        /// Gets the text of the line.
        /// </summary>
        internal string Text 
        {
            get { return this.Line.GetText(); } 
        }

        /// <summary>
        /// Gets the view where the line is.
        /// </summary>
        internal ITextView View { get; private set; }

        /// <summary>
        /// Gets the caret point at the time the line was created
        /// </summary>
        internal SnapshotPoint? CaretPoint { get; set; }
        
        /// <summary>
        /// Gets the caret position at the time the line was created
        /// </summary>
        internal int CaretPosition 
        { 
            get { return this.CaretPoint.Value.Position; } 
        }

        /// <summary>
        /// Gets the snapshot for the regular expression parameter of the regex ctor.
        /// </summary>
        internal SnapshotSpan? Expression
        {
            get
            {
                // Get the position of the first quote
                int start = this.Text.IndexOf(Quote);
                if (start != -1)
                {
                    // Get the position of the last quote
                    int end = this.Text.LastIndexOf(Quote);

                    if (end > start)
                    {
                        return new SnapshotSpan(this.View.TextSnapshot, start + 1 + this.Line.Start.Position, end - start - 1);
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Creates the regex line for the current position in the view.
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        internal static RegexLineData CreateFromCaretPosition(IWpfTextView view)
        {
            RegexLineData data = new RegexLineData();

            data.View = view;

            data.CaretPoint = view.Caret.Position.Point.GetPoint(
                    view.TextBuffer,
                    PositionAffinity.Predecessor);
        
            data.Line = view.TextSnapshot.GetLineFromPosition(data.CaretPosition);

            return data;
        }

        /// <summary>
        /// Creates the regex line for the current position in the view.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        internal static RegexLineData CreateFromTextViewLine(ITextView view, ITextViewLine line)
        {
            RegexLineData data = new RegexLineData();

            data.View = view;

            // TODO: RC1
            //data.Line = line.SnapshotLine;
            data.Line = line.Snapshot.GetLineFromPosition(line.Start.Position);

            return data;
        }
    }
}