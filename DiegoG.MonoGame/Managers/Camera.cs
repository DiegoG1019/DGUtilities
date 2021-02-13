using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DiegoG.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DiegoG.MonoGame.Managers
{
    public class Camera : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPC([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private readonly object SyncRoot = new();

        public (float min, float max) ZoomLimit
        {
            get { lock (SyncRoot) return ZoomLimitField; }
            set { lock (SyncRoot) ZoomLimitField = value; NotifyPC(); }
        }
        (float min, float max) ZoomLimitField = (.35f, 2f);

        public float CurrentZoom
        {
            get { lock (SyncRoot) return CurrentZoomField; }
            protected set { lock (SyncRoot) CurrentZoomField = value; NotifyPC(); }
        }
        float CurrentZoomField = 1;

        public float GoalZoom
        {
            get { lock (SyncRoot) return GoalZoomField; }
            protected set { lock (SyncRoot) GoalZoomField = value; NotifyPC(); }
        }
        float GoalZoomField = 1;

        public Vector2 CurrentPosition
        {
            get { lock (SyncRoot) return CurrentPositionField; }
            protected set { lock (SyncRoot) CurrentPositionField = value; NotifyPC(); }
        }
        Vector2 CurrentPositionField = Vector2.Zero;

        public Vector2 GoalPosition
        {
            get { lock (SyncRoot) return GoalPositionField; }
            protected set { lock (SyncRoot) GoalPositionField = value; NotifyPC(); }
        }
        Vector2 GoalPositionField = Vector2.Zero;

        /// <summary>
        /// Sets both Goal and Current, gets Goal
        /// </summary>
        protected Vector2 BothPositions
        {
            get => GoalPosition;
            set
            {
                CurrentPosition = value;
                GoalPosition = value;
            }
        }

        /// <summary>
        /// Sets both Goal and Current, gets Goal
        /// </summary>
        protected float BothZooms
        {
            get => GoalZoom;
            set
            {
                CurrentZoom = value;
                GoalZoom = value;
            }
        }

        public Rectangle Bounds
        {
            get { lock (SyncRoot) return BoundsField; }
            protected set { lock (SyncRoot) BoundsField = value; NotifyPC(); }
        }
        Rectangle BoundsField;

        public Rectangle VisibleArea
        {
            get { lock (SyncRoot) return VisibleAreaField; }
            protected set { lock (SyncRoot) VisibleAreaField = value; NotifyPC(); }
        }
        Rectangle VisibleAreaField;

        public bool IsVisible(Rectangle drawBox) => VisibleArea.Contains(drawBox);

        public Matrix Transform
        {
            get { lock (SyncRoot) return TransformField; }
            protected set { lock (SyncRoot) TransformField = value; NotifyPC(); }
        }
        Matrix TransformField;

        public Camera(Viewport viewport) => Bounds = viewport.Bounds;

        private void UpdateVisibleArea()
        {
            var inverseViewMatrix = Matrix.Invert(Transform);

            var tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
            var tr = Vector2.Transform(new Vector2(Bounds.X, 0), inverseViewMatrix);
            var bl = Vector2.Transform(new Vector2(0, Bounds.Y), inverseViewMatrix);
            var br = Vector2.Transform(new Vector2(Bounds.Width, Bounds.Height), inverseViewMatrix);

            var min = new Vector2(
                MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
                MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
            var max = new Vector2(
                MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
                MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
            VisibleArea = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        private void UpdateMatrix()
        {
            Transform = Matrix.CreateTranslation(new Vector3(-CurrentPosition.X, -CurrentPosition.Y, 0)) *
                        Matrix.CreateScale(CurrentZoom) *
                        Matrix.CreateTranslation(new Vector3(Bounds.Width * 0.5f, Bounds.Height * 0.5f, 0));
            UpdateVisibleArea();
        }

        public void MoveCamera(Vector2 toPosition) => GoalPosition += toPosition;
        public void SetCamera(Vector2 toPosition) => GoalPosition = toPosition;
        public void ForceMoveCamera(Vector2 toPosition) => BothPositions += toPosition;
        public void ForceSetCamera(Vector2 toPosition) => BothPositions = toPosition;
        public void SetZoom(float toZoom) => GoalZoom = toZoom.Cap(ZoomLimit);
        public void ForceSetZoom(float toZoom) => CurrentZoom = toZoom.Cap(ZoomLimit);

        public async Task UpdateCameraAsync(GameTime gt, Viewport bounds) => await Task.Run(() => UpdateCamera(gt, bounds));
        public void UpdateCamera(GameTime gt, Viewport bounds)
        {
            Bounds = bounds.Bounds;
            float gtts = (float)gt.ElapsedGameTime.TotalSeconds;
            CurrentPosition -= (CurrentPosition - GoalPosition) * 10f * gtts;
            CurrentZoom -= (CurrentZoom - GoalZoom) * 10f * gtts;
            UpdateMatrix();
        }
    }
}
