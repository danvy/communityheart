using CommunityHeart.Services;
using CommunityHeart.Tools;
using Microsoft.Band;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace CommunityHeart.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        IBandClient _client;
        private int _heartRate;
        private DateTimeOffset _heartTimeStamp;
        private bool _started;
        private ICommand _startStopCommand;
        private ICommand _settingsCommand;
        public MainViewModel()
        {
        }
        public async Task InitAsync()
        {
            if (_client != null)
                return;
            var bands = await BandClientManager.Instance.GetBandsAsync();
            if (bands.Length > 0)
            {
                _client = await BandClientManager.Instance.ConnectAsync(bands[0]);
            }
            if (_client != null)
            {
                if (_client.SensorManager.HeartRate.IsSupported)
                {
                    var intervals = _client.SensorManager.HeartRate.SupportedReportingIntervals;
                    _client.SensorManager.HeartRate.ReadingChanged += HeartRate_ReadingChanged;
                }
            }
        }
        public ICommand StartStopCommand
        {
            get
            {
                return _startStopCommand ?? (_startStopCommand = new RelayCommand(async () =>
                {
                    if (Started)
                    {
                        await Stop();
                    }
                    else
                    {
                        await Start();
                    }
                }));
            }
        }
        public ICommand SettingsCommand
        {
            get
            {
                return _settingsCommand ?? (_settingsCommand = new RelayCommand(() =>
                {
                    IoC.Instance.Resolve<INavigationService>().Navigate<SettingsViewModel>();
                }));
            }
        }
        public bool Started
        {
            get
            {
                return _started;
            }
            private set
            {
                if (value == _started)
                    return;
                _started = value;
                RaisePropertyChanged();
            }
        }
        public async Task Start()
        {
            if (_client == null)
                return;
            if (_client.SensorManager.HeartRate.IsSupported)
                await _client.SensorManager.HeartRate.StartReadingsAsync(new CancellationToken());
            Started = true;
        }
        public async Task Stop()
        {
            if (_client == null)
                return;
            if (_client.SensorManager.HeartRate.IsSupported)
                await _client.SensorManager.HeartRate.StopReadingsAsync();
            Started = false;
        }
        async void HeartRate_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandHeartRateReading> e)
        {
            if (e.SensorReading.Quality == Microsoft.Band.Sensors.HeartRateQuality.Locked)
            {
                _heartTimeStamp = e.SensorReading.Timestamp;
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => { HeartRate = e.SensorReading.HeartRate; });                
            }
        }
        public int HeartRate
        {
            get
            {
                return _heartRate;
            }
            set
            {
                if (value == _heartRate)
                    return;
                _heartRate = value;
                RaisePropertyChanged();
            }
        }
        public bool HeartBeating
        {
            get
            {
                return _heartTimeStamp < DateTime.UtcNow.AddSeconds(3);
            }
        }
    }

}
