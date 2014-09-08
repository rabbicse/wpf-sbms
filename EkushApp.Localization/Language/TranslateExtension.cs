using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace EkushApp.Localization.Language
{
    /// <summary>
    /// The Translate Markup extension returns a binding to a TranslationData
    /// that provides a translated resource of the specified key
    /// </summary>
    public class TranslateExtension : MarkupExtension
    {
        #region Private Members
        private string _key;
        private string _format;
        private Mode _mode;
        private CultureInfo _culture;
        #endregion

        #region Construction
        public TranslateExtension() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateExtension"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public TranslateExtension(string key)
        {
            _key = key;
        }
        public TranslateExtension(string key, string format)
        {
            _key = key;
            _format = format;
        }        
        #endregion

        [ConstructorArgument("key")]
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        [ConstructorArgument("format")]
        public string Format
        {
            get { return _format; }
            set { _format = value; }
        }

        [ConstructorArgument("mode")]
        public Mode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        [ConstructorArgument("culture")]
        public CultureInfo Culture
        {
            get { return _culture; }
            set { _culture = value; }
        }

        /// <summary>
        /// See <see cref="MarkupExtension.ProvideValue" />
        /// </summary>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding("Value")
            {
                Source = new TranslationData
                {
                    Key = _key,
                    Format = _format,
                    Mode = _mode,
                    Culture = _culture
                }
            };
            return binding.ProvideValue(serviceProvider);
        }
    }
}
