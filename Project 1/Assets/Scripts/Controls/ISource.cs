namespace InputController
{
    public interface ISource
    {
        string GetName();
        SourceType GetSourceType();
    }
}
