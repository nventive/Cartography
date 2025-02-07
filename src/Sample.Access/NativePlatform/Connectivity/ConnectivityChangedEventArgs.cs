using System;

namespace Sample.DataAccess;

public sealed class ConnectivityChangedEventArgs : EventArgs
{
	public ConnectivityChangedEventArgs(ConnectivityState state)
	{
		State = state;
	}

	public ConnectivityState State { get; }
}
