using EkushApp.Localization.Contracts;
using EkushApp.Localization.Fonts;
using EkushApp.Localization.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Resources;
using System.Reflection;
using EkushApp.Localization.Properties;
using EkushApp.Localization.FontSizes;

namespace EkushApp.Localization
{
    public class LocaleManager
    {
        #region Property(s)
        private static List<CultureBean> _defaultCultureCollection;
        public static List<CultureBean> DefaultCultureCollection
        {
            get
            {
                if (_defaultCultureCollection == null)
                {
                    _defaultCultureCollection = new List<CultureBean>();
                    _defaultCultureCollection.Add(new CultureBean
                    {
                        CultureType = CultureNames.ENGLISH
                    });
                }
                return _defaultCultureCollection;
            }
        }
        #endregion

        #region Method(s)
        public static void SetApplicationCultures(params CultureNames[] cultures)
        {
            if (_defaultCultureCollection == null)
            {
                _defaultCultureCollection = new List<CultureBean>();
            }
            _defaultCultureCollection.AddRange(cultures.Select(culture => new CultureBean
            {
                CultureType = culture
            }));
        }        

        public static void SetCurrentLanguage(string culture)
        {
            LanguageLoader.FlushCache();
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);
            TranslationManager.Instance.OnLanguageChanged();
            FontFamilyManager.Instance.OnFontFamilyChanged();
            FontSizeManager.Instance.OnFontSizeChanged();
        }

        public static void SetLocaleAssembly(string baseName, Assembly assembly)
        {
            try
            {
                LanguageLoader.ResourceManager = new ResourceManager(baseName, assembly);
            }
            catch (Exception x)
            {
                throw x;
            }
        }
        #endregion
    }
}
