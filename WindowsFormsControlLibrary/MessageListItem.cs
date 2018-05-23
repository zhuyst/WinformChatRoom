using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ChatRoom.Model;

namespace WindowsFormsControlLibrary
{
    public partial class MessageListItem: UserControl
    {
        public ChatMessage Message { get; set; }

        private string _fileName;

        public MessageListItem()
        {
            InitializeComponent();
        }

        private void MessageListItem_Load(object sender, EventArgs e)
        {
            if (Message is ChatFile file)
            {
                Message.Text += " 点击打开文件";

                var cursor = Cursors.Hand;
                Cursor = cursor;
                MessageLabel.Cursor = cursor;
                TimeLabel.Cursor = cursor;

                _fileName = file.FileInfo.Name;
                ChatRoomFileHelper.SaveFile(file.FileInfo);

                Click += (o, args) => OpenFile();
                MessageLabel.Click += (o, args) => OpenFile();
                TimeLabel.Click += (o, args) => OpenFile();
            }
            else if (Message is ChatImage image)
            {
                _fileName = image.Image.Name;
                ChatRoomFileHelper.SaveFile(image.Image);

                var path = ChatRoomFileHelper.SavePath + _fileName;

                var fromImage = Image.FromFile(path);
                ImageBox.Height = fromImage.Height;
                Height += fromImage.Height;

                ImageBox.Load(path);
            }

            string text;
            switch (Message.Type)
            {
                case MessageType.All:
                    text = $@"{Message.SendUser.Name}：{Message.Text}";
                    break;
                case MessageType.From:
                    text = $@"你悄悄对 {Message.ToUser.Name} 说：{Message.Text}";
                    break;
                case MessageType.To:
                    text = $@"{Message.SendUser.Name} 悄悄对你说：{Message.Text}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            MessageLabel.Text = text;
            TimeLabel.Text = $@"—— {Message.SendTime}";
        }

        private void OpenFile()
        {
            var path = ChatRoomFileHelper.SavePath + _fileName;
            Process.Start(path);
        }
    }
}
