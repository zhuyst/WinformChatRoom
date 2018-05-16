using System;
using System.Collections.Generic;

namespace ChatRoom.Model
{

    public class ChatRoomRemote : MarshalByRefObject
    {
        public static Func<User,User> AddUserEvent;

        public static Action<ChatMessage> AddMessageEvent;

        public static Func<List<User>> GetUsersEvent;

        public static Func<List<ChatMessage>> GetMessagesEvent;

        public User AddUser(User user)
        {
            return AddUserEvent?.Invoke(user);
        }

        public void AddMessage(ChatMessage message)
        {
            AddMessageEvent?.Invoke(message);
        }

        public List<User> Users()
        {
            return GetUsersEvent?.Invoke();
        }

        public List<ChatMessage> Messages()
        {
            return GetMessagesEvent?.Invoke();
        }
    }
}
