using Robust.Shared.Maths;
using System.Threading.Tasks;

namespace Robust.Client.Graphics.RGB;

/// <summary>
/// Interface for managing RGB peripheral devices.
/// </summary>
public interface IClientRGBManager
{
    /// <summary>
    /// Initialize the RGB manager and attempt to connect to the RGB control service.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Set the color of a specific RGB device.
    /// </summary>
    Task SetDeviceColor(string deviceName, Color color);

    /// <summary>
    /// Set the color of all connected RGB devices.
    /// </summary>
    Task SetAllDevicesColor(Color color);

    /// <summary>
    /// Whether the RGB manager is connected and available.
    /// </summary>
    bool IsAvailable { get; }
}
