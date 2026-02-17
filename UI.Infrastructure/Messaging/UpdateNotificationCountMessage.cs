using CommunityToolkit.Mvvm.Messaging.Messages;

namespace UI.Infrastructure.Messaging;

public sealed class UpdateNotificationCountMessage(int value) : ValueChangedMessage<int>(value);