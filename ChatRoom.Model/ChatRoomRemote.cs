using System;
using System.Collections.Generic;

namespace ChatRoom.Model
{
    public delegate void ReceivedMessageHandler(IChatRoom chatRoom);

    public class OnLineUser : MarshalByRefObject
    {
        public User User { get; set; }

        public ReceivedMessageHandler ReceivedMessageEvent { get; set; }

        public void CallBack(IChatRoom chatRoom)
        {
            ReceivedMessageEvent?.Invoke(chatRoom);
        }
    }

    public class ChatRoomRemote : MarshalByRefObject
    {
        public Dictionary<int,User> Users => GetUsersEvent?.Invoke();

        public static User SystemUser = new User()
        {
            Name = "系统"
        };

        public static Func<User,User> LoginEvent;

        public static Action<User> LogoutEvent;

        public static Action<ChatMessage> AddMessageEvent;

        public static Func<Dictionary<int,User>> GetUsersEvent;

        public Dictionary<int,OnLineUser> OnLineUsers = new Dictionary<int, OnLineUser>();

        public OnLineUser Login(OnLineUser onLineUser)
        {
            var user = onLineUser.User;
            user = LoginEvent?.Invoke(user);
            onLineUser.User = user;

            OnLineUsers[user.Id] = onLineUser;

            Broadcast(user);
            SendSystemMessage($"欢迎\t{user.Name}\t进入聊天室");

            return onLineUser;
        }

        public void Logout(OnLineUser onLineUser)
        {
            OnLineUsers.Remove(onLineUser.User.Id);
            LogoutEvent?.Invoke(onLineUser.User);

            SendSystemMessage($"用户\t{onLineUser.User.Name}\t离开了聊天室");
        }

        public void AddMessage(ChatMessage message)
        {
            AddMessageEvent?.Invoke(message);

            if (message.ToUser != null)
            {
                var fromMessage = message.Clone();
                fromMessage.Type = MessageType.From;
                OnLineUsers[message.SendUser.Id].CallBack(fromMessage);

                var toMessage = message.Clone();
                toMessage.Type = MessageType.To;
                OnLineUsers[message.ToUser.Id].CallBack(toMessage);
            }
            else
            {
                Broadcast(message);
            }
        }

        private void Broadcast(IChatRoom chatRoom)
        {
            foreach (var online in OnLineUsers.Values)
            {
                online.CallBack(chatRoom);
            }
        }

        private void SendSystemMessage(string message)
        {
            Broadcast(new ChatMessage()
            {
                SendUser = SystemUser,
                Text = message,
                SendTime = DateTime.Now
            });
        }
    }
}
