using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Adornments;

namespace Microsoft.VisualStudio.RegularExpression.BraceMatching
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType(RegexContentType.ContentTypeName)]
	[TextViewRole(PredefinedTextViewRoles.Document)]
	internal class BraceMatchingPresenterFactory : IWpfTextViewCreationListener
    {
        [Import(typeof(ITextMarkerProviderFactory))]
        private ITextMarkerProviderFactory TextMakerProviderFactory { get; set; }

        [ImportMany(typeof(IBraceMatcherProvider))]
        private IEnumerable<Lazy<IBraceMatcherProvider>> BraceMatcherProviders { get; set; }

        public void TextViewCreated(IWpfTextView textView)
        {
            textView.Properties.GetOrCreateSingletonProperty<BraceMatchingPresenter>(delegate
            {
                var providers = BraceMatcherProviders.Select(prov => prov.Value);
                return new BraceMatchingPresenter(textView, providers.ToList(), TextMakerProviderFactory);
            });
        }
    }
}