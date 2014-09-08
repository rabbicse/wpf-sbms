using EkushApp.Localization.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EkushApp.Localization.Fonts
{
    public class FontFamilyManager
    {
        private static FontFamilyManager _fontFamilyManager;
        public event EventHandler FontFamilyChanged;

        public static FontFamilyManager Instance
        {
            get
            {
                if (_fontFamilyManager == null) _fontFamilyManager = new FontFamilyManager();
                return _fontFamilyManager;
            }
        }

        ~FontFamilyManager() { }

        public void OnFontFamilyChanged()
        {
            if (FontFamilyChanged != null)
            {
                FontFamilyChanged(this, EventArgs.Empty);
            }
        }

        public FontFamily FontFamily(string cultureName)
        {
            switch (cultureName)
            {
                case Cultures.ENGLISH:
                    return FontFamilyConstants.ARIAL;
                case Cultures.BANGLA:
                    return FontFamilyConstants.SIYAM_RUPALI;
                case Cultures.RUSSIA:
                    return FontFamilyConstants.TREBUCHET_MS;
                case Cultures.TAJIK_CYRL:
                    return FontFamilyConstants.TIMES_NEW_ROMAN_TJ;
                default:
                    return FontFamilyConstants.ARIAL;
            }
        }
    }
}
