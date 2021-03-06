namespace DpdtInject.Extension.Shared.Dto
{
    public interface IDpdtBindingDetail
    {
        string BindScope
        {
            get;
        }

        bool ConditionalBinding
        {
            get;
        }
    }
}
