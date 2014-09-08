using System;
using System.Collections.Generic;
using System.Resources;
using System.Threading;
using EkushApp.Localization.Contracts;
using EkushApp.Localization.Properties;

namespace EkushApp.Localization.Language
{
    internal static class LanguageLoader
    {

        //use this dictionary to cache resouce
        private static Dictionary<string, string> _cachedText = new Dictionary<string, string>();
        internal static void FlushCache()
        {
            _cachedText.Clear();
        }
        // used for thread-safety
        private static Object threadLock = new Object();
        /// <summary>
        /// Gets the value from the dictionary with the provided key.
        /// </summary>
        /// <param name="key">The key to search from the Dictionary.</param>
        /// <returns>The value of the provided key.</returns>
        internal static string GetText(string key)
        {
            try
            {
                lock (threadLock)
                {
                    if (_cachedText.ContainsKey(key)) return _cachedText[key];

                    var strLookup = ResourceManager.GetString(key, Thread.CurrentThread.CurrentUICulture);
                    if (string.IsNullOrEmpty(strLookup))
                    {
                        strLookup = ResourceManager.GetString(key, new System.Globalization.CultureInfo(Cultures.ENGLISH));
                    }

                    _cachedText.Add(key, strLookup);
                    return strLookup;
                }
            }
            catch (System.ArgumentNullException)
            {
                return String.Format(System.Globalization.CultureInfo.CurrentCulture, "Error 0001 - Unknown Error.");
            }
            catch (System.InvalidOperationException)
            {
                return String.Format(System.Globalization.CultureInfo.CurrentCulture, "Error 0002 - Unknown Error.");
            }
            catch (System.Resources.MissingManifestResourceException)
            {
                return String.Format(System.Globalization.CultureInfo.CurrentCulture, "Error 0003 - Unknown Error.");
            }
        }


        internal static string GetText(string key, System.Globalization.CultureInfo culture)
        {
            try
            {
                lock (threadLock)
                {
                    if (_cachedText.ContainsKey(key)) return _cachedText[key];

                    var strLookup = ResourceManager.GetString(key, culture);
                    if (string.IsNullOrEmpty(strLookup))
                    {
                        strLookup = ResourceManager.GetString(key, new System.Globalization.CultureInfo(Cultures.ENGLISH));
                    }

                    _cachedText.Add(key, strLookup);
                    return strLookup;
                }
            }
            catch (System.ArgumentNullException)
            {
                return String.Format(System.Globalization.CultureInfo.CurrentCulture, "Error 0001 - Unknown Error.");
            }
            catch (System.InvalidOperationException)
            {
                return String.Format(System.Globalization.CultureInfo.CurrentCulture, "Error 0002 - Unknown Error.");
            }
            catch (System.Resources.MissingManifestResourceException)
            {
                return String.Format(System.Globalization.CultureInfo.CurrentCulture, "Error 0003 - Unknown Error.");
            }
        }


        /// <summary>
        /// Gets the value ofr the provided key, and if not found, the default string will
        /// be shown.
        /// </summary>
        /// <param name="key">The key to search from the Dictionary.</param>
        /// <param name="defaultstr">the string that'll be shown if the key is not found.</param>
        /// <returns>The value/default string.</returns>
        internal static string GetText(string key, string defaultstr)
        {
            try
            {
                lock (threadLock)
                {
                    if (_cachedText.ContainsKey(key)) return _cachedText[key];
                    string result = ResourceManager.GetString(key, Thread.CurrentThread.CurrentUICulture);

                    if (String.IsNullOrEmpty(result))
                    {
                        return defaultstr;
                    }
                    else
                    {
                        _cachedText.Add(key, result);
                        return result;
                    }
                }
            }
            catch (System.ArgumentNullException)
            {
                return defaultstr;
            }
            catch (System.InvalidOperationException)
            {
                return defaultstr;
            }
            catch (System.Resources.MissingManifestResourceException)
            {
                return defaultstr;
            }
        }

        private static ResourceManager _resourceManager;
        internal static ResourceManager ResourceManager
        {
            get
            {
                if (_resourceManager == null)
                {
                    _resourceManager = Resources.ResourceManager;
                }
                return _resourceManager;
            }
            set { _resourceManager = value; }
        }
    }
}
