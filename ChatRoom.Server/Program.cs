using System;
using System.Collections;
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

        private static int _idNow;

        static void Main(string[] args)
        {
            Console.WriteLine("正在启动聊天室服务器");

            ChatRoomRemote.LoginEvent += user =>
            {
                Console.WriteLine($"新用户\t{user.Name}\t加入到了聊天室");
                OnlineUsers.Add(user);
                user.Id = ++_idNow;
                return user;
            };
            ChatRoomRemote.LogoutEvent += user =>
            {
                Console.WriteLine($"用户\t{user.Name}\t离开了聊天室");
                OnlineUsers.Remove(user);
            };
            ChatRoomRemote.AddMessageEvent += message =>
            {
                Console.WriteLine($"接收到消息\t{message.Text}");
            };
            ChatRoomRemote.GetUsersEvent += () => OnlineUsers;

            var tcpProperties = new Hashtable
            {
                ["name"] = "ChatRoom",
                ["port"] = 8080
            };

            var tcpClientSinkProvider = new BinaryClientFormatterSinkProvider();
            var tcpServerSinkProvider = new BinaryServerFormatterSinkProvider
            {
                TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full
            };

            var channel = new TcpChannel(tcpProperties,tcpClientSinkProvider,tcpServerSinkProvider);
            ChannelServices.RegisterChannel(channel,false);

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(ChatRoomRemote),
                "ChatRoom", WellKnownObjectMode.Singleton);

            Console.WriteLine("服务器已启动，输入任意键关闭");
            Console.ReadLine();

            ChannelServices.UnregisterChannel(channel);
        }
    }
}
