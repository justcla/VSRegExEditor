using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.VisualStudio.RegularExpression.LaunchingPoints
{
    [ContentType("code")]
    [Export(typeof(IMouseProcessorProvider))]
    [Name("Launch Editor MouseProcessor")]
    [Order(Before = "default")]
	[TextViewRole(PredefinedTextViewRoles.Document)]
	internal class LaunchEditorMouseProcessorProvider : IMouseProcessorProvider
    {
        [Import(typeof(RegexEditorService))]
        private RegexEditorService RegexEditorService { get; set; }

        [Import]
        private IToolTipProviderFactory ToolTipProviderFactory { get; set; }

        #region IMouseProcessorProvider Members

        public IMouseProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            return new LaunchEditorMouseProcessor(wpfTextView, RegexEditorService, ToolTipProviderFactory);
        }

        #endregion
    }
}