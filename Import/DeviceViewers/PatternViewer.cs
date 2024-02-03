using System;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using Import.Devices;
using Import.Elements;
using Import.Windows;

namespace Import.DeviceViewers {
    public class PatternViewer: UserControl {
        public static readonly string DeviceIdentifier = "pattern";

        void InitializeComponent() => AvaloniaXamlLoader.Load(this);
        
        Pattern _pattern;

        public PatternViewer() => new InvalidOperationException();

        public PatternViewer(Pattern pattern) {
            InitializeComponent();

            _pattern = pattern;
        }

        void Unloaded(object sender, VisualTreeAttachmentEventArgs e) => _pattern = null;

        void Pattern_Popout() => PatternWindow.Create(_pattern, Track.Get(_pattern)?.Window);
    }
}
