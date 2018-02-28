using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.RegularExpression.Parser
{
    /// <summary>
    /// King of tokens
    /// </summary>
    internal enum TokenKind
    {
        CharGroup,
        Repetition,
        EscapedExpression,
        Expression,
        Multiplier,
        Delimiter,
        Capture,
        CaptureName
    }
}
