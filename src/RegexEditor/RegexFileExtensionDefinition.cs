using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.RegularExpression
{
    internal sealed class RegexFileExtensionDefinition
    {
        [Export, FileExtension(RegexFileExtension), ContentType(RegexContentType.ContentTypeName)]
        internal static FileExtensionToContentTypeDefinition FileExtensionDefinition;
        
        internal const string RegexFileExtension = ".regex";
    }
}