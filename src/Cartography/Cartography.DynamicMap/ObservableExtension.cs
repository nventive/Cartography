using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Nventive.Location.DynamicMap
{
	public static class ObservableExtension
	{
		/// <summary>
		/// This operator will discard all but one element (the last) per specified period.
		/// </summary>
		public static IObservable<T> MinDelaySample<T>(this IObservable<T> source, TimeSpan minDelayBetweenElements, IScheduler scheduler)
		{
			return Observable
				.Create<T>(observer =>
				{
					var nextValueTimeStamp = DateTimeOffset.MinValue;
					var serial = new SerialDisposable();

					return source
						.Finally(serial.Dispose)
						.Subscribe(
							value =>
							{
								serial.Disposable = scheduler.Schedule(nextValueTimeStamp, () =>
								{
									nextValueTimeStamp = scheduler.Now + minDelayBetweenElements;
									observer.OnNext(value);
								});
							},
							observer.OnError,
							observer.OnCompleted);
				});
		}
	}
}
