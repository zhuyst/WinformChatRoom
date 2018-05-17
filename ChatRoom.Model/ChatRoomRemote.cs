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
        public List<User> Users => GetUsersEvent?.Invoke();

        public List<ChatMessage> Messages => GetMessagesEvent?.Invoke();

        public static User SystemUser = new User()
        {
            Name = "系统"
        };

        public static Func<User,User> LoginEvent;

        public static Action<User> LogoutEvent;

        public static Action<ChatMessage> AddMessageEvent;

        public static Func<List<User>> GetUsersEvent;

        public static Func<List<ChatMessage>> GetMessagesEvent;

        public List<OnLineUser> OnLineUsers = new List<OnLineUser>();

        public OnLineUser Login(OnLineUser onLineUser)
        {
            OnLineUsers.Add(onLineUser);

            var user = onLineUser.User;
            user = LoginEvent?.Invoke(user);
            onLineUser.User = user;

            Broadcast(user);
            SendSystemMessage($"欢迎\t{user.Name}\t进入聊天室");

            return onLineUser;
        }

        public void Logout(OnLineUser onLineUser)
        {
            OnLineUsers.Remove(onLineUser);
            LogoutEvent?.Invoke(onLineUser.User);

            SendSystemMessage($"用户\t{onLineUser.User.Name}\t离开了聊天室");
        }

        public void AddMessage(ChatMessage message)
        {
            AddMessageEvent?.Invoke(message);
            Broadcast(message);
        }

        private void Broadcast(IChatRoom chatRoom)
        {
            foreach (var online in OnLineUsers)
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
