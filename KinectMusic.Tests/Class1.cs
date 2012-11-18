using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace KinectMusic.Tests
{
    [TestFixture]
    public class TestClass
    {
        [Test]
        // In Key Of c
        public void PitchCorrection_GivenInputNoteValueOf60_Return60()
        {
            var result = PitchCorrection(60);
            Assert.AreEqual(result, 60);
        }

        [Test]
        public void PitchCorrection_GivenInputNoteValueOf61_Return60()
        {
            var result = PitchCorrection(61);
            Assert.AreEqual(result, 60);
        }

        [Test]
        public void PitchCorrection_GivenInputNoteValueOf62_Return62()
        {
            var result = PitchCorrection(62);
            Assert.AreEqual(result, 62);
        }

        [Test]
        public void PitchCorrection_GivenInputNoteValueOf63_Return62()
        {
            var result = PitchCorrection(63);
            Assert.AreEqual(result, 62);
        }

        [Test]
        public void PitchCorrection_GivenInputNoteValueOf65_Return65()
        {
            var result = PitchCorrection(65);
            Assert.AreEqual(result, 65);
        }

        public int PitchCorrection(int note)
        {
            // Want to have tone, tone, semitone, tone, tone, tone, semitone

            // 0,2,4,5,7,9,11 is okay
            int r = note % 12;

            // if not one of the above, round down
            if ((r == 0) || (r == 2) || (r == 4) || (r == 5) || (r == 7) || (r == 9) || (r == 11)) 
            { }
            else
            {
                note -= 1;
            }

            return note;
        }
    }
}
