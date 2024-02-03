﻿using System;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

using Import.Components;
using Import.Core;
using Import.Devices;
using Import.Enums;
using Import.Structures;

namespace Import.DeviceViewers {
    public class HoldViewer: UserControl {
        public static readonly string DeviceIdentifier = "hold";

        void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);

            Duration = this.Get<Dial>("Duration");
            Gate = this.Get<Dial>("Gate");
            HoldMode = this.Get<ComboBox>("HoldMode");
            Release = this.Get<CheckBox>("Release");
        }
        
        Hold _hold;

        Dial Duration, Gate;
        ComboBox HoldMode;
        CheckBox Release;

        public HoldViewer() => new InvalidOperationException();

        public HoldViewer(Hold hold) {
            InitializeComponent();

            _hold = hold;

            Duration.UsingSteps = _hold.Time.Mode;
            Duration.Length = _hold.Time.Length;
            Duration.RawValue = _hold.Time.Free;

            Gate.RawValue = _hold.Gate * 100;

            SetHoldMode(_hold.HoldMode); // required to set Dial Enabled properties

            Release.IsChecked = _hold.Release;
        }

        void Unloaded(object sender, VisualTreeAttachmentEventArgs e) => _hold = null;

        void Duration_Changed(Dial sender, double value, double? old) {
            if (old != null && old != value) 
                Program.Project.Undo.AddAndExecute(new Hold.DurationUndoEntry(
                    _hold,
                    (int)old.Value, 
                    (int)value
                ));
        }

        public void SetDurationValue(int duration) => Duration.RawValue = duration;

        void Duration_ModeChanged(bool value, bool? old) {
            if (old != null && old != value)
                Program.Project.Undo.AddAndExecute(new Hold.DurationModeUndoEntry(
                    _hold, 
                    old.Value, 
                    value
                ));
        }

        public void SetMode(bool mode) => Duration.UsingSteps = mode;

        void Duration_StepChanged(int value, int? old) {
            if (old != null && old != value) 
                Program.Project.Undo.AddAndExecute(new Hold.DurationStepUndoEntry(
                    _hold, 
                    old.Value, 
                    value
                ));
        }

        public void SetDurationStep(Length duration) => Duration.Length = duration;

        void Gate_Changed(Dial sender, double value, double? old) {
            if (old != null && old != value)
                Program.Project.Undo.AddAndExecute(new Hold.GateUndoEntry(
                    _hold, 
                    old.Value, 
                    value
                ));
        }

        public void SetGate(double gate) => Gate.RawValue = gate * 100;

        void HoldMode_Changed(object sender, SelectionChangedEventArgs e) {
            HoldType selected = (HoldType)HoldMode.SelectedIndex;

            if (_hold.HoldMode != selected) 
                Program.Project.Undo.AddAndExecute(new Hold.HoldModeUndoEntry(
                    _hold,
                    _hold.HoldMode,
                    selected,
                    HoldMode.Items
                ));
        }

        public void SetHoldMode(HoldType value) {
            HoldMode.SelectedIndex = (int)value;
            
            Duration.Enabled = Gate.Enabled = value != HoldType.Infinite;
            Release.IsEnabled = value != HoldType.Minimum;
        }

        void Release_Changed(object sender, RoutedEventArgs e) {
            bool value = Release.IsChecked.Value;

            if (_hold.Release != value) 
                Program.Project.Undo.AddAndExecute(new Hold.ReleaseUndoEntry(
                    _hold, 
                    _hold.Release, 
                    value
                ));
        }

        public void SetRelease(bool value) => Release.IsChecked = value;
    }
}
