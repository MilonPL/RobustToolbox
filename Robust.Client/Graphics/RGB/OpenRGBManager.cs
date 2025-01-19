using System;
using System.Threading.Tasks;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Configuration;
using OpenRGB.NET;
using Robust.Shared;
using Color = OpenRGB.NET.Color;

namespace Robust.Client.Graphics.RGB;

internal sealed class OpenRGBManager : IClientRGBManager, IDisposable
{
    [Dependency] private readonly ILogManager _logManager = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;

    private ISawmill _sawmill = default!;
    private OpenRgbClient? _client;
    private Device[]? _devices;
    private bool _initialized;

    public bool IsAvailable => _initialized && _client != null;

    public void Initialize()
    {
        _sawmill = _logManager.GetSawmill("rgb.openrgb");

        if (!_cfg.GetCVar(CVars.RgbEnabled))
        {
            _sawmill.Info("RGB support disabled by configuration");
            return;
        }

        try
        {
            var host = _cfg.GetCVar(CVars.RgbHost);
            var port = _cfg.GetCVar(CVars.RgbPort);

            _client = new OpenRgbClient(name: "Space Station 14", port: port, ip: host);
            _client.Connect();
            _devices = _client.GetAllControllerData();

            _initialized = true;
            _sawmill.Info($"Successfully connected to OpenRGB at {host}:{port}");

            foreach (var device in _devices)
                _sawmill.Info($"Found RGB device: {device.Name}");
        }
        catch (Exception e)
        {
            _sawmill.Warning($"Failed to connect to OpenRGB: {e.Message}");
            _initialized = false;
        }
    }

    public Task SetDeviceColor(string deviceName, Shared.Maths.Color color)
    {
        if (!IsAvailable || _client == null || _devices == null)
            return Task.CompletedTask;

        try
        {
            for (var i = 0; i < _devices.Length; i++)
            {
                var device = _devices[i];
                if (device.Name != deviceName || device.Leds.Length == 0)
                    continue;

                var colors = new Color[device.Leds.Length];
                var rgbColor = new Color(color.RByte, color.GByte, color.BByte);
                for (var j = 0; j < colors.Length; j++)
                {
                    colors[j] = rgbColor;
                }

                _client.UpdateLeds(i, colors);
                break;
            }
        }
        catch (Exception e)
        {
            _sawmill.Error($"Failed to set device color: {e.Message}");
        }

        return Task.CompletedTask;
    }

    public Task SetAllDevicesColor(Shared.Maths.Color color)
    {
        if (!IsAvailable || _client == null || _devices == null)
            return Task.CompletedTask;

        try
        {
            for (var i = 0; i < _devices.Length; i++)
            {
                var device = _devices[i];
                if (device.Leds.Length == 0)
                    continue;

                var colors = new Color[device.Leds.Length];
                var rgbColor = new Color(color.RByte, color.GByte, color.BByte);
                for (var j = 0; j < colors.Length; j++)
                {
                    colors[j] = rgbColor;
                }

                _client.UpdateLeds(i, colors);
            }
        }
        catch (Exception e)
        {
            _sawmill.Error($"Failed to set all device colors: {e.Message}");
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}
