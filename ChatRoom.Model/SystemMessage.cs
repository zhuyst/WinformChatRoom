using System;

namespace ChatRoom.Model
{
    [Serializable()]
    public class SystemMessage : IChatRoom
    {
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SengTime { get; set; }

        /// <summary>
        /// 系统信息
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 登出用户的ID
        /// </summary>
        public int RemoveUserId { get; set; }
    }
}
