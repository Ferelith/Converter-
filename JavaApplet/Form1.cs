using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JavaApplet
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            finished = false;
            x1 = 640;
            y1 = 480;
            
            xy = (float)x1 / (float)y1;
            finished = true;
        }
        class HSB
        {//djm added, it makes it simpler to have this code in here than in the C#
            public float rChan, gChan, bChan;
            public HSB()
            {
                rChan = gChan = bChan = 0;
            }
            public void fromHSB(float h, float s, float b)
            {
                float red = b;
                float green = b;
                float blue = b;
                if (s != 0)
                {
                    float max = b;
                    float dif = b * s / 255f;
                    float min = b - dif;

                    float h2 = h * 360f / 255f;

                    if (h2 < 60f)
                    {
                        red = max;
                        green = h2 * dif / 60f + min;
                        blue = min;
                    }
                    else if (h2 < 120f)
                    {
                        red = -(h2 - 120f) * dif / 60f + min;
                        green = max;
                        blue = min;
                    }
                    else if (h2 < 180f)
                    {
                        red = min;
                        green = max;
                        blue = (h2 - 120f) * dif / 60f + min;
                    }
                    else if (h2 < 240f)
                    {
                        red = min;
                        green = -(h2 - 240f) * dif / 60f + min;
                        blue = max;
                    }
                    else if (h2 < 300f)
                    {
                        red = (h2 - 240f) * dif / 60f + min;
                        green = min;
                        blue = max;
                    }
                    else if (h2 <= 360f)
                    {
                        red = max;
                        green = min;
                        blue = -(h2 - 360f) * dif / 60 + min;
                    }
                    else
                    {
                        red = 0;
                        green = 0;
                        blue = 0;
                    }
                }

                rChan = (float)Math.Round(Math.Min(Math.Max(red, 0f), 255));
                gChan = (float)Math.Round(Math.Min(Math.Max(green, 0), 255));
                bChan = (float)Math.Round(Math.Min(Math.Max(blue, 0), 255));

            }
        }



        const int MAX = 256;      // max iterations
        const double SX = -2.025; // start value real
        const double SY = -1.125; // start value imaginary
        const double EX = 0.6;    // end value real
        const double EY = 1.125;  // end value imaginary
        private static int x1, y1, xs, ys, xe, ye;
        private static double xstart, ystart, xende, yende, xzoom, yzoom;
        private static float xy;
        private Image picture;
        private Graphics g1;

        

        private Cursor c1, c2;
        private HSB HSBcol = new HSB();
        private Pen pen;
        Bitmap bm = new Bitmap(640, 480);


        private void Form1_Load(object sender, EventArgs e)
        {
            action = false;
            rectangle = false;
            initvalues();
            xzoom = (xende - xstart) / (double)x1;
            yzoom = (yende - ystart) / (double)y1;
            mandelbrot();
         
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
           
        }
        public void update(Graphics g1)
        {
            g1.DrawImage(picture, 0, 0);
            if (rectangle)
            {
                if (xs < xe)
                {
                    if (ys < ye) g1.DrawRectangle(pen, xs, xe, (xe - xs), (ye - ys));
                    else g1.DrawRectangle(pen, xs, ye, (xe - xs), (ys - ye));
                }
            }
            else
            {
                if (ys < ye) g1.DrawRectangle(pen, xe, ys, (xs - xe), (ye - ys));
                else g1.DrawRectangle(pen, xe, ye, (xs - xe), (ys - ye));
            }

        }
        private void mandelbrot() // calculate all points
        {
            int x, y;
            float h, b, alt = 0.0f;

            action = false;
            Console.WriteLine(x1 + " " + y1);
            for (x = 0; x < x1; x += 2)
                for (y = 0; y < y1; y++)
                {
                    h = pointcolour(xstart + xzoom * (double)x, ystart + yzoom * (double)y); // color
                    if (h != alt)
                    {
                        b = 1.0f - h * h; // brightnes
                                          ///djm added
                                          ///HSBcol.fromHSB(h,0.8f,b); //convert hsb to rgb then make a Java Color
                                          ///Color col = new Color(0,HSBcol.rChan,HSBcol.gChan,HSBcol.bChan);
                                          ///g1.setColor(col);
                        //djm end
                        //djm added to convert to RGB from HSB
                        //djm test https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.modeling.diagrams.hslcolor.aspx and use ToRGBColor
                        HSBcol.fromHSB(h, 0.8f, b);
                        int red = (int)(HSBcol.rChan * 255);
                        int green = (int)(HSBcol.gChan * 255);
                        int blue = (int)(HSBcol.bChan * 255);
                        Color color = Color.FromArgb(red, green, blue);
                        bm.SetPixel(x, y, color);
                        bm.SetPixel(x + 1, y, color);
                        //djm 
                        alt = h;
                    }

                }
            pictureBox1.Image = bm;
            action = true;
        }
        private static bool action, rectangle, finished;
       
       
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

    }
}
