using System;
using System.IO;

namespace ChatRoom.Model
{
    public class ChatRoomFileHelper
    {
        /// <summary>
        /// 文件保存路径
        /// </summary>
        public static readonly string SavePath =
            $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\ChatRoom\";

        static ChatRoomFileHelper()
        {
            Directory.CreateDirectory(SavePath);
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="fileInfo"></param>
        public static void SaveFile(FileInfo fileInfo)
        {
            var filePath = SavePath + fileInfo.Name;
            fileInfo.CopyTo(filePath, true);
        }
    }
}
