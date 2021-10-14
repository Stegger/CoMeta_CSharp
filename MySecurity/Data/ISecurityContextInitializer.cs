namespace MySecurity.Data
{
    public interface ISecurityContextInitializer
    {

        void Initialize(SecurityContext context);

    }
}