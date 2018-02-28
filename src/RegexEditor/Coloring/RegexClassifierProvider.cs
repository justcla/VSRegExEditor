using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.RegularExpression.Parser;

namespace Microsoft.VisualStudio.RegularExpression.Coloring
{   
    [Export(typeof(IClassifierProvider))]
    [ContentType(RegexContentType.ContentTypeName)]
    internal sealed class RegexClassifierProvider : IClassifierProvider
    {
        [Import]
        //The ClassificationTypeRegistryService is used to discover the types defined in ClassificationTypeDefinitions
        private IClassificationTypeRegistryService ClassificationTypeRegistry { get; set; }

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            buffer.Properties.GetOrCreateSingletonProperty<ParserRunner>(() => new ParserRunner(buffer));

            return new RegexClassifier(buffer, ClassificationTypeRegistry);
        }
    }
}