using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.Localization.Contracts
{
    public class CultureBean : INotifyPropertyChanged
    {
        public short CultureId { get; set; }
        public CultureNames CultureType { get; set; }
        public string CultureCode
        {
            get
            {
                switch (CultureType)
                {
                    case CultureNames.ENGLISH:
                        return Cultures.ENGLISH;
                    case CultureNames.BANGLA:
                        return Cultures.BANGLA;
                    case CultureNames.RUSSIA:
                        return Cultures.RUSSIA;
                    case CultureNames.TAJIK:
                        return Cultures.TAJIK_CYRL;
                    default:
                        return string.Empty;
                }
            }
        }
        public System.Windows.Media.FontFamily CultureFont
        {
            get
            {
                switch (CultureType)
                {
                    case CultureNames.ENGLISH:
                        return new System.Windows.Media.FontFamily("Arial");
                    case CultureNames.BANGLA:
                        return new System.Windows.Media.FontFamily("Siyam Rupali");
                    case CultureNames.RUSSIA:
                        return new System.Windows.Media.FontFamily("Trebuchet MS");
                    case CultureNames.TAJIK:
                        return new System.Windows.Media.FontFamily("Times New Roman Tj");
                    default:
                        return new System.Windows.Media.FontFamily("Arial");
                }
            }
        }
        public string CultureName
        {
            get
            {
                switch (CultureType)
                {
                    case CultureNames.ENGLISH:
                        return "English";
                    case CultureNames.BANGLA:
                        return "\u09AC\u09BE\u0982\u09B2\u09BE";
                    case CultureNames.RUSSIA:
                        return "русский";
                    case CultureNames.TAJIK:
                        return "таджик";
                    default:
                        return string.Empty;
                }
            }
        }
        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
