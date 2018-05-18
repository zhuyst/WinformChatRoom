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
            MessageLabel.Text = $@"{Message.SendUser.Name}：{Message.Text}";
            TimeLabel.Text = $@"—— {Message.SendTime}";
        }
    }
}
