using CommunityToolkit.Mvvm.Messaging.Messages;

namespace UI.Infrastructure.Messaging;

public sealed class SwitchViewMessage(object value) : ValueChangedMessage<object>(value);