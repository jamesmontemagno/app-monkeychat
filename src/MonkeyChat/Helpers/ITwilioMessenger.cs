using System;
using System.Threading.Tasks;

namespace MonkeyChat
{
    public interface ITwilioMessenger
    {
        Task<bool> InitializeAsync();

        void SendMessage(string text);

        Action<Message> MessageAdded { get; set; }
    }
}

