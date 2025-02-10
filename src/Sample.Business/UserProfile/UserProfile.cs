﻿using System;
using System.Collections.Generic;
using System.Text;
using Sample.DataAccess;

namespace Sample.Business;

public record UserProfile
{
	public string Id { get; init; }

	public string FirstName { get; init; }

	public string LastName { get; init; }

	public string Email { get; init; }

	public static UserProfile FromData(UserProfileData data)
	{
		if (data is null)
		{
			return null;
		}

		return new UserProfile
		{
			Id = data.Id,
			FirstName = data.FirstName,
			LastName = data.LastName,
			Email = data.Email
		};
	}

	public UserProfileData ToData()
	{
		return new UserProfileData(Id, FirstName, LastName, Email);
	}
}
