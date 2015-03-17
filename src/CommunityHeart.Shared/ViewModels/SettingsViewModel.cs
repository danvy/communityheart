using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
#if NETFX_CORE
using Windows.Storage;
#endif
namespace CommunityHeart.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private ICommand _saveCommand;
        private int _heartRateMin = 50;
        private int _heartRateMax = 100;
        public static readonly SettingsViewModel Instance = new SettingsViewModel();
        private SettingsViewModel()
        {
        }
        public int HeartRateMin
        {
            get
            {
                return _heartRateMin;
            }
            set
            {
                if (value == _heartRateMin)
                    return;
                _heartRateMin = value;
                RaisePropertyChanged();
            }
        }
        public int HeartRateMax
        {
            get
            {
                return _heartRateMax;
            }
            set
            {
                if (value == _heartRateMax)
                    return;
                _heartRateMax = value;
                RaisePropertyChanged();
            }
        }
        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new RelayCommand(() =>
                {
                    Save();
                }));
            }
        }
        public void Save()
        {
#if NETFX_CORE
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values["HeartRateMin"] = HeartRateMin;
            settings.Values["HeartRateMax"] = HeartRateMax;
#endif
        }
        public void Load()
        {
#if NETFX_CORE
            var settings = ApplicationData.Current.LocalSettings;
            if (settings.Values.ContainsKey("HeartRateMin"))
            {
                HeartRateMin = (int)settings.Values["HeartRateMin"];
            }
            if (settings.Values.ContainsKey("HeartRateMax"))
            {
                HeartRateMin = (int)settings.Values["HeartRateMax"];
            }
#endif
        }
    }
}
