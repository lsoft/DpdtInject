namespace DpdtInject.Injector.Src.Bind.Settings.Constructor
{
    public abstract class SubsetNoOrderConstructorSetting : ConstructorSetting
    {
        public override ConstructorSettingsEnum CheckMode => ConstructorSettingsEnum.SubsetNoOrder;
    }

    public class SubsetNoOrderConstructorSetting<T1> : SubsetNoOrderConstructorSetting
    {
        public SubsetNoOrderConstructorSetting()
        {
            AddRange(
                typeof(T1)
                );
        }
    }

    public class SubsetNoOrderConstructorSetting
        <T1, T2> : SubsetNoOrderConstructorSetting
    {
        public SubsetNoOrderConstructorSetting()
        {
            AddRange(
                typeof(T1),
                typeof(T2)
                );
        }
    }

    public class SubsetNoOrderConstructorSetting<T1, T2, T3> : SubsetNoOrderConstructorSetting
    {
        public SubsetNoOrderConstructorSetting()
        {
            AddRange(
                typeof(T1),
                typeof(T2),
                typeof(T3)
                );
        }
    }

    public class SubsetNoOrderConstructorSetting<T1, T2, T3, T4> : SubsetNoOrderConstructorSetting
    {
        public SubsetNoOrderConstructorSetting()
        {
            AddRange(
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4)
                );
        }
    }

    public class SubsetNoOrderConstructorSetting<T1, T2, T3, T4, T5> : SubsetNoOrderConstructorSetting
    {
        public SubsetNoOrderConstructorSetting()
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

    public class SubsetNoOrderConstructorSetting<T1, T2, T3, T4, T5, T6> : SubsetNoOrderConstructorSetting
    {
        public SubsetNoOrderConstructorSetting()
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

    public class SubsetNoOrderConstructorSetting<T1, T2, T3, T4, T5, T6, T7> : SubsetNoOrderConstructorSetting
    {
        public SubsetNoOrderConstructorSetting()
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

    public class SubsetNoOrderConstructorSetting<T1, T2, T3, T4, T5, T6, T7, T8> : SubsetNoOrderConstructorSetting
    {
        public SubsetNoOrderConstructorSetting()
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
