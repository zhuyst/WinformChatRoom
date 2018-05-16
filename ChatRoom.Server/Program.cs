using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using ChatRoom.Model;

namespace ChatRoom.Server
{
    class Program
    {
        private static readonly List<User> OnlineUsers = new List<User>();

        private static readonly List<ChatMessage> Messages = new List<ChatMessage>();

        private static int _idNow;

        static void Main(string[] args)
        {
            Console.WriteLine("正在启动聊天室服务器");

            ChatRoomRemote.AddUserEvent += user =>
            {
                Console.WriteLine($"新用户\t{user.Name}\t加入到了聊天室");
                OnlineUsers.Add(user);
                user.Id = ++_idNow;
                return user;
            };
            ChatRoomRemote.AddMessageEvent += message =>
            {
                Console.WriteLine($"接收到消息\t{message.Text}");
                Messages.Add(message);
            };
            ChatRoomRemote.GetUsersEvent += () => OnlineUsers;
            ChatRoomRemote.GetMessagesEvent += () => Messages;

            var channel = new TcpServerChannel(8080);
            ChannelServices.RegisterChannel(channel,true);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(ChatRoomRemote),
                "ChatRoom", WellKnownObjectMode.Singleton);

            Console.WriteLine("服务器已启动，输入任意键关闭");
            Console.ReadLine();
        }
    }
}
