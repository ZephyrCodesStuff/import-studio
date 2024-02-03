﻿using System;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

using Import.Components;
using Import.Elements;
using Import.Selection;

namespace Import.Viewers {
    public class CollapsedDeviceViewer: DeviceViewer {
        protected override void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);

            Root = this.Get<StackPanel>("Root");
            Header = this.Get<Border>("Contents");
            
            Draggable = this.Get<Grid>("Draggable");
            DeviceAdd = this.Get<DeviceAdd>("DropZoneAfter");
            Indicator = this.Get<Indicator>("Indicator");

            TitleText = this.Get<TextBlock>("Title");

            DeviceMute = this.Get<MenuItem>("DeviceMute");
            GroupMute = this.Get<MenuItem>("GroupMute");
            ChokeMute = this.Get<MenuItem>("ChokeMute");
        }

        protected override void ApplyHeaderBrush(string resource) {
            IBrush brush = (IBrush)Application.Current.Styles.FindResource(resource);

            if (IsArrangeValid) Header.Background = brush;
            else this.Resources["TitleBrush"] = brush;
        }

        public CollapsedDeviceViewer() => new InvalidOperationException();

        public CollapsedDeviceViewer(Device device) {
            TitleText.Text = device.Name;

            _device = device;
            _device.Viewer = this;
            Deselect();

            DeviceContextMenu = (ImportContextMenu)this.Resources["DeviceContextMenu"];
            GroupContextMenu = (ImportContextMenu)this.Resources["GroupContextMenu"];
            ChokeContextMenu = (ImportContextMenu)this.Resources["ChokeContextMenu"];
            
            DragDrop = new DragDropManager(this);

            SetEnabled();
        }

        protected override void Unloaded(object sender, VisualTreeAttachmentEventArgs e) {
            base.ResetEvents();

            SpecificViewer = null;
            _device.Viewer = null;
            _device = null;

            DeviceContextMenu = GroupContextMenu = ChokeContextMenu = null;

            DragDrop.Dispose();
            DragDrop = null;
        }

        public override void SetEnabled() {
            Header.BorderBrush = (IBrush)Application.Current.Styles.FindResource(_device.Enabled? "ThemeBorderMidBrush" : "ThemeBorderLowBrush");
            TitleText.Foreground = (IBrush)Application.Current.Styles.FindResource(_device.Enabled? "ThemeForegroundBrush" : "ThemeForegroundLowBrush");
        }
    }
}
