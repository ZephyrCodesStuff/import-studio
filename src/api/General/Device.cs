using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

using api;

namespace api.Devices {
    public abstract class Device: IRequest {
        public static readonly string Identifier = "device";
        public readonly string DeviceIdentifier;

        public IDeviceParent Parent = null;
        public int? ParentIndex;
        public Action<Signal> MIDIExit = null;

        public abstract Device Clone();
        
        protected Device(string device) {
            DeviceIdentifier = device;
        }

        public abstract void MIDIEnter(Signal n);

        public static Device Decode(string jsonString) {
            Dictionary<string, object> json = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
            if (json["object"].ToString() != Identifier) return null;

            Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json["data"].ToString());
            
            foreach (Type device in (from type in Assembly.GetExecutingAssembly().GetTypes() where (type.Namespace.StartsWith("api.Devices") && !type.Namespace.StartsWith("api.Devices.Device")) select type)) {
                var parsed = device.GetMethod("DecodeSpecific").Invoke(null, new object[] {json["data"].ToString()});
                if (parsed != null) return (Device)parsed;
            }
            return null;
        }

        public abstract string EncodeSpecific();
        public string Encode() {
            StringBuilder json = new StringBuilder();

            using (JsonWriter writer = new JsonTextWriter(new StringWriter(json))) {
                writer.WriteStartObject();

                    writer.WritePropertyName("object");
                    writer.WriteValue(Identifier);

                    writer.WritePropertyName("data");
                    writer.WriteRawValue(EncodeSpecific());

                writer.WriteEndObject();
            }
            
            return json.ToString();
        }
        
        public string Request(string type, Dictionary<string, object> content) => Parent.Request("forward", new Dictionary<string, object>() {
            ["forward"] = Identifier,
            ["index"] = ParentIndex,
            ["message"] = Communication.UI.EncodeMessage(Identifier, type, content, DeviceIdentifier)
        });

        public abstract ObjectResult RespondSpecific(string jsonString);
    }
}