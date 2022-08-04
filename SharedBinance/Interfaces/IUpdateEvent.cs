using System;

using Shared.Models;

namespace SharedBinance.Interfaces
{

	public interface IUpdateEvent : IComparable<IUpdateEvent>
	{

		string Identifier { get; }
		long Time { get; }
		UpdateEventState State { get; set; }


	}

	public static class UpdateEventExtension
	{
		public static T WithState<T>(this T @event, UpdateEventState state) where T : IUpdateEvent
		{
			@event.State = state;
			return @event;
		}
		public static bool CheckTime<A, B>(this A old, B @new) where A : IUpdateEvent where B : IUpdateEvent
			=> old.Time < @new.Time;

		public static IUpdateEvent Toggle(this IUpdateEvent self, IUpdateEvent other)
		{
			if (self.Time == other.Time)
			{
				if (self.State == UpdateEventState.OPEN && other.State == UpdateEventState.CLOSED)
				{
					other.State = UpdateEventState.CHECKED;
				}
				return other;
			}
			if (!self.CheckTime(other))
			{
				(self, other) = (other, self);
			}
			return (self.State, other.State, self.CheckTime(other)) switch
			{
				(UpdateEventState.CLOSED, UpdateEventState.OPEN, true) => other,
				(UpdateEventState.CLOSED, UpdateEventState.CHECKED, true) => other,
				(UpdateEventState.CLOSED, UpdateEventState.CLOSED, true) => other,
				(UpdateEventState.OPEN, UpdateEventState.CLOSED, true) => other.WithState(UpdateEventState.CHECKED),
				(UpdateEventState.OPEN, UpdateEventState.CHECKED, true) => other,
				(UpdateEventState.OPEN, UpdateEventState.OPEN, true) => other,
				(UpdateEventState.CHECKED, UpdateEventState.OPEN, true) => other,
				(UpdateEventState.CHECKED, UpdateEventState.CLOSED, true) => other,
				_ => throw new ArgumentOutOfRangeException()
			};
		}

	}
}
