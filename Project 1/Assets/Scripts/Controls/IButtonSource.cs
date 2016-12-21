namespace InputController
{
    public interface IButtonSource : ISource
    {
        bool IsDown();
    }
}