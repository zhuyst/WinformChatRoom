using System;
using System.IO;

namespace ChatRoom.Model
{
    [Serializable()]
    public class ChatFile : ChatMessage
    {
        public FileInfo FileInfo { get; set; }
    }
}
