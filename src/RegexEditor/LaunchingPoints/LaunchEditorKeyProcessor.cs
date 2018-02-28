using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Microsoft.VisualStudio.RegularExpression.LaunchingPoints
{
    /// <summary>
    /// Handles the key pressed event to launch the regex editor.
    /// </summary>
    internal class LaunchEditorKeyProcessor : KeyProcessor
    {
        private readonly static Regex NewRegexStatementForCSharpOrVisualBasic = new Regex(@"[nN]ew\s+(Regex|System\.Text\.RegularExpressions\.Regex)$", RegexOptions.Singleline | RegexOptions.Compiled);

        private IWpfTextView view;
        private RegexEditorService regexEditorService;
        private KeyTypeConverter keyTypeConverter;

        internal LaunchEditorKeyProcessor(IWpfTextView view, RegexEditorService regexEditorService)
        {
            this.view = view;
            this.regexEditorService = regexEditorService;

            this.keyTypeConverter = new KeyTypeConverter();
        }

        public override void KeyDown(KeyEventArgs args)
        {
            char? key = this.keyTypeConverter.ConvertToChar(args.Key);
            // Check if the parethesis key was pressed
            if (key.HasValue && key.Value == '(')
            {
                // Create a regex line data
                RegexLineData regexLineData = RegexLineData.CreateFromCaretPosition(this.view);

                // Check if the line contains a new Regex statement
                if (IsValid(regexLineData))
                {
                    this.view.TextBuffer.Insert(regexLineData.CaretPosition, key.ToString());
                    // Show the regex editor
                    RegexEditorResult editorResult = this.regexEditorService.ShowEditor();
                    // Insert the edited pattern in the line
                    if (editorResult.Result.HasValue && editorResult.Result.Value)
                    {
                        this.view.TextBuffer.Insert(regexLineData.CaretPosition + 1, string.Format("@\"{0}\"", editorResult.Pattern));
                    }

                    args.Handled = true;
                }
            }
        }

        /// <summary>
        /// Returns true if the regex line ends with a "new Regex" statement (for C# or VB)
        /// </summary>
        /// <param name="regexLineData"></param>
        /// <returns></returns>
        private bool IsValid(RegexLineData regexLineData)
        {
            return NewRegexStatementForCSharpOrVisualBasic.IsMatch(regexLineData.Text.TrimEnd());
        }
    }
}