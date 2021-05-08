namespace DpdtInject.Injector.Src.Bind.Settings.Constructor
{
    public abstract class SubsetAndOrderConstructorSetting : ConstructorSetting
    {
        public override ConstructorSettingsEnum CheckMode => ConstructorSettingsEnum.SubsetAndOrder;
    }

    public class SubsetAndOrderConstructorSetting<T1> : SubsetAndOrderConstructorSetting
    {
        public SubsetAndOrderConstructorSetting()
        {
            AddRange(
                typeof(T1)
                );
        }
    }

    public class SubsetAndOrderConstructorSetting
        <T1, T2> : SubsetAndOrderConstructorSetting
    {
        public SubsetAndOrderConstructorSetting()
        {
            AddRange(
                typeof(T1),
                typeof(T2)
                );
        }
    }

    public class SubsetAndOrderConstructorSetting<T1, T2, T3> : SubsetAndOrderConstructorSetting
    {
        public SubsetAndOrderConstructorSetting()
        {
            AddRange(
                typeof(T1),
                typeof(T2),
                typeof(T3)
                );
        }
    }

    public class SubsetAndOrderConstructorSetting<T1, T2, T3, T4> : SubsetAndOrderConstructorSetting
    {
        public SubsetAndOrderConstructorSetting()
        {
            AddRange(
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4)
                );
        }
    }

    public class SubsetAndOrderConstructorSetting<T1, T2, T3, T4, T5> : SubsetAndOrderConstructorSetting
    {
        public SubsetAndOrderConstructorSetting()
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

    public class SubsetAndOrderConstructorSetting<T1, T2, T3, T4, T5, T6> : SubsetAndOrderConstructorSetting
    {
        public SubsetAndOrderConstructorSetting()
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

    public class SubsetAndOrderConstructorSetting<T1, T2, T3, T4, T5, T6, T7> : SubsetAndOrderConstructorSetting
    {
        public SubsetAndOrderConstructorSetting()
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

    public class SubsetAndOrderConstructorSetting<T1, T2, T3, T4, T5, T6, T7, T8> : SubsetAndOrderConstructorSetting
    {
        public SubsetAndOrderConstructorSetting()
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
