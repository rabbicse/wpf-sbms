using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace EkushApp.Localization.Fonts
{
    internal class FontFamilyData : IWeakEventListener, INotifyPropertyChanged, IDisposable
    {
        #region Property(s)
        private CultureInfo _culture;
        public CultureInfo Culture
        {
            get { return _culture; }
            set { _culture = value; }
        }
        public FontFamily Value
        {
            get
            {
                return FontFamilyManager.Instance.FontFamily(_culture != null ? _culture.Name : Thread.CurrentThread.CurrentUICulture.Name);
            }
        }
        #endregion

        #region Constructor(s)
        internal FontFamilyData()
        {
            FontFamilyChangedEventManager.AddListener(FontFamilyManager.Instance, this);
        }
        ~FontFamilyData()
        {
            Dispose(false);
        }
        #endregion        

        #region IWeakEventListener Members
        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(FontFamilyChangedEventManager))
            {
                OnFontFamilyChanged(sender, e);
                return true;
            }
            return false;
        }

        private void OnFontFamilyChanged(object sender, EventArgs e)
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
                FontFamilyChangedEventManager.RemoveListener(FontFamilyManager.Instance, this);
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
