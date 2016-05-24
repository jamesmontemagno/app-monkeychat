using System;
using Twilio.IPMessaging;
using Twilio.Common;
using System.Threading.Tasks;
using Foundation;

namespace MonkeyChat.iOS
{
    public class TwilioMessenger : NSObject, ITwilioMessenger,  ITwilioIPMessagingClientDelegate, ITwilioAccessManagerDelegate
    {

        // Our chat client
        TwilioIPMessagingClient client;
        // The channel we'll chat in
        Channel generalChannel;

        public Action<Message> MessageAdded { get; set; }

        public async Task<bool> InitializeAsync()
        {
            var task = new TaskCompletionSource<bool>();
            try
            {
                var token = await TwilioHelper.GetTokenAsync();

                var accessManager = TwilioAccessManager.Create(token, this);
                client = TwilioIPMessagingClient.Create(accessManager, this);

                client.GetChannelsList((result, channels) =>
                {
                    generalChannel = channels.GetChannelWithUniqueName("general");
                    if (generalChannel != null)
                    {
                        generalChannel.Join(r =>
                        {
                            Console.WriteLine("successfully joined general channel!");
                            task.SetResult(true);
                        });
                    }
                    else
                    {
                        var options = new NSDictionary("TWMChannelOptionFriendlyName", "General Chat Channel", "TWMChannelOptionType", 0);

                        channels.CreateChannel(options, (creationResult, channel) =>
                        {
                            if (creationResult.IsSuccessful())
                            {
                                generalChannel = channel;
                                generalChannel.Join(r =>
                                {
                                    generalChannel.SetUniqueName("general", res => { });
                                    task.SetResult(true);
                                });
                            }
                            else
                            {
                                task.SetResult(false);
                            }
                        });
                    }
                });
            }
            catch
            {

                return false;
            }


            return await task.Task.ConfigureAwait(false);
        }

        public void SendMessage(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;
            
            var msg = generalChannel.Messages.CreateMessage(text);

            generalChannel.Messages.SendMessage(msg, r =>
            {
                
            });
        }

        [Foundation.Export("ipMessagingClient:channel:messageAdded:")]
        public void TwilioMessageAdded(TwilioIPMessagingClient client, Channel channel, Twilio.IPMessaging.Message message)
        {
            //we have our own
            if (message.Author == TwilioHelper.Identity)
                return;

            if (message.Body.StartsWith("attach:", StringComparison.InvariantCulture))
            {
                MessageAdded?.Invoke(new Message
                {
                    IsIncoming = true,
                    MessageDateTime = DateTime.Parse(message.Timestamp),
                    Text = "I am here",
                    AttachementUrl = message.Body.Replace("attach:", string.Empty)
                });
            }
            else
            {
                MessageAdded?.Invoke(new Message
                {
                    IsIncoming = true,
                    MessageDateTime = DateTime.Parse(message.Timestamp),
                    Text = message.Body
                });
            }
        }


        [Foundation.Export("accessManagerTokenExpired:")]
        public void TokenExpired(Twilio.Common.TwilioAccessManager accessManager)
        {
           
        }

        [Foundation.Export("accessManager:error:")]
        public void Error(Twilio.Common.TwilioAccessManager accessManager, Foundation.NSError error)
        {
           
        }
    }
}

