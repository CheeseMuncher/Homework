namespace LeedsBeerQuest;

public interface IVenueRepository
{
    HashSet<string> GetAllTags();
}

public class VenueRepository
{
    public HashSet<string> GetAllTags() => null;
}
