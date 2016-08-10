using System;
using MTChat.Common;
using MTChat.Common.Messages;

namespace MTChat.Client.Wcf
{
    public class ChatServiceProxy : WcfAdapter<IChatService>, IChatService
    {
        public ChatServiceProxy(IChatServiceCallback callbackInstance, Uri address) 
            : base(callbackInstance, address)
        {
        }

        public OperationResult Say(TextMessage msg)
        {
            return ExecuteCommand(proxy => proxy.Say(msg));
        }

        public OperationResult Whisper(PersonalTextMessage msg)
        {
            return ExecuteCommand(proxy => proxy.Whisper(msg));
        }

        public OperationResult<Person[]> Join(Person person)
        {
            return ExecuteCommand(proxy => proxy.Join(person));
        }

        public OperationResult Leave()
        {
            return ExecuteCommand(proxy => proxy.Leave());
        }
    }
}
