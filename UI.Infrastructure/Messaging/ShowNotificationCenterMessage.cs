using CommunityToolkit.Mvvm.Messaging.Messages;

namespace UI.Infrastructure.Messaging;

public sealed class ShowNotificationCenterMessage(bool value) : ValueChangedMessage<bool>(value);