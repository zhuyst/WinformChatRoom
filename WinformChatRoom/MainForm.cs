using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        /// <summary>
        /// 服务端的远程对象
        /// </summary>
        private readonly ChatRoomRemote _chatRoom;

        /// <summary>
        /// 当前客户端的远程对象
        /// </summary>
        private readonly OnLineUser _onLineUser = new OnLineUser();

        /// <summary>
        /// 当前选择的私聊用户
        /// </summary>
        private User _selectUser;

        private readonly List<ChatMessage> _messages = new List<ChatMessage>();

        public MainForm(string name)
        {
            InitializeComponent();

            // 设置远程对象
            _onLineUser.User = new User
            {
                Name = name
            };

            _onLineUser.ReceivedMessageEvent += ReceivedMessage;

            // 配置信息
            var tcpProperties = new Hashtable
            {
                ["name"] = "ChatRoom",
                ["port"] = 0
            };

            // 设置序列化等级
            var tcpClientSinkProvider = new BinaryClientFormatterSinkProvider();
            var tcpServerSinkProvider = new BinaryServerFormatterSinkProvider
            {
                TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full
            };

            // 注册信道
            _channel = new TcpChannel(tcpProperties,tcpClientSinkProvider,tcpServerSinkProvider);
            ChannelServices.RegisterChannel(_channel, false);

            // 获取服务端远程对象
            _chatRoom = (ChatRoomRemote)Activator.GetObject(typeof(ChatRoomRemote),Url);

            // 读取历史聊天记录
            var logs = ChatLogHelper.ReadLog();
            foreach (var log in logs)
            {
                ReceivedMessage(log);
            }

            // 执行登陆
            // 将客户端远程对象放入服务端的远程对象中
            _onLineUser = _chatRoom.Login(_onLineUser);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Text = $@"聊天室 - {_onLineUser.User.Name}";
            RefreshUserList();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 执行注销操作
            _chatRoom.Logout(_onLineUser);
            ChannelServices.UnregisterChannel(_channel);

            // 保存聊天记录
            ChatLogHelper.SaveLog(_messages);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// 初始化用户列表
        /// </summary>
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

        /// <summary>
        /// 服务端调用客户端的方法
        /// </summary>
        /// <param name="chatRoom"></param>
        private void ReceivedMessage(IChatRoom chatRoom)
        {
            // 服务端调用客户端时会触发InvokeRequired
            if (InvokeRequired)
            {
                Action<IChatRoom> action = ReceivedMessage;
                BeginInvoke(action, chatRoom);
            }

            // 通过BeginInvoke跳回UI线程
            else
            {
                switch (chatRoom)
                {
                    case User user:
                        AddUser(user);
                        break;

                    case SystemMessage systemMessage:

                        // RemoveUserId不为0，则有用户执行了登出
                        if (systemMessage.RemoveUserId != 0)
                        {
                            RemoveUser(systemMessage.RemoveUserId);
                        }
                        else
                        {
                            AddMessage(new ChatMessage()
                            {
                                SendUser = User.SystemUser,
                                SendTime = systemMessage.SengTime,
                                Text = systemMessage.Text
                            });
                        }
                        break;

                    case ChatMessage message:
                        _messages.Add(message);
                        AddMessage(message);
                        break;
                }
            }
        }
    
        /// <summary>
        /// 往UserListView新增用户
        /// </summary>
        /// <param name="user"></param>
        private void AddUser(User user)
        {
            var item = new ListViewItem
            {
                Text = user.Id.ToString(),
                Name = user.Id.ToString()
            };
            item.SubItems.Add(user.Name);
            UserListView.Items.Add(item);
        }

        /// <summary>
        /// 去除UserListView中的用户
        /// </summary>
        /// <param name="userId"></param>
        private void RemoveUser(int userId)
        {
            UserListView.Items.RemoveByKey(userId.ToString());
        }

        /// <summary>
        /// 新增聊天信息
        /// </summary>
        /// <param name="message"></param>
        private void AddMessage(ChatMessage message)
        {
            var item = new MessageListItem()
            {
                Message = message
            };

            MessageListPanel.Controls.Add(item);
            MessageListPanel.VerticalScroll.Value = MessageListPanel.VerticalScroll.Maximum;
        }

        /// <summary>
        /// 获取当前私聊用户ID
        /// </summary>
        /// <returns></returns>
        private int GetSelectUserId()
        {
            return UserListView.SelectedIndices.Count > 0 ?
                int.Parse(UserListView.Items[UserListView.SelectedIndices[0]].Text) : 0;
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            // 判断是否对自己私聊
            if (_selectUser != null && _selectUser.Id == _onLineUser.User.Id)
            {
                MessageBox.Show(@"你不能对自己发起私聊！");
                return;
            }

            // 收集、发送信息
            var text = MessageTextBox.Text;
            var message = new ChatMessage()
            {
                SendUser = _onLineUser.User,
                SendTime = DateTime.Now,
                Text = text,
                ToUser = _selectUser
            };
            _chatRoom.AddMessage(message);

            // 清空TextBox
            MessageTextBox.Text = string.Empty;
        }

        private void UserListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 选择/变更私聊用户
            var selectUserId = GetSelectUserId();
            _selectUser = selectUserId != 0 ? _chatRoom.Users[selectUserId] : null;
        }

        private void FileButton_Click(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog() { Multiselect = false };
            if (fileDialog.ShowDialog() != DialogResult.OK) return;

            var fileInfo = new FileInfo(fileDialog.FileName);
            var chatFile = new ChatFile()
            {
                SendUser = _onLineUser.User,
                ToUser = _selectUser,
                SendTime = DateTime.Now,
                Text = fileInfo.Name,
                FileInfo = fileInfo
            };
            _chatRoom.AddMessage(chatFile);
        }

        private void ImageButton_Click(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog()
            {
                Multiselect = false,
                Filter = @"图片|*.jpg;*.png"
            };
            if (fileDialog.ShowDialog() != DialogResult.OK) return;

            var fileInfo = new FileInfo(fileDialog.FileName);
            var chatImage = new ChatImage()
            {
                SendUser = _onLineUser.User,
                ToUser = _selectUser,
                SendTime = DateTime.Now,
                Image = fileInfo
            };
            _chatRoom.AddMessage(chatImage);
        }
    }
}
