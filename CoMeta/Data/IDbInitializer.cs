namespace CoMeta.Data
{
    public interface IDbInitializer
    {
        void Initialize(CoMetaContext context);
    }
}