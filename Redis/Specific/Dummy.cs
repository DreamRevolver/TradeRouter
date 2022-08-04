using Communication.Redis.Models;

namespace Communication.Redis.Specific
{

	internal static class Dummy
	{
		internal static class Topic
		{
			private enum DummyEnum { Dummy }
			internal static readonly RedisTopic SUBSCRIBE_TOPIC
				= RedisTopic.Builder.Subscription.Add(DummyEnum.Dummy).Build();
			internal static readonly RedisTopic PUBLISH_TOPIC
				= RedisTopic.Builder.Publish.Add(DummyEnum.Dummy).Build();
		}
	}

}
