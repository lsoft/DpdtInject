using Microsoft.Xaml.Behaviors;
using System;
using System.Windows.Input;

namespace DpdtInject.Extension.UI.Helper
{

    public class SpaceKeyDownEventTrigger : EventTrigger
    {

        public SpaceKeyDownEventTrigger()
            : base("KeyUp")
        {
        }

        protected override void OnEvent(EventArgs eventArgs)
        {
            var e = eventArgs as KeyEventArgs;
            if (e != null && e.Key == Key.Space)
                InvokeActions(eventArgs);
        }
    }
}
