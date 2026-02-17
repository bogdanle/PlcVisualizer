using CommunityToolkit.Mvvm.Messaging.Messages;
using UI.Controls;

namespace UI.Infrastructure.Messaging;

public sealed class NotificationMessageEnvelope(NotificationMessageData value) : ValueChangedMessage<NotificationMessageData>(value)
{
}
