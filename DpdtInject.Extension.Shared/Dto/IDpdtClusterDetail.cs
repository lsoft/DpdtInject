namespace DpdtInject.Extension.Shared.Dto
{
    public interface IDpdtClusterDetail
    {
        string? ClassNamespace
        {
            get;
        }

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
