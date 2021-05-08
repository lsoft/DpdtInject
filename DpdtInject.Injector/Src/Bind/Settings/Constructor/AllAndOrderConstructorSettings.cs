namespace DpdtInject.Injector.Src.Bind.Settings.Constructor
{
    public abstract class AllAndOrderConstructorSetting : ConstructorSetting
    {
        public override ConstructorSettingsEnum CheckMode => ConstructorSettingsEnum.AllAndOrder;
    }

    public class AllAndOrderConstructorSetting<T1> : AllAndOrderConstructorSetting
    {
        public AllAndOrderConstructorSetting()
        {
            AddRange(
                typeof(T1)
                );
        }
    }

    public class AllAndOrderConstructorSetting
        <T1, T2> : AllAndOrderConstructorSetting
    {
        public AllAndOrderConstructorSetting()
        {
            AddRange(
                typeof(T1),
                typeof(T2)
                );
        }
    }

    public class AllAndOrderConstructorSetting<T1, T2, T3> : AllAndOrderConstructorSetting
    {
        public AllAndOrderConstructorSetting()
        {
            AddRange(
                typeof(T1),
                typeof(T2),
                typeof(T3)
                );
        }
    }

    public class AllAndOrderConstructorSetting<T1, T2, T3, T4> : AllAndOrderConstructorSetting
    {
        public AllAndOrderConstructorSetting()
        {
            AddRange(
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4)
                );
        }
    }

    public class AllAndOrderConstructorSetting<T1, T2, T3, T4, T5> : AllAndOrderConstructorSetting
    {
        public AllAndOrderConstructorSetting()
        {
            AddRange(
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5)
                );
        }
    }

    public class AllAndOrderConstructorSetting<T1, T2, T3, T4, T5, T6> : AllAndOrderConstructorSetting
    {
        public AllAndOrderConstructorSetting()
        {
            AddRange(
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6)
                );
        }
    }

    public class AllAndOrderConstructorSetting<T1, T2, T3, T4, T5, T6, T7> : AllAndOrderConstructorSetting
    {
        public AllAndOrderConstructorSetting()
        {
            AddRange(
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7)
                );
        }
    }

    public class AllAndOrderConstructorSetting<T1, T2, T3, T4, T5, T6, T7, T8> : AllAndOrderConstructorSetting
    {
        public AllAndOrderConstructorSetting()
        {
            AddRange(
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7),
                typeof(T8)
                );
        }
    }
}
