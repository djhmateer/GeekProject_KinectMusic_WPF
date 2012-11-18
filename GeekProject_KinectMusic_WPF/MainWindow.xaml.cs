using System;
using System.Threading;
using System.Windows;
using System.Windows.Shapes;
using MusicToolKit;

namespace GeekProject_KinectMusic_WPF
{
    public partial class MainWindow : Window
    {
        // For playing notes
        MidiModule midiMod = new MidiModule();
       

        // For getting hand information
        KinectController kinect;
        
        bool noteCurrentlyOn = false;
        int last_v = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void KinectTest_Click(object sender, RoutedEventArgs e)
        {
            // Connect to a Kinect
            kinect = new KinectController();

            // Set up the event Ksupdate to fire ProcessKinectUpdate here
            kinect.Ksupdate += new KinectController.KinectupdateDelegate(ProcessKinectUpdate);
        }

        // Main controller method
        private void ProcessKinectUpdate()
        {
            // Draw stickpeople
            this.StickPeopleCanvas.Children.Clear();
            foreach (Line l in kinect.getStickPeople())
            {
                this.StickPeopleCanvas.Children.Add(l);
            }

            //get Relative RH and LH positions
            double relativeRHx = kinect.getRightHandXYZ()[0] - kinect.getCoreXYZ()[0];
            double relativeRHy = kinect.getRightHandXYZ()[1] - kinect.getCoreXYZ()[1];
            double relativeRHz = kinect.getRightHandXYZ()[2] - kinect.getCoreXYZ()[2];

            double relativeLHx = kinect.getLeftHandXYZ()[0] - kinect.getCoreXYZ()[0];
            double relativeLHy = kinect.getLeftHandXYZ()[1] - kinect.getCoreXYZ()[1];
            double relativeLHz = kinect.getLeftHandXYZ()[2] - kinect.getCoreXYZ()[2];

            // Show in text right hand xyz
            this.rhx.Content = relativeRHx.ToString("0.00");
            this.rhy.Content = relativeRHy.ToString("0.00");
            this.rhz.Content = relativeRHz.ToString("0.00");

            //LH Z
            this.lhy.Content = relativeLHy.ToString("0.00");
            this.lhz.Content = relativeLHz.ToString("0.00");

            int v = Convert.ToInt16(relativeRHx * 35) + 40;


            if (relativeRHz < -0.2 && !noteCurrentlyOn || last_v != v)
            {
                //midimod.PlayShortNote(v, 100);
                midiMod.NoteOn(v, 100,1);
                noteCurrentlyOn = true;
            }

            if (relativeRHz > -0.2 && noteCurrentlyOn)
            {
                midiMod.NoteOff(1);
                noteCurrentlyOn = false;
            }

            last_v = v;

            //sound expression with LEFT Hand


            int LHcontrollerValue = Convert.ToInt16(relativeLHy * 95) + 65;
            midiMod.ContinuousContoller(LHcontrollerValue,1);



            if (relativeLHz < -0.45)
            {
                midiMod.octShift = -12;
                midiMod.NoteOn(v, 100,2);

            }
            else
            {
                midiMod.NoteOff(2);


            }


            ////patch change with LEFT Hand
            //if (relativeLHz > -0.2)
            //{
            //    int LHcontrollerValue = Convert.ToInt16(relativeLHx * 50) + 50;
            //    midiMod.ChangePatch(LHcontrollerValue);

            //}
        }

        public void MakeMusic()
        {
            while (true)
            {
                System.Console.WriteLine(".");
                midiMod.NoteOn(60, 100,1);
                System.Threading.Thread.Sleep(100);
                midiMod.NoteOn(67, 100,1);
                System.Threading.Thread.Sleep(70);
                midiMod.NoteOff(1);
                System.Threading.Thread.Sleep(100);
            }
        }

        private void MIDITest_Click(object sender, RoutedEventArgs e)
        {
            //Start makeMusic method on new thread
            Thread thread = new Thread(MakeMusic);
            thread.Start();


        }
    }
}
