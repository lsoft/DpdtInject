namespace DpdtInject.Generator.Core.Producer
{
    public interface IWritable
    {
        void Write(IndentedTextWriter2 writer, ShortTypeNameGenerator sng);
    }

}
