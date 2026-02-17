using CommunityToolkit.Mvvm.Messaging.Messages;

namespace UI.Infrastructure.Messaging;

public sealed class PopupActiveMessage(bool value) : ValueChangedMessage<bool>(value)
{
}