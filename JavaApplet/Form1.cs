using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace JavaApplet
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            init();
            finished = false;
            x1 = 640;
            y1 = 480;

            xy = (float)x1 / (float)y1;
            finished = true;
        }
        public struct HSBColor /* To set the colours */
        {
            float h;
            float s;
            float b;
            int a;

            public HSBColor(float h, float s, float b)
            {
                this.a = 0xff;
                this.h = Math.Min(Math.Max(h, 0), 255);
                this.s = Math.Min(Math.Max(s, 0), 255);
                this.b = Math.Min(Math.Max(b, 0), 255);
            }

            public HSBColor(int a, float h, float s, float b)
            {
                this.a = a;
                this.h = Math.Min(Math.Max(h, 0), 255);
                this.s = Math.Min(Math.Max(s, 0), 255);
                this.b = Math.Min(Math.Max(b, 0), 255);
            }
            public float H
            {
                get { return h; }
            }
            public float S
            {
                get { return s; }
            }
            public float B
            {
                get { return b; }
            }
            public int A
            {
                get { return a; }
            }
            public Color Color
            {
                get
                {
                    return FromHSB(this);
                }
            }
            public static Color FromHSB(HSBColor hsbColor)
            {
                float r = hsbColor.b;
                float g = hsbColor.b;
                float b = hsbColor.b;
                if (hsbColor.s != 0)
                {
                    float max = hsbColor.b;
                    float dif = hsbColor.b * hsbColor.s / 255f;
                    float min = hsbColor.b - dif;
                    float h = hsbColor.h * 360f / 255f;
                    if (h < 60f)
                    {
                        r = max;
                        g = h * dif / 60f + min;
                        b = min;
                    }
                    else if (h < 120f)
                    {
                        r = -(h - 120f) * dif / 60f + min;
                        g = max;
                        b = min;
                    }
                    else if (h < 180f)
                    {
                        r = min;
                        g = max;
                        b = (h - 120f) * dif / 60f + min;
                    }
                    else if (h < 240f)
                    {
                        r = min;
                        g = -(h - 240f) * dif / 60f + min;
                        b = max;
                    }
                    else if (h < 300f)
                    {
                        r = (h - 240f) * dif / 60f + min;
                        g = min;
                        b = max;
                    }
                    else if (h <= 360f)
                    {
                        r = max;
                        g = min;
                        b = -(h - 360f) * dif / 60 + min;
                    }
                    else
                    {
                        r = 0;
                        g = 0;
                        b = 0;
                    }
                }
                return Color.FromArgb
                    (
                        hsbColor.a,
                        (int)Math.Round(Math.Min(Math.Max(r, 0), 255)),
                        (int)Math.Round(Math.Min(Math.Max(g, 0), 255)),
                        (int)Math.Round(Math.Min(Math.Max(b, 0), 255))
                    );
            }
        }
        private const int MAX = 256;
        private const double SX = -2.025;
        private const double SY = -1.125;
        private const double EX = 0.6;
        private const double EY = 1.125;
        private static int x1, y1, xs, ys, xe, ye;
        private static double xstart, ystart, xende, yende, xzoom, yzoom;
        private static bool action, rectangle, finished;
        private static float xy;
        private Graphics g1;
        private HSBColor HSBcol = new HSBColor();
        private System.Drawing.Bitmap bitmap;

        public void init() // all instances will be prepared
        {

            this.Size = new Size(640, 480);
            finished = false;
            x1 = 640;
            y1 = 480;
            xy = (float)x1 / (float)y1;
            bitmap = new Bitmap(x1, y1);
            g1 = Graphics.FromImage(bitmap);
            finished = true;
            start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_MouseUp_1(object sender, MouseEventArgs e)
        {

        }

        public void destroy() // delete all instances 
        {
            if (finished)
            {
                bitmap = null;
                g1 = null;
                GC.Collect();
            }
        }
        public void start()
        {
            action = false;
            rectangle = false;
            initvalues();
            xzoom = (xende - xstart) / (double)x1;
            yzoom = (yende - ystart) / (double)y1;
            mandelbrot();
        }
        public void stop()
        {
        }

        public void paint(Graphics g1)
        {
            update(g1);
        }
        public void update(Graphics g1)
        {

        }
        private void mandelbrot() // calculate all points
        {
            int x, y;
            float h, b, alt = 0.0f;
            action = false;
            Color color = Color.Black;
            Pen myPen = new Pen(color);
            for (x = 0; x < x1; x += 2)
                for (y = 0; y < y1; y++)
                {
                    h = pointcolour(xstart + xzoom * (double)x, ystart + yzoom * (double)y);
                    if (h != alt)
                    {
                        b = 1.0f - h * h; //brightness
                        
                        color = HSBColor.FromHSB(new HSBColor(h * 255, 0.8f * 255, b * 255));
                        
                        myPen = new Pen(color);
                        alt = h;
                    }
                    g1.DrawLine(myPen, x, y, x + 1, y);
                }
                action = true;
        }
        private float pointcolour(double xwert, double ywert) // color value from 0.0 to 1.0 by iterations
        {
            double r = 0.0, i = 0.0, m = 0.0;
            int j = 0;
            while ((j < MAX) && (m < 4.0))
            {
                j++;
                m = r * r - i * i;
                i = 2.0 * r * i + ywert;
                r = m + xwert;
            }
            return (float)j / (float)MAX;
        }
        private void initvalues() // reset start values
        {
            xstart = SX;
            ystart = SY;
            xende = EX;
            yende = EY;
            if ((float)((xende - xstart) / (yende - ystart)) != xy)
                xstart = xende - (yende - ystart) * (double)xy;
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g1 = e.Graphics;
            g1.DrawImage(bitmap, 0, 0, x1, y1);
            g1.Dispose();
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (action)
            {
                xe = e.X;
                ye = e.Y;
                rectangle = true;
            }
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (action)
            {
                xs = e.X;
                ys = e.Y;
            }
        }
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            int z, w;
        
            if (action)
            {
                xe = e.X;
                ye = e.Y;
                if (xs > xe)
                {
                    z = xs;
                    xs = xe;
                    xe = z;
                }
                if (ys > ye)
                {
                    z = ys;
                    ys = ye;
                    ye = z;
                }
                w = (xe - xs);
                z = (ye - ys);
                if ((w < 2) && (z < 2)) initvalues();
                else
                {
                    if (((float)w > (float)z * xy)) ye = (int)((float)ys + (float)w / xy);
                    else xe = (int)((float)xs + (float)z * xy);
                    xende = xstart + xzoom * (double)xe;
                    yende = ystart + yzoom * (double)ye;
                    xstart += xzoom * (double)xs;
                    ystart += yzoom * (double)ys;
                }
                xzoom = (xende - xstart) / (double)x1;
                yzoom = (yende - ystart) / (double)y1;
                mandelbrot();
                rectangle = false;
                Refresh();
            }
        }



    }
}
