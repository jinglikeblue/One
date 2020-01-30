using Chat;
using Google.Protobuf;

namespace OneClient.Sources.Tests
{
    /// <summary>
    /// 测试Protobuf
    /// </summary>
    class ProtobufTest
    {
        public ProtobufTest()
        {
            req_chat msg = new req_chat()
            {
                Content = "hello",
                Channel = chat_channel.ChatTypeRoom,
                Target = 1
            };

            Dumper.Dump(msg);

            byte[] bytes = msg.ToByteArray();

            var obj = req_chat.Parser.ParseFrom(bytes);
            Dumper.Dump(obj);
        }
    }
}
