using System;
using System.Collections;
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

        private User _selectUser;

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
            Text = $@"聊天室 - {_onLineUser.User.Name}";
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
            UserListView.BeginUpdate();

            UserListView.Items.Clear();
            foreach (var user in _chatRoom.Users.Values)
            {
                AddUser(user);
            }

            UserListView.EndUpdate();
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
                        AddUser(user);
                        break;
                    case ChatMessage message:
                        AddMessage(message);
                        break;
                }
            }
        }

        private void AddUser(User user)
        {
            var item = new ListViewItem { Text = user.Id.ToString() };
            item.SubItems.Add(user.Name);
            UserListView.Items.Add(item);
        }

        private void AddMessage(ChatMessage message)
        {
            var item = new MessageListItem()
            {
                Message = message
            };

            MessageListPanel.Controls.Add(item);
            MessageListPanel.VerticalScroll.Value = MessageListPanel.VerticalScroll.Maximum;
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (_selectUser != null && _selectUser.Id == _onLineUser.User.Id)
            {
                MessageBox.Show(@"你不能对自己发起私聊！");
                return;
            }

            var text = MessageTextBox.Text;
            var message = new ChatMessage()
            {
                SendUser = _onLineUser.User,
                SendTime = DateTime.Now,
                Text = text,
                ToUser = _selectUser
            };
            _chatRoom.AddMessage(message);
            MessageTextBox.Text = string.Empty;
        }

        private void UserListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectUserId = UserListView.SelectedIndices.Count > 0 ?
                int.Parse(UserListView.Items[UserListView.SelectedIndices[0]].Text) : 0;
            _selectUser = selectUserId != 0 ? _chatRoom.Users[selectUserId] : null;
        }
    }
}
