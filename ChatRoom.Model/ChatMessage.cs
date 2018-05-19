using System;

namespace ChatRoom.Model
{
    [Serializable()]
    public class ChatMessage : IChatRoom
    {
        public User SendUser { get; set; }

        public User ToUser { get; set; }

        public DateTime SendTime { get; set; }

        public string Text { get; set; }

        public MessageType Type { get; set; } = MessageType.All;

        public ChatMessage Clone()
        {
            return MemberwiseClone() as ChatMessage;
        }
    }

    public enum MessageType
    {
        From,To,All
    }
}
