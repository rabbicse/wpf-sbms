using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EkushApp.Localization.Fonts
{
    internal class FontFamilyChangedEventManager : WeakEventManager
    {
        internal static void AddListener(FontFamilyManager source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedAddListener(source, listener);
        }

        internal static void RemoveListener(FontFamilyManager source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedRemoveListener(source, listener);
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            DeliverEvent(sender, e);
        }

        protected override void StartListening(object source)
        {
            var manager = (FontFamilyManager)source;
            manager.FontFamilyChanged += OnLanguageChanged;
        }

        protected override void StopListening(Object source)
        {
            var manager = (FontFamilyManager)source;
            manager.FontFamilyChanged -= OnLanguageChanged;
        }

        private static FontFamilyChangedEventManager CurrentManager
        {
            get
            {
                var managerType = typeof(FontFamilyChangedEventManager);
                var manager = (FontFamilyChangedEventManager)GetCurrentManager(managerType);
                if (manager == null)
                {
                    manager = new FontFamilyChangedEventManager();
                    SetCurrentManager(managerType, manager);
                }
                return manager;
            }
        }
    }
}
