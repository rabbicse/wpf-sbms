using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EkushApp.WpfControls.Controls
{
    public class DigitOnlyTextBox : TextBox
    {
        public DigitOnlyTextBox()
        {
            this.PreviewTextInput += DigitOnlyTextBox_PreviewTextInput;
        }

        void DigitOnlyTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
           if (!string.IsNullOrEmpty(e.Text) && !e.Text.All(c => char.IsDigit(c)))
            {
                e.Handled = true;
            }
            else if (!string.IsNullOrEmpty(e.Text) && Convert.ToUInt32(this.Text + e.Text) > MaxValue) 
            {
                e.Handled = true;
            }
        }

        public static readonly DependencyProperty MaxValueProperty =
        DependencyProperty.Register("MaxValue", typeof(uint), typeof(DigitOnlyTextBox), new PropertyMetadata(default(uint)));

        public uint MaxValue
        {
            get { return (uint)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
    }
}
