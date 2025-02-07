using Windows.System.Profile;

namespace Sample;

public sealed class DeviceInformationProvider : IDeviceInformationProvider
{
	public string DeviceType => AnalyticsInfo.DeviceForm;

	public string OperatingSystem => AnalyticsInfo.VersionInfo.DeviceFamily;

	public string OperatingSystemVersion => AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
}
