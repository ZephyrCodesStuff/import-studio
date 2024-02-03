﻿using System;

using Import.RtMidi.Unmanaged.Devices;

namespace Import.RtMidi.Devices {
    public interface IMidiDevice: IDisposable {
        bool IsOpen { get; }
        string Name { get; }

        bool Open();
        void Close();
    }

    public abstract class MidiDevice: IMidiDevice {
        readonly RtMidiDevice _rtMidiDevice;
        bool _disposed;

        internal MidiDevice(RtMidiDevice rtMidiDevice, string name) {
            _rtMidiDevice = rtMidiDevice?? throw new ArgumentNullException(nameof(rtMidiDevice));
            Name = name;
        }

        public bool IsOpen => _rtMidiDevice.IsOpen;
        public string Name { get; private set; }
        public bool Open() => _rtMidiDevice.Open();
        public void Close() => _rtMidiDevice.Close();
        
        public void Dispose() {
            if (_disposed) return;

            try {
                Disposing();
                _rtMidiDevice.Dispose();

            } finally {
                _disposed = true;
            }
        }

        protected virtual void Disposing() {}
    }
}
