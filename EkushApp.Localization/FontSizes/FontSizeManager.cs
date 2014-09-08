using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EkushApp.Localization.Contracts;

namespace EkushApp.Localization.FontSizes
{
    public class FontSizeManager
    {
        private static FontSizeManager _fontsizeManager;
        public event EventHandler FontSizeChanged;

        public static FontSizeManager Instance
        {
            get
            {
                if (_fontsizeManager == null) _fontsizeManager = new FontSizeManager();
                return _fontsizeManager;
            }
        }

        ~FontSizeManager() { }

        public void OnFontSizeChanged()
        {
            if (FontSizeChanged != null)
            {
                FontSizeChanged(this, EventArgs.Empty);
            }
        }

        public double FontSize(double size, string cultureName)
        {
            switch (cultureName)
            {
                case Cultures.BANGLA:
                    return Math.Round(size * FontSizeConstants.BANGLA_RATIO);
                case Cultures.RUSSIA:
                    return Math.Round(size * FontSizeConstants.RUSSIAN_RATIO);
                case Cultures.TAJIK_CYRL:
                    return Math.Round(size * FontSizeConstants.TAJIK_RATIO);
                case Cultures.ENGLISH:
                default:
                    return size;
            }
        }
    }
}
