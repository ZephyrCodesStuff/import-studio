using System;
using System.Collections.Generic;
using System.Linq;

using Avalonia.Input;

using Import.Core;
using Import.Devices;

namespace Import.Selection {
    public class SelectionManager {
        public ISelect Start { get; private set; } = null;
        public ISelect End { get; private set; } = null;

        public List<ISelect> Selection {
            get {
                if (Start != null) {
                    if (End != null) {
                        ISelect left = (Start.IParentIndex.Value < End.IParentIndex.Value)? Start : End;
                        ISelect right = (Start.IParentIndex.Value < End.IParentIndex.Value)? End : Start;

                        return left.IParent.IChildren.Skip(left.IParentIndex.Value).Take(right.IParentIndex.Value - left.IParentIndex.Value + 1).ToList();
                    }
                    
                    return new List<ISelect>() {Start};
                }

                return new List<ISelect>();
            }
        }

        Func<ISelect> TargetDefault;

        public SelectionManager(Func<ISelect> targetDefault) {
            TargetDefault = targetDefault;
            SelectDefault();
        }

        public void Select(ISelect select, bool shift = false) {
            if (Start != null)
                if (End != null)
                    foreach (ISelect selected in Selection)
                        selected.IInfo?.Deselect();
                else Start.IInfo?.Deselect();

            if (shift && Start != null && Start.IParent == select.IParent && Start != select)
                End = select;

            else {
                Start = select;
                End = null;
            }

            if (Start != null)
                if (End != null)
                    foreach (ISelect selected in Selection)
                        selected.IInfo?.Select();
                else Start.IInfo?.Select();
        }

        public void SelectDefault() {
            ISelect target = TargetDefault.Invoke();
            
            if (target != null) Select(target, false);
        }

        public void SelectAll() {
            ISelectParent target = Start.IParent;
            Select(target.IChildren.First());
            Select(target.IChildren.Last(), true);
        }

        public bool Move(bool right, bool shift = false) {
            ISelect target = (shift? (End?? Start) : Start);
            
            if (target == null) {
                SelectDefault();
                return true;
            }

            if (right) {
                if (target.IParentIndex.Value + 1 >= target.IParent.IChildren.Count) return false;
                target = target.IParent.IChildren[target.IParentIndex.Value + 1];

            } else {
                if (target.IParentIndex.Value == 0 && target.IParent is ISelect && !target.IParent.IRoot) target = (ISelect)target.IParent;
                else if (target.IParentIndex.Value - 1 < 0) return false;
                else target = target.IParent.IChildren[target.IParentIndex.Value - 1];
            }

            Select(target, shift);
            return true;
        }

        public void Expand() {
            if (Start.IParent.IViewer.IExpanded != Start.IParentIndex)
                Start.IParent.IViewer.Expand(Start.IParentIndex);
        }

        public void MoveChild() {
            Expand();

            if (Start is ISelectParent parent && parent.IChildren.Count > 0)
                Select(parent.IChildren[0]);
        }

        public void Action(string action) {
            if (Start == null) return;

            ISelectParent parent = Start.IParent;
            
            int left = Start.IParentIndex.Value;
            int right = (End == null)? left: End.IParentIndex.Value;
            
            if (left > right) {
                int temp = left;
                left = right;
                right = temp;
            }

            Action(action, parent, left, right);
        }

        public void Action(string action, ISelectParent parent, int index) => Action(action, parent, index, index);

        public void Action(string action, ISelectParent parent, int left, int right) {
            if (parent is Pattern pattern && pattern.Window?.Locked == true) return;

            if (action == "Cut") Operations.Copy(parent, left, right, true);
            else if (action == "Copy") Operations.Copy(parent, left, right);
            else if (action == "Duplicate") Operations.Duplicate(parent, left, right);
            else if (action == "Paste") Operations.Paste(parent, right);
            else if (action == "Replace") Operations.Replace(parent, left, right);
            else if (action == "Delete") Operations.Delete(parent, left, right);
            else if (action == "Group") Operations.Group(parent, left, right);
            else if (action == "Ungroup") Operations.Ungroup(parent, left);
            else if (action == "Choke") Operations.Choke(parent, left, right);
            else if (action == "Unchoke") Operations.Unchoke(parent, left);
            else if (action == "Mute" || action == "Unmute") Operations.Mute(parent, left, right);
            else if (action == "Rename") Operations.Rename(parent, left, right);
            else if (action == "Export") Operations.Export(parent, left, right);
            else if (action == "Import") Operations.Import(parent, right);
        }

        public bool HandleKey(KeyEventArgs e) {
            if (Start == null) {
                SelectDefault();
                return true;
            }

            if (e.KeyModifiers == (App.ControlKey | KeyModifiers.Shift)) {
                if (e.Key == Key.V) Action("Replace");
                else if (e.Key == Key.G) Action("Choke");
                else if (e.Key == Key.U) Action("Unchoke");
                else return false;
            
            } else if (e.KeyModifiers == App.ControlKey) {
                if (e.Key == Key.X) Action("Cut");
                else if (e.Key == Key.C) Action("Copy");
                else if (e.Key == Key.D) Action("Duplicate");
                else if (e.Key == Key.V) Action("Paste");
                else if (e.Key == Key.G) Action("Group");
                else if (e.Key == Key.U) Action("Ungroup");
                else if (e.Key == Key.R) Action("Rename");
                else if (e.Key == Key.E) Action("Export");
                else if (e.Key == Key.I) Action("Import");
                else if (e.Key == Key.A) SelectAll();
                else return false;

            } else if (e.KeyModifiers == KeyModifiers.None) {
                if (e.Key == Key.Delete || e.Key == Key.Back) Action("Delete");
                else if (e.Key == Key.D0 || e.Key == Key.NumPad0) Action("Mute");
                else if (e.Key == Key.F2) Action("Rename");
                else return false;
            
            } else return false;

            return true;
        }

        public void Dispose() => Start = End = null;
    }
}