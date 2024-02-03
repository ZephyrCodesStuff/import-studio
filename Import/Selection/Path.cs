using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Import.Core;
using Import.Devices;
using Import.Elements;

namespace Import.Selection {
    public class Path<T> {
        List<int> path = null;

        TRet Next<TRet>(ISelectParent current, int index) {
            if (current == null) return default;

            try {
                return (TRet)(
                    (path[index] == -1)
                        ? (ISelect)((Multi)current).Preprocess
                        : (current.IChildren[path[index]] is Choke choke && (index != 0 || typeof(T) != typeof(Choke)))
                            ? (ISelect)choke.Chain
                            : current.IChildren[path[index]]
                );
            
            } catch (Exception ex) {
                if (ex is ArgumentOutOfRangeException || ex is InvalidCastException)
                    return default;

                throw;
            }
        }

        public T Resolve(int skip) {
            if (path == null) return (T)(ISelectParent)Program.Project;

            ISelectParent item = Program.Project[path.Last()].Chain;

            if (path.Count - skip == 1) return (T)item;

            for (int i = path.Count - 2; i > skip; i--)
                item = Next<ISelectParent>(item, i);

            return Next<T>(item, skip);
        }

        public T Resolve() => (T)Resolve(0);

        public Path(T item) {
            if (item is ISelect child) {
                path = new List<int>();

                while (true) {
                    if (child is Chain chain && (chain.Parent is Choke || chain.IRoot))
                        child = (ISelect)chain.Parent;

                    path.Add(child.IParentIndex?? -1);

                    if (child is Track) break;

                    child = (ISelect)child.IParent;
                }

            } else if (!(item is ISelectParent)) throw new ArgumentException("Invalid type for Path<T>");
        }

        public Path(BinaryReader reader, int version) {
            int count = reader.ReadInt32();
            
            if (count != -1)
                path = Enumerable.Range(0, count).Select(i => reader.ReadInt32()).ToList();
        }

        public void Encode(BinaryWriter writer) {
            if (path == null) writer.Write(-1); // In case T is ISelectParent (only happens for Project)
            else {
                writer.Write(path.Count);
                for (int i = 0; i < path.Count; i++)
                    writer.Write(path[i]);
            }
        }
    }
}