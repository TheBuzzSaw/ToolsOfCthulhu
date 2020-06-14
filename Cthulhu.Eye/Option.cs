namespace Cthulhu.Eye
{
    static class Option
    {
        public static Option<T> Create<T>(string displayText, T value) => new Option<T>(displayText, value);
    }
    
    readonly struct Option<T>
    {
        public readonly string DisplayText { get; }
        public readonly T Value { get; }

        public Option(string displayText, T value)
        {
            DisplayText = displayText;
            Value = value;
        }
    }
}