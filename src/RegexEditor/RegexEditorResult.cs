using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.RegularExpression
{
    /// <summary>
    /// Represents the result of showing the regex dialog.
    /// </summary>
    internal class RegexEditorResult
    {
        internal RegexEditorResult(string pattern, bool? result)
        {
            Result = result;
            Pattern = pattern;
        }

        /// <summary>
        /// Gets the result of showing the dialog. If true the user accepted the dialog.
        /// </summary>
        internal bool? Result { get; private set; }

        /// <summary>
        /// Gets the regex expression.
        /// </summary>
        internal string Pattern { get; private set; }
    }
}
