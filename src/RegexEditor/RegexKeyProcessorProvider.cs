using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.VisualStudio.RegularExpression
{
    [Export(typeof(IKeyProcessorProvider))]
    [Name("Regex Key Processor")]
    [Order(Before="default")]
    [ContentType(RegexContentType.ContentTypeName)]
    [TextViewRole(PredefinedTextViewRoles.Document)]	
    internal sealed class RegexKeyProcessorProvider : IKeyProcessorProvider
    {
        public KeyProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            return (new RegexKeyProcessor(wpfTextView));
        }
    }
}