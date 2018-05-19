using System;

namespace ChatRoom.Model
{
    [Serializable()]
    public class User : IChatRoom
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 系统用户
        /// </summary>
        public static User SystemUser = new User()
        {
            Name = "系统"
        };
    }
}
