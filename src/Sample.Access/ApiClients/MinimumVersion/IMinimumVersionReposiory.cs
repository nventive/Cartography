﻿using System;

namespace Sample.DataAccess;

/// <summary>
/// Gets the minimum version required to use the application.
/// </summary>
public interface IMinimumVersionReposiory
{
	/// <summary>
	/// Checks the minimum required version.
	/// </summary>
	void CheckMinimumVersion();

	/// <summary>
	/// An observable that emits the minimum version required to use the application.
	/// </summary>
	IObservable<Version> MinimumVersionObservable { get; }
}
