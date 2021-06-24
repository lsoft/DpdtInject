using System.ComponentModel;

namespace DpdtInject.Extension.Options
{
    internal class GeneralOptions : BaseOptionModel<GeneralOptions>
    {
        [Category("General")]
        [DisplayName("Enabled")]
        [Description("Specifies whether to activate the CodeLens for Dpdt or not.")]
        [DefaultValue(true)]
        public bool Enabled { get; set; } = true;

        [Category("General")]
        [DisplayName("EnableWhitespaceNormalization")]
        [Description("Specifies whether to normalize cluster source file while adding a new binging or not.")]
        [DefaultValue(false)]
        public bool EnableWhitespaceNormalization { get; set; } = false;

    }
}
