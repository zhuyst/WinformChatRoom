using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Windows.Forms;
using ChatRoom.Model;

namespace WinformChatRoom
{
    public partial class MainForm : Form
    {
        private const string Url = "tcp://localhost:8080/ChatRoom";

        private readonly ChatRoomRemote _chatRoom;

        private OnLineUser _onLineUser = new OnLineUser();

        public MainForm(string name)
        {
            InitializeComponent();

            _onLineUser.User = new User
            {
                Name = name
            };

            _onLineUser.ReceivedMessageEvent += ReceivedMessage;

            var tcpProperties = new Hashtable
            {
                ["name"] = "ChatRoom",
                ["port"] = 0
            };

            var tcpClientSinkProvider = new BinaryClientFormatterSinkProvider();
            var tcpServerSinkProvider = new BinaryServerFormatterSinkProvider
            {
                TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full
            };

            var channel = new TcpChannel(tcpProperties,tcpClientSinkProvider,tcpServerSinkProvider);
            ChannelServices.RegisterChannel(channel, false);

            _chatRoom = (ChatRoomRemote)Activator.GetObject(typeof(ChatRoomRemote),Url);
            _onLineUser = _chatRoom.AddUser(_onLineUser);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            RefreshUserList();
            RefreshMessageList();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void RefreshUserList()
        {
            UserListBox.BeginUpdate();
            foreach (var user in _chatRoom.Users)
            {
                UserListBox.Items.Add(user.Name);
            }
            UserListBox.EndUpdate();
        }

        private void RefreshMessageList()
        {
            MessageListBox.BeginUpdate();
            foreach (var message in _chatRoom.Messages)
            {
                MessageListBox.Items.Add(message.Text);
            }
            MessageListBox.EndUpdate();
        }

        private void ReceivedMessage(IChatRoom chatRoom)
        {
            Action<IChatRoom> action = ReceivedMessageHandler;
            Invoke(action,chatRoom);
        }

        private void ReceivedMessageHandler(IChatRoom chatRoom)
        {
            switch (chatRoom)
            {
                case User user:
                    Debug.WriteLine(user.Name);
                    break;
                case ChatMessage message:
                    Debug.WriteLine(message.Text);
                    break;
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            var text = MessageTextBox.Text;
            var message = new ChatMessage()
            {
                SendUser = _onLineUser.User,
                SendTime = DateTime.Now,
                Text = text
            };
            _chatRoom.AddMessage(message);
        }
    }
}
