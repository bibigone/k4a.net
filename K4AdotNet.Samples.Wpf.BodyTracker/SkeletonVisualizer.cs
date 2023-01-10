using K4AdotNet.BodyTracking;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace K4AdotNet.Samples.Wpf.BodyTracker
{
    internal sealed class SkeletonVisualizer
    {
        public SkeletonVisualizer(Dispatcher dispatcher, int widthPixels, int heightPixels, Func<Joint, Float2?> jointToImageProjector)
        {
            if (dispatcher.Thread != Thread.CurrentThread)
            {
                throw new InvalidOperationException(
                    "Call this constructor from UI thread please, because it creates ImageSource object for UI");
            }

            this.dispatcher = dispatcher;
            this.jointToImageProjector = jointToImageProjector;

            // WPF stuff to draw skeleton
            drawingRect = new(0, 0, widthPixels, heightPixels);
            drawingGroup = new(){ ClipGeometry = new RectangleGeometry(drawingRect) };
            ImageSource = new DrawingImage(drawingGroup);

            // Default visualization settings
            JointCircleRadius = heightPixels / 70;
            JointBorder = new(Brushes.Black, JointCircleRadius / 4);
            BonePen = new(Brushes.White, JointCircleRadius * 2 / 3);
            JointFill = Brushes.LightGreen;
            JointFillLow = Brushes.Yellow;
            JointFillNone = Brushes.OrangeRed;
        }

        /// <summary>
        /// Image with visualized skeletons (<c>Astra.BodyFrame</c>). You can use this property in WPF controls/windows.
        /// </summary>
        public ImageSource ImageSource { get; }

        #region Visualization settings (colors, sizes, etc.)

        /// <summary>Visualization setting: pen for border around joint circles.</summary>
        public Pen JointBorder { get; set; }

        /// <summary>Visualization setting: brush to fill joint circle.</summary>
        public Brush JointFill { get; set; }

        /// <summary>Visualization setting: brush to fill joint circle for Low confidence.</summary>
        public Brush JointFillLow { get; set; }

        /// <summary>Visualization setting: brush to fill joint circle for None confidence.</summary>
        public Brush JointFillNone { get; set; }

        /// <summary>Visualization setting: radius of joint circle.</summary>
        public double JointCircleRadius { get; set; }

        /// <summary>Visualization setting: pen to draw bone.</summary>
        public Pen BonePen { get; set; }

        #endregion

        public void Update(BodyFrame? bodyFrame)
        {
            // Is compatible?
            if (bodyFrame == null || bodyFrame.IsDisposed)
                return;

            // 1st step: get information about bodies
            lock (skeletonsSync)
            {
                var bodyCount = bodyFrame.BodyCount;
                if (skeletons.Length != bodyCount)
                    skeletons = new Skeleton[bodyCount];
                for (var i = 0; i < bodyCount; i++)
                    bodyFrame.GetBodySkeleton(i, out skeletons[i]);
            }

            // 2nd step: we can update ImageSource only from its owner thread (as a rule, UI thread)
            dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(DrawBodies));
        }

        private void DrawBodies()
        {
            lock (skeletonsSync)
            {
                using (var dc = drawingGroup.Open())
                {
                    // Our image must fit depth map, this why we have to draw transparent rectangle to set size of our image
                    // There is no other way to set size of DrawingImage
                    dc.DrawRectangle(Brushes.Transparent, null, drawingRect);

                    // Draw skeleton for each tracked body
                    foreach (var skeleton in skeletons)
                    {
                        DrawBones(dc, skeleton);
                        DrawJoints(dc, skeleton);
                    }
                }
            }
        }

        // Draws bone as line (stick) between two joints
        private void DrawBones(DrawingContext dc, Skeleton skeleton)
        {
            foreach (var jointType in JointTypes.All)
            {
                if (!jointType.IsRoot() && !jointType.IsFaceFeature())
                {
                    var parentJoint = skeleton[jointType.GetParent()];
                    var endJoint = skeleton[jointType];
                    var parentPoint2D = ProjectJointToImage(parentJoint);
                    var endPoint2D = ProjectJointToImage(endJoint);
                    if (parentPoint2D.HasValue && endPoint2D.HasValue)
                        dc.DrawLine(BonePen, parentPoint2D.Value, endPoint2D.Value);
                }
            }
        }

        private Point? ProjectJointToImage(Joint joint)
        {
            var res = jointToImageProjector(joint);
            if (!res.HasValue)
                return null;
            return new(res.Value.X, res.Value.Y);
        }

        // Draws joint as circle
        private void DrawJoints(DrawingContext dc, Skeleton skeleton)
        {
            foreach (var jointType in JointTypes.All)
            {
                var joint = skeleton[jointType];
                var point2D = ProjectJointToImage(joint);
                if (point2D.HasValue)
                {
                    var radius = JointCircleRadius;
                    // smaller radius for face features (eyes, ears, nose)
                    if (jointType.IsFaceFeature())
                        radius /= 2;
                    var fillBrush = JointConfidenceLevelToToBrush(joint.ConfidenceLevel);
                    dc.DrawEllipse(fillBrush, JointBorder, point2D.Value, radius, radius);
                }
            }
        }

        private Brush JointConfidenceLevelToToBrush(JointConfidenceLevel confidenceLevel)
        {
            switch (confidenceLevel)
            {
                case JointConfidenceLevel.None: return JointFillNone;
                case JointConfidenceLevel.Low: return JointFillLow;
                default: return JointFill;
            }
        }

        private readonly Dispatcher dispatcher;
        private readonly Func<Joint, Float2?> jointToImageProjector;
        private readonly Rect drawingRect;
        private readonly DrawingGroup drawingGroup;
        private Skeleton[] skeletons = Array.Empty<Skeleton>();
        private readonly object skeletonsSync = new();
    }
}
