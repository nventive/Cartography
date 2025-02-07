using Chinook.DynamicMvvm;

namespace Sample.Presentation;

public class ShellViewModel : ViewModel
{
	public DiagnosticsOverlayViewModel DiagnosticsOverlay => this.GetChild<DiagnosticsOverlayViewModel>();

	public MenuViewModel Menu => this.GetChild<MenuViewModel>();
}
