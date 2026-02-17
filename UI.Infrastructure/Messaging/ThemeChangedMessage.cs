using CommunityToolkit.Mvvm.Messaging.Messages;

namespace UI.Infrastructure.Messaging;

public sealed class ThemeChangedMessage(Theme value) : ValueChangedMessage<Theme>(value);