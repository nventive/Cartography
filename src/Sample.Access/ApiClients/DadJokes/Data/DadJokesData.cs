using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Sample.DataAccess;

public partial class DadJokesData
{
	public DadJokesData(DadJokeChildData[] children)
	{
		Children = children;
	}

	[JsonPropertyName("children")]
	public DadJokeChildData[] Children { get; }
}
