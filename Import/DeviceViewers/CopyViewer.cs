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
    public class CopyViewer: UserControl {
        public static readonly string DeviceIdentifier = "copy";

        void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);

            Rate = this.Get<Dial>("Rate");
            Gate = this.Get<Dial>("Gate");
            Pinch = this.Get<PinchDial>("Pinch");

            CopyMode = this.Get<ComboBox>("CopyMode");
            GridMode = this.Get<ComboBox>("GridMode");
            Wrap = this.Get<CheckBox>("Wrap");

            Reverse = this.Get<CheckBox>("Reverse");
            Infinite = this.Get<CheckBox>("Infinite");
            
            Contents = this.Get<StackPanel>("Contents").Children;
            OffsetAdd = this.Get<HorizontalAdd>("OffsetAdd");
        }

        Copy _copy;

        Dial Rate, Gate;
        PinchDial Pinch;
        ComboBox CopyMode, GridMode;
        CheckBox Wrap, Reverse, Infinite;

        Controls Contents;
        HorizontalAdd OffsetAdd;

        public void Contents_Insert(int index, Offset offset, int angle) {
            CopyOffset viewer = new CopyOffset(offset, angle, _copy);
            viewer.OffsetAdded += Offset_Insert;
            viewer.OffsetRemoved += Offset_Remove;

            Contents.Insert(index + 1, viewer);
            OffsetAdd.AlwaysShowing = false;
        }

        public void Contents_Remove(int index) {
            Contents.RemoveAt(index + 1);
            if (Contents.Count == 1) OffsetAdd.AlwaysShowing = true;
        }

        public CopyViewer() => new InvalidOperationException();

        public CopyViewer(Copy copy) {
            InitializeComponent();

            _copy = copy;

            Rate.UsingSteps = _copy.Time.Mode;
            Rate.Length = _copy.Time.Length;
            Rate.RawValue = _copy.Time.Free;

            Gate.RawValue = _copy.Gate * 100;
            Pinch.RawValue = _copy.Pinch;
            Pinch.IsBilateral = _copy.Bilateral;

            Reverse.IsChecked = _copy.Reverse;
            Infinite.IsChecked = _copy.Infinite;

            GridMode.SelectedIndex = (int)_copy.GridMode;
            
            SetCopyMode(_copy.CopyMode);

            Wrap.IsChecked = _copy.Wrap;

            for (int i = 0; i < _copy.Offsets.Count; i++)
                Contents_Insert(i, _copy.Offsets[i], _copy.GetAngle(i));
        }

        void Unloaded(object sender, VisualTreeAttachmentEventArgs e) => _copy = null;

        void Rate_ValueChanged(Dial sender, double value, double? old) {
            if (old != null && old != value) 
                Program.Project.Undo.AddAndExecute(new Copy.RateUndoEntry(
                    _copy,
                    (int)old.Value, 
                    (int)value
                ));
        }

        public void SetRateValue(int rate) => Rate.RawValue = rate;

        void Rate_ModeChanged(bool value, bool? old) {
            if (old != null && old != value) 
                Program.Project.Undo.AddAndExecute(new Copy.RateModeUndoEntry(
                    _copy, 
                    old.Value, 
                    value
                ));
        }

        public void SetMode(bool mode) => Rate.UsingSteps = mode;

        void Rate_StepChanged(int value, int? old) {
            if (old != null && old != value) 
                Program.Project.Undo.AddAndExecute(new Copy.RateStepUndoEntry(
                    _copy, 
                    old.Value, 
                    value
                ));
        }

        public void SetRateStep(Length rate) => Rate.Length = rate;

        void Gate_Changed(Dial sender, double value, double? old) {
            if (old != null && old != value) 
                Program.Project.Undo.AddAndExecute(new Copy.GateUndoEntry(
                    _copy, 
                    old.Value, 
                    value
                ));
        }

        public void SetGate(double gate) => Gate.RawValue = gate * 100;

        void CopyMode_Changed(object sender, SelectionChangedEventArgs e) {
            CopyType selected = (CopyType)CopyMode.SelectedIndex;

            if (_copy.CopyMode != selected)
                Program.Project.Undo.AddAndExecute(new Copy.CopyModeUndoEntry(
                    _copy,
                    _copy.CopyMode,
                    selected,
                    CopyMode.Items
                ));
        }

        public void SetCopyMode(CopyType mode) {
            CopyMode.SelectedIndex = (int)mode;

            Rate.Enabled = Gate.Enabled = mode.SupportsRate();
            Pinch.Enabled = Reverse.IsEnabled = Infinite.IsEnabled = mode.SupportsPinch();
            
            for (int i = 1; i < Contents.Count; i++)
                ((CopyOffset)Contents[i]).AngleEnabled = mode.SupportsAngle();
        }

        void GridMode_Changed(object sender, SelectionChangedEventArgs e) {
            GridType selected = (GridType)GridMode.SelectedIndex;

            if (_copy.GridMode != selected) 
                Program.Project.Undo.AddAndExecute(new Copy.GridModeUndoEntry(
                    _copy, 
                    _copy.GridMode, 
                    selected,
                    GridMode.Items
                ));
        }

        public void SetGridMode(GridType mode) => GridMode.SelectedIndex = (int)mode;

        void Pinch_Changed(Dial sender, double value, double? old) {
            if (old != null && old != value)
                Program.Project.Undo.AddAndExecute(new Copy.PinchUndoEntry(
                    _copy, 
                    old.Value, 
                    value
                ));
        }

        public void SetPinch(double pinch) => Pinch.RawValue = pinch;
        
        void Bilateral_Changed(bool value, bool? old) {
            if (old != null && old != value)
                Program.Project.Undo.AddAndExecute(new Copy.BilateralUndoEntry(
                    _copy, 
                    old.Value, 
                    value
                ));
        }
        
        public void SetBilateral(bool bilateral) => Pinch.IsBilateral = bilateral;

        void Reverse_Changed(object sender, RoutedEventArgs e) {
            bool value = Reverse.IsChecked.Value;

            if (_copy.Reverse != value)
                Program.Project.Undo.AddAndExecute(new Copy.ReverseUndoEntry(
                    _copy, 
                    _copy.Reverse, 
                    value
                ));
        }

        public void SetReverse(bool value) => Reverse.IsChecked = value;

        void Infinite_Changed(object sender, RoutedEventArgs e) {
            bool value = Infinite.IsChecked.Value;

            if (_copy.Infinite != value)
                Program.Project.Undo.AddAndExecute(new Copy.InfiniteUndoEntry(
                    _copy, 
                    _copy.Infinite, 
                    value
                ));
        }

        public void SetInfinite(bool value) => Infinite.IsChecked = value;

        void Wrap_Changed(object sender, RoutedEventArgs e) {
            bool value = Wrap.IsChecked.Value;

            if (_copy.Wrap != value)
                Program.Project.Undo.AddAndExecute(new Copy.WrapUndoEntry(
                    _copy, 
                    _copy.Wrap, 
                    value
                ));
        }

        public void SetWrap(bool value) => Wrap.IsChecked = value;

        void Offset_InsertStart() => Offset_Insert(0);

        void Offset_Insert(int index) => Program.Project.Undo.AddAndExecute(new Copy.OffsetInsertUndoEntry(
            _copy, 
            index
        ));

        void Offset_Remove(int index) => Program.Project.Undo.AddAndExecute(new Copy.OffsetRemoveUndoEntry(
            _copy, 
            _copy.Offsets[index].Clone(), 
            _copy.GetAngle(index),
            index
        ));

        public void SetOffset(int index, Offset offset) => ((CopyOffset)Contents[index + 1]).SetOffset(offset);
        
        public void SetOffsetAngle(int index, double angle) => ((CopyOffset)Contents[index + 1]).SetAngle(angle);
    }
}
