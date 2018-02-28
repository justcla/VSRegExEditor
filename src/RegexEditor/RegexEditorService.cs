using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.RegularExpression.UI;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Editor;
using System.Windows.Controls;

namespace Microsoft.VisualStudio.RegularExpression
{
    /// <summary>
    /// Regular expression editor service.
    /// </summary>
    [Export(typeof(RegexEditorService))]
    internal class RegexEditorService
    {
        [Import(typeof(ITextEditorFactoryService))]
        private ITextEditorFactoryService TextEditorFactoryService { get; set; }

        [Import(typeof(ITextBufferFactoryService))]
        private ITextBufferFactoryService TextBufferFactoryService { get; set; }

        [Import(typeof(IContentTypeRegistryService))]
        private IContentTypeRegistryService ContentTypeRegistryService { get; set; }

        /// <summary>
        /// Shows the regex editor.
        /// </summary>
        /// <returns>The regular expression.</returns>
        internal RegexEditorResult ShowEditor()
        {
            return this.ShowEditor(string.Empty);
        }

        /// <summary>
        /// Shows the regex editor.
        /// </summary>
        /// <param name="pattern">The regular expression to be edited.</param>
        /// <returns>The regular expression.</returns>
        internal RegexEditorResult ShowEditor(string pattern)
        {
            // Create the regex editor
            IContentType regexContentType = this.ContentTypeRegistryService.GetContentType(RegexContentType.ContentTypeName);

            ITextBuffer textBuffer = this.TextBufferFactoryService.CreateTextBuffer(pattern, regexContentType);
            // TODO: RC1
            IWpfTextView view = this.TextEditorFactoryService.CreateTextView(textBuffer);
            IWpfTextViewHost editor = this.TextEditorFactoryService.CreateTextViewHost(view, true);
            editor.TextView.Properties.AddProperty(TextViewEventManager.Key, new TextViewEventManager(editor.TextView));
            HideTextViewMargins(editor);

            string result = pattern;
            bool? dialogResult = RegexEditorDialog.ShowDialog(pattern, editor, out result);

            return new RegexEditorResult(result, dialogResult);
        }

        private void HideTextViewMargins(IWpfTextViewHost view)
        {
            HideTextViewMargin(view, "Left");
            HideTextViewMargin(view, "Right");
            HideTextViewMargin(view, "Top");
            HideTextViewMargin(view, "Bottom");
        }

        private void HideTextViewMargin(IWpfTextViewHost view, string marginName)
        {
            IWpfTextViewMargin margin = view.GetTextViewMargin(marginName) as IWpfTextViewMargin;
            if (margin != null)
            {
//                margin.VisualElement.Width = 0;
                margin.VisualElement.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}