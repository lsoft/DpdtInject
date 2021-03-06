namespace DpdtInject.Extension.Shared.Dto
{
    public interface IDpdtClusterDetail
    {
        string ClassFullName
        {
            get;
        }

        string MethodName
        {
            get;
        }

        string FullName
        {
            get;
        }
    }
}
