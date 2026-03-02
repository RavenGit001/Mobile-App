using Microsoft.AspNetCore.SignalR.Client;
using Thesis.Mobile.Models;

namespace Thesis.Mobile.Services;

public class SignalRService
{
    private static SignalRService? _instance;
    public static SignalRService Instance => _instance ??= new SignalRService();

    private HubConnection? _hubConnection;
    public event Action<DetectionLog>? OnMoldDetected;

    private SignalRService() { }

    public async Task ConnectAsync(string hubUrl)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<DetectionLog>("ReceiveAlert", (log) =>
        {
            OnMoldDetected?.Invoke(log);
        });

        await _hubConnection.StartAsync();
    }

    public async Task DisconnectAsync()
    {
        if (_hubConnection != null)
            await _hubConnection.StopAsync();
    }
}