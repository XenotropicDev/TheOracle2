using Discord.WebSocket;

namespace TheOracle2
{
    internal interface IButtonAction
    {
        bool CanHandleButton(string buttonId);
        Task HandleButton(SocketMessageComponent component);
    }
}