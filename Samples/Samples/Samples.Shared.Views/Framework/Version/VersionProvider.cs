using System;

namespace Samples;

public sealed partial class VersionProvider : IVersionProvider
{
	public string VersionString => Version.ToString();
}
