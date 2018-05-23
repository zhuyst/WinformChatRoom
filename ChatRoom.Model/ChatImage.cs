using System;
using System.IO;

namespace ChatRoom.Model
{
    [Serializable()]
    public class ChatImage : ChatMessage
    {
        public FileInfo Image { get; set; }
    }
}
