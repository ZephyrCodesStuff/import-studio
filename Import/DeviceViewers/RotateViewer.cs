﻿using System;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

using Import.Core;
using Import.Devices;
using Import.Enums;

namespace Import.DeviceViewers {
    public class RotateViewer: UserControl {
        public static readonly string DeviceIdentifier = "rotate";

        void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
            
            RotateMode = this.Get<ComboBox>("RotateMode");
            Bypass = this.Get<CheckBox>("Bypass");
        }
        
        Rotate _rotate;
        ComboBox RotateMode;
        CheckBox Bypass;

        public RotateViewer() => new InvalidOperationException();

        public RotateViewer(Rotate rotate) {
            InitializeComponent();

            _rotate = rotate;

            RotateMode.SelectedIndex = (int)_rotate.Mode;
            Bypass.IsChecked = _rotate.Bypass;
        }

        void Unloaded(object sender, VisualTreeAttachmentEventArgs e) => _rotate = null;

        void Mode_Changed(object sender, SelectionChangedEventArgs e) {
            RotateType selected = (RotateType)RotateMode.SelectedIndex;

            if (_rotate.Mode != selected)
                Program.Project.Undo.AddAndExecute(new Rotate.ModeUndoEntry(
                    _rotate, 
                    _rotate.Mode,
                    selected,
                    RotateMode.Items
                ));
        }

        public void SetMode(RotateType mode) => RotateMode.SelectedIndex = (int)mode;

        void Bypass_Changed(object sender, RoutedEventArgs e) {
            bool value = Bypass.IsChecked.Value;

            if (_rotate.Bypass != value)
                Program.Project.Undo.AddAndExecute(new Rotate.BypassUndoEntry(
                    _rotate, 
                    _rotate.Bypass, 
                    value
                ));
        }

        public void SetBypass(bool value) => Bypass.IsChecked = value;
    }
}
