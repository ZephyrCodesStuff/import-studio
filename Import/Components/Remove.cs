﻿using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Import.Components {
    public class Remove: IconButton {
        void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);

            Path = this.Get<Path>("Path");
        }

        Path Path;

        protected override IBrush Fill {
            get => Path.Stroke;
            set => Path.Stroke = value;
        }
        
        public Remove() {
            InitializeComponent();

            base.MouseLeave(this, null);
        }
    }
}
