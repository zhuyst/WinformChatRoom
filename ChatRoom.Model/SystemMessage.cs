using System;

namespace ChatRoom.Model
{
    [Serializable()]
    public class SystemMessage : IChatRoom
    {
        public DateTime SengTime { get; set; }

        public string Text { get; set; }

        public int RemoveUserId { get; set; }
    }
}
