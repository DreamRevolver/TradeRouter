using TelegramBotAPI.Attributes;

// ReSharper disable NotAccessedField.Global
// ReSharper disable CheckNamespace
// ReSharper disable UnassignedReadonlyField
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBeInternal
// ReSharper disable sealed classNeverInstantiated.Global
// ReSharper disable CheckNamespace

namespace TelegramBotAPI.Requests
{

    [ApiTarget("logOut")]
    public sealed class LogOutRequest { }

    [ApiTarget("getMe")]
    public sealed class GetMeRequest { }

    [ApiTarget("getUpdates"), JsonEncoded]
    public sealed class GetUpdatesRequest
    {
        public long? offset;
        public long? limit;
        public long? timeout;
        public string[] allowed_updates;
    }

    [ApiTarget("getChat"), JsonEncoded]
    public sealed class GetChatByStringRequest
    {
        public string chat_id;
    }

    [ApiTarget("getChat"), JsonEncoded]
    public sealed class GetChatByIntRequest
    {
        public long? chat_id;
    }

    [ApiTarget("sendMessage"), JsonEncoded]
    public sealed class SendMessageByIntRequest
    {
        public long? chat_id;
        public string text;
        public string parse_mode = "Markdown";
    }

}
