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

        public static Func<User,User> AddUserEvent;

        public static Action<ChatMessage> AddMessageEvent;

        public static Func<List<User>> GetUsersEvent;

        public static Func<List<ChatMessage>> GetMessagesEvent;

        public List<OnLineUser> OnLineUsers = new List<OnLineUser>();

        public OnLineUser AddUser(OnLineUser onLineUser)
        {
            OnLineUsers.Add(onLineUser);

            var user = onLineUser.User;
            user = AddUserEvent?.Invoke(user);
//            Broadcast(user);

            return onLineUser;
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
    }
}
