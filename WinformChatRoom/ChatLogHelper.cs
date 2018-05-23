using System.Collections.Generic;
using System.IO;
using ChatRoom.Model;
using Newtonsoft.Json;

namespace WinformChatRoom
{
    public class ChatLogHelper
    {
        /// <summary>
        /// 聊天记录文件名
        /// </summary>
        private const string FileName = "logs.json";

        private static readonly string SavePath =
            ChatRoomFileHelper.SavePath + FileName;

        /// <summary>
        /// 保存聊天记录
        /// </summary>
        /// <param name="messages">聊天记录列表</param>
        public static void SaveLog(List<ChatMessage> messages)
        {
            var json = JsonConvert.SerializeObject(messages);
            File.WriteAllText(SavePath,json);
        }

        /// <summary>
        /// 读取聊天记录
        /// </summary>
        /// <returns>聊天记录列表</returns>
        public static List<ChatMessage> ReadLog()
        {
            if (!File.Exists(SavePath)) return new List<ChatMessage>();

            var json = File.ReadAllText(SavePath);
            return JsonConvert.DeserializeObject<List<ChatMessage>>(json);
        }
    }
}
