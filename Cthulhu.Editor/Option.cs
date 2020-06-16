using System;
using System.Collections.Generic;
using System.Text;

namespace Cthulhu.Editor
{
    class Option<T>
    {
        public string DisplayText { get; }
        public T Value { get; }

        public Option(string displayText, T value)
        {
            DisplayText = displayText;
            Value = value;
        }

        public override string ToString() => DisplayText;
    }
}
