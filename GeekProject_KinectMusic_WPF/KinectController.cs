using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace GeekProject_KinectMusic_WPF
{
    class KinectController
    {
        // When this is set, it will delegate responsibility to whatever method
        public delegate void KinectupdateDelegate();

        public event KinectupdateDelegate Ksupdate;

        KinectSensor ks = KinectSensor.KinectSensors[0];
        private Skeleton[] skeletons = new Skeleton[0];
        private List<Line> skeletonlines = new List<Line>();
        
        //track RIGHT HAND
       private  double[] RH_xyz = new double[3];
       //track LEFT HAND
       private double[] LH_xyz = new double[3];
        //track body centre
        private double[] Core_xyz = new double[3];

        public KinectController()
        {
            ks.Start();
            ks.ElevationAngle = 11;
            
            ks.SkeletonStream.Enable();
            ks.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
            ks.SkeletonFrameReady += this.OnSkeletonFrameReady;
        }

        private static readonly JointType[][] SkeletonSegmentRuns = new JointType[][]
        {
            new JointType[] 
            { 
                JointType.Head, JointType.ShoulderCenter  //, JointType.HipCenter 
            },
            new JointType[] 
            { 
                JointType.HandLeft, JointType.WristLeft, JointType.ElbowLeft, JointType.ShoulderLeft,
                JointType.ShoulderCenter,
                JointType.ShoulderRight, JointType.ElbowRight, JointType.WristRight, JointType.HandRight
            }
        };

        private Point GetJointPoint(Skeleton skeleton, JointType jointType)
        {
            var joint = skeleton.Joints[jointType];

            // Points are centered on the StickMen canvas and scaled according to its height allowing
            // approximately +/- 1.5m from center line.
            var point = new Point
            {
                X = (150 / 2) + (150 * joint.Position.X / 3),
                Y = (150 / 2) - (150 * joint.Position.Y / 3)
            };

            return point;
        }

        private void OnSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            // Get the frame.
            using (var frame = e.OpenSkeletonFrame())
            {
                // Ensure we have a frame.
                if (frame != null)
                {
                    // Resize the skeletons array if a new size (normally only on first call).
                    if (this.skeletons.Length != frame.SkeletonArrayLength)
                    {
                        this.skeletons = new Skeleton[frame.SkeletonArrayLength];
                    }

                    // Get the skeletons.
                    frame.CopySkeletonDataTo(this.skeletons);

                    this.DrawStickMen(this.skeletons);
                }
            }
        }


        private void DrawStickMen(Skeleton[] skeletons)
        {
            skeletonlines.Clear();

            int skeleton_id = 0;

            foreach (var skeleton in skeletons)
            {
                // Only draw tracked skeletons.
                if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                {
                    // Draw a background for the next pass.
                    Brush brush = Brushes.Green;

                    if (skeleton_id == 1) { brush = Brushes.LightBlue; }
                    if (skeleton_id == 2) { brush = Brushes.Lavender; }
                    if (skeleton_id == 3) { brush = Brushes.IndianRed; }
                    if (skeleton_id == 4) { brush = Brushes.HotPink; }

                    this.DrawStickMan(skeleton_id, skeleton, brush, 5);
                    skeleton_id++;
                }
            }
        }

      
        private void DrawStickMan(int skelid, Skeleton skeleton, Brush brush, int thickness)
        {
            foreach (var run in SkeletonSegmentRuns)
            {
                var next = this.GetJointPoint(skeleton, run[0]);
                for (var i = 1; i < run.Length; i++)
                {
                    var prev = next;
                    next = this.GetJointPoint(skeleton, run[i]);

                    var line = new Line
                    {
                        Stroke = brush,
                        StrokeThickness = thickness,
                        X1 = prev.X,
                        Y1 = prev.Y,
                        X2 = next.X,
                        Y2 = next.Y,
                        StrokeEndLineCap = PenLineCap.Round,
                        StrokeStartLineCap = PenLineCap.Round
                    };

                    //if only tracking upper body then need to remove lines to mid screen
                    skeletonlines.Add(line);
                }
            }

            if (skelid == 0)
            {

                //Right Hand
                RH_xyz[0] = skeleton.Joints[JointType.HandRight].Position.X;
                RH_xyz[1] = skeleton.Joints[JointType.HandRight].Position.Y;
                RH_xyz[2] = skeleton.Joints[JointType.HandRight].Position.Z;

                //Left Hand
                LH_xyz[0] = skeleton.Joints[JointType.HandLeft].Position.X;
                LH_xyz[1] = skeleton.Joints[JointType.HandLeft].Position.Y;
                LH_xyz[2] = skeleton.Joints[JointType.HandLeft].Position.Z;

                //core of skeleton
                Core_xyz[0] = skeleton.Joints[JointType.ShoulderCenter].Position.X;
                Core_xyz[1] = skeleton.Joints[JointType.ShoulderCenter].Position.Y;
                Core_xyz[2] = skeleton.Joints[JointType.ShoulderCenter].Position.Z;

            }
            Ksupdate();  //raises an event in MainWindow to say an update happened
        }

        public List<Line> getStickPeople()
        {
         return skeletonlines;
        }

        public double[] getRightHandXYZ()
        {
            return RH_xyz;
        }

        public double[] getLeftHandXYZ()
        {

            return LH_xyz;
        }


        public double[] getCoreXYZ()
        {
            return Core_xyz;
        }
    }
}
