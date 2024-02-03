using System;

using Import.Core;
using Import.Devices;
using Import.Elements;
using Import.Helpers;
using Import.Structures;
using Import.Undo;

namespace Import.Binary {
    public static class Common {
        public const int version = 32;

        public static readonly Type[] id = new[] {
            typeof(Preferences),
            typeof(Copyable),

            typeof(Project),
            typeof(Track),
            typeof(Chain),
            typeof(Device),
            typeof(Launchpad),

            typeof(Group),
            typeof(Copy),
            typeof(Delay),
            typeof(Fade),
            typeof(Flip),
            typeof(Hold),
            typeof(KeyFilter),
            typeof(Layer),
            typeof(Move),
            typeof(Multi),
            typeof(Output),
            typeof(MacroFilter),
            typeof(Switch),
            typeof(Paint),
            typeof(Pattern),
            typeof(Preview),
            typeof(Rotate),
            typeof(Tone),

            typeof(Color),
            typeof(Frame),
            typeof(Length),
            typeof(Offset),
            typeof(Time),

            typeof(Choke),
            typeof(ColorFilter),
            typeof(Clear),
            typeof(LayerFilter),
            typeof(Loop),
            typeof(Refresh),
            typeof(UndoManager)
        };
    }
}