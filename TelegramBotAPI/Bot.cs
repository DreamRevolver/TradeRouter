using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TelegramBotAPI.Attributes;
using TelegramBotAPI.Requests;
using TelegramBotAPI.Types;
using Utf8Json;
using Utility;

#pragma warning disable 649

namespace TelegramBotAPI
{

    public class Bot
    {

        private static readonly ConcurrentDictionary<string, Bot> POOL = new ConcurrentDictionary<string, Bot>();

        private static class ApiRequest<T>
        {
            static ApiRequest()
            {
                Target = (Attribute.GetCustomAttribute(typeof(T), typeof(ApiTargetAttribute)) as ApiTargetAttribute)?.Target;
                JsonEncoded = Attribute.IsDefined(typeof(T), typeof(JsonEncodedAttribute));
            }
            public static string Target { get; }
            public static bool JsonEncoded { get; }
            public static string GetUrl(string token)
                => $"https://api.telegram.org/bot{token}/{Target}";
        }

        private class ApiResponse<T>
        {
            public bool ok;
            public T result;
        }

        private async Task<ApiResponse<R>> HttpRequest<S, R>(S json)
        {
            var request = (HttpWebRequest)WebRequest.Create(ApiRequest<S>.GetUrl(Token));
            request.Method = "POST";
            request.ContentType = "application/json";
            try
            {
                if (ApiRequest<S>.JsonEncoded)
                {
                    JsonSerializer.Serialize(request.GetRequestStream(), json);
                }
                return JsonSerializer.Deserialize<ApiResponse<R>>((await request.GetResponseAsync() as HttpWebResponse).GetResponseStream());
            }
            catch (Exception)
            {
                return null;
            }
        }

        private event Action<Update> _onUpdate = delegate { };
        public event Action<Update> OnUpdate
        {
            add
            {
                lock (_onUpdate)
                {
                    _onUpdate += value;
                }
            }
            remove
            {
                lock (_onUpdate)
                {
                    _onUpdate -= value;
                }
            }
        }
        private ManualResetEvent PollingReset { get; } = new ManualResetEvent(false);
        private CancellationTokenSource CancellationToken { get; } = new CancellationTokenSource();
        private long PollingOffset { get; set; }
        public long PollingLimit { get; } = 32;
        public long PollingTimeout { get; } = 60;
        public string Token { get; }
        private bool IsEmpty
        {
            get
            {
                lock (_onUpdate)
                {
                    return _onUpdate.GetInvocationList().Length == 1;
                }
            }
        }

        private void UpdateHandler(Update update)
        {
            PollingOffset = update.update_id is long id && id >= PollingOffset ? id + 1 : PollingOffset;
            lock (_onUpdate)
            {
                _onUpdate(update);
            }
        }

        private static readonly string[] AllowAllUpdatesArray =
        {
            "message", "edited_message", "channel_post", "edited_channel_post",
            "inline_query", "chosen_inline_result", "callback_query", "shipping_query",
            "pre_checkout_query", "poll", "poll_answer"
        };
        private async void LongPolling()
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                while (!PollingReset.WaitOne(1000))
                {
                    if (!IsEmpty)
                    {
                        PollingReset.Set();
                    }
                }
                var updates = await GetUpdates(PollingOffset, PollingLimit, PollingTimeout, AllowAllUpdatesArray);
                if (updates == null)
                {
                    await Task.Delay(1000);
                    continue;
                }
                foreach (var i in updates)
                {
                    UpdateHandler(i);
                }
            }
        }

        private Bot(string token)
        {
            Token = token;
            Task.Factory.StartLongRunning(() => LongPolling(), CancellationToken.Token);
        }

        public static Bot GetBot(string token)
            => POOL.TryGetValue(token, out var bot) ? bot
                : POOL.GetOrAdd(token, new Bot(token));

        private async Task<Update[]> GetUpdates(long offset, long limit, long timeout, string[] allowedUpdates)
        {
            var response = await HttpRequest<GetUpdatesRequest, Update[]>(new GetUpdatesRequest
            {
                offset = offset,
                limit = limit,
                timeout = timeout,
                allowed_updates = allowedUpdates
            });
            return response?.ok == true ? response.result : null;
        }
/*        public async Task<User?> GetMe()
        {
            var response = await HttpRequest<GetMeRequest, User>(new GetMeRequest { });
            return response?.ok == true ? (User?)response.result : null;
        }
        public async Task<Chat> GetChat(string chatId)
        {
            var response = await HttpRequest<GetChatByStringRequest, Chat>(new GetChatByStringRequest
            {
                chat_id = chatId
            });
            return response?.ok == true ? response.result : null;
        }
        public async Task<Chat> GetChat(long chatId)
        {
            var response = await HttpRequest<GetChatByIntRequest, Chat>(new GetChatByIntRequest
            {
                chat_id = chatId
            });
            return response?.ok == true ? response.result : null;
        }*/
        public async Task<Message> SendMessage(long chatId, string text)
        {
            var response = await HttpRequest<SendMessageByIntRequest, Message>(new SendMessageByIntRequest
            {
                chat_id = chatId,
                text = text
            });
            return response?.ok == true ? response.result : null;
        }

    }

}
