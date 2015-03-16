using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;

namespace CommunityHeart.Services
{
    public class NavigationService : INavigationService
    {
        public void Navigate<T>()
        {
            Navigate<T>(null);
        }
        public void Navigate<T>(object parameter)
        {
            var t = Type.GetType(typeof(T).FullName.Replace("ViewModel", "View"));
            var frame = Window.Current.Content as Windows.UI.Xaml.Controls.Frame;
            if (frame != null)
                frame.Navigate(t, parameter);
        }
        public void GoBack()
        {
            var frame = Window.Current.Content as Windows.UI.Xaml.Controls.Frame;
            if (frame != null)
                frame.GoBack();
        }
        public bool CanGoBack()
        {
            var frame = Window.Current.Content as Windows.UI.Xaml.Controls.Frame;
            return frame != null ? frame.CanGoBack : false;
       }
    }
}
