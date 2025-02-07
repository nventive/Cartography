using System;
using System.Threading.Tasks;
using Sample.Presentation;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;
using MallardMessageHandlers;

namespace Sample.Presentation;

public partial class WelcomePageViewModel : ViewModel
{
	public IDynamicCommand NavigateToOnboarding => this.GetCommandFromTask(async ct =>
	{
		await this.GetService<ISectionsNavigator>().Navigate(ct, () => new OnboardingPageViewModel());
	});
}
