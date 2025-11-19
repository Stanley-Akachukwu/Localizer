namespace Localizer.Mobile.Services.Device;

public interface IDeviceService
{
    void SetScreenBrightness(float brightness);
    void SetStatusBarColor(Color color, bool isLight);
}