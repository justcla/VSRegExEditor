using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace Microsoft.VisualStudio.RegularExpression.LaunchingPoints
{
    internal class LaunchEditorMouseProcessor : MouseProcessorBase
    {
        private readonly static Regex NewRegexStatementForCSharpOrVisualBasic = new Regex(@"[nN]ew\s+(Regex|System\.Text\.RegularExpressions\.Regex)\s*\(\s*", RegexOptions.Singleline | RegexOptions.Compiled);

        private IWpfTextView view;
        private RegexEditorService regexEditorService;
        private IToolTipProviderFactory toolTipProviderFactory;

        internal LaunchEditorMouseProcessor(IWpfTextView view, RegexEditorService regexEditorService, IToolTipProviderFactory toolTipProviderFactory)
        {
            this.view = view;
            this.regexEditorService = regexEditorService;
            this.toolTipProviderFactory = toolTipProviderFactory;
            this.IsToolTipShown = false;
        }

        public override void PreprocessMouseMove(MouseEventArgs e)
        {
            // Look for regex lines to show the tooltip/hand cursor
            foreach (ITextViewLine line in this.view.TextViewLines)
            {
                RegexLineData regexLineData = RegexLineData.CreateFromTextViewLine(this.view, line);

                // Check if the regex line contains a new Regex statement
                if (IsValid(regexLineData))
                {
                    // Get the pattern parameter
                    SnapshotSpan? expression = regexLineData.Expression;

                    if (expression.HasValue)
                    {
                        // Get the marker geometry of the pattern parameter
                        Geometry expressionGeometry = view.TextViewLines.GetMarkerGeometry(expression.Value);
                        if (expressionGeometry != null && expressionGeometry.Bounds.Contains(GetPointRelativeToView(e)))
                        {
                            e.Handled = true;

                            if (!IsToolTipShown)
                            {
                                IsToolTipShown = true;
                                IToolTipProvider toolTipProvider = this.toolTipProviderFactory.GetToolTipProvider(this.view); 

                                // Show the tooltip
                                toolTipProvider.ShowToolTip(expression.Value.Snapshot.CreateTrackingSpan(
                                    expression.Value.Span,
                                    SpanTrackingMode.EdgeExclusive), "Ctrl+Click to open the regex editor",
                                    PopupStyles.DismissOnMouseLeaveText);

                            }
                            // If the ctrl key is pressed change the cursor to hand
                            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                            {
                                e.MouseDevice.OverrideCursor = Cursors.Hand;
                            }
                            return;
                        }
                    }
                }
            }

            IsToolTipShown = false;
            e.MouseDevice.OverrideCursor = null;
        }

        private Point GetPointRelativeToView(MouseEventArgs e)
        {
            Point point = e.GetPosition(view.VisualElement);

            // Adjust the point by the viewport's location
            point.X += view.ViewportLeft;
            point.Y += view.ViewportTop;
            return point;
        }


        public override void PreprocessMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            // Check if ctrl+click
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                // Create a regex line data
                RegexLineData regexLineData = RegexLineData.CreateFromCaretPosition(this.view);

                // Check if the line contains a new Regex statement
                if (IsValid(regexLineData))
                {
                    // Get the regular expression parameter of the Regex ctor
                    SnapshotSpan? expression = regexLineData.Expression;

                    if (expression.HasValue)
                    {
                        // Check if the click was made over the regular expression parameter.
                        if (regexLineData.CaretPosition >= expression.Value.Start.Position && regexLineData.CaretPosition <= expression.Value.End.Position)
                        {
                            RegexEditorResult editorResult = this.regexEditorService.ShowEditor(regexLineData.Expression.Value.GetText());
                            if (editorResult.Result.HasValue && editorResult.Result.Value)
                            {
                                this.view.TextBuffer.Replace(regexLineData.Expression.Value.Span, editorResult.Pattern);
                            }

                            e.Handled = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if the regex line contains a new Regex statement
        /// </summary>
        /// <param name="regexLineData"></param>
        /// <returns></returns>
        private bool IsValid(RegexLineData regexLineData)
        {
            return NewRegexStatementForCSharpOrVisualBasic.IsMatch(regexLineData.Text);
        }

        private bool IsToolTipShown { get; set; }
    }
}
