using System.Threading.Tasks;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;

namespace Robust.Client.Graphics.RGB;

/// <summary>
/// Core RGB system that provides an API for content to control RGB peripherals.
/// </summary>
public sealed class ClientRGBSystem : EntitySystem
{
    [Dependency] private readonly IClientRGBManager _rgbManager = default!;
    [Dependency] private readonly ILogManager _logManager = default!;

    private ISawmill _sawmill = default!;

    public override void Initialize()
    {
        base.Initialize();
        _sawmill = _logManager.GetSawmill("rgb.system");
    }

    /// <summary>
    /// Sets color for all available RGB devices.
    /// </summary>
    public async Task SetColor(Color color)
    {
        if (!_rgbManager.IsAvailable)
            return;

        await _rgbManager.SetAllDevicesColor(color);
    }

    /// <summary>
    /// Sets color for a specific RGB device by name.
    /// </summary>
    public async Task SetDeviceColor(string deviceName, Color color)
    {
        if (!_rgbManager.IsAvailable)
            return;

        await _rgbManager.SetDeviceColor(deviceName, color);
    }

    /// <summary>
    /// Whether RGB peripheral control is available.
    /// </summary>
    public bool IsAvailable => _rgbManager.IsAvailable;
}
