using System;
using System.IO;
using System.Media;

namespace Updater
{
    // the following code goes inside your namespace
    public class Beep
    {
        // I added this function because amplitude should just always be 1000
        // < 1000 sounds muffled and > 1000 throws an exception
        public static void Play(double frequency, double duration)
        {
            BeepBeep(1000, frequency, duration);
        }

        private static void BeepBeep(double amplitude, double frequency, double duration)
        {
            var amp = ((amplitude * (Math.Pow(2, 15))) / 1000) - 1;
            var deltaFt = 2 * Math.PI * frequency / 44100.0;

            var samples = (int)(441.0 * duration / 10.0);
            var bytes = samples * sizeof(int);
            int[] hdr = { 0X46464952, 36 + bytes, 0X45564157, 0X20746D66, 16, 0X20001, 44100, 176400, 0X100004, 0X61746164, bytes };

            using (var ms = new MemoryStream(44 + bytes))
            {
                using (var bw = new BinaryWriter(ms))
                {
                    foreach (var t in hdr)
                    {
                        bw.Write(t);
                    }
                    for (var T = 0; T < samples; T++)
                    {
                        var sample = Convert.ToInt16(amp * Math.Sin(deltaFt * T));
                        bw.Write(sample);
                        bw.Write(sample);
                    }

                    bw.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    using (var sp = new SoundPlayer(ms))
                    {
                        sp.PlaySync();
                    }
                }
            }
        }
    }
}
