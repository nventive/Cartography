using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Views;
using Windows.UI.Xaml;
using System.Reactive.Subjects;
using Umbrella.Environment.Permissions;

namespace Umbrella.Location.Samples.Uno.Droid
{
	[Activity(
			MainLauncher = true,
			ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize,
			WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden
		)]
	public class MainActivity : Windows.UI.Xaml.ApplicationActivity
	{
        public static new MainActivity Current { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Current = this;
        }

        public readonly Subject<PermissionsService.PermissionsResult> PermissionResults =
            new Subject<PermissionsService.PermissionsResult>();

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionResults.OnNext(new PermissionsService.PermissionsResult(requestCode, permissions, grantResults));
        }
    }
}

