using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace EkushApp.Localization.FontSizes
{
    internal class FontSizeData : IWeakEventListener, INotifyPropertyChanged, IDisposable
    {
        #region Property(s)
        private double _size;
        public double Size
        {
            get { return _size; }
            set { _size = value; }
        }
        private CultureInfo _culture;
        public CultureInfo Culture
        {
            get { return _culture; }
            set { _culture = value; }
        }
        public double Value
        {
            get
            {
                return _culture == null ? FontSizeManager.Instance.FontSize(_size, Thread.CurrentThread.CurrentUICulture.Name) : _size;
            }
        }
        #endregion

        #region Constructor(s)
        internal FontSizeData()
        {
            FontSizeChangedEventManager.AddListener(FontSizeManager.Instance, this);
        }
        ~FontSizeData()
        {
            Dispose(false);
        }
        #endregion

        #region IWeakEventListener Members
        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(FontSizeChangedEventManager))
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
                FontSizeChangedEventManager.RemoveListener(FontSizeManager.Instance, this);
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
