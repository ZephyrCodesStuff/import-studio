using System.Collections.Generic;

using Import.Core;
using Import.Enums;
using Import.Rendering;
using Import.RtMidi.Devices.Infos;
using Import.Structures;

namespace Import.Elements {
    public class VirtualLaunchpad: Launchpad {
        public int VirtualIndex = 0;

        public override void Send(List<RawUpdate> n, Color[] snapshot) {
            foreach (RawUpdate i in n)
                Window?.Render(i);
        }

        public override void ForceClear() {
            if (!Usable) return;
            
            CreateScreen();

            Window?.Clear();
        }

        public VirtualLaunchpad(string name, int index) {
            Type = LaunchpadType.Pro;
            Name = name;
            VirtualIndex = index;
        }

        public override void Connect(IMidiInputDeviceInfo input = null, IMidiOutputDeviceInfo output = null) {
            Available = true;

            Program.Log($"MIDI Created {Name}");
        }
        
        public override void Disconnect(bool actuallyClose = true) {
            Program.Log($"MIDI Disconnected {Name}");

            Available = false;
        }
    }
}