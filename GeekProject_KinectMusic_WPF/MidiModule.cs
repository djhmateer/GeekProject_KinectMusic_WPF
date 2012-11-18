using System;
using System.Threading;
using NAudio.Midi;

namespace MusicToolKit
{
    class MidiModule
    {
        //this is a monophonic instrument
        int[] currentNoteOn = new int[12];
    

        Boolean forceScaleCorrection = true;
    


        // 1 is keys, 10 is drums




        // 1 is the CoolSoft VirtualMidiSynth
        MidiOut midiOut = new MidiOut(1);

        public MidiModule()
        {
            string[] returnDevices = new string[MidiOut.NumberOfDevices];
            octShift = 0;
            currentNoteOn[1] = -1;
            currentNoteOn[2] = -1;
            this.SetVolume(120,1); //default vol
            this.SetVolume(100, 2); //default vol
            this.ChangePatch(81,1); //default synth sound
            this.ChangePatch(1, 2); //default synth sound
        }

        private Boolean valid8bit(int val)
        {
            if (val < 1) { return false; }
            if (val > 127) { return false; }
            return true;
        }


        public int octShift { get; set; }

        private int pitchCorrection(int note)
        {
            //key of C Major (TTSTTTS)

            int rem = note % 12;

            string s = "["+rem.ToString()+"]";

            if ("[0][2][4][5][7][9][11]".Contains(s))
            {

                return note;

            }
            else
            {
                return note - 1;
            }
        }



        public void NoteOn(int note, int vel,int channel)
        {
            if (valid8bit(note) && valid8bit(vel))
            {
                //for (int arp = 0; arp < 5; arp++)
                //{
                    if (forceScaleCorrection)
                    {
                        note = pitchCorrection(note+octShift);
                    }

                    if (currentNoteOn[channel] > 0) { NoteOff(channel); }  //turn off any currently sounding notes = monophonic instrument

                    currentNoteOn[channel] = note;
                    midiOut.Send(MidiMessage.StartNote(note, vel, channel).RawData);

                    //Thread.Sleep(10);
                //}
            
            }
        }

        public void PlayShortNote(int note, int vel, int channel)
        {
            NoteOn(note, vel,channel);
            Thread.Sleep(100);
            NoteOff(1);
        }

        public void PlayLongNote(int note, int vel, int channel)
        {
            NoteOn(note, vel, channel);
            Thread.Sleep(400);
            NoteOff(1);
        }

        public void NoteOff(int channel)
        {
            if (valid8bit(currentNoteOn[channel]))
            {
                midiOut.Send(MidiMessage.StopNote(currentNoteOn[channel], 0, channel).RawData);
                currentNoteOn[channel] = -1;
            }
        }

        public void ContinuousContoller(int val, int channel)
        {
            
            if (valid8bit(val))
            {
                midiOut.Send(MidiMessage.ChangeControl(71, val, channel).RawData);
  
            }
        }

       

        public void ChangePatch(int patchid, int channel)
        {
            if (valid8bit(patchid))
            {
                midiOut.Send(MidiMessage.ChangePatch(patchid, channel).RawData);  //setup sound
            }
        }
        
        public void SetVolume(int vol,int channel)
        {
            if (valid8bit(vol))
            {
                midiOut.Send(MidiMessage.ChangeControl(7, vol, channel).RawData);  //setup vol
            }
        }

    }
}
