namespace CoMeta.Models
{
    public class Role
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public enum RoleTypes
    {
        User = 1,
        Administrator = 2
    }
}