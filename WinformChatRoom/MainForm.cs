using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Windows.Forms;
using ChatRoom.Model;

namespace WinformChatRoom
{
    public partial class MainForm : Form
    {
        private readonly ChatRoomRemote _chatRoom;

        private User _user;

        public MainForm(string name)
        {
            InitializeComponent();

            _user = new User
            {
                Name = name
            };

            ChannelServices.RegisterChannel(new TcpClientChannel(), true);
            _chatRoom = (ChatRoomRemote)Activator.GetObject(typeof(ChatRoomRemote),
                "tcp://localhost:8080/ChatRoom");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _user = _chatRoom.AddUser(_user);

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
            foreach (var user in _chatRoom.Users())
            {
                UserListBox.Items.Add(user.Name);
            }
            UserListBox.EndUpdate();
        }

        private void RefreshMessageList()
        {
            MessageListBox.BeginUpdate();
            foreach (var message in _chatRoom.Messages())
            {
                MessageListBox.Items.Add(message.Text);
            }
            MessageListBox.EndUpdate();
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            var text = MessageTextBox.Text;
            var message = new ChatMessage()
            {
                SendUser = _user,
                SendTime = DateTime.Now,
                Text = text
            };
            _chatRoom.AddMessage(message);
        }
    }
}
