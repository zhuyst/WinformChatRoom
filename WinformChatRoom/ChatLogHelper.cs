using System;
using System.Collections.Generic;
using System.IO;
using ChatRoom.Model;
using Newtonsoft.Json;

namespace WinformChatRoom
{
    public class ChatLogHelper
    {
        private const string FileName = "logs.json";

        private static readonly string SavePath =
            ChatRoomFileHelper.SavePath + FileName;

        public static void SaveLog(List<ChatMessage> messages)
        {
            var json = JsonConvert.SerializeObject(messages);
            File.WriteAllText(SavePath,json);
        }

        public static List<ChatMessage> ReadLog()
        {
            if (!File.Exists(SavePath)) return new List<ChatMessage>();

            var json = File.ReadAllText(SavePath);
            return JsonConvert.DeserializeObject<List<ChatMessage>>(json);
        }
    }
}
