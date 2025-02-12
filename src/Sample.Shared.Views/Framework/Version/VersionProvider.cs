using System;

namespace Sample;

public sealed partial class VersionProvider : IVersionProvider
{
	public string VersionString => Version.ToString();
}
