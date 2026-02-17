using CommunityToolkit.Mvvm.Messaging.Messages;

namespace UI.Infrastructure.Messaging;

public sealed class ShowBusyIndicatorWithProgressMessage(bool value) : ValueChangedMessage<bool>(value)
{
}