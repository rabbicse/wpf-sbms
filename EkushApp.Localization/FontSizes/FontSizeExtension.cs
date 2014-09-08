using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace EkushApp.Localization.FontSizes
{
    public class FontSizeExtension : MarkupExtension
    {
        #region private member(s)
        private double _size;
        private CultureInfo _culture;
        #endregion

        #region Constructor Argument(s)
        [ConstructorArgument("size")]
        public double Size
        {
            get { return _size; }
            set { _size = value; }
        }
        [ConstructorArgument("culture")]
        public CultureInfo Culture
        {
            get { return _culture; }
            set { _culture = value; }
        }        
        #endregion

        #region Constructor(s)
        public FontSizeExtension(double size) 
        {
            _size = size;
        }
        ~FontSizeExtension() { }
        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding("Value")
            {
                Source = new FontSizeData { Culture = _culture, Size = _size }
            };
            return binding.ProvideValue(serviceProvider);
        }
    }
}
