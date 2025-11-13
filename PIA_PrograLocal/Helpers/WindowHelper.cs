using System;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;

namespace PIA_PrograLocal.Helpers
{
    public static class WindowHelper
    {
        private static readonly ConditionalWeakTable<UIElement, Microsoft.UI.Xaml.Window> table = new();

        public static void RegisterWindow(UIElement element, Microsoft.UI.Xaml.Window window)
        {
            if (element == null || window == null) return;
            lock (table)
            {
                try { table.Remove(element); } catch { }
                try { table.Add(element, window); } catch { }
            }
        }

        public static void UnregisterWindow(UIElement element)
        {
            if (element == null) return;
            lock (table)
            {
                try { table.Remove(element); } catch { }
            }
        }

        public static Microsoft.UI.Xaml.Window? GetWindowForElement(UIElement? element)
        {
            if (element == null) return null;
            if (table.TryGetValue(element, out var window)) return window;
            if (element is FrameworkElement fe && fe.Parent is UIElement parent) return GetWindowForElement(parent);
            return null;
        }
    }
}
