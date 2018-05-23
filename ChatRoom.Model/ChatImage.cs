using System;
using System.IO;

namespace ChatRoom.Model
{
    [Serializable()]
    public class ChatImage : ChatMessage
    {
        /// <summary>
        /// 图片文件信息
        /// </summary>
        public FileInfo Image { get; set; }
    }
}
