namespace Lyra.DisplaySample
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using WaveParser;

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = new Form();

            form.Paint += new PaintEventHandler(paintHandler);
            Application.Run(form);
        }

        public static void paintHandler(Object sender, PaintEventArgs e)
        {
            Audio audio = new Audio("3.wav");
            Pen blackPen = new Pen(Color.Black, 1);
            /*
            //graph of front 1024 points
            e.Graphics.DrawLine(blackPen, new Point(0, 600), new Point(1024, 600));
            for (int i = 0; i < 1024; ++i)
            {
                e.Graphics.DrawLine(blackPen, new Point(i, 600 - audio.data[i]), new Point(i, 600));
            }
            */

            double[] fftData = audio.GetFFTResult(4736);
            for (int i = 1; i < audio.fftLength / 2; ++i)
            {
                e.Graphics.DrawLine(blackPen, new Point(i, (int)(600 - fftData[i])), new Point(i, 600));
            }
            e.Graphics.DrawLine(blackPen, new Point(130 * audio.fftLength / audio.fs, 700), new Point(130 * audio.fftLength / audio.fs, 600));
            e.Graphics.DrawLine(blackPen, new Point(262 * audio.fftLength / audio.fs, 700), new Point(262 * audio.fftLength / audio.fs, 600));
            e.Graphics.DrawLine(blackPen, new Point(520 * audio.fftLength / audio.fs, 700), new Point(520 * audio.fftLength / audio.fs, 600));
            e.Graphics.DrawLine(blackPen, new Point(1024 * audio.fftLength / audio.fs, 700), new Point(1024 * audio.fftLength / audio.fs, 600));
        }
    }
}
