using System;
using Prism.Events;

namespace UI.Infrastructure.Events
{
    public class InitializeViewModelEvent : PubSubEvent<Type>
    {
    }
}