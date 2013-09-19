﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;

namespace DigitalCircuitDesign
{
    public partial class Form1 : Form
    {
        int nWidth = 20;
        int nHeight = 8;
        int start = 50;
        int width = 60;
        int height = 60;
        int error = 10;

        string[,] array = new string[100, 100];
        public Form1()
        {
            InitializeComponent();
            clearScreen();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            DrawGrid();
            DrawComponents();
            DrawLinks();
         //   RecognizeSpeech();
            //base.OnPaint(e);
            //DrawLShapeLine(this.CreateGraphics(), 10, 10, 20, 40);
        }
        public void DrawGrid()
        {
            System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.Black);
            System.Drawing.Graphics formGraphics = this.CreateGraphics();

            Font myFont = new System.Drawing.Font("Helvetica", 10, FontStyle.Italic);
            Brush myBrush = new SolidBrush(System.Drawing.Color.Red);

            //drawing rows
            int i;
            for (i = 0; i < nHeight; i++)
            {
                int pos = i * height + start;
                if (pos == start)
                    formGraphics.DrawString("R0", myFont, myBrush, start - (start / 2), pos + (height / 2) - error);
                else
                    formGraphics.DrawString("R" + i, myFont, myBrush, start - (start / 2), pos + (height / 2) - error);
                formGraphics.DrawLine(myPen, start, pos, start + nWidth * width, pos);
            }
            //draw final ending row
            formGraphics.DrawLine(myPen, start, i * height + start, start + nWidth * width, i * height + start);
            //drawing cols
            for (i = 0; i < nWidth; i++)
            {
                int pos = i * width + start;
                if (pos == start)
                    formGraphics.DrawString("C0", myFont, myBrush, pos + (width / 2) - error, start - (start / 2));
                else
                    formGraphics.DrawString("C" + i, myFont, myBrush, pos + (width / 2) - error, start - (start / 2));

                formGraphics.DrawLine(myPen, pos, start, pos, start + nHeight * height);
            }
            //draw final ending col
            formGraphics.DrawLine(myPen, i * width + start, start, i * width + start, start + nHeight * height);
            myPen.Dispose();
            formGraphics.Dispose();

        }
        public void clearScreen()
        {
            for (int variable1 = 0; variable1 < nHeight; variable1++)
            {
                for (int variable2 = 0; variable2 < nWidth; variable2++)
                {
                    array[variable1, variable2] = " ";
                }
            }
        }
        public void DrawComponents()
        {
            for (int variable1 = 0; variable1 < nHeight; variable1++)
            {
                for (int variable2 = 0; variable2 < nWidth; variable2++)
                {
                    int x = start + variable2 * height + 1;
                    int y = start + variable1 * width + error;
                    String imageFolder ="E:/Workspace/github/SpeechRecognition/DigitalCircuitDesign/images/";
                    switch (array[variable1, variable2])
                    {
                        case "A":
                            addImage(x, y, imageFolder + "and.jpg");
                            break;
                        case "O":
                            addImage(x, y, imageFolder + "or.jpg");
                            break;
                        case "NA":
                            addImage(x, y, imageFolder + "nand.jpg");
                            break;
                        case "NO":
                            addImage(x, y, imageFolder + "nor.jpg");
                            break;
                        case "N":
                            addImage(x, y, imageFolder + "not.jpg");
                            break;
                        case "XO":
                            addImage(x, y, imageFolder + "xor.jpg");
                            break;
                        case "XN":
                            addImage(x, y, imageFolder + "xnor.jpg");
                            break;
                    }
                }
            }
        }
        public void addImage(int x, int y, String imageLocation)
        {
            PictureBox box = new PictureBox();
            box.Location = new Point(x, y);
            box.Image = Image.FromFile(imageLocation);
            box.Size = new Size(width - 1, height - error);
            box.Parent = this;
        }
        public void addGates(String gate, String row, String col)
        {
            //row and col should be of the format R0 and C1 
            int x = Convert.ToInt32(row.Substring(1));
            int y = Convert.ToInt32(col.Substring(1));
            if (x >= 0 && y >= 0 && x < nHeight && y < nWidth)
            {
                switch (gate)
                {
                    case "and": array[x, y] = "A"; break;
                    case "or": array[x, y] = "O"; break;
                    case "nand": array[x, y] = "NA"; break;
                    case "nor": array[x, y] = "NO"; break;
                    case "not": array[x, y] = "N"; break;
                    case "xor": array[x, y] = "XO"; break;
                    case "xnor": array[x, y] = "XN"; break;
                }
            }
            this.Invalidate();
        }
        public void DrawLinks(){
            char[] c = new char[1];
            int[] offset=new int[1];
            c[0] = 'U';
            offset[0] = 0;
            connectGates(2, 2, c,offset);
            c[0] = 'L';
            offset[0] = 1;
            connectGates(4, 4, c, offset);
            c[0] = 'R';
            offset[0] = 5;
            connectGates(5, 5, c, offset);
            c[0] = 'D';
            offset[0] = 3;
            connectGates(6, 6, c, offset);

            c = new char[7];
            c[0] = 'L';
            c[1] = 'U';
            c[2] = 'R';
            c[3] = 'R';
            c[4] = 'R';
            c[5] = 'D';
            c[6] = 'D';
            offset = new int[7];
            offset[0] = -5;
            offset[1] = -10;
            offset[2] = -20;
            offset[3] = -10;
            offset[4] = 0;
            offset[5] = -10;
            offset[6] = -10;
            connectGates(9, 3, c, offset);

            c = new char[3];
            c[0] = 'R';
            c[1] = 'L';
            c[2] = 'R';
            offset = new int[3];
            offset[0] = 0;
            offset[1] = 5;
            offset[2] = 0;
            connectGates(12, 3, c, offset);


        }
        public void connectGates(int rowS, int colS, char[] path, int[] offset)
        {
            //lasth and v offset keep track of recent offset difference
            int lasthoffset=0, lastvoffset=0;
            int currx = rowS * width + start, curry = colS * height + start;
            //Method implement to add connection between gates
            //additional 4 for start, and 3 arrow points
            Point[] points =new Point[path.Length+4];
            if (path.Length > 1)
            {
                if (path[0] == 'L' || path[0] == 'R')
                {
                    curry += offset[0];
                    lastvoffset = offset[0];
                }
                else
                {
                    currx += offset[0];
                    lasthoffset = offset[0];
                }
            }
            points[0] = new Point(currx, curry);

            //calculate remaining points
            for (int i = 0; i < path.Length; i++)
            {
                int nextOffset=(i!=path.Length-1 && 
                    (((path[i]=='L' || path[i]=='R') && (path[i+1]=='U' || path[i+1]=='D')) || 
                    ((path[i]=='U' || path[i]=='D') && (path[i+1]=='L' || path[i+1]=='R'))))?offset[i+1]:0;
                //also avoids the small offset difference that gets accumulated till the end
                switch (path[i])
                {
                    case 'L': currx =currx-lasthoffset-width + nextOffset; lasthoffset=nextOffset; break;
                    case 'R': currx = currx-lasthoffset+width + nextOffset; lasthoffset = nextOffset; break;
                    case 'U': curry = curry-lastvoffset-height +nextOffset; lastvoffset = nextOffset; break;
                    case 'D': curry = curry-lastvoffset+height +nextOffset; lastvoffset = nextOffset; break;
                }
                points[i + 1] = new Point(currx,curry);
            }
            int arrowSize = 5;
            int x = points[path.Length].X;
            int y = points[path.Length].Y;
            // Arrow
            switch(path[path.Length-1]){
                case 'L':
                    //left arrow
                    points[path.Length+1] = new Point(x+ arrowSize, y - arrowSize);
            points[path.Length+2]=new Point(x + arrowSize, y + arrowSize);
            points[path.Length+3]=new Point(x , y);break;
                case 'R':
                    //right arrow
                    points[path.Length+1] = new Point(x- arrowSize, y - arrowSize);
            points[path.Length+2]=new Point(x - arrowSize, y + arrowSize);
            points[path.Length+3]=new Point(x , y);break;
                case 'U':
                    //up arrow
                    points[path.Length+1] = new Point(x- arrowSize, y + arrowSize);
            points[path.Length+2]=new Point(x + arrowSize, y + arrowSize);
            points[path.Length+3]=new Point(x , y);break;
                case 'D':
                    //down arrow
                    points[path.Length+1] = new Point(x- arrowSize, y - arrowSize);
            points[path.Length+2]=new Point(x + arrowSize, y - arrowSize);
            points[path.Length+3]=new Point(x , y);break;
        }
            
            Pen myPen = new Pen(Color.Blue);
            myPen.Width = 3;
            this.CreateGraphics().DrawLines(myPen, points);
        }
        
        public void DrawLShapeLine(System.Drawing.Graphics g, int intMarginLeft, int intMarginTop, int intWidth, int intHeight)
        {
            Pen myPen = new Pen(Color.Black);
            myPen.Width = 2;
            // Create array of points that define lines to draw.
            int marginleft = intMarginLeft;
            int marginTop = intMarginTop;
            int width = intWidth;
            int height = intHeight;
            int arrowSize = 3;
            Point[] points =
             {
                new Point(marginleft, marginTop),
                new Point(marginleft, height + marginTop),
                new Point(marginleft + width, marginTop + height),
                // Arrow
                new Point(marginleft + width - arrowSize, marginTop + height - arrowSize),
                new Point(marginleft + width - arrowSize, marginTop + height + arrowSize),
                new Point(marginleft + width, marginTop + height)
             };

            g.DrawLines(myPen, points);
        }

        public void RecognizeSpeech()
        {
            SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();                     

            recognizer.LoadGrammarCompleted += new EventHandler<LoadGrammarCompletedEventArgs>(LoadGrammarCompleted);
            recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognized);
            recognizer.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(SpeechRejected);  
            recognizer.SetInputToDefaultAudioDevice();

            GrammarBuilder clear = new GrammarBuilder("Clear");
 
            GrammarBuilder insert = new GrammarBuilder("Insert");
            Choices gates = new Choices(new string[] { "and", "or", "not", "exor", "nor", "nand" });
            Choices columns = new Choices(new string[] { "zero","one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" });
            Choices rows = new Choices(new string[] { "zero" ,"one", "two", "three", "four", "five", "six", "seven", "eight", "nine" });
            //Choices orientation = new Choices(new string[] { "left", "right", "up", "down" });
            insert.Append(gates);
            insert.Append("R");
            insert.Append(rows);
            insert.Append("C");
            insert.Append(columns);
            
            //insert.Append("towards");
            //insert.Append(orientation);

            GrammarBuilder connect = new GrammarBuilder("Connect");
            connect.Append("output");
            connect.Append(columns);
            connect.Append(rows);
            connect.Append("to");
            connect.Append("input");
            connect.Append(columns);
            connect.Append(rows);
            
            Grammar _clear_grammar = new Grammar(clear);
            Grammar _insert_grammar = new Grammar(insert);
            Grammar _connect_grammar = new Grammar(connect);

            recognizer.LoadGrammarAsync(_clear_grammar);
            recognizer.LoadGrammarAsync(_insert_grammar);
            recognizer.LoadGrammarAsync(_connect_grammar);
            
            //recognizer.RecognizeAsync(RecognizeMode.Multiple);
            while (true)
            {
                recognizer.Recognize();
            }
            
        }
            
        public void LoadGrammarCompleted(object sender, LoadGrammarCompletedEventArgs e)
        {
            //Console.WriteLine(e.Grammar.Name + " successfully loaded");            
            //MessageBox.Show("Done Loading Grammer");
        }

        public void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //Console.WriteLine("Speech recognized: " + e.Result.Text);
            MessageBox.Show(e.Result.Text);
            string command = e.Result.Text;
            string[] tokens = command.Split(' ');
            if (tokens[0] == "Insert")
            {
                if(tokens.Length != 6)
                {  return;   }
                
                string gate= tokens[1];
                string row = "R";
                string column = "C";
                int r = GetNumber(tokens[3]);
                int c = GetNumber(tokens[5]);
                if (r == -1 || c == -1)
                    return;
                row = row + r;
                column = column + c;
                addGates(gate, row, column);
            }
            else if (tokens[0] == "Connect")
                {
                    if (tokens.Length != 8)
                    { return; }

                    int rowS = GetNumber(tokens[2]);
                    int colS = GetNumber(tokens[3]);
                    int rowE = GetNumber(tokens[6]);
                    int colE = GetNumber(tokens[7]);
                    MessageBox.Show(e.Result.Text);
                    //connectGates(rowS, colS, rowE, colE);
                }
            else if (tokens[0] == "Clear")
            { }
      
        }

        public void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            //Console.WriteLine("Speech input failed");
            MessageBox.Show("Failed");
        }

        public int GetNumber(string numberString)
        {
            int toReturn = -1;
            switch(numberString)
            {
                case "zero": toReturn = 0; break;
                case "one":  toReturn = 1; break;
                case "two":  toReturn = 2; break;
                case "three": toReturn = 3; break;
                case "four": toReturn = 4;  break;
                case "five": toReturn = 5;  break;
                case "six": toReturn = 6;  break;
                case "seven": toReturn = 7;  break;
                case "eight": toReturn = 8;  break;
                case "nine": toReturn = 9;  break;
                case "ten": toReturn = 10; break;
                case "eleven": toReturn = 11; break;
                case "twelve": toReturn = 12;  break;
                case "thirteen": toReturn = 13; break;
                case "fourteen": toReturn = 14;  break;
                case "fifteen":  toReturn = 15;  break;
                case "sixteen": toReturn = 16;  break;
                case "seventeen": toReturn = 17;  break;
                case "eighteen": toReturn = 18;  break;
                case "nineteen": toReturn = 19;  break;
            }

            return toReturn;
        }

        private void button1_Click(object sender, EventArgs e)
        {
           addGates("and", "R0", "C0");
            addGates("or", "R0", "C1");
            addGates("not", "R0", "C2");
            addGates("xor", "R0", "C3");
            addGates("nand", "R0", "C4");
            addGates("nor", "R0", "C5");
            addGates("xnor", "R0", "C10");
            addGates("xnor", "R7", "C19");
        }
    }
}
