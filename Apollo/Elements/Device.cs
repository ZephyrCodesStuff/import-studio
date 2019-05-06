using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

using Apollo.Core;
using Apollo.Structures;
using Apollo.Viewers;

namespace Apollo.Elements {
    public abstract class Device: ISelect {
        public readonly string DeviceIdentifier;

        public ISelectViewer IInfo {
            get => Viewer;
        }

        public ISelectParent IParent {
            get => Parent;
        }

        public int? IParentIndex {
            get => ParentIndex;
        }

        public DeviceViewer Viewer { get; set; }
        
        public Chain Parent;
        public int? ParentIndex;
        public virtual Action<Signal> MIDIExit { get; set; } = null;

        public abstract Device Clone();
        
        protected Device(string device) => DeviceIdentifier = device;

        public abstract void MIDIEnter(Signal n);

        public bool Disposed { get; private set; } = false;

        public virtual void Dispose() {
            MIDIExit = null;
            Disposed = true;
        }

        public static bool Move(List<Device> source, Device target, bool copy = false) {
            if (!copy)
                for (int i = 0; i < source.Count; i++)
                    if (source[i] == target) return false;
            
            List<Device> moved = new List<Device>();

            for (int i = 0; i < source.Count; i++) {
                if (!copy) {
                    source[i].Parent.Viewer.Contents_Remove(source[i].ParentIndex.Value);
                    source[i].Parent.Remove(source[i].ParentIndex.Value, false);
                }

                moved.Add(copy? source[i].Clone() : source[i]);

                target.Parent.Viewer.Contents_Insert(target.ParentIndex.Value + i + 1, moved.Last());
                target.Parent.Insert(target.ParentIndex.Value + i + 1, moved.Last());
            }

            Track track = Track.Get(moved.First());
            track.Window.Selection.Select(moved.First());
            track.Window.Selection.Select(moved.Last(), true);
            
            return true;
        }

        public static bool Move(List<Device> source, Chain target, bool copy = false) {
            if (!copy)
                if (target.Count > 0 && source[0] == target[0]) return false;
            
            List<Device> moved = new List<Device>();

            for (int i = 0; i < source.Count; i++) {
                if (!copy) {
                    source[i].Parent.Viewer.Contents_Remove(source[i].ParentIndex.Value);
                    source[i].Parent.Remove(source[i].ParentIndex.Value, false);
                }

                moved.Add(copy? source[i].Clone() : source[i]);

                target.Viewer.Contents_Insert(i, moved.Last());
                target.Insert(i, moved.Last());
            }

            Track track = Track.Get(moved.First());
            track.Window.Selection.Select(moved.First());
            track.Window.Selection.Select(moved.Last(), true);
            
            return true;
        }

        public static Device Create(Type device, Chain parent) {
            object obj = FormatterServices.GetUninitializedObject(device);
            device.GetField("Parent").SetValue(obj, parent);

            ConstructorInfo ctor = device.GetConstructors()[0];
            ctor.Invoke(
                obj,
                BindingFlags.OptionalParamBinding,
                null, Enumerable.Repeat(Type.Missing, ctor.GetParameters().Count()).ToArray(),
                CultureInfo.CurrentCulture
            );
            
            return (Device)obj;
        }
    }
}