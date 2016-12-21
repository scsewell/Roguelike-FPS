namespace InputController
{
    public interface ISource<T>
    {
        T GetValue();
        string GetName();
        SourceType GetSourceType();
    }
}
