using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.VisualStudio.RegularExpression.LaunchingPoints
{
    [ContentType("code")]
    [Export(typeof(IKeyProcessorProvider))]
    [Name("Launch Editor KeyProcessor")]
    [Order(Before = "Regex Key Processor")]
	[TextViewRole(PredefinedTextViewRoles.Document)]
	internal sealed class LaunchEditorKeyProcessorProvider : IKeyProcessorProvider
    {
        [Import(typeof(RegexEditorService))]
        private RegexEditorService RegexEditorService { get; set; }

        #region IKeyProcessorProvider Members

        public KeyProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            return new LaunchEditorKeyProcessor(wpfTextView, RegexEditorService);
        }

        #endregion
    }
}