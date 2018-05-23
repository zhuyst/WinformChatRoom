using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ChatRoom.Model;

namespace WindowsFormsControlLibrary
{
    public partial class MessageListItem: UserControl
    {
        public ChatMessage Message { get; set; }

        private string _filePath;

        private static readonly string SavePath =
            $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\ChatRoom\";

        public MessageListItem()
        {
            InitializeComponent();
            Directory.CreateDirectory(SavePath);
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

                var fileInfo = file.FileInfo;
                _filePath = SavePath + fileInfo.Name;

                file.FileInfo.CopyTo(_filePath,true);
                Click += (o, args) => OpenFile();
                MessageLabel.Click += (o, args) => OpenFile();
                TimeLabel.Click += (o, args) => OpenFile();
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
            Process.Start(_filePath);
        }
    }
}
