namespace DpdtInject.Injector
{
    public interface IResolutionFast<TR>
    {
        TR GetFast(TR unused);
    }
}