using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.DataAccess;

public partial class DadJokeChildData
{
	public DadJokeChildData(DadJokeContentData data)
	{
		Data = data;
	}

	public DadJokeContentData Data { get; }
}
