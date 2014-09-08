using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EkushApp.Localization.FontSizes
{
    internal class FontSizeChangedEventManager : WeakEventManager
    {
        internal static void AddListener(FontSizeManager source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedAddListener(source, listener);
        }

        internal static void RemoveListener(FontSizeManager source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedRemoveListener(source, listener);
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            DeliverEvent(sender, e);
        }

        protected override void StartListening(object source)
        {
            var manager = (FontSizeManager)source;
            manager.FontSizeChanged += OnLanguageChanged;
        }

        protected override void StopListening(Object source)
        {
            var manager = (FontSizeManager)source;
            manager.FontSizeChanged -= OnLanguageChanged;
        }

        private static FontSizeChangedEventManager CurrentManager
        {
            get
            {
                var managerType = typeof(FontSizeChangedEventManager);
                var manager = (FontSizeChangedEventManager)GetCurrentManager(managerType);
                if (manager == null)
                {
                    manager = new FontSizeChangedEventManager();
                    SetCurrentManager(managerType, manager);
                }
                return manager;
            }
        }
    }
}
