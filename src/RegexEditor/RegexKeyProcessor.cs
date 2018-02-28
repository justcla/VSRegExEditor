using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.VisualStudio.Text.Editor;

namespace Microsoft.VisualStudio.RegularExpression
{
    /// <summary>
    /// Provides key events.
    /// </summary>
    internal class RegexKeyProcessor : KeyProcessor
    {
        private IWpfTextView view;

        internal static event KeyEventHandler KeyDownEvent;
        internal static event KeyEventHandler KeyUpEvent;

        internal RegexKeyProcessor(IWpfTextView view)
        {
            this.view = view;
        }

        public override void KeyDown(KeyEventArgs args)
        {
            this.OnKeyDown(args);
            base.KeyDown(args);
        }

        public override void KeyUp(KeyEventArgs args)
        {
            this.OnKeyUp(args);
            base.KeyUp(args);
        }

        protected virtual void OnKeyDown(KeyEventArgs args)
        {
            if (KeyDownEvent != null)
            {
                KeyDownEvent(view, args);
            }
        }

        protected virtual void OnKeyUp(KeyEventArgs args)
        {
            if (KeyUpEvent != null)
            {
                KeyUpEvent(view, args);
            }
        }
    }
}