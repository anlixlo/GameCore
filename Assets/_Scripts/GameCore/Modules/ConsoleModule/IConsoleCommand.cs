namespace GameCore
{
    public interface IConsoleCommand
    {
        string commandWord { get; }

        bool Process(string[] args);
    }
}