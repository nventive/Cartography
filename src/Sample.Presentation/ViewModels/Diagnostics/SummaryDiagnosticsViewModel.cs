using Chinook.DynamicMvvm;

namespace Sample.Presentation;

public partial class SummaryDiagnosticsViewModel : ViewModel
{
	public string Summary => this.Get(this.GetService<IDiagnosticsService>().GetSummary);

	public IDynamicCommand SendSummary => this.GetCommandFromTask(async ct =>
	{
		await this.GetService<IDiagnosticsService>().SendSummary(ct);
	});
}
