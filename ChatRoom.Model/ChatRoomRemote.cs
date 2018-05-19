using System;
using System.Collections.Generic;

namespace ChatRoom.Model
{
    /// <summary>
    /// 客户端接收到服务端推送时调用委托
    /// </summary>
    /// <param name="chatRoom"></param>
    public delegate void ReceivedMessageHandler(IChatRoom chatRoom);

    public class OnLineUser : MarshalByRefObject
    {
        /// <summary>
        /// 用户
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// 客户端接收到服务端推送时调用方法
        /// </summary>
        public ReceivedMessageHandler ReceivedMessageEvent { get; set; }

        /// <summary>
        /// 回调，服务端调用该方法进行server push
        /// </summary>
        /// <param name="chatRoom"></param>
        public void CallBack(IChatRoom chatRoom)
        {
            ReceivedMessageEvent?.Invoke(chatRoom);
        }
    }

    public class ChatRoomRemote : MarshalByRefObject
    {
        /// <summary>
        /// 当前在线用户
        /// </summary>
        public Dictionary<int,User> Users => GetUsersEvent?.Invoke();

        /// <summary>
        /// 登陆
        /// </summary>
        public static Func<User,User> LoginEvent;

        /// <summary>
        /// 登出
        /// </summary>
        public static Action<User> LogoutEvent;

        /// <summary>
        /// 添加聊天信息
        /// </summary>
        public static Action<ChatMessage> AddMessageEvent;

        /// <summary>
        /// 获取当前在线用户
        /// </summary>
        public static Func<Dictionary<int,User>> GetUsersEvent;

        /// <summary>
        /// 客户端放在服务端的远程对象
        /// </summary>
        public Dictionary<int,OnLineUser> OnLineUsers = new Dictionary<int, OnLineUser>();

        /// <summary>
        /// 登陆
        /// 将客户端远程对象放入服务端的远程对象中
        /// </summary>
        /// <param name="onLineUser">新用户</param>
        /// <returns>带着服务端生成的自增ID的用户</returns>
        public OnLineUser Login(OnLineUser onLineUser)
        {
            var user = onLineUser.User;

            // 通过LoginEvent获取自增ID
            user = LoginEvent?.Invoke(user);
            onLineUser.User = user;

            OnLineUsers[user.Id] = onLineUser;

            Broadcast(user);
            SendSystemMessage($"欢迎\t{user.Name}\t进入聊天室");

            return onLineUser;
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="onLineUser"></param>
        public void Logout(OnLineUser onLineUser)
        {
            OnLineUsers.Remove(onLineUser.User.Id);
            LogoutEvent?.Invoke(onLineUser.User);

            SendSystemMessage($"用户\t{onLineUser.User.Name}\t离开了聊天室");

            // 广播用户登出信息
            Broadcast(new SystemMessage()
            {
                RemoveUserId = onLineUser.User.Id
            });
        }

        /// <summary>
        /// 添加聊天信息
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(ChatMessage message)
        {
            AddMessageEvent?.Invoke(message);

            // ToUser不为null时，则为私聊
            if (message.ToUser != null)
            {
                var fromMessage = message.Clone();
                fromMessage.Type = MessageType.From;
                OnLineUsers[message.SendUser.Id].CallBack(fromMessage);

                var toMessage = message.Clone();
                toMessage.Type = MessageType.To;
                OnLineUsers[message.ToUser.Id].CallBack(toMessage);
            }

            // 否则为广播
            else
            {
                Broadcast(message);
            }
        }

        /// <summary>
        /// 服务端向客户端广播信息
        /// </summary>
        /// <param name="chatRoom"></param>
        private void Broadcast(IChatRoom chatRoom)
        {
            foreach (var online in OnLineUsers.Values)
            {
                online.CallBack(chatRoom);
            }
        }

        /// <summary>
        /// 发送系统消息
        /// </summary>
        /// <param name="message"></param>
        private void SendSystemMessage(string message)
        {
            Broadcast(new SystemMessage()
            {
                Text = message,
                SengTime = DateTime.Now
            });
        }
    }
}
