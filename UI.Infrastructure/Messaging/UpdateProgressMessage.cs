using CommunityToolkit.Mvvm.Messaging.Messages;

namespace UI.Infrastructure.Messaging;

public sealed class UpdateProgressMessage(int value) : ValueChangedMessage<int>(value);