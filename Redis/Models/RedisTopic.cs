using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Communication.Redis.Models
{

	public readonly struct RedisTopic
	{

		public struct Builder
		{

			private readonly bool _forPublish;
			private readonly IEnumerable<string> _topics;
			private readonly RedisTopic _construct;
			private ImmutableDictionary<long, Builder> _cache;

			private Builder(bool forPublish)
			{
				_forPublish = forPublish;
				_topics = Enumerable.Empty<string>();
				_cache = ImmutableDictionary<long, Builder>.Empty;
				_construct = default;
			}

			private Builder(Builder other, string topic)
			{
				_forPublish = other._forPublish;
				_topics = other._topics.Append(topic);
				_cache = ImmutableDictionary<long, Builder>.Empty;
				_construct = new RedisTopic($"{(_forPublish ? "PUBLISH" : "PSUBSCRIBE")} {string.Join(".", _topics)} ".GetBytes());
			}

			private Builder Make<TEnum>(long label) where TEnum : Enum
				=> new Builder(
					this, _forPublish ? label.AsLabel<TEnum>().GetLabelTopic() :
					label is 0 ? "*" : label.AsLabel<TEnum>().GetLabelSubscriptionTopic()
				);

			public Builder Add<TEnum>(TEnum label) where TEnum : Enum
				=> ImmutableInterlocked.GetOrAdd(ref _cache, label.AsLong(), Make<TEnum>);

			public static Builder Publish => new Builder(true);
			public static Builder Subscription => new Builder(false);
			public RedisTopic Build() => _construct;

		}

		private readonly byte[] _payload;

		private RedisTopic(byte[] payload)
			=> _payload = payload;

		public static implicit operator ReadOnlyMemory<byte>(RedisTopic set) => set._payload;
		public override string ToString() => Encoding.UTF8.GetString(_payload);

	}

}
