using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EkushApp.Localization.Language
{
    public class TranslationManager
    {
        #region Declaration(s)
        public event EventHandler LanguageChanged;
        #endregion

        #region Property(s)
        private static TranslationManager _translationManager;
        public static TranslationManager Instance
        {
            get
            {
                if (_translationManager == null) _translationManager = new TranslationManager();
                return _translationManager;
            }
        }
        #endregion

        #region Constructor(s)
        private TranslationManager() { }
        ~TranslationManager() { }
        #endregion

        public void OnLanguageChanged()
        {
            if (LanguageChanged != null)
            {
                LanguageChanged(this, EventArgs.Empty);
            }
        }

        public string Translate(string key, Mode mode = Mode.NORMAL)
        {
            return ChangeLanguageMode(LanguageLoader.GetText(key), mode, Thread.CurrentThread.CurrentUICulture);
        }

        public string Translate(string key, string format, Mode mode = Mode.NORMAL)
        {
            return string.Format(format, ChangeLanguageMode(LanguageLoader.GetText(key), mode, Thread.CurrentThread.CurrentUICulture));
        }

        public string Translate(string key, CultureInfo culture, Mode mode = Mode.NORMAL)
        {
            return ChangeLanguageMode(LanguageLoader.GetText(key, culture), mode, culture);
        }
        public string Translate(string key, string format, CultureInfo culture, Mode mode = Mode.NORMAL)
        {
            return string.Format(format, ChangeLanguageMode(LanguageLoader.GetText(key), mode, culture));
        }
        private string ChangeLanguageMode(string value, Mode mode, CultureInfo currentCulture)
        {
            switch (mode)
            {
                case Mode.LOWERCASE:
                    return !string.IsNullOrEmpty(value) ? currentCulture.TextInfo.ToLower(value) : string.Empty;
                case Mode.UPPERCASE:
                    return !string.IsNullOrEmpty(value) ? currentCulture.TextInfo.ToUpper(value) : string.Empty;
                case Mode.CAMELCASE:
                    return !string.IsNullOrEmpty(value) ? currentCulture.TextInfo.ToTitleCase(value) : string.Empty;
                case Mode.NORMAL:
                default:
                    return value;
            }
        }
    }
}
