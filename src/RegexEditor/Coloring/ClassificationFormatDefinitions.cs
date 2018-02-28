using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Editor;

namespace Microsoft.VisualStudio.RegularExpression.Coloring
{
    //Classification Formats are used to provide default color for the types defined in Classification Types.

    [Export(typeof(EditorFormatDefinition))]
    [UserVisible(false)]
    [ClassificationType(ClassificationTypeNames = ClassificationTypes.CharGroup)]
    [Name("RegexCharGroupFormatDefinition")]
    [DisplayName("Regexp Character Group")]
    [Order]
	[TextViewRole(PredefinedTextViewRoles.Document)]
	internal sealed class RegexCharGroupFormatDefinition : ClassificationFormatDefinition
    {
        public RegexCharGroupFormatDefinition()
        {
            ForegroundBrush = Brushes.ForestGreen;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [UserVisible(false)]
    [ClassificationType(ClassificationTypeNames = ClassificationTypes.Repetition)]
    [Name("RegexRepetitionFormatDefinition")]
    [DisplayName("Regexp Repetition")]
    [Order]
	[TextViewRole(PredefinedTextViewRoles.Document)]
	internal sealed class RegexRepetitionFormatDefinition : ClassificationFormatDefinition
    {
        public RegexRepetitionFormatDefinition()
        {
            ForegroundBrush = Brushes.Gray;
            IsItalic = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [UserVisible(false)]
    [ClassificationType(ClassificationTypeNames = ClassificationTypes.Capture)]
    [Name("RegexCaptureFormatDefinition")]
    [DisplayName("Regexp Capture")]
    [Order]
	[TextViewRole(PredefinedTextViewRoles.Document)]
	internal sealed class RegexCaptureFormatDefinition : ClassificationFormatDefinition
    {
        public RegexCaptureFormatDefinition()
        {
            ForegroundBrush = Brushes.Blue;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [UserVisible(false)]
    [ClassificationType(ClassificationTypeNames = ClassificationTypes.EscapedExpression)]
    [Name("RegexEscapedExpressionDefinition")]
    [DisplayName("Regexp Escaped Expression")]
    [Order]
	[TextViewRole(PredefinedTextViewRoles.Document)]
	internal sealed class RegexEscapedExpressionFormatDefinition : ClassificationFormatDefinition
    {
        public RegexEscapedExpressionFormatDefinition()
        {
            ForegroundBrush = Brushes.LightGray;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [UserVisible(false)]
    [ClassificationType(ClassificationTypeNames = ClassificationTypes.Expression)]
    [Name("RegexExpressionDefinition")]
    [DisplayName("Regexp Expression")]
    [Order]
	[TextViewRole(PredefinedTextViewRoles.Document)]
	internal sealed class RegexExpressionFormatDefinition : ClassificationFormatDefinition
    {
        public RegexExpressionFormatDefinition()
        {
            ForegroundBrush = Brushes.Blue;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [UserVisible(false)]
    [ClassificationType(ClassificationTypeNames = ClassificationTypes.Multiplier)]
    [Name("RegexMultiplierDefinition")]
    [DisplayName("Regexp Multiplier")]
    [Order]
	[TextViewRole(PredefinedTextViewRoles.Document)]
	internal sealed class RegexMultiplierFormatDefinition : ClassificationFormatDefinition
    {
        public RegexMultiplierFormatDefinition()
        {
            ForegroundBrush = Brushes.Gray;
            IsItalic = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [UserVisible(false)]
    [ClassificationType(ClassificationTypeNames = ClassificationTypes.Delimiter)]
    [Name("RegexDelimiterDefinition")]
    [DisplayName("Regexp Delimiter")]
    [Order]
	[TextViewRole(PredefinedTextViewRoles.Document)]
	internal sealed class RegexDelimiterFormatDefinition : ClassificationFormatDefinition
    {
        public RegexDelimiterFormatDefinition()
        {
            ForegroundBrush = Brushes.DarkBlue;
            IsItalic = true;
        }
    }
}
