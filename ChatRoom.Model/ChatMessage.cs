using System;

namespace ChatRoom.Model
{
    [Serializable()]
    public class ChatMessage : IChatRoom
    {
        public User SendUser { get; set; }

        public DateTime SendTime { get; set; }

        public string Text { get; set; }

        public override string ToString()
        {
            return $"{SendUser.Name}：{Text}\t——{SendTime}";
        }
    }
}
