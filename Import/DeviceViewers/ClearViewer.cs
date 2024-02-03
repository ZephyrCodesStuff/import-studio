﻿using System;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using Import.Core;
using Import.Devices;
using Import.Enums;

namespace Import.DeviceViewers {
    public class ClearViewer: UserControl {
        public static readonly string DeviceIdentifier = "clear";

        void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);

            ClearMode = this.Get<ComboBox>("ClearMode");
        }
        
        Clear _clear;
        ComboBox ClearMode;

        public ClearViewer() => new InvalidOperationException();

        public ClearViewer(Clear clear) {
            InitializeComponent();

            _clear = clear;

            ClearMode.SelectedIndex = (int)_clear.Mode;
        }

        void Unloaded(object sender, VisualTreeAttachmentEventArgs e) => _clear = null;

        void Mode_Changed(object sender, SelectionChangedEventArgs e) {
            ClearType selected = (ClearType)ClearMode.SelectedIndex;

            if (_clear.Mode != selected) 
                Program.Project.Undo.AddAndExecute(new Clear.ModeUndoEntry(
                    _clear, 
                    _clear.Mode, 
                    selected
                ));
        }

        public void SetMode(ClearType mode) => ClearMode.SelectedIndex = (int)mode;
    }
}
