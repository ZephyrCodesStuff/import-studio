using System;
using System.IO;

using Import.Devices;
using Import.Elements;
using Import.Selection;
using Import.Viewers;

namespace Import.Undo {
    public static class UndoBinary {
        public const int Version = 2;

        public static Type DecodeID(BinaryReader reader) => id[reader.ReadUInt16()];
        public static void EncodeID(BinaryWriter writer, Type type) => writer.Write((ushort)Array.IndexOf(id, type));

        public static readonly Type[] id = new [] { // Only non-abstract UndoEntries go here
            typeof(UndoEntry),
            
            typeof(Chain.DeviceInsertedUndoEntry),

            typeof(Project.AuthorChangedUndoEntry),
            typeof(Project.BPMChangedUndoEntry),
            typeof(Project.TrackInsertedUndoEntry),

            typeof(Choke.TargetUndoEntry),

            typeof(Clear.ModeUndoEntry),
            
            typeof(ColorFilter.HueToleranceUndoEntry),
            typeof(ColorFilter.HueUndoEntry),
            typeof(ColorFilter.SaturationToleranceUndoEntry),
            typeof(ColorFilter.SaturationUndoEntry),
            typeof(ColorFilter.ValueToleranceUndoEntry),
            typeof(ColorFilter.ValueUndoEntry),

            typeof(Copy.BilateralUndoEntry),
            typeof(Copy.CopyModeUndoEntry),
            typeof(Copy.GateUndoEntry),
            typeof(Copy.GridModeUndoEntry),
            typeof(Copy.InfiniteUndoEntry),
            typeof(Copy.OffsetAbsoluteUndoEntry),
            typeof(Copy.OffsetAngleUndoEntry),
            typeof(Copy.OffsetInsertUndoEntry),
            typeof(Copy.OffsetRelativeUndoEntry),
            typeof(Copy.OffsetRemoveUndoEntry),
            typeof(Copy.OffsetSwitchedUndoEntry),
            typeof(Copy.PinchUndoEntry),
            typeof(Copy.RateModeUndoEntry),
            typeof(Copy.RateStepUndoEntry),
            typeof(Copy.RateUndoEntry),
            typeof(Copy.ReverseUndoEntry),
            typeof(Copy.WrapUndoEntry),

            typeof(Delay.DurationModeUndoEntry),
            typeof(Delay.DurationStepUndoEntry),
            typeof(Delay.DurationUndoEntry),
            typeof(Delay.GateUndoEntry),

            typeof(Fade.ColorUndoEntry),
            typeof(Fade.DurationModeUndoEntry),
            typeof(Fade.DurationStepUndoEntry),
            typeof(Fade.DurationUndoEntry),
            typeof(Fade.EndHereUndoEntry),
            typeof(Fade.EqualizeUndoEntry),
            typeof(Fade.GateUndoEntry),
            typeof(Fade.PlaybackModeUndoEntry),
            typeof(Fade.ReverseUndoEntry),
            typeof(Fade.StartHereUndoEntry),
            typeof(Fade.ThumbInsertUndoEntry),
            typeof(Fade.ThumbMoveUndoEntry),
            typeof(Fade.ThumbRemoveUndoEntry),
            typeof(Fade.ThumbTypeUndoEntry),

            typeof(Flip.BypassUndoEntry),
            typeof(Flip.ModeUndoEntry),

            typeof(Group.ChainInsertedUndoEntry),

            typeof(Hold.DurationModeUndoEntry),
            typeof(Hold.DurationStepUndoEntry),
            typeof(Hold.DurationUndoEntry),
            typeof(Hold.GateUndoEntry),
            typeof(Hold.HoldModeUndoEntry),
            typeof(Hold.ReleaseUndoEntry),

            typeof(KeyFilter.ChangedUndoEntry),

            typeof(Layer.ModeUndoEntry),
            typeof(Layer.RangeUndoEntry),
            typeof(Layer.TargetUndoEntry),

            typeof(LayerFilter.RangeUndoEntry),
            typeof(LayerFilter.TargetUndoEntry),

            typeof(Loop.GateUndoEntry),
            typeof(Loop.HoldUndoEntry),
            typeof(Loop.RateModeUndoEntry),
            typeof(Loop.RateStepUndoEntry),
            typeof(Loop.RateUndoEntry),
            typeof(Loop.RepeatsUndoEntry),

            typeof(MacroFilter.FilterUndoEntry),
            typeof(MacroFilter.TargetUndoEntry),

            typeof(Move.GridModeUndoEntry),
            typeof(Move.OffsetAbsoluteUndoEntry),
            typeof(Move.OffsetSwitchedUndoEntry),
            typeof(Move.OffsetRelativeUndoEntry),
            typeof(Move.WrapUndoEntry),

            typeof(Multi.FilterChangedUndoEntry),
            typeof(Multi.ModeUndoEntry),

            typeof(Output.IndexRemovedFix),
            typeof(Output.TargetUndoEntry),

            typeof(Paint.ColorUndoEntry),
            
            typeof(Pattern.BilateralUndoEntry),
            typeof(Pattern.DurationModeUndoEntry),
            typeof(Pattern.DurationStepUndoEntry),
            typeof(Pattern.DurationValueUndoEntry),
            typeof(Pattern.FrameChangedUndoEntry),
            typeof(Pattern.FrameInsertedUndoEntry),
            typeof(Pattern.FrameInvertedUndoEntry),
            typeof(Pattern.FrameReversedUndoEntry),
            typeof(Pattern.GateUndoEntry),
            typeof(Pattern.ImportUndoEntry),
            typeof(Pattern.InfiniteUndoEntry),
            typeof(Pattern.PinchUndoEntry),
            typeof(Pattern.PlaybackModeUndoEntry),
            typeof(Pattern.RepeatsUndoEntry),
            typeof(Pattern.RootKeyUndoEntry),
            typeof(Pattern.WrapUndoEntry),

            typeof(Refresh.MacroUndoEntry),

            typeof(Rotate.BypassUndoEntry),
            typeof(Rotate.ModeUndoEntry),

            typeof(Switch.TargetUndoEntry),
            typeof(Switch.ValueUndoEntry),

            typeof(Tone.HueUndoEntry),
            typeof(Tone.SatHighUndoEntry),
            typeof(Tone.SatLowUndoEntry),
            typeof(Tone.ValueHighUndoEntry),
            typeof(Tone.ValueLowUndoEntry),
            
            typeof(DragDropManager.DragDropUndoEntry),

            typeof(Operations.ChokeUndoEntry),
            typeof(Operations.DeleteUndoEntry),
            typeof(Operations.DuplicateUndoEntry),
            typeof(Operations.GroupUndoEntry),
            typeof(Operations.InsertCopyableUndoEntry),
            typeof(Operations.MuteUndoEntry),
            typeof(Operations.ReplaceUndoEntry),
            typeof(Operations.UnchokeUndoEntry),
            typeof(Operations.UngroupUndoEntry),
            
            typeof(RenameManager.RenamedUndoEntry),
            
            typeof(ChainInfo.DeviceAsChainUndoEntry),
        };
    }
}