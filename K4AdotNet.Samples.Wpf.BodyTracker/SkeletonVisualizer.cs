using K4AdotNet.BodyTracking;
using System;
using System.Collections.Generic;
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
            this.widthPixels = widthPixels;
            this.heightPixels = heightPixels;
            this.jointToImageProjector = jointToImageProjector;

            // WPF stuff to draw skeleton
            drawingRect = new Rect(0, 0, widthPixels, heightPixels);
            drawingGroup = new DrawingGroup { ClipGeometry = new RectangleGeometry(drawingRect) };
            ImageSource = new DrawingImage(drawingGroup);

            // Default visualization settings
            JointCircleRadius = widthPixels / 90;
            JointBorder = new Pen(Brushes.Black, JointCircleRadius / 4);
            BonePen = new Pen(Brushes.White, JointCircleRadius * 2 / 3);
            JointFill = Brushes.LightGray;
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

        /// <summary>Visualization setting: radius of joint circle.</summary>
        public double JointCircleRadius { get; set; }

        /// <summary>Visualization setting: pen to draw bone.</summary>
        public Pen BonePen { get; set; }

        #endregion

        public void Update(BodyFrame bodyFrame)
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
            foreach (var bone in bones)
            {
                var parentJoint = skeleton[bone.ParentJointType];
                var endJoint = skeleton[bone.EndJointType];
                var parentPoint2D = ProjectJointToImage(parentJoint);
                var endPoint2D = ProjectJointToImage(endJoint);
                if (parentPoint2D.HasValue && endPoint2D.HasValue)
                    dc.DrawLine(BonePen, parentPoint2D.Value, endPoint2D.Value);
            }
        }

        private Point? ProjectJointToImage(Joint joint)
        {
            var res = jointToImageProjector(joint);
            if (!res.HasValue)
                return null;
            return new Point(res.Value.X, res.Value.Y);
        }

        // Draws joint as circle
        private void DrawJoints(DrawingContext dc, Skeleton skeleton)
        {
            foreach (var joint in skeleton)
            {
                var point2D = ProjectJointToImage(joint);
                if (point2D.HasValue)
                    dc.DrawEllipse(JointFill, JointBorder, point2D.Value, JointCircleRadius, JointCircleRadius);
            }
        }

        private readonly Dispatcher dispatcher;
        private readonly int widthPixels;
        private readonly int heightPixels;
        private readonly Func<Joint, Float2?> jointToImageProjector;
        private readonly Rect drawingRect;
        private readonly DrawingGroup drawingGroup;
        private Skeleton[] skeletons = new Skeleton[0];
        private readonly object skeletonsSync = new object();

        #region Bones structure

        /// <summary>
        /// Bone is connector of two joints
        /// </summary>
        private struct Bone
        {
            public JointType ParentJointType;
            public JointType EndJointType;

            public Bone(JointType parentJointType, JointType endJointType)
            {
                ParentJointType = parentJointType;
                EndJointType = endJointType;
            }
        }

        /// <summary>
        /// Skeleton structure = list of bones = list of joint connectors
        /// </summary>
        private static readonly IReadOnlyList<Bone> bones = new Bone[]
        {
            // spine, neck, and head
            new Bone(JointType.Pelvis, JointType.SpineNaval),
            new Bone(JointType.SpineNaval, JointType.SpineChest),
            new Bone(JointType.SpineChest, JointType.Neck),
            new Bone(JointType.Neck, JointType.Head),
            // left shoulder and arm
            new Bone(JointType.SpineChest, JointType.ClavicleLeft),
            new Bone(JointType.ClavicleLeft, JointType.ShoulderLeft),
            new Bone(JointType.ShoulderLeft, JointType.ElbowLeft),
            new Bone(JointType.ElbowLeft, JointType.WristLeft),
            // right shoulder and arm
            new Bone(JointType.SpineChest, JointType.ClavicleRight),
            new Bone(JointType.ClavicleRight, JointType.ShoulderRight),
            new Bone(JointType.ShoulderRight, JointType.ElbowRight),
            new Bone(JointType.ElbowRight, JointType.WristRight),
            // left leg
            new Bone(JointType.Pelvis, JointType.HipLeft),
            new Bone(JointType.HipLeft, JointType.KneeLeft),
            new Bone(JointType.KneeLeft, JointType.AnkleLeft),
            new Bone(JointType.AnkleLeft, JointType.FootLeft),
            // right leg
            new Bone(JointType.Pelvis, JointType.HipRight),
            new Bone(JointType.HipRight, JointType.KneeRight),
            new Bone(JointType.KneeRight, JointType.AnkleRight),
            new Bone(JointType.AnkleRight, JointType.FootRight),
        };

        #endregion

    }
}
