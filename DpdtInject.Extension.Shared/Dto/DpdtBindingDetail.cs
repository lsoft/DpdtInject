namespace DpdtInject.Extension.Shared.Dto
{
    public class DpdtBindingDetail : IDpdtBindingDetail
    {
        public string BindScope
        {
            get;
            set;
        }

        public bool ConditionalBinding
        {
            get;
            set;
        }

        public bool ConventionalBinding
        {
            get;
            set;
        }

        public DpdtBindingDetail(
            string bindScope,
            bool conditionalBinding,
            bool conventionalBinding
            )
        {
            BindScope = bindScope;
            ConditionalBinding = conditionalBinding;
            ConventionalBinding = conventionalBinding;
        }
    }
}