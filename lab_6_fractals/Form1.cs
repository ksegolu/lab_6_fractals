using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Windows.Forms;
using System.IO;


namespace lab_6_fractals
{
    public partial class Form1 : Form
    {
        const int cellSize = 25;
        const int celllInLine = 1024 / cellSize;
        const int cellCount = celllInLine * celllInLine;
        float[,,] u0, u, b;
        int w, h;
        const int delta = 11;
        double[] res = new double[delta];
        double[] res_2 = new double[delta];
        float[,] vol = new float[cellCount, delta];

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string path = "unh_4.jpg";
            Image<Bgr, Byte> image = new Image<Bgr, Byte>(path);
            Image<Bgr, Byte> image2 = image.Convert<Bgr, Byte>().Resize(1024, 1024, Emgu.CV.CvEnum.Inter.Linear);
            image2.Save("edited.png");
            Bitmap img_2 = new Bitmap("edited.png");

            h = img_2.Height;
            w = img_2.Width;
            u0 = new float[cellCount, cellSize, cellSize];
            u = new float[cellCount, cellSize, cellSize];
            b = new float[cellCount, cellSize, cellSize];
            setU0(img_2);
            float s;
            //calc u and b 2 times
            for (int d = 1; d <= delta; d++)
            {
                calcU(d);
                calcB(d);
                for (int k = 0; k < cellCount; k++)
                {
                    s = 0;
                    for (int i = cellSize - 1; i >= 0; i--)
                    {
                        for (int j = cellSize - 1; j >= 0; j--)
                        {
                            s += u[k, i, j] - b[k, i, j];
                        }
                    }
                    vol[k, d - 1] = s;
                }

            }

            for (int d = 2; d < delta; d++)
            {
                s = 0;
                for (int k = 0; k < cellCount; k++)
                {
                    s += (vol[k, d] - vol[k, d - 1]) / 2;
                }
                res[d] = Math.Log(s) / Math.Log(d);
            }


            for (int i = 2; i < delta; i++)
            {
                chart1.Series["Series1"].Points.AddXY(i, res[i]);
            }
        }
        private void setU0(Bitmap img)
        {
            for (int k = 0; k < cellCount; k++)
            {
                for (int i = cellSize - 1; i >= 0; i--)
                {
                    for (int j = cellSize - 1; j >= 0; j--)
                    {
                        u0[k, i, j] = getIntense(img.GetPixel(i + (cellSize * (k % celllInLine)), j + (cellSize * (k / celllInLine))));
                    }
                }
            }

            return;
        }
        //верхняя поверхность uδ(i,j)
        private void calcU(int delta)
        {
            if (delta > 1)
            {
                for (int k = 0; k < cellCount; k++)
                {
                    for (int i = cellSize - 1; i >= 0; i--)
                    {
                        for (int j = cellSize - 1; j >= 0; j--)
                        {
                            u0[k, i, j] = u[k, i, j];
                        }
                    }
                }
            }
            for (int k = 0; k < cellCount; k++)
            {
                for (int i = cellSize - 1; i >= 0; i--)
                {
                    for (int j = cellSize - 1; j >= 0; j--)
                    {
                        u[k, i, j] = Math.Max(
                            u0[k, i, j] + 1,
                            Math.Max(
                                Math.Max(
                                    i > 0 ? u0[k, i - 1, j] : 0,
                                    j > 0 ? u0[k, i, j - 1] : 0
                                    ),
                                Math.Max(
                                    i < cellSize - 1 ? u0[k, i + 1, j] : 0,
                                    j < cellSize - 1 ? u0[k, i, j + 1] : 0
                                    )
                                )
                            );
                    }
                }
            }
            return;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void calcB(int delta)
        {
            if (delta > 1)
            {
                for (int k = 0; k < cellCount; k++)
                {
                    for (int i = cellSize - 1; i >= 0; i--)
                    {
                        for (int j = cellSize - 1; j >= 0; j--)
                        {
                            u0[k, i, j] = b[k, i, j];
                        }
                    }
                }
            }
            for (int k = 0; k < cellCount; k++)
            {
                for (int i = cellSize - 1; i >= 0; i--)
                {
                    for (int j = cellSize - 1; j >= 0; j--)
                    {
                        b[k, i, j] = Math.Min(
                            u0[k, i, j] - 1,
                            Math.Min(
                                Math.Min(
                                    i > 0 ? u0[k, i - 1, j] : 256,
                                    j > 0 ? u0[k, i, j - 1] : 256
                                    ),
                                Math.Min(
                                    i < cellSize - 1 ? u0[k, i + 1, j] : 256,
                                    j < cellSize - 1 ? u0[k, i, j + 1] : 256
                                    )
                                )
                            );
                    }
                }
            }
            return;
        }

        private float getIntense(Color color)
        {
            return 255 - (color.R + color.G + color.B) / 3;
        }
    }
}
