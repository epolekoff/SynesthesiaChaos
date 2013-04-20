using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;

namespace SynesthesiaChaos
{
    public enum WindowState { Full = 0, Snap1Quarter = 1, Snap3Quarter = 2 };

    public static class Windows8
    {
        public static WindowState _windowState;
        public static CoreWindow _window;
        public static Rect _windowsBounds;

        public static void Initialize()
        {
            _window = CoreWindow.GetForCurrentThread();
            _windowsBounds = _window.Bounds;
            _windowState = WindowState.Full;
            _window.SizeChanged += _window_SizeChanged;

            CoreApplication.Suspending += CoreApplication_Suspending;
            CoreApplication.Resuming += CoreApplication_Resuming;
        }

        static void CoreApplication_Resuming(object sender, object e)
        {
            // coming back from suspend, probably don't need to do anything as current state is in memory
        }

        static void CoreApplication_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            // suspending, save appropriate game and user state
        }

        //called when the window is resized
        static void _window_SizeChanged(CoreWindow sender, WindowSizeChangedEventArgs args)
        {
            if (args.Size.Width == _windowsBounds.Width)
            {
                _windowState = WindowState.Full;
            }
            else if (args.Size.Width <= 320.00)
            {
                _windowState = WindowState.Snap1Quarter;
            }
            else
            {
                _windowState = WindowState.Snap3Quarter;
            }

            //_windowsBounds.Height = args.Size.Height;
            //_windowsBounds.Width = args.Size.Width;
        }
    }
}
