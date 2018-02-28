using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using System.Windows;
using System.ComponentModel;

namespace Microsoft.VisualStudio.RegularExpression.UI
{
    /// <summary>
    /// Handles key in the regex editor
    /// </summary>
    internal class RegexEditorKeyProcessor : KeyProcessor
    {
        private IEditorOperations editorOperations;
        private KeyTypeConverter keyTypeConverter;

        internal RegexEditorKeyProcessor(IEditorOperations editorOperations)
        {
            this.editorOperations = editorOperations;
            this.keyTypeConverter = new KeyTypeConverter();
        }

        public override void KeyDown(KeyEventArgs args)
        {
            // Convert the Key into a char
            char? key = keyTypeConverter.ConvertToChar(args.Key);

            if (key.HasValue)
            {
                if (IsCtrlKeyDown())
                {
                    switch (key)
                    {
                        // Paste command
                        case 'v':
                        case 'V':
                            if (editorOperations.CanPaste)
                            {
                                this.editorOperations.Paste();
                            }
                            return;

                        // Copy command
                        case 'c':
                        case 'C':
                            this.editorOperations.CopySelection();
                            return;

                        // Cut command
                        case 'x':
                        case 'X':
                            if (editorOperations.CanCut)
                            {
                                this.editorOperations.CutSelection();
                            }
                            return;

                        // Select All command
                        case 'a':
                        case 'A':
                            editorOperations.SelectAll();
                            return;
                    }
                }
                else
                {
                    this.editorOperations.InsertText(key.Value.ToString());
                }
                args.Handled = true;
            }
            else
            {
                args.Handled = true;
                switch (args.Key)
                {
                    case Key.Left: this.editorOperations.MoveToPreviousCharacter(IsShiftKeyDown()); return;
                    case Key.Right: this.editorOperations.MoveToNextCharacter(IsShiftKeyDown()); return;
                    case Key.Home: this.editorOperations.MoveToStartOfLine(IsShiftKeyDown()); return;
                    case Key.End: this.editorOperations.MoveToEndOfLine(IsShiftKeyDown()); return;
                    case Key.Delete: this.editorOperations.Delete(); return;  
                }
                args.Handled = false;
            }                
        }

        public override void KeyUp(KeyEventArgs args)
        {
            if (args.Key == Key.Back)
            {
                this.editorOperations.Backspace();
                args.Handled = true;
            }
        }

        /// <summary>
        /// Returns true if the Shift Key is pressed
        /// </summary>
        /// <returns></returns>
        private bool IsShiftKeyDown()
        {
            return Keyboard.IsKeyDown(Key.RightShift) || Keyboard.IsKeyDown(Key.LeftShift);
        }

        /// <summary>
        /// Returns true if Ctrl key is pressed
        /// </summary>
        /// <returns></returns>
        private bool IsCtrlKeyDown()
        {
            return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        }

    }
}