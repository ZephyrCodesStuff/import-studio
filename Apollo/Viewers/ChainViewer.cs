﻿using System;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

using Apollo.Components;
using Apollo.Elements;

namespace Apollo.Viewers {
    public class ChainViewer: UserControl {
        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
        
        Chain _chain;
        Controls Contents;

        private void Contents_Insert(int index, Device device) {
            DeviceViewer viewer = new DeviceViewer(device);
            viewer.DeviceAdded += Device_Insert;
            viewer.DeviceRemoved += Device_Remove;
            Contents.Insert(index + 1, viewer);
        }

        public ChainViewer(Chain chain, bool backgroundBorder = false) {
            InitializeComponent();

            _chain = chain;

            Contents = this.Get<StackPanel>("Contents").Children;

            if (_chain.Count == 0) this.Get<DeviceAdd>("DeviceAdd").AlwaysShowing = true;

            for (int i = 0; i < _chain.Count; i++)
                Contents_Insert(i, _chain[i]);
            
            if (backgroundBorder) {
                this.Get<Grid>("Root").Children.Insert(0, new DeviceBackground());
                Background = (IBrush)Application.Current.Styles.FindResource("ThemeControlDarkenBrush");
            }
        }

        private void Device_Insert(int index, Type device) {
            _chain.Insert(index, Device.Create(device, _chain));
            Contents_Insert(index, _chain[index]);
            this.Get<DeviceAdd>("DeviceAdd").AlwaysShowing = false;
        }

        private void Device_InsertStart(Type device) => Device_Insert(0, device);

        private void Device_Remove(int index) {
            Contents.RemoveAt(index + 1);
            _chain.Remove(index);

            if (_chain.Count == 0) this.Get<DeviceAdd>("DeviceAdd").AlwaysShowing = true;
        }
    }
}
