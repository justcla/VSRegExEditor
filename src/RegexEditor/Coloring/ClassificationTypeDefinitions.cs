using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.VisualStudio.RegularExpression.Coloring
{
    //Claffication types are used to define coloring groups. They are linked to Classification Formats by the name
    internal static class ClassificationTypes
    {
        internal const string CharGroup = "regex char group";
        internal const string Expression = "regex expression";
        internal const string EscapedExpression = "regex escaped expression";
        internal const string Multiplier = "regex multiplier";
        internal const string Delimiter = "regex delimiter";
        internal const string Capture = "regex capture";
        internal const string Repetition = "regex repetition";

        [Export, Name(ClassificationTypes.CharGroup)]
        internal static ClassificationTypeDefinition RegexCharGroupClassificationType;

        [Export, Name(ClassificationTypes.Repetition)]
        internal static ClassificationTypeDefinition RegexRepetitionClassificationType;

        [Export, Name(ClassificationTypes.EscapedExpression)]
        internal static ClassificationTypeDefinition RegexEscapedExpressionClassificationType;

        [Export, Name(ClassificationTypes.Expression)]
        internal static ClassificationTypeDefinition RegexExpressionClassificationType;

        [Export, Name(ClassificationTypes.Multiplier)]
        internal static ClassificationTypeDefinition RegexMultiplierClassificationType;

        [Export, Name(ClassificationTypes.Delimiter)]
        internal static ClassificationTypeDefinition RegexDelimiterClassificationType;

        [Export, Name(ClassificationTypes.Capture)]
        internal static ClassificationTypeDefinition RegexCaptureClassificationType;

    }
}