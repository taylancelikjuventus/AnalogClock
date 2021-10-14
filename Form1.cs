using System;
using System.Globalization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AnalogClock
{
    public partial class Form1 : Form
    {

        //data fields
        //Timer
        Timer t = new Timer();
        int WIDTH = 300, HEIGHT = 300, secHAND = 140, minHAND = 120, hourHAND = 70;

        //center
        int cx, cy;

        // referencens to Bitmap and Graphics object
        Bitmap bmp;
        Graphics g;

        //stores updated coordinates for minutr/hour/second hands
        int[] handCoord = new int[2];
        int[] coord = new int[2];

        Point numPosition;
        DateTimeFormatInfo dtformat;

        int sec, min, hour, day, month;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //create bitmap
            bmp = new Bitmap(WIDTH +10 , HEIGHT +10 );
            
           

            //place center
            cx = WIDTH / 2 ;
            cy = HEIGHT / 2 ;

            //bg color
            this.BackColor = Color.FromArgb(255, 40, 40, 40);


            //timer
            t.Interval = 1000;
            //add tick event to timer
            t.Tick += new EventHandler(this.t_Tick);
            t.Start();

            

        }

        private void drawClock()
        {
            //get Canvas of bmp
            g = Graphics.FromImage(bmp);

            //get current time
            sec = DateTime.Now.Second;
            min = DateTime.Now.Minute;
            hour = DateTime.Now.Hour;
            day = DateTime.Now.Day;
            month = DateTime.Now.Month;

            hour = hour % 12;

            Console.WriteLine("Time  : " + hour + ":" + min + ":" + sec);

            //set bg color,smoothing filter and left top corner of drawing
            g.Clear(Color.WhiteSmoke);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TranslateTransform(5, 5);

            //draw circle
            g.DrawEllipse(new Pen(Color.SandyBrown, 6f), 0, 0, WIDTH, HEIGHT);


            //change position of origin
            g.TranslateTransform(cx, cy);

            GraphicsState s = g.Save();

            numPosition = new Point(-12, -12);
            for (int i = 1; i <= 12; i++)     //repeat followings from 1 to 12
            {
                g.Restore(s);
                s = g.Save();
                g.RotateTransform(30 * i);       //first rotate on origin
                g.TranslateTransform(0, -130);   //translate origin to the point where we draw Number
                g.FillRectangle(Brushes.DarkSlateGray, -2, -15, 5, 5);//insert some indicator beyond number
                g.RotateTransform(-30 * i);      //rotate according to new origin so that numbers would look horizontally and DrawString  
                g.DrawString(i.ToString(), new Font("Verdana", 12, FontStyle.Bold), Brushes.DodgerBlue, numPosition);
            }



            g.ResetTransform();
            g.TranslateTransform(cx, cy);
            GraphicsState center = g.Save();

            //center
            g.FillEllipse(Brushes.LightGray, -10, -10, 20, 20);

            //draw seconds hand
            g.ResetTransform();

        }



   
        //tick handler
        private void t_Tick(object sender , EventArgs e)
        {
            //draw clock's graphics
            drawClock();

            //update second hand's position
            handCoord = calculatePositionOfSecondHands(sec, secHAND);
            g.DrawLine(new Pen(Color.HotPink, 2f), new Point(cx, cy), new Point(handCoord[0], handCoord[1]));
            Console.WriteLine(handCoord[0] + "  " + handCoord[1]);

            //update minute hand's position
            handCoord = calculatePositionOfMinuteHands( (min*60+sec), minHAND);
            g.DrawLine(new Pen(Color.BurlyWood, 3f), new Point(cx, cy), new Point(handCoord[0], handCoord[1]));
            Console.WriteLine(handCoord[0] + "  " + handCoord[1]);

            //update hour hand's position
            handCoord = calculatePositionOfHourHands( (hour*3600+min*60+sec), hourHAND);
            g.DrawLine(new Pen(Color.RosyBrown, 5f), new Point(cx, cy), new Point(handCoord[0], handCoord[1]));
            Console.WriteLine(handCoord[0] + "  " + handCoord[1]);


            //green point on the center
            g.FillEllipse(Brushes.SeaGreen, cx-5, cy-5, 10, 10);

            
            Color start = Color.FromArgb(30, Color.Tomato);
            Color stop = Color.FromArgb(30, Color.White);
      LinearGradientBrush lb = new LinearGradientBrush(new Point(cx-50,cy-50), new Point(cx+50,cy+50), start, stop);

          //transparent circle
          g.FillEllipse(lb, cx-50, cy - 50, 100, 100);
          
            

            //SHOW date as well
            dtformat = new DateTimeFormatInfo();
            string monthName = dtformat.GetAbbreviatedMonthName(month);
            Console.WriteLine("day : " + day + "Month : " + DateTime.Now.DayOfWeek);
            g.ResetTransform();
            g.DrawString(day.ToString(), new Font("Verdana", 10, FontStyle.Regular), Brushes.Black, cx+80, cy-5);
            g.DrawString(monthName, new Font("Verdana", 10, FontStyle.Regular), Brushes.Black, cx+80, cy+10);
            g.DrawString(DateTime.Now.DayOfWeek.ToString(), new Font("Verdana", 10, FontStyle.Regular), Brushes.Black, cx+80, cy+25);

            //add bmp to picturebox
            pictureBox1.Image = bmp;

            //clean up
            lb.Dispose();
            dtformat = null;
            monthName = null;
            //handCoord = null;

        }


        //second hand rotates 6 degree in 1 second
        private int[] calculatePositionOfSecondHands(int val,int rad)
        {
            val *= 6;

            //we rotate in CW order so use Sin to find next x coordinate of the end of the line
            //If we used Cos second hand would rotate in CCW order.
            coord[0] = cx + (int)(rad * Math.Sin(Math.PI * val / 180));
            coord[1] = cy - (int)(rad * Math.Cos(Math.PI * val / 180));

            return coord;
        }

        //minute hand rotates 0.1 degree in 1 second
        private int[] calculatePositionOfMinuteHands(int val, int rad)
        {
            double deg = val * 0.1;
            coord[0] = cx + (int)(rad * Math.Sin(Math.PI * deg / 180));
            coord[1] = cy - (int)(rad * Math.Cos(Math.PI * deg / 180));

            return coord;
        }

        //hour hand rotates 1/120 degree in 1 second
        private int[] calculatePositionOfHourHands(int val, int rad)
        {
            double deg = val * 1/120;
           
            coord[0] = cx + (int)(rad * Math.Sin(Math.PI * deg / 180));
            coord[1] = cy - (int)(rad * Math.Cos(Math.PI * deg / 180));

            return coord;
        }


      /////////////////////////////////END///////////////////////////////////////


    }

}
