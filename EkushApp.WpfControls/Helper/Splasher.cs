/*
 *
 * Ref: http://www.codeproject.com/Articles/38291/Implement-Splash-Screen-with-WPF
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EkushApp.WpfControls.Helper
{
    /// <summary>
    /// Helper to show or close given splash window
    /// </summary>
    public static class Splasher
    {
        /// <summary>
        /// 
        /// </summary>
        private static Window _splashWindow;

        /// <summary>
        /// Get or set the splash screen window
        /// </summary>
        public static Window SplashWindow
        {
            get
            {
                return _splashWindow;
            }
            set
            {
                _splashWindow = value;
            }
        }

        /// <summary>
        /// Show splash screen
        /// </summary>
        public static void ShowSplash()
        {
            if (_splashWindow != null)
            {
                _splashWindow.Show();
            }
        }
        public static void HideSplash()
        {
            if (_splashWindow != null)
            {
                _splashWindow.Hide();
            }
        }
        /// <summary>
        /// Close splash screen
        /// </summary>
        public static void CloseSplash()
        {
            if (_splashWindow != null)
            {
                _splashWindow.Close();

                if (_splashWindow is IDisposable) (_splashWindow as IDisposable).Dispose();
            }
        }
    }
}
