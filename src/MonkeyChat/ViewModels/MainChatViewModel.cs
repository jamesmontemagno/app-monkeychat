using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmHelpers;
using Plugin.Geolocator;
using Xamarin.Forms;
using System.Globalization;

namespace MonkeyChat
{
    public class MainChatViewModel : BaseViewModel
    {

        public ObservableRangeCollection<Message> Messages { get; }
        ITwilioMessenger twilioMessenger;

        string outgoingText = string.Empty;

        public string OutGoingText
        {
            get { return outgoingText; }
            set { SetProperty(ref outgoingText, value); }
        }

        public ICommand SendCommand { get; set; }


        public ICommand LocationCommand { get; set; }

        public MainChatViewModel()
        {
            // Initialize with default values
            twilioMessenger = DependencyService.Get<ITwilioMessenger>();



            Messages = new ObservableRangeCollection<Message>();

            SendCommand = new Command(() =>
            {
                var message = new Message
                {
                    Text = OutGoingText,
                    IsIncoming = false,
                    MessageDateTime = DateTime.Now
                };


                Messages.Add(message);
                twilioMessenger.SendMessage(message.Text);

                OutGoingText = string.Empty;
            });


            LocationCommand = new Command(async () =>
            {
                try
                {
                    var local = await CrossGeolocator.Current.GetPositionAsync(10000);
                    var map = $"https://maps.googleapis.com/maps/api/staticmap?center={local.Latitude.ToString(CultureInfo.InvariantCulture)},{local.Longitude.ToString(CultureInfo.InvariantCulture)}&zoom=17&size=400x400&maptype=street&markers=color:red%7Clabel:%7C{local.Latitude.ToString(CultureInfo.InvariantCulture)},{local.Longitude.ToString(CultureInfo.InvariantCulture)}&key=";

                    var message = new Message
                    {
                        Text = "I am here",
                        AttachementUrl = map,
                        IsIncoming = false,
                        MessageDateTime = DateTime.Now
                    };

                    Messages.Add(message);
                    twilioMessenger.SendMessage("attach:" + message.AttachementUrl);

                }
                catch (Exception ex)
                {

                }
            });


            twilioMessenger.MessageAdded = (message) =>
            {
                Messages.Add(message);
            };                      
        }

    }
    
}
