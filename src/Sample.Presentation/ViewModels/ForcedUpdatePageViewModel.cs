﻿using Chinook.DynamicMvvm;

namespace Sample.Presentation;

/// <summary>
/// The ViewModel for the forced update page.
/// </summary>
public sealed class ForcedUpdatePageViewModel : ViewModel
{
	/// <summary>
	/// Navigates to the App Store.
	/// </summary>
	public IDynamicCommand NavigateToStore => this.GetCommandFromTask(async ct =>
	{
		var uriProvider = this.GetService<IAppStoreUriProvider>();

		var uri = uriProvider.GetAppStoreUri();

		await this.GetService<ILauncherService>().Launch(uri);
	});
}
