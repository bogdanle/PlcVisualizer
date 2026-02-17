using CommunityToolkit.Mvvm.Messaging.Messages;

namespace UI.Infrastructure.Messaging;

public sealed class ShowBusyIndicatorMessage(bool value) : ValueChangedMessage<bool>(value)
{
}