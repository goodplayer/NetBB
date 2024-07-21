namespace NetBB.Sources.Utilities
{
    public class LongIdGenerator
    {
        public string GenerateId()
        {
            Guid g = Guid.NewGuid();
            return g.ToString();
        }
    }
}
