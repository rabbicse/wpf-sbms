using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EkushApp.Localization.Language
{
    public enum Mode { NORMAL, UPPERCASE, LOWERCASE, CAMELCASE }
    internal class TranslationData : IWeakEventListener, INotifyPropertyChanged, IDisposable
    {
        #region Private Members
        private string _key;
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        private string _format;
        public string Format
        {
            get { return _format; }
            set { _format = value; }
        }
        private Mode _mode;
        public Mode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }
        private CultureInfo _culture;
        public CultureInfo Culture
        {
            get { return _culture; }
            set { _culture = value; }
        }
        #endregion

        #region Constructor(s)
        internal TranslationData()
        {
            LanguageChangedEventManager.AddListener(TranslationManager.Instance, this);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationData"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        internal TranslationData(string key, Mode mode = Mode.NORMAL)
        {
            _key = key;
            _mode = mode;
            LanguageChangedEventManager.AddListener(TranslationManager.Instance, this);
        }
        internal TranslationData(string key, string format, Mode mode = Mode.NORMAL)
        {
            _key = key;
            _format = format;
            _mode = mode;
            LanguageChangedEventManager.AddListener(TranslationManager.Instance, this);
        }
        internal TranslationData(string key, string format, CultureInfo culture, Mode mode = Mode.NORMAL)
        {
            _key = key;
            _format = format;
            _culture = culture;
            _mode = mode;
            LanguageChangedEventManager.AddListener(TranslationManager.Instance, this);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="TranslationData"/> is reclaimed by garbage collection.
        /// </summary>
        ~TranslationData()
        {
            Dispose(false);
        }
        #endregion

        public string Value
        {
            get
            {
                if (string.IsNullOrEmpty(_key))
                {
                    return string.Empty;
                }
                else if (_format != null && _format.Length > 0 && _culture != null)
                {
                    return TranslationManager.Instance.Translate(_key, _format, _culture, _mode);
                }
                else if (_format != null && _format.Length > 0)
                {
                    return TranslationManager.Instance.Translate(_key, _format, _mode);
                }
                else if (_culture != null)
                {
                    return TranslationManager.Instance.Translate(_key, _culture, _mode);
                }
                else
                {
                    return TranslationManager.Instance.Translate(_key, _mode);
                }
            }
        }

        #region IWeakEventListener Members

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(LanguageChangedEventManager))
            {
                OnLanguageChanged(sender, e);
                return true;
            }
            return false;
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Code to dispose the managed resources of the class
                LanguageChangedEventManager.RemoveListener(TranslationManager.Instance, this);
            }
            // Code to dispose the un-managed resources of the class
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
