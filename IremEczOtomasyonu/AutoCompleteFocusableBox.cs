using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace IremEczOtomasyonu
{
    /// <summary>
    /// AutoCompleteBox with solved focus problem.
    /// </summary>
    public class AutoCompleteFocusableBox : AutoCompleteBox
    {
        public new void Focus()
        {
            var textbox = Template.FindName("Text", this) as TextBox;
            if (textbox != null)
            {
                textbox.Focus();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Enter)
            {
                RaiseEnterKeyDownEvent();
            }
        }

        public event Action<object> EnterKeyDown;
        private void RaiseEnterKeyDownEvent()
        {
            var handler = EnterKeyDown;
            if (handler != null) handler(this);
        }
    }
}
