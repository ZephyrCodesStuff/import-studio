using System;

using Import.Core;

namespace Import.Structures {
    public class Length {
        public delegate void ChangedEventHandler();
        public event ChangedEventHandler Changed;

        public static string[] Steps = new []
            {"1/128", "1/64", "1/32", "1/16", "1/8", "1/4", "1/2", "1/1", "2/1", "4/1"};

        int _value;
        public int Step {
            get => _value;
            set {
                if (0 <= value && value <= 9 && _value != value) {
                    _value = value;
                    Changed?.Invoke();
                }
            }
        }
        
        public double Value {
            get => Convert.ToDouble(Math.Pow(2, _value - 7));
        }

        public Length Clone() => new Length(Step);

        public Length(int step = 5) => Step = step;

        public override bool Equals(object obj) {
            if (!(obj is Length)) return false;
            return this == (Length)obj;
        }

        public static bool operator ==(Length a, Length b) {
            if (a is null || b is null) return ReferenceEquals(a, b);
            return a.Step == b.Step;
        }
        public static bool operator !=(Length a, Length b) => !(a == b);

        public override int GetHashCode() => HashCode.Combine(Step);

        public static implicit operator int(Length x) => (int)(x.Value * 240000 / Program.Project.BPM);
        public static implicit operator double(Length x) => x.Value * 240000 / Program.Project.BPM;

        public override string ToString() => Steps[_value];

        public void Dispose() => Changed = null;
    }
}