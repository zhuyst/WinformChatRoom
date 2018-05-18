using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Windows.Forms;
using WindowsFormsControlLibrary;
using ChatRoom.Model;

namespace WinformChatRoom
{
    public partial class MainForm : Form
    {
        private const string Url = "tcp://localhost:8080/ChatRoom";

        private readonly TcpChannel _channel;

        private readonly ChatRoomRemote _chatRoom;

        private readonly OnLineUser _onLineUser = new OnLineUser();

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

            _channel = new TcpChannel(tcpProperties,tcpClientSinkProvider,tcpServerSinkProvider);
            ChannelServices.RegisterChannel(_channel, false);

            _chatRoom = (ChatRoomRemote)Activator.GetObject(typeof(ChatRoomRemote),Url);
            _onLineUser = _chatRoom.Login(_onLineUser);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            RefreshUserList();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _chatRoom.Logout(_onLineUser);
            ChannelServices.UnregisterChannel(_channel);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void RefreshUserList()
        {
            UserListBox.BeginUpdate();

            UserListBox.Items.Clear();
            foreach (var user in _chatRoom.Users)
            {
                UserListBox.Items.Add(user.Name);
            }

            UserListBox.EndUpdate();
        }

        private void ReceivedMessage(IChatRoom chatRoom)
        {
            if (InvokeRequired)
            {
                Action<IChatRoom> action = ReceivedMessage;
                BeginInvoke(action, chatRoom);
            }
            else
            {
                switch (chatRoom)
                {
                    case User user:
                        UserListBox.Items.Add(user.Name);
                        break;
                    case ChatMessage message:
                        var item = new MessageListItem()
                        {
                            Message = message
                        };
                        MessageListPanel.Controls.Add(item);
                        MessageListPanel.VerticalScroll.Value = MessageListPanel.VerticalScroll.Maximum;
                        break;
                }
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
            MessageTextBox.Text = string.Empty;
        }
    }
}
