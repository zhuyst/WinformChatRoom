using System;
using System.Windows.Forms;
using ChatRoom.Model;

namespace WindowsFormsControlLibrary
{
    public partial class MessageListItem: UserControl
    {
        public ChatMessage Message { get; set; }

        public MessageListItem()
        {
            InitializeComponent();
        }

        private void MessageListItem_Load(object sender, System.EventArgs e)
        {
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
    }
}
