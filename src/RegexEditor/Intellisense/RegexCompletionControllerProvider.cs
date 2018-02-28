using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Editor;

namespace Microsoft.VisualStudio.RegularExpression.Intellisense
{
    [Export(typeof(IIntellisenseControllerProvider))]
    [Name("Regex Completion Controller")]
    [Order(Before = "Default Completion Controller")]
    [ContentType(RegexContentType.ContentTypeName)]
    internal class RegexCompletionControllerProvider : IIntellisenseControllerProvider
    {
        [Import]
        private IClassificationTypeRegistryService ClassificationTypeRegistry { get; set; }

        [Import]
        private ICompletionBroker CompletionBroker { get; set; }

        public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
        {
            return new RegexCompletionController(subjectBuffers, textView, this.CompletionBroker);
        }
    }
}