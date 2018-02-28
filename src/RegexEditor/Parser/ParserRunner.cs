using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.RegularExpression.Parser
{
    /// <summary>
    /// Executes the parsing when the text buffer changes.
    /// </summary>
    internal sealed class ParserRunner
    {
        private ITextBuffer textBuffer;
        internal Parser Parser { get; private set; }

        internal ParserRunner(ITextBuffer textBuffer)
        {
            this.textBuffer = textBuffer;
            this.Parser = new Parser();

            textBuffer.Changed += new EventHandler<TextContentChangedEventArgs>(TextBuffer_Changed);

            Parse();
        }

        private void TextBuffer_Changed(object source, TextContentChangedEventArgs e)
        {
            Parse();
        }

        private void Parse()
        {
            this.Parser.Parse(textBuffer.CurrentSnapshot.GetText());
        }
    }
}