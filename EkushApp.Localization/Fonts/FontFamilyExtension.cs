using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace EkushApp.Localization.Fonts
{
    public class FontFamilyExtension : MarkupExtension
    {
        #region private member(s)
        private CultureInfo _culture;
        #endregion

        #region Constructor Argument(s)
        [ConstructorArgument("culture")]
        public CultureInfo Culture
        {
            get { return _culture; }
            set { _culture = value; }
        }
        #endregion

        #region Constructor(s)
        public FontFamilyExtension() { }
        ~FontFamilyExtension() { }
        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding("Value")
            {
                Source = new FontFamilyData { Culture = _culture }
            };
            return binding.ProvideValue(serviceProvider);
        }
    }
}
