using System;
using System.IO;

namespace ChatRoom.Model
{
    public class ChatRoomFileHelper
    {
        public static readonly string SavePath =
            $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\ChatRoom\";

        static ChatRoomFileHelper()
        {
            Directory.CreateDirectory(SavePath);
        }

        public static void SaveFile(FileInfo fileInfo)
        {
            var filePath = SavePath + fileInfo.Name;
            fileInfo.CopyTo(filePath, true);
        }
    }
}
