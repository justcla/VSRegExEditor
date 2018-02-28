using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace Microsoft.VisualStudio.RegularExpression
{
    internal class RegexContentType
    {
        [Export, Name(ContentTypeName), BaseDefinition("code")]
        internal static ContentTypeDefinition RegexContentTypeDefinition;

        internal const string ContentTypeName = "regex";
    }
}