using System;

namespace ChatRoom.Model
{
    [Serializable()]
    public class ChatMessage
    {
        public User SendUser { get; set; }

        public DateTime SendTime { get; set; }

        public string Text { get; set; }
    }
}
