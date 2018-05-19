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
        /// <summary>
        /// 在线用户
        /// </summary>
        private static readonly Dictionary<int,User> OnlineUsers = new Dictionary<int, User>();

        /// <summary>
        /// 当前最新的用户ID
        /// </summary>
        private static int _idNow;

        static void Main(string[] args)
        {
            Console.WriteLine("正在启动聊天室服务器");

            ChatRoomRemote.LoginEvent += user =>
            {
                Console.WriteLine($"新用户\t{user.Name}\t加入到了聊天室");

                // 进行自增ID生成
                user.Id = ++_idNow;
                OnlineUsers[user.Id] = user;
                return user;
            };
            ChatRoomRemote.LogoutEvent += user =>
            {
                Console.WriteLine($"用户\t{user.Name}\t离开了聊天室");
                OnlineUsers.Remove(user.Id);
            };
            ChatRoomRemote.AddMessageEvent += message =>
            {
                Console.WriteLine($"用户\t{message.SendUser.Name}\t发送消息\t{message.Text}");
            };
            ChatRoomRemote.GetUsersEvent += () => OnlineUsers;

            // 配置信息
            var tcpProperties = new Hashtable
            {
                ["name"] = "ChatRoom",
                ["port"] = 8080
            };

            // 设置序列化等级
            var tcpClientSinkProvider = new BinaryClientFormatterSinkProvider();
            var tcpServerSinkProvider = new BinaryServerFormatterSinkProvider
            {
                TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full
            };

            // 注册信道
            var channel = new TcpChannel(tcpProperties,tcpClientSinkProvider,tcpServerSinkProvider);
            ChannelServices.RegisterChannel(channel,false);

            // 注册远程对象
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(ChatRoomRemote),
                "ChatRoom", WellKnownObjectMode.Singleton);

            Console.WriteLine("服务器已启动，输入任意键关闭");
            Console.ReadLine();

            // 注销信道
            ChannelServices.UnregisterChannel(channel);
        }
    }
}
