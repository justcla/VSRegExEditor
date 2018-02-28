using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Microsoft.VisualStudio.RegularExpression
{
    /// <summary>
    /// Allows to communicate different object that user an instance of <see cref="ITextView"/>
    /// </summary>
    internal class TextViewEventManager
    {
        public TextViewEventManager(ITextView textView)
        {
            this.TextView = textView;

        }

        internal const string Key = "TextViewEventManagerKey";

        internal event EventHandler IntellisenseSessionStart;
        internal event EventHandler IntellisenseSessionEnd;

        public ITextView TextView { get; private set; }

        internal virtual void OnIntellisenseSessionStart(EventArgs e)
        {
            if (IntellisenseSessionStart != null)
            {
                IntellisenseSessionStart(this, e);
            }
        }

        internal virtual void OnIntellisenseSessionEnd(EventArgs e)
        {
            if (IntellisenseSessionEnd != null)
            {
                IntellisenseSessionEnd(this, e);
            }
        }
    }
}
