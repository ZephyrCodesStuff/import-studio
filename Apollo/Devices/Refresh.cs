using System.Linq;

using Apollo.DeviceViewers;
using Apollo.Elements;
using Apollo.Structures;
using Apollo.Core;
using Apollo.Undo;

namespace Apollo.Devices {
    public class Refresh: Device {
        bool[] _macros = new bool[4];

        public bool GetMacro(int index) => _macros[index];
        public void SetMacro(int index, bool macro) {
            _macros[index] = macro;

            if (Viewer?.SpecificViewer != null) ((RefreshViewer)Viewer.SpecificViewer).SetMacro(index, macro);
        }
        
        public override Device Clone() => new Refresh(_macros.ToArray()) {
            Collapsed = Collapsed,
            Enabled = Enabled
        };

        public Refresh(bool[] macros = null): base("refresh") {
            if (macros == null || macros.Length != 4) macros = new bool[4];
            _macros = macros;
        }

        public override void MIDIProcess(Signal n) {
            for (int i = 0; i < 4; i++) {
                if (_macros[i])
                    n.Macros[i] = (int)Program.Project.GetMacro(i + 1);
            }
            
            InvokeExit(n);
        }
        
        public class MacroUndoEntry: PathUndoEntry<Refresh> {
            int index;

            bool u, r;
            
            protected override void UndoPath(params Refresh[] items) => items[0].SetMacro(index, u);
            
            protected override void RedoPath(params Refresh[] items) => items[0].SetMacro(index, r);
            
            public MacroUndoEntry(Refresh refresh, int index, bool u, bool r)
            : base($"Refresh Macro {index + 1} changed to {(r? "Enabled" : "Disabled")}", refresh){
                this.index = index;
                this.u = u;
                this.r = r;
            }
        }
    }
}