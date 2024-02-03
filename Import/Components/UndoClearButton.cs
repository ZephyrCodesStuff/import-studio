using Avalonia.Input;

using Import.Core;

namespace Import.Components {
    public class UndoClearButton: ClearButton {
        protected override void Click(PointerReleasedEventArgs e) => Program.Project.Undo.Clear();
    }
}
