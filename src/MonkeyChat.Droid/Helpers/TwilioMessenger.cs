using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Android.Widget;
using Plugin.DeviceInfo;

using Twilio.Common;
using Twilio.IPMessaging;


namespace MonkeyChat.Droid
{
    public class TwilioMessenger : Java.Lang.Object, ITwilioMessenger, IPMessagingClientListener, IChannelListener, ITwilioAccessManagerListener
    {
        public ITwilioIPMessagingClient Client { get; private set; }
        public static IChannel GeneralChannel { get; private set; }

        public Action<Message> MessageAdded { get; set; }

        public void SendMessage(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || GeneralChannel == null)
                return;

            var msg = GeneralChannel.Messages.CreateMessage(text);

            GeneralChannel.Messages.SendMessage(msg, new StatusListener
            {
                SuccessHandler = () =>
                {
                    Debug.WriteLine("Success");
                }
            });
        }


        public async Task<bool> InitializeAsync()
        {
            var task = new TaskCompletionSource<bool>();
            if (!TwilioIPMessagingSDK.IsInitialized)
            {
                TwilioIPMessagingSDK.InitializeSDK(Xamarin.Forms.Forms.Context, new InitListener
                {
                    InitializedHandler = async delegate
                    {
                        await SetupAsync();
                        task.SetResult(true);
                    },
                    ErrorHandler = err =>
                    {
                        task.SetResult(false);
                    }
                });
            }
            else
            {
                return await SetupAsync();
            }

            return await task.Task.ConfigureAwait(false);
        }

        async Task<bool> SetupAsync()
        {
            var task = new TaskCompletionSource<bool>();
            var token = await TwilioHelper.GetTokenAsync();
            var accessManager = TwilioAccessManagerFactory.CreateAccessManager(token, this);
            Client = TwilioIPMessagingSDK.CreateIPMessagingClientWithAccessManager(accessManager, this);

            Client.Channels.LoadChannelsWithListener(new StatusListener
            {
                SuccessHandler = () =>
                {
                    GeneralChannel = Client.Channels.GetChannelByUniqueName("general");

                    if (GeneralChannel != null)
                    {
                        GeneralChannel.Listener = this;
                        JoinGeneralChannel();
                    }
                    else
                    {
                        CreateAndJoinGeneralChannel();
                    }

                    task.SetResult(true);
                }
            });

            return await task.Task.ConfigureAwait(false);
        }

       

        void JoinGeneralChannel()
        {
            GeneralChannel?.Join(new StatusListener
            {
                SuccessHandler = () =>
                {

                }
            });
        }

        void CreateAndJoinGeneralChannel()
        {
            var options = new Dictionary<string, Java.Lang.Object>();
            options["friendlyName"] = "General Chat Channel";
            options["ChannelType"] = ChannelChannelType.ChannelTypePublic;
            Client.Channels.CreateChannel(options, new CreateChannelListener
            {
                OnCreatedHandler = channel =>
                {
                    GeneralChannel = channel;
                    channel.SetUniqueName("general", new StatusListener
                    {
                        SuccessHandler = () => { Console.WriteLine("set unique name successfully!"); }
                    });
                    JoinGeneralChannel();
                },
                OnErrorHandler = () => { }
            });
        }



        public void OnAttributesChange(string p0)
        {
        }

        public void OnChannelAdd(IChannel p0)
        {
        }

        public void OnChannelChange(IChannel p0)
        {
        }

        public void OnChannelDelete(IChannel p0)
        {
        }

        public void OnChannelHistoryLoaded(IChannel p0)
        {
        }

        public void OnError(IErrorInfo p0)
        {
        }

        public void OnUserInfoChange(IUserInfo p0)
        {
        }

        public void OnAttributesChange(IDictionary<string, string> p0)
        {
        }

        public void OnMemberChange(IMember p0)
        {
        }

        public void OnMemberDelete(IMember p0)
        {
        }

        public void OnMemberJoin(IMember member)
        {
            Toast.MakeText(Xamarin.Forms.Forms.Context, $"{member.UserInfo.Identity} joined", ToastLength.Long).Show();
        }

        public void OnMessageAdd(IMessage message)
        {
            if (message.Author == TwilioHelper.Identity)
                return;

            if (message.MessageBody.StartsWith("attach:", StringComparison.InvariantCulture))
            {
                MessageAdded?.Invoke(new Message
                {
                    IsIncoming = true,
                    MessageDateTime = DateTime.Parse(message.TimeStamp),
                    Text = "I am here",
                    AttachementUrl = message.MessageBody.Replace("attach:", string.Empty)
                });
            }
            else
            {
                MessageAdded?.Invoke(new Message
                {
                    IsIncoming = true,
                    MessageDateTime = DateTime.Parse(message.TimeStamp),
                    Text = message.MessageBody
                });
            }
            

        }

        public void OnMessageChange(IMessage p0)
        {
        }

        public void OnMessageDelete(IMessage p0)
        {
        }

        public void OnTypingEnded(IMember p0)
        {
        }

        public void OnTypingStarted(IMember p0)
        {
        }

        public void OnError(ITwilioAccessManager p0, string p1)
        {
        }

        public void OnTokenExpired(ITwilioAccessManager p0)
        {
        }

        public void OnTokenUpdated(ITwilioAccessManager p0)
        {
        }
    }

    public class CreateChannelListener : ConstantsCreateChannelListener
    {
        public Action<IChannel> OnCreatedHandler { get; set; }
        public Action OnErrorHandler { get; set; }

        public override void OnCreated(IChannel channel)
        {
            OnCreatedHandler?.Invoke(channel);
        }

        public override void OnError(IErrorInfo errorInfo)
        {
            base.OnError(errorInfo);
        }
    }


}

