using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;

namespace Microsoft.VisualStudio.RegularExpression.Intellisense
{
    /// <summary>
    /// Customizes how the regex completion are shown.
    /// </summary>
    internal class RegexCompletion : Completion
    {
        /// <summary>
        /// Defines a modifier for the final position where the caret will be after commiting the completion
        /// </summary>
        internal int CommitPositionModifier { get; set; }

        internal RegexCompletion(string displayText, string insertionText, int commitPositionModifier)
            : base(displayText)
        {
            this.InsertionText = insertionText;
            this.CommitPositionModifier = commitPositionModifier;
            // Add the icon to the completion
            base.Properties.AddProperty(typeof(IconDescription), new IconDescription(StandardGlyphGroup.GlyphCSharpExpansion, StandardGlyphItem.GlyphItemPublic));
        }
    }
}