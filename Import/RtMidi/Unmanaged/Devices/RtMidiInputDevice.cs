﻿using System;
using System.Runtime.InteropServices;

using Import.Core;
using Import.RtMidi.Unmanaged.API;

namespace Import.RtMidi.Unmanaged.Devices {
    internal class RtMidiInputDevice: RtMidiDevice {
        // Ensure delegate is not garbage collected (https://stackoverflow.com/questions/6193711/call-has-been-made-on-garbage-collected-delegate-in-c)
        readonly RtMidiCallback _rtMidiCallbackDelegate;

        internal RtMidiInputDevice(uint portNumber): base(portNumber)
            => _rtMidiCallbackDelegate = HandleRtMidiCallback;

        public delegate void MessageEventHandler(byte[] msg);
        public event MessageEventHandler Message;

        protected override IntPtr CreateDevice() {
            IntPtr handle = IntPtr.Zero;

            try {
                Program.DebugLog("Creating default input device");
                handle = RtMidiC.Input.CreateDefault();
                CheckForError(handle);

                Program.DebugLog("Setting types to ignore");
                RtMidiC.Input.IgnoreTypes(handle, false /* needed for SysEx */, true, true);
                CheckForError(handle);

                Program.DebugLog("Setting input callback");
                RtMidiC.Input.SetCallback(handle, _rtMidiCallbackDelegate, IntPtr.Zero);
                CheckForError(handle);

                return handle;

            } catch (Exception) {
                Program.Log("Unable to create default input device");

                if (handle != IntPtr.Zero) {
                    Program.DebugLog("Freeing input device handle");

                    try {
                        RtMidiC.Input.Free(handle);
                        CheckForError(handle);

                    } catch (Exception) {
                        Program.Log("Unable to free input device");
                    }
                }

                return IntPtr.Zero;
            }
        }

        void HandleRtMidiCallback(double timestamp, IntPtr messagePtr, UIntPtr messageSize, IntPtr userData) {
            if (Message == null) return;

            byte[] message;

            try {
                // Copy unmanaged message to managed byte array
                int size = (int)messageSize;
                message = new byte[size];
                Marshal.Copy(messagePtr, message, 0, size);

            } catch (Exception) {
                Program.Log("Unexpected exception occurred while receiving MIDI message");
                return;
            }

            Message.Invoke(message);
        }

        protected override void DestroyDevice() {
            try {
                Program.DebugLog("Cancelling input callback");
                RtMidiC.Input.CancelCallback(Handle);
                CheckForError();

                Program.DebugLog("Freeing input device handle");
                RtMidiC.Input.Free(Handle);
                CheckForError();
                
            } catch (Exception) {
                Program.Log("Error while freeing input device handle");
            }
        }
    }
}
