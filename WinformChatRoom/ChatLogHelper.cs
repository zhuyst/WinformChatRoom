using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChatRoom.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        private static readonly ChatMessageCoverter Coverter = new ChatMessageCoverter();

        /// <summary>
        /// 保存聊天记录
        /// </summary>
        /// <param name="messages">聊天记录列表</param>
        public static void SaveLog(List<ChatMessage> messages)
        {
            var list = messages.Select(s => new ChatMessageJsonObject(s)).ToList();
            var json = JsonConvert.SerializeObject(list);
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
            var list = JsonConvert.DeserializeObject<List<ChatMessageJsonObject>>(json,Coverter);

            return list.Select(s => s.Value).ToList();
        }

        private class ChatMessageCoverter : JsonConverter<ChatMessageJsonObject>
        {
            public override void WriteJson(JsonWriter writer, ChatMessageJsonObject value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override ChatMessageJsonObject ReadJson(JsonReader reader, Type objectType, ChatMessageJsonObject existingValue, bool hasExistingValue,
                JsonSerializer serializer)
            {
                var jsonObject = JObject.Load(reader);
                var typeName = jsonObject["TypeName"].ToString();

                var type = Type.GetType(typeName);
                var target = (ChatMessage)Activator.CreateInstance(type);

                serializer.Populate(jsonObject["Value"].CreateReader(),target);
                return new ChatMessageJsonObject(target);
            }
        }

        private class ChatMessageJsonObject
        {
            public string TypeName { get; }

            public ChatMessage Value { get; }

            public ChatMessageJsonObject(ChatMessage message)
            {
                Value = message;
                TypeName = Value.GetType().AssemblyQualifiedName;
            }
        }
    }
}
