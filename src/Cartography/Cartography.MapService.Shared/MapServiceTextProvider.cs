#if __IOS__
namespace Cartography.MapService
{
	/// <summary>
	/// This class aggregates alert parameters.
	/// </summary>
	public class MapServiceTextProvider
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MapServiceTextProvider"/> class.
		/// </summary>
		/// <param name="cancelTitle">Text displayed on the cancel button</param>
		/// <param name="title">Title displayed on the app chooser</param>
		/// <param name="description">Description displayed on the app chooser</param>
		public MapServiceTextProvider(string cancelTitle, string title = null, string description = null)
		{
			CancelTitle = cancelTitle;
			Title = title;
			Description = description;
		}

		/// <summary>
		/// Gets the alert cancel button text.
		/// </summary>
		public string CancelTitle { get; }

		/// <summary>
		/// Gets the alert cancel button text.
		/// </summary>
		public string Title { get; }

		/// <summary>
		/// Gets the alert cancel button text.
		/// </summary>
		public string Description { get; }
	}
}
#endif
