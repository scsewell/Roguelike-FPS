namespace InputController
{
    public interface ButtonSource
    {
        bool IsDown();
        string GetName();
    }
}