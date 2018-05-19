using System;

namespace ChatRoom.Model
{
    [Serializable()]
    public class User : IChatRoom
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public static User SystemUser = new User()
        {
            Name = "系统"
        };
    }
}
