using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Shared.Interfaces;

using SharedBinance.interfaces;

namespace CommonExchange
{

	public sealed class WebSocketWrapper : IDataStream
	{

		private CancellationTokenSource _source;

		private const uint KEEP_ALIVE_SOCKET_INTERVAL = 3;
		private const uint KEEP_ALIVE_SECONDS = 5;

		private ClientWebSocket _webSocketClient;

		private IPeriodicTask _tickerChannel;

		private readonly string _uri;
		private readonly string _channelName;
		private readonly string _name;
		private readonly ILogger _logger;
		private readonly bool _excludeZero;

		private long _bufferSize;

		public bool IsConnected
			=> _webSocketClient?.State is WebSocketState.Open;

		public WebSocketWrapper([NotNull] string uri, [CanBeNull] string channelName, [NotNull] ILogger logger, [CanBeNull] string name, bool excludeZero = true)
		{
			_uri = uri;
			_name = name;
			_logger = logger;
			_channelName = channelName;
			_excludeZero = excludeZero;
		}

		private async Task SocketConnect([NotNull] ParseMessage parseMessage)
		{
			Debug.Assert(_logger != null, nameof(_logger) + " != null");
			Debug.Assert(_source != null, nameof(_source) + " != null");
			Debug.Assert(_webSocketClient != null, nameof(_webSocketClient) + " != null");
			Debug.Assert(_uri != null, nameof(_uri) + " != null");
			if (!IsConnected)
			{
				try
				{
					await _webSocketClient.ConnectAsync(new Uri(_uri), new CancellationToken()).ConfigureAwait(false);
				} catch (Exception)
				{
					_logger.Log(LogPriority.Debug, "Connection unavailable", _name);
				}
			}

			if (IsConnected)
			{
				var connected = new SocketEventHandlerArgs
				{
					channelName = _channelName,
					type = StreamMessageType.Logon
				};
				await (parseMessage(connected) ?? throw new Exception("SOCKET CONNECT PARSE MESSAGE DELEGATE RETURN NULL"));
				while (IsConnected && _source?.Token.IsCancellationRequested != true)
				{
					using (var stream = new MemoryStream(1024))
					{
						WebSocketReceiveResult webSocketReceiveResult;

						do
						{
							var receiveBuffer = new ArraySegment<byte>(new byte[1024 * 8]);

							try
							{
								webSocketReceiveResult = await (_webSocketClient.ReceiveAsync(receiveBuffer, _source.Token)
									?? throw new Exception("SOCKET RECEIVE MESSAGE AWAIT NULL"));
								Debug.Assert(webSocketReceiveResult != null, nameof(webSocketReceiveResult) + " != null");
								if (webSocketReceiveResult.MessageType == WebSocketMessageType.Close)
								{
									_logger.Log(LogPriority.Debug, "Websocket received close frame", _name);
									throw new Exception("Websocket received close frame");
								}
							} catch (Exception ex)
							{
								if (ex.Message == "The operation was canceled.")
								{
									_logger.Log(LogPriority.Warning, "Connection closed by client side", _name);
								} 
								else if (ex.Message == "Websocket received close frame")
								{
									_logger.Log(LogPriority.Warning, $"Connection has been closed. | Message: {ex.InnerException?.Message}", _name);
								}
								else
								{
									_logger.Log(LogPriority.Warning, $"Connection has been closed according to some problems. It will restart automatically  | Message: {ex.InnerException?.Message}", _name);
									_logger.Log(LogPriority.Debug, $"Exception in WebSocketWrapper.SocketConnect | Message: {ex.Message} | StackTrace: {ex.StackTrace} | Inner exc message: {ex.InnerException?.Message}", _name);
								}

								await (_webSocketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "", new CancellationToken())
									?? throw new Exception("SOCKET CLOSE ASYNC AWAIT NULL"));
								var disconnected = new SocketEventHandlerArgs
								{
									channelName = _channelName,
									type = StreamMessageType.Logout
								};

								try
								{
									await (parseMessage(disconnected) ?? throw new Exception("SOCKET CONNECT PARSE MESSAGE DELEGATE RETURN NULL"));
								} catch (Exception ex2)
								{
									_logger.Log(LogPriority.Error, $"Exception2 in WebSocketWrapper.SocketConnect | Message: {ex2.Message}", _name);
									_logger.Log(LogPriority.Debug, $"Exception2 in WebSocketWrapper.SocketConnect | Message: {ex2.Message} | StackTrace: {ex2.StackTrace}", _name);
								}

								return;
							}

							await (stream.WriteAsync(receiveBuffer.Array, receiveBuffer.Offset, webSocketReceiveResult.Count)
								?? throw new Exception("STREAM WRITE ASYNC AWAIT NULL"));
							_bufferSize = stream.Length;

						} while (!webSocketReceiveResult.EndOfMessage);

						var streamToMsg = _excludeZero ? stream.GetBuffer().Where(b => b != 0).ToArray() : stream.ToArray();

						var message = new SocketEventHandlerArgs
						{
							msg = streamToMsg,
							channelName = _channelName,
							type = StreamMessageType.Data
						};

						try
						{
							await (parseMessage(message) ?? throw new Exception("SOCKET CONNECT PARSE MESSAGE DELEGATE RETURN NULL"));
						} catch (Exception ex)
						{
							try
							{
								await (parseMessage(new SocketEventHandlerArgs
								{
									msg = Encoding.ASCII.GetBytes($"Exception: {ex.Message}  stack {ex.StackTrace}"),
									channelName = _channelName,
									type = StreamMessageType.Error
								}) ?? throw new Exception("SOCKET CONNECT PARSE MESSAGE DELEGATE RETURN NULL"));
							} catch (Exception ex2)
							{
								_logger.Log(LogPriority.Error, $"Exception3 in WebSocketWrapper.SocketConnect | Message: {ex2.Message}", _name);
								_logger.Log(LogPriority.Debug, $"Exception3 in WebSocketWrapper.SocketConnect | Message: {ex2.Message} | StackTrace: {ex.StackTrace}", _name);
							}
						}
					}
				}
			}
		}

		public async Task Start([NotNull] ParseMessage parseMessage)
		{
			_source = new CancellationTokenSource();

			async Task KeepChannel(object o, CancellationToken t)
			{
				// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
				switch (_webSocketClient?.State)
				{
				case WebSocketState.Open:
				case WebSocketState.Connecting:
					return;
				case null:
					break;
				default:
					_webSocketClient.Dispose();

					break;
				}

				_webSocketClient = new ClientWebSocket();
				Debug.Assert(_webSocketClient.Options != null, "_webSocketClient.Options != null");
				_webSocketClient.Options.KeepAliveInterval = new TimeSpan(KEEP_ALIVE_SOCKET_INTERVAL * TimeSpan.TicksPerSecond);
				await SocketConnect(parseMessage);
			}

			_tickerChannel = new PeriodicTaskWrapper(_logger, _name);
			await _tickerChannel.Start(KeepChannel, new PeriodicTaskParams {period = (int) KEEP_ALIVE_SECONDS * 1000}, _source.Token);
		}

		[NotNull]
		public Task Stop()
		{
			_source?.Cancel();

			return Task.CompletedTask;
		}

		public async Task SendCommand([NotNull] byte[] bytes)
		{
			if (IsConnected)
			{
				await (_webSocketClient?.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None)?.ConfigureAwait(false)
				 ?? throw new Exception("SOCKET SEND COMMAND AWAIT NULL"));
			}
		}

		[NotNull, UsedImplicitly]
		public Task<long> GetBufferSize()
			=> Task.FromResult(_bufferSize);

	}

}
