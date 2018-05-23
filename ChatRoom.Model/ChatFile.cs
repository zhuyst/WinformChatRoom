using System;
using System.IO;

namespace ChatRoom.Model
{
    [Serializable()]
    public class ChatFile : ChatMessage
    {
        /// <summary>
        /// 文件信息
        /// </summary>
        public FileInfo FileInfo { get; set; }
    }
}
