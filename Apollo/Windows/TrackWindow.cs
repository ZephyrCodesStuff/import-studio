﻿using System;
using System.ComponentModel;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform;

using Apollo.Core;
using Apollo.Devices;
using Apollo.Elements;
using Apollo.Helpers;
using Apollo.Interfaces;
using Apollo.Viewers;

namespace Apollo.Windows {
    public class TrackWindow: Window {
        void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);

            TitleText = this.Get<TextBlock>("Title");
            TitleCenter = this.Get<TextBlock>("TitleCenter");

            CenteringLeft = this.Get<StackPanel>("CenteringLeft");
            CenteringRight = this.Get<StackPanel>("CenteringRight");

            Contents = this.Get<ScrollViewer>("Contents");
        }

        Track _track;

        ScrollViewer Contents;
        Grid Root;
        
        TextBlock TitleText, TitleCenter;
        StackPanel CenteringLeft, CenteringRight;
        
        void UpdateTitle() => UpdateTitle(_track.ParentIndex.Value, _track.ProcessedName);
        void UpdateTitle(int index) => UpdateTitle(index, _track.ProcessedName);
        void UpdateTitle(string name) => UpdateTitle(_track.ParentIndex.Value, name);
        void UpdateTitle(int index, string name)
            => Title = TitleText.Text = TitleCenter.Text = $"{name}{((Program.Project.FilePath != "")? $" - {Program.Project.FileName}" : "")}";

        void UpdateTopmost(bool value) => Topmost = value;

        void UpdateContentAlignment(bool value) => Root.ColumnDefinitions[0] = new ColumnDefinition(1, value? GridUnitType.Star : GridUnitType.Auto);

        public SelectionManager Selection = new SelectionManager();

        public TrackWindow() => new InvalidOperationException();

        public TrackWindow(Track track) {
            InitializeComponent();
            #if DEBUG
                this.AttachDevTools();
            #endif

            UpdateTopmost(Preferences.AlwaysOnTop);
            Preferences.AlwaysOnTopChanged += UpdateTopmost;

            _track = track;

            ChainViewer chainViewer = new ChainViewer(_track.Chain);
            chainViewer.PointerWheelChanged += Track_Scroll;

            Root = chainViewer.Get<Grid>("Layout");
            UpdateContentAlignment(Preferences.CenterTrackContents);
            Preferences.CenterTrackContentsChanged += UpdateContentAlignment;

            Contents.Content = chainViewer;

            SetEnabled();

            this.GetObservable(Visual.BoundsProperty).Subscribe(Bounds_Updated);
            TitleText.GetObservable(Visual.BoundsProperty).Subscribe(Bounds_Updated);
            TitleCenter.GetObservable(Visual.BoundsProperty).Subscribe(Bounds_Updated);
            CenteringLeft.GetObservable(Visual.BoundsProperty).Subscribe(Bounds_Updated);
            CenteringRight.GetObservable(Visual.BoundsProperty).Subscribe(Bounds_Updated);
        }

        void Loaded(object sender, EventArgs e) {
            Position = new PixelPoint(Position.X, Math.Max(0, Position.Y));

            Program.Project.PathChanged += UpdateTitle;
            _track.ParentIndexChanged += UpdateTitle;
            _track.NameChanged += UpdateTitle;
            UpdateTitle();
        }

        void Unloaded(object sender, CancelEventArgs e) {
            _track.Window = null;
            _track.ParentIndexChanged -= UpdateTitle;
            _track.NameChanged -= UpdateTitle;
            _track = null;

            ((ChainViewer)Contents.Content).PointerWheelChanged -= Track_Scroll;
            
            Program.Project.PathChanged -= UpdateTitle;
            Preferences.AlwaysOnTopChanged -= UpdateTopmost;
            Preferences.CenterTrackContentsChanged -= UpdateContentAlignment;

            Selection.Dispose();

            this.Content = null;

            App.WindowClosed(this);
        }
        
        public void Bounds_Updated(Rect bounds) {
            if (Bounds.IsEmpty || TitleText.Bounds.IsEmpty || TitleCenter.Bounds.IsEmpty || CenteringLeft.Bounds.IsEmpty || CenteringRight.Bounds.IsEmpty) return;

            int result = Convert.ToInt32((Bounds.Width - TitleText.Bounds.Width) / 2 <= Math.Max(CenteringLeft.Bounds.Width, CenteringRight.Bounds.Width) + 10);

            TitleText.Opacity = result;
            TitleCenter.Opacity = 1 - result;
        }

        public virtual void SetEnabled() => Background = (IBrush)Application.Current.Styles.FindResource(_track.Enabled? "ThemeControlMidBrush" : "ThemeControlLowBrush");

        void Track_Scroll(object sender, PointerWheelEventArgs e) => Contents.Offset = Contents.Offset.WithX(Contents.Offset.X - e.Delta.Y * 20);

        bool InChoke() => Selection.Start is Device &&
            Selection.Start.IParent is Chain &&
            ((Chain)Selection.Start.IParent).Parent?.GetType() == typeof(Choke);

        bool InMultiPreprocess() => Selection.Start is Device &&
            Selection.Start.IParent is ISelect &&
            ((ISelect)Selection.Start.IParent).IParentIndex == null &&
            ((ISelect)Selection.Start.IParent).IParent?.GetType() == typeof(Multi);

        async void Window_KeyDown(object sender, KeyEventArgs e) {
            if (e.Modifiers == App.ControlKey && e.Key == Key.P) {
                if (_track.Launchpad.Available && _track.Launchpad.GetType() != typeof(VirtualLaunchpad) || _track.Launchpad.Window != null)
                    LaunchpadWindow.Create(_track.Launchpad, this);
                
                return;
            }

            if (App.WindowKey(this, e) || await Program.Project.HandleKey(this, e) || Program.Project.Undo.HandleKey(e) || Selection.HandleKey(e))
                return;

            bool vertical = Selection.Start.GetType() == typeof(Chain);

            if (vertical) {
                if (e.Key == Key.Up) Selection.Move(false, e.Modifiers == InputModifiers.Shift);
                else if (e.Key == Key.Down) Selection.Move(true, e.Modifiers == InputModifiers.Shift);
                else if (e.Key == Key.Right) Selection.MoveChild();
                else if (e.Key == Key.Enter) Selection.Expand();
                else if (e.Key == Key.Left && Selection.Start.IParent.GetType() == typeof(Multi))
                    Selection.Select(((Multi)Selection.Start.IParent).Preprocess.Devices.Last());

            } else if (e.Key == Key.Left) {
                ISelect left = Selection.Selection.First();

                if (left.IParentIndex.Value == 0) {
                    if (InChoke()) {
                        Selection.Select((ISelect)((Chain)Selection.Start.IParent).Parent, e.Modifiers == InputModifiers.Shift);
                        return;

                    } else if (InMultiPreprocess()) return;
                }
                
                Selection.Move(false, e.Modifiers == InputModifiers.Shift);
            
            } else if (e.Key == Key.Right) {
                ISelect right = Selection.Selection.Last();

                if (right.IParentIndex.Value == right.IParent.IChildren.Count - 1) {
                    if (InChoke()) {
                        Selection.Select((ISelect)((Chain)Selection.Start.IParent).Parent, e.Modifiers == InputModifiers.Shift);
                        e.Modifiers = InputModifiers.None;

                    } else if (InMultiPreprocess()) {
                        Selection.Select((ISelect)((ISelect)Selection.Start.IParent).IParent, e.Modifiers == InputModifiers.Shift);
                        return;
                    }
                }
                
                Selection.Move(true, e.Modifiers == InputModifiers.Shift);

            } else if (e.Key == Key.Down) {
                if (Selection.Start.GetType() == typeof(Choke)) {
                    Chain chain = ((Choke)Selection.Start).Chain;
                    if (chain.Count > 0) Selection.Select(chain[0]);
                    return;
                }
                Selection.MoveChild();
            }
        }

        void Window_Focus(object sender, PointerPressedEventArgs e) => this.Focus();

        void MoveWindow(object sender, PointerPressedEventArgs e) {
            if (e.ClickCount == 2) Expand(e.Device);
            else BeginMoveDrag();

            Topmost = false;
            Topmost = Preferences.AlwaysOnTop;
            Activate();
        }

        void Minimize() => WindowState = WindowState.Minimized;
        
        void Expand(IPointerDevice e) {
            Point pointerRelative = e.GetPosition(this);

            PixelPoint pointerAbsolute = new PixelPoint(
                (int)(Position.X + pointerRelative.X),
                (int)(Position.Y + pointerRelative.Y)
            );

            Screen result = null;

            foreach (Screen screen in Screens.All)
                if (screen.Bounds.Contains(pointerAbsolute)) {
                    result = screen;
                    break;
                }

            if (result != null) {
                Position = new PixelPoint(result.Bounds.X, Position.Y);
                Width = result.Bounds.Width;
            }
        }

        void ResizeWest(object sender, PointerPressedEventArgs e) => BeginResizeDrag(WindowEdge.West);

        void ResizeEast(object sender, PointerPressedEventArgs e) => BeginResizeDrag(WindowEdge.East);

        public static void Create(Track track, Window owner) {
            if (track.Window == null) {
                track.Window = new TrackWindow(track) {Owner = owner};
                track.Window.Show();
                track.Window.Owner = null;
            } else {
                track.Window.WindowState = WindowState.Normal;
                track.Window.Activate();
            }
        }
    }
}