namespace DpdtInject.Injector.Bind
{
    public interface IConventionalBinding
    {
        IConventionalBinding2 SelectAllWith<T1>();
    }

    public interface IConventionalBinding2
    {
        IConventionalBinding2 SelectAllWith<T1>();

        IConventionalBinding2 ExcludeAllWith<T1>();


        IConventionalBinding3 FromAllInterfaces();
        
        IConventionalBinding3 FromItself();

        IConventionalBinding3 From<T1>();

        IConventionalBinding3 From<T1, T2>();

        IConventionalBinding3 From<T1, T2, T3>();

        IConventionalBinding3 From<T1, T2, T3, T4>();
        
        IConventionalBinding3 From<T1, T2, T3, T4, T5>();
        
        IConventionalBinding3 From<T1, T2, T3, T4, T5, T6>();
    }

    public interface IConventionalBinding3
    {
        IScopeBinding ToItself();
    }
}