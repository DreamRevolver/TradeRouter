using System;

using JetBrains.Annotations;

// ReSharper disable CheckNamespace

namespace TelegramBotAPI.Attributes
{

	[AttributeUsage(AttributeTargets.Class), PublicAPI]
	public sealed class ApiTargetAttribute : Attribute
	{
		public string Target { get; }
		public ApiTargetAttribute([NotNull] string target) => Target = target;
	}

	[AttributeUsage(AttributeTargets.Class), PublicAPI]
	public class JsonEncodedAttribute : Attribute { }

}
