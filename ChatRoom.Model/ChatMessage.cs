using System;

namespace ChatRoom.Model
{
    [Serializable()]
    public class ChatMessage : IChatRoom
    {
        /// <summary>
        /// 信息发送用户
        /// </summary>
        public User SendUser { get; set; }

        /// <summary>
        /// 信息接收用户
        /// 只有要私聊时才使用该属性
        /// </summary>
        public User ToUser { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendTime { get; set; }

        /// <summary>
        /// 聊天信息
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 信息类型
        /// </summary>
        public MessageType Type { get; set; } = MessageType.All;

        public ChatMessage Clone()
        {
            return MemberwiseClone() as ChatMessage;
        }
    }

    public enum MessageType
    {
        /// <summary>
        /// 私聊 —— 发送者
        /// </summary>
        From,

        /// <summary>
        /// 私聊 —— 接受者
        /// </summary>
        To,

        /// <summary>
        /// 群聊
        /// </summary>
        All
    }
}
