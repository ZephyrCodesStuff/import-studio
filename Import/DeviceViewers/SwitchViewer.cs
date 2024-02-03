﻿using System;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using Import.Components;
using Import.Core;
using Import.Devices;

namespace Import.DeviceViewers {
    public class SwitchViewer: UserControl {
        public static readonly string DeviceIdentifier = "switch";

        void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
            
            Target = this.Get<Dial>("Target");
            Value = this.Get<Dial>("Value");
        }
        
        Switch _switch;
        
        Dial Target;
        Dial Value;

        public SwitchViewer() => new InvalidOperationException();

        public SwitchViewer(Switch macroswitch) {
            InitializeComponent();

            _switch = macroswitch;

            Target.RawValue = _switch.Target;
            Value.RawValue = _switch.Value;
        }

        void Unloaded(object sender, VisualTreeAttachmentEventArgs e) => _switch = null;

        void Target_Changed(Dial sender, double value, double? old){
            if (old != null && old != value)
                Program.Project.Undo.AddAndExecute(new Switch.TargetUndoEntry(
                    _switch, 
                    (int)old.Value, 
                    (int)value
                ));
        }
       
        public void SetTarget(int target) => Target.RawValue = target;
        
        void Value_Changed(Dial sender, double value, double? old){
            if (old != null && old != value)
                Program.Project.Undo.AddAndExecute(new Switch.ValueUndoEntry(
                    _switch, 
                    (int)old.Value, 
                    (int)value
                ));
        }
        
        public void SetValue(int value) => Value.RawValue = value;
    }
}
