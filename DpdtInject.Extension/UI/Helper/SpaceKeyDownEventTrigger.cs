using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
