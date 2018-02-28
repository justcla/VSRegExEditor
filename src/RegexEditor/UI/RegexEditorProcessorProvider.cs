using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Operations;

namespace Microsoft.VisualStudio.RegularExpression.UI
{
    [Export(typeof(IKeyProcessorProvider))]
    [Name("Regex Editor Key Processor")]
    [Order(After="Regex Key Processor")]
    [ContentType(RegexContentType.ContentTypeName)]
	[TextViewRole(PredefinedTextViewRoles.Document)]
	internal sealed class RegexEditorKeyProcessorProvider : IKeyProcessorProvider
    {
        [Import(typeof(IEditorOperationsFactoryService))]
        private IEditorOperationsFactoryService EditorOperationsFactoryService { get; set; }

        public KeyProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            // Create the key processor only if the target buffer is of content type "regex"
            if (wpfTextView.TextBuffer.ContentType.TypeName == RegexContentType.ContentTypeName)
            {
                IEditorOperations editorOperations = this.EditorOperationsFactoryService.GetEditorOperations(wpfTextView);

                return new RegexEditorKeyProcessor(editorOperations);
            }

            return null;
        }

    }
}
