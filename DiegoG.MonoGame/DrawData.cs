using DiegoG.Utilities;
using DiegoG.Utilities.Measures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DiegoG.MonoGame
{
    /// <summary>
    /// Represents a storage location for SpriteBatch drawing parameters, Thread-Safe and implements INotifyPropertyChanged
    /// </summary>
    public class DrawData : INotifyPropertyChanged
    {
        public static bool WarnIfRelativeToIsNull { get; set; } = false;
        public static bool WarnIfRenderAreaIsNull { get; set; } = false;

        /// <summary>
        /// Disable Notify Property Changed
        /// </summary>
        bool DNPC
        {
            get
            {
                lock (dnpc)
                    return Dnpc;
            }
            set
            {
                lock (dnpc)
                    Dnpc = value;
            }
        }
        bool Dnpc;
        readonly object dnpc = new();

        private readonly object Sync = new();

        public Func<Matrix> PositionTransformationMatrixRef
        {
            get
            {
                lock (Sync)
                    return PositionTransformationMatrixRefField;
            }
            set { lock (Sync) PositionTransformationMatrixRefField = value; NotifyPC(); }
        }
        Func<Matrix> PositionTransformationMatrixRefField;
        public DrawData SetPositionTransformationMatrixRef(Func<Matrix> rt) { PositionTransformationMatrixRef = rt; return this; }

        /// <summary>
        /// A specified boundary that defines whether this DrawBox should be rendered or not. Set to null to ignore. Ignored for Canvases.
        /// </summary>
        public Func<Rectangle> RenderAreaRef
        {
            get
            {
                lock (Sync)
                    return RenderAreaRefField;
            }
            set { lock (Sync) RenderAreaRefField = value; NotifyPC(); }
        }
        Func<Rectangle> RenderAreaRefField;
        public DrawData SetRenderAreaRef(Func<Rectangle> ra) { RenderAreaRef = ra; return this; }

        public Rectangle? SourceRectangle
        {
            get
            {
                lock (Sync)
                    return SourceRectangleField;
            }
            set { lock (Sync) SourceRectangleField = value; NotifyPC(); }
        }
        Rectangle? SourceRectangleField;
        public DrawData SetSourceRectangle(Rectangle? r) { SourceRectangle = r; return this; }

        /// <summary>
        /// Setting DestinationRectangleRef to not null will ignore Position, RelativePosition and PositionTransformation. Setting this property will override DestinationRectangle
        /// </summary>
        public Func<Rectangle> DestinationRectangleRef
        {
            get { lock (Sync) return DestinationRectangleRefField; }
            set { lock (Sync) DestinationRectangleRefField = value; DestinationRectangleField = null; NotifyPC(); NotifyPC(nameof(DestinationRectangle)); }
        }
        Func<Rectangle> DestinationRectangleRefField;

        /// <summary>
        /// Setting DestinationRectangle to not null will ignore Position, RelativePosition and PositionTransformation. Setting this property will null DestinationRectangleRef
        /// </summary>
        public Rectangle? DestinationRectangle
        {
            get
            {
                lock (Sync)
                    return DestinationRectangleField ?? DestinationRectangleRefField();
            }
            set { lock (Sync) DestinationRectangleField = value; NotifyPC(); }
        }
        Rectangle? DestinationRectangleField;
        public DrawData SetDestinationRectangle(Rectangle? r) { DestinationRectangle = r; return this; }

        /// <summary>
        /// While NOT null, controlled Texture will be ignored. Setting this to null will default to ControlledTexture
        /// </summary>
        public Texture2D TextureOverride
        {
            get { lock(Sync) return TextureOverrideField; }
            set
            {
                lock (Sync)
                    TextureOverrideField = value;
                NotifyPC(nameof(Texture));
                NotifyPC();
            }
        }
        Texture2D TextureOverrideField;

        public bool TextureIsControlled => TextureOverride is null;
        public Texture2D Texture
        {
            get
            {
                lock (Sync)
                    return TextureOverrideField ?? TextureField.Asset;
            }
        }
        public Assets.ControlledTexture2D ControlledTexture
        {
            get
            {
                lock (Sync)
                    return TextureField;
            }
            set
            {
                lock (Sync)
                    TextureField = value;

                NotifyPC(nameof(Texture));
                NotifyPC();
            }
        }
        Assets.ControlledTexture2D TextureField;
        public DrawData SetTextureRef(Assets.ControlledTexture2D tref) { ControlledTexture = tref; return this; }

        public Vector2 TextureCenter { get; private set; }

        /// <summary>
        /// If not null, SpecificPosition will be ignored.
        /// </summary>
        public Func<LengthVector2> PositionRelativeVectorRef
        {
            get
            {
                lock (Sync)
                    return PositionRelativeField;
            }
            set
            {
                lock (Sync)
                    PositionRelativeField = value;
                NotifyPC();
            }
        }
        Func<LengthVector2> PositionRelativeField;
        public DrawData SetPositionRelativeVectorRef(Func<LengthVector2> posre) { PositionRelativeVectorRef = posre; return this; }

        /// <summary>
        /// Relative to PositionRelativeVectorRef. if PositionRelativeVectorRef is null, it will be relative to 0, 0
        /// </summary>
        public LengthVector2 RelativePosition
        {
            get => SetPositionField;
            set { SetPositionField = value; NotifyPC(); }
        }
        LengthVector2 SetPositionField;
        public DrawData SetRelativePosition(LengthVector2 specpos) { RelativePosition = specpos; return this; }

        public LengthVector2 Position => PositionRelativeVectorRef is not null ? RelativePosition + PositionRelativeVectorRef() : RelativePosition;

        public float LayerDepth
        {
            get
            {
                lock (Sync)
                    return LayerDepthField;
            }
            set
            {
                lock (Sync)
                    LayerDepthField = value;

                NotifyPC();
            }
        }
        float LayerDepthField;
        public DrawData SetLayerDepth(float depth) { LayerDepth = depth; return this; }

        /// <summary>
        /// Setter notifies for Fx, not Flipped
        /// </summary>
        public (bool Horizontal, bool Vertical) Flipped
        {
            get => (Fx == SpriteEffects.FlipHorizontally, Fx == SpriteEffects.FlipVertically);
            set => Fx = (SpriteEffects)((value.Horizontal ? 1 : 0) | (value.Vertical ? 2 : 0));
        }
        /// <summary>
        /// Setter notifies for Fx, not Flipped
        /// </summary>
        public DrawData SetFlipped((bool Horizontal, bool Vertical) flipped) { Flipped = flipped; return this; }

        /// <summary>
        /// Setter notifies for Fx, not FlippedHorizontal
        /// </summary>
        public bool FlippedHorizontal
        {
            get => Flipped.Horizontal;
            set => Flipped = (value, Flipped.Vertical);
        }
        /// <summary>
        /// Setter notifies for Fx, not FlippedHorizontal
        /// </summary>
        public DrawData SetFlippedHorizontal(bool flipped) { FlippedHorizontal = flipped; return this; }

        /// <summary>
        /// Setter notifies for Fx, not FlippedVertical
        /// </summary>
        public bool FlippedVertical
        {
            get => Flipped.Vertical;
            set => Flipped = (Flipped.Horizontal, value);
        }
        /// <summary>
        /// Setter notifies for Fx, not FlippedVertical
        /// </summary>
        public DrawData SetFlippedVertical(bool flipped) { FlippedVertical = flipped; return this; }

        public Color Color
        {
            get
            {
                lock (Sync)
                    return ColorField;
            }
            set
            {
                lock (Sync)
                    ColorField = value;

                NotifyPC();
            }
        }
        Color ColorField = Color.White;
        public DrawData SetColor(Color color) { Color = color; return this; }

        public Angle Rotation
        {
            get
            {
                lock (Sync)
                    return RotationField;
            }
            set
            {
                lock (Sync)
                    RotationField = value;

                NotifyPC();
            }
        }
        Angle RotationField;
        public DrawData SetRotation(Angle rot) { Rotation = rot; return this; }
        public DrawData SetRotationDeg(double rot) => SetRotation(new((decimal)rot, Angle.Units.Degree));
        public DrawData SetRotationRad(float rot) => SetRotation(new((decimal)rot, Angle.Units.Radian));
        public DrawData SetRotationGrad(float rot) => SetRotation(new((decimal)rot, Angle.Units.Gradian));

        /// <summary>
        /// Setting this will set OriginCentered back to false
        /// </summary>
        public Vector2 Origin
        {
            get => OriginField;
            set { OriginField = value; OriginCentered = false; NotifyPC(); }
        }
        Vector2 OriginField;
        public DrawData SetOrigin(Vector2 origin) { Origin = origin; return this; }

        /// <summary>
        /// Setting this to true will override Vector2 Origin
        /// </summary>
        public bool OriginCentered
        {
            get
            {
                lock (Sync)
                    return OriginCenteredField;
            }
            set
            {
                lock (Sync)
                    OriginCenteredField = value;

                NotifyPC();
            }
        }
        bool OriginCenteredField;
        public DrawData SetOriginCentered(bool oricent) { OriginCentered = oricent; return this; }

        public Vector2 Scale
        {
            get
            {
                lock (Sync)
                    return ScaleField;
            }
            set
            {
                lock (Sync)
                    ScaleField = value;

                NotifyPC();
            }
        }
        Vector2 ScaleField = Vector2.One;
        public DrawData SetScale(Vector2 scale) { Scale = scale; return this; }

        public SpriteEffects Fx
        {
            get
            {
                lock (Sync)
                    return SpriteEffectsField;
            }
            set
            {
                lock (Sync)
                    SpriteEffectsField = value;
                NotifyPC();
            }
        }
        SpriteEffects SpriteEffectsField;
        public DrawData SetFx(SpriteEffects fx) { Fx = fx; return this; }

        public Vector2 TruePosition => Vector2.Transform(Position.ToVector2(Length.Units.Pixel), PositionTransformationMatrixRef());
        public Vector2 CurrentOrigin => OriginCentered ? TextureCenter : Origin;

        public Rectangle DrawBox
        {
            get
            {
                if (DestinationRectangle != null)
                    return (Rectangle)DestinationRectangle;
                if (ReCalcDrawBox.CheckToggle())
                {
                    Vector2 newvector = TruePosition;
                    newvector *= Scale;
                    newvector -= CurrentOrigin;
                    Point newpoint = newvector.ToPoint();
                    DrawBoxCache = new Rectangle(newpoint.X, newpoint.Y, Texture.Width, Texture.Height);
                }
                return DrawBoxCache;
            }
        }
        private Rectangle DrawBoxCache;
        private bool ReCalcDrawBox = true;

        public DrawData(Func<Rectangle> renderAreaRef, Func<LengthVector2> positionRelativeVectorRef = null, Func<Matrix> positionTransformationMatrixRef = null)
        {
            DNPC = true;

            RelativePosition = new LengthVector2(Vector2.Zero, Length.Units.Pixel);
            LayerDepth = 0f;
            Color = Color.White;
            Rotation = new();
            Scale = Vector2.One;
            Origin = Vector2.Zero;
            Fx = SpriteEffects.None;
            if (renderAreaRef is not null)
                RenderAreaRef = renderAreaRef;
            if(positionTransformationMatrixRef is not null)
                PositionTransformationMatrixRef = positionTransformationMatrixRef;
            if (positionRelativeVectorRef is not null)
                PositionRelativeVectorRef = positionRelativeVectorRef;

            DNPC = false;

            PropertyChanged += DrawData_PropertyChanged;
        }
        public Info NewEmptyStateInfo() => new Info() { DrawData = this };
        public Info ExportStateInfo() => new Info(this);
        public void ImportStateInfo(Info ddi)
        {
            if (MustReExport.CheckSetFalse())
                OriginalStateInfo = ExportStateInfo();
            DNPC = true;

                SourceRectangle = ddi.SourceRectangle ?? SourceRectangle;
                DestinationRectangle = ddi.DestinationRectangle ?? DestinationRectangle;
                LayerDepth = ddi.LayerDepth ?? LayerDepth;
                Rotation = ddi.Rotation ?? Rotation;
                Origin = ddi.Origin ?? Origin;
                OriginCentered = ddi.OriginCentered ?? OriginCentered;
                Scale = ddi.Scale ?? Scale;
                Fx = ddi.Fx ?? Fx;

            DNPC = false;
        }

        /// <summary>
        /// Set ONLY to state right before Importing. Will only set again after changing MANUALLY, not after importing. This means DrawData can import n times and still go back to its state before an import chain
        /// </summary>
        public Info OriginalStateInfo
        {
            get
            {
                lock (Sync)
                    return PastInfoField;
            }
            private set
            {
                lock (Sync)
                    PastInfoField = value;
                NotifyPC();
            }
        }
        public bool MustReExport = true;
        Info PastInfoField;

        private readonly ConcurrentQueue<Info> DDIQueue = new();
        public int StateInfoQueueCount => DDIQueue.Count;

        /// <summary>
        /// ImportNextDrawDataInfo and DrawAllDrawDataInfo will both check if DDIQueue is empty automatically.
        /// </summary>
        public bool StateInfoQueueIsEmpty => DDIQueue.IsEmpty;

        /// <summary>
        /// Enqueued DrawData states are final. They cannot be modified once they are in, they can only be used.
        /// </summary>
        /// <param name="ddi"></param>
        private void EnqueueDrawDataInfo(Info ddi)
        {
            if (ReferenceEquals(ddi.DrawData, this))
                DDIQueue.Enqueue(ddi);
        }
        public bool ImportNextStateInfo()
        {
            var success = DDIQueue.TryDequeue(out Info ddi);
            if (success)
                ImportStateInfo(ddi);
            return success;
        }
        public void DrawAllStateInfo(SpriteBatch spriteBatch)
        {
            while (!DDIQueue.IsEmpty)
            {
                ImportNextStateInfo();
                DrawTo(spriteBatch);
            }
        }

        public Info NewStateInfo() => new Info(this);

        public void DrawTo(SpriteBatch spriteBatch)
        {
            if(PositionTransformationMatrixRef is null && WarnIfRelativeToIsNull)
                Log.Debug("RelativeTo property of a DrawData Object is null. Warn if Relative To is Null is set to true, set a breakpoint here and use debugger to trace the cause");
            if (RenderAreaRef is null)
            {
                if(WarnIfRenderAreaIsNull)
                    Log.Debug("RenderArea property of a DrawData Object is null. Warn if Render Area is Null is set to true, set a breakpoint here and use debugger to trace the cause");
                goto draw;
            }

            if (!RenderAreaRef().Contains(DrawBox))
                return;
            draw:;

            Vector2 realpos = PositionTransformationMatrixRef is not null ? Vector2.Transform(Position.ToVector2(Length.Units.Pixel), PositionTransformationMatrixRef()) : Position.ToVector2(Length.Units.Pixel);
            if (DestinationRectangle is not null)
            {
                Rectangle desrec = (Rectangle)DestinationRectangle;
                desrec.Inflate(desrec.Width * Scale.X, desrec.Height * Scale.Y);

                spriteBatch.Draw(Texture, desrec, SourceRectangle, Color, Rotation.DegreeF, CurrentOrigin, Fx, LayerDepth);
                return;
            }
            spriteBatch.Draw(Texture, realpos, SourceRectangle, Color, Rotation.DegreeF, CurrentOrigin, Scale, Fx, LayerDepth);
        }
        public void DrawTo(Canvas canvas, bool clear = false, string debug = "", string verbose = "")
            => canvas.Draw(this, clear, debug, verbose);

        private readonly static string[] TextureCenterChecks = { nameof(Texture) };
        private readonly static string[] DrawBoxCacheChecks = { nameof(RelativePosition), nameof(PositionTransformationMatrixRef), nameof(Scale), nameof(Origin), nameof(Texture), nameof(OriginCentered) };
        private void DrawData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OriginalStateInfo))
                return;

            MustReExport = true;

            if (TextureCenterChecks.Any(d => d == e.PropertyName))
            {
                TextureCenter = new(Texture.Width / 2, Texture.Height / 2);
                return;
            }

            if (DrawBoxCacheChecks.Any(d => d == e.PropertyName))
            {
                ReCalcDrawBox = true;
                return;
            }
        }

        protected void NotifyPC([CallerMemberName] string propertyName = "")
        {
            if (!DNPC)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// A record dedicated to DrawData state storage. All properties set to null will be ignored when importing
        /// </summary>
        public record Info
        {
            public DrawData DrawData { get; init; }

            public Rectangle? SourceRectangle { get; set; }
            public Info DefaultSourceRectangle() { SourceRectangle = null; return this; }
            public Info SetSourceRectangle(Rectangle r) { SourceRectangle = r; return this; }

            public Rectangle? DestinationRectangle { get; set; }
            public Info DefaultDestinationRectangle() { DestinationRectangle = null; return this; }
            public Info SetDestinationRectangle(Rectangle r) { DestinationRectangle = r; return this; }

            public float? LayerDepth { get; set; }
            public Info SetLayerDepth(float depth) { LayerDepth = depth; return this; }

            public Angle Rotation { get; set; }
            public Info DefaultRotation() { Rotation = null; return this; }
            public Info SetRotation(Angle rot) { Rotation = rot; return this; }
            public Info SetRotationDeg(float rot) => SetRotation(new((decimal)rot, Angle.Units.Degree));
            public Info SetRotationRad(float rot) => SetRotation(new((decimal)rot, Angle.Units.Radian));
            public Info SetRotationGrad(float rot) => SetRotation(new((decimal)rot, Angle.Units.Gradian));

            public Vector2? Origin { get; set; }
            public Info DefaultOrigin() { Origin = null; return this; }
            public Info SetOrigin(Vector2 origin) { Origin = origin; return this; }

            public bool? OriginCentered { get; set; }
            public Info DefaultOriginCentered() { OriginCentered = null; return this; }
            public Info SetOriginCentered(bool origincenter) { OriginCentered = origincenter; return this; }

            public Vector2? Scale { get; set; }
            public Info DefaultScale() { Scale = null; return this; }
            public Info SetScale(Vector2 scale) { Scale = scale; return this; }

            public (bool Horizontal, bool Vertical) Flipped
            {
                get => (Fx == SpriteEffects.FlipHorizontally, Fx == SpriteEffects.FlipVertically);
                set => Fx = (SpriteEffects)((value.Horizontal ? 1 : 0) | (value.Vertical ? 2 : 0));
            }
            public Info DefaultFlipped() => DefaultFx();
            public Info SetFlipped((bool Horizontal, bool Vertical) flipped) { Flipped = flipped; return this; }

            public bool FlippedHorizontal
            {
                get => Flipped.Horizontal;
                set => Flipped = (value, Flipped.Vertical);
            }
            public Info SetFlippedHorizontal(bool flipped) { FlippedHorizontal = flipped; return this; }

            public bool FlippedVertical
            {
                get => Flipped.Vertical;
                set => Flipped = (Flipped.Horizontal, value);
            }
            public Info SetFlippedVertical(bool flipped) { FlippedVertical = flipped; return this; }

            public SpriteEffects? Fx { get; set; }
            public Info DefaultFx() { Fx = null; return this; }
            public Info SetFx(SpriteEffects fx) { Fx = fx; return this; }

            /// <summary>
            /// Clones the current instance and enqueues it into the given DrawData object. Further modification of this record will not be reflected on the enqueued list.
            /// </summary>
            public void CloneAndEnqueue() => DrawData.EnqueueDrawDataInfo(new(this));

            public Info(Info inf)
            {
                DrawData = inf.DrawData;
                SourceRectangle = inf.SourceRectangle;
                DestinationRectangle = inf.DestinationRectangle;
                LayerDepth = inf.LayerDepth;
                Rotation = Rotation is not null ? new(inf.Rotation) : null;
                Origin = inf.Origin;
                OriginCentered = inf.OriginCentered;
                Scale = inf.Scale;
                Fx = inf.Fx;
            }
            internal Info() { }
            internal Info(DrawData dd)
            {
                DrawData = dd;
                SourceRectangle = dd.SourceRectangle;
                DestinationRectangle = dd.DestinationRectangle;
                LayerDepth = dd.LayerDepth;
                Rotation = new(dd.Rotation);
                Origin = dd.Origin;
                OriginCentered = dd.OriginCentered;
                Scale = dd.Scale;
                Fx = dd.Fx;
            }
        }
    }
}
