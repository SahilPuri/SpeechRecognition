using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Windows.Forms.VisualStyles;

namespace DigitalCircuitDesign
{
    public partial class Form1 : Form
    {

        SecondUIThreadForm secondThreadForm;
        bool droidReady = false;
        string historyOfdroids = "Command History: \r\n";
        string yesNoForRemoveGateOrLink;
        int sourceAlphabet = 65;


        int nWidth = 17;
        int nHeight = 7;
        int start = 50;
        int width = 70;
        int height = 70;
        int error = 10;
        //also modified in constructor
        int picHeightError = 10;
        int outPinWidth = 11;
        int inPinWidth = 15;
        static String imageFolder = "C:/Users/patrick/Documents/GitHub/SpeechRecognition/DigitalCircuitDesign/images/";

        /*Start of State of the System*/
        Dictionary<String, Layout> layout = new Dictionary<String, Layout>();
        Dictionary<int, LinkDirections> links = new Dictionary<int, LinkDirections>();
        ConnectLink clObject;
        int linkCnt = 0;
        /*End of State of the System*/

        /*state history*/
        StackData[] arr = new StackData[5];
        int currStart = 0;
        int currEnd = 0;

        StackData[] arr2 = new StackData[5];
        int currStart2 = 0;
        int currEnd2 = 0;

        PictureBox[,] pic = new PictureBox[100, 100];

        public Form1()
        {
            picHeightError += (height - 55) / 2;
            outPinWidth += (width - 70) / 2;
            inPinWidth += (width - 70) / 2;
            InitializeComponent();
            clearScreen();
            clObject = new ConnectLink(nHeight, nWidth, 2);
            textBox2.Text = historyOfdroids;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            clearAllImages();
            DrawGrid();
            DrawComponents();
            //RecognizeSpeech();
            //base.OnPaint(e);
            //DrawLShapeLine(this.CreateGraphics(), 10, 10, 20, 40);
        }
        public void DrawGrid()
        {
            this.BackColor = Color.White;
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
            layout = new Dictionary<String, Layout>();
            links = new Dictionary<int, LinkDirections>();
            linkCnt = 0;
        }
        public void DrawComponents()
        {
            foreach (LinkDirections ld in links.Values)
            {
                connectGates(ld.x, ld.y, ld.destX, ld.destY, ld.directions, ld.offset);
            }
            foreach (Layout lay in layout.Values)
            {
                int y = start + lay.xcord * height;
                int x = start + lay.ycord * width;

                switch (lay.type)
                {
                    case Gates.AND:
                        addImage(x, y, lay.xcord, lay.ycord, "and.jpg");
                        break;
                    case Gates.OR:
                        addImage(x, y, lay.xcord, lay.ycord, "or.jpg");
                        break;
                    case Gates.NAND:
                        addImage(x, y, lay.xcord, lay.ycord, "nand.jpg");
                        break;
                    case Gates.NOR:
                        addImage(x, y, lay.xcord, lay.ycord, "nor.jpg");
                        break;
                    case Gates.NOT:
                        addImage(x, y, lay.xcord, lay.ycord, "not.jpg");
                        break;
                    case Gates.EXOR:
                        addImage(x, y, lay.xcord, lay.ycord, "xor.jpg");
                        break;
                    case Gates.SOURCE:
                        switch (lay.sourceno)
                        {
                            case "A": addImage(x, y, lay.xcord, lay.ycord, "A.jpg"); break;
                            case "B": addImage(x, y, lay.xcord, lay.ycord, "B.jpg"); break;
                            case "C": addImage(x, y, lay.xcord, lay.ycord, "C.jpg"); break;
                            case "D": addImage(x, y, lay.xcord, lay.ycord, "D.jpg"); break;
                            case "E": addImage(x, y, lay.xcord, lay.ycord, "E.jpg"); break;
                            case "F": addImage(x, y, lay.xcord, lay.ycord, "F.jpg"); break;
                            case "G": addImage(x, y, lay.xcord, lay.ycord, "G.jpg"); break;
                            default:
                                addImage(x, y, lay.xcord, lay.ycord, "source.jpg"); break;
                        }
                        break;
                    case Gates.OUTPUT:
                        addImage(x, y, lay.xcord, lay.ycord, "destination.jpg");
                        break;
                    /*case "XN":
                        addImage(x, y, imageFolder + "xnor.jpg");
                        break;*/
                }
            }
        }
        public void clearAllImages()
        {
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    if (pic[i, j] != null)
                        pic[i, j].Image = null;
                }
            }

        }
        public void addImage(int x, int y, int xcord, int ycord, String image)
        {
            String imageLocation = imageFolder + image;
            PictureBox box = pic[xcord, ycord];
            if (box == null)
            {
                box = new PictureBox();
                box.Location = new Point(x + inPinWidth, y + picHeightError);
                box.Size = new Size(width - inPinWidth - outPinWidth, height - (picHeightError * 2));
                box.Parent = this;
                pic[xcord, ycord] = box;
            }
            box.Image = Image.FromFile(imageLocation);
        }


        //replace
        public void addGates(String gate, String row, String col,String sourceno)
        {
            //row and col should be of the format R0 and C1 
            int x = Convert.ToInt32(row.Substring(1));
            int y = Convert.ToInt32(col.Substring(1));
            if (x >= 0 && y >= 0 && x < nHeight && y < nWidth)
            {
                Gates type = new Gates();
                switch (gate)
                {
                    case "and": type = Gates.AND; break;
                    case "or": type = Gates.OR; break;
                    case "nand": type = Gates.NAND; break;
                    case "nor": type = Gates.NOR; break;
                    case "not": type = Gates.NOT; break;
                    case "xor": type = Gates.EXOR; break;
                    case "source": type = Gates.SOURCE; break;
                    case "output": type = Gates.OUTPUT; break;
                    //case "xnor": array[x, y] = "XN"; break;
                }
                addToUndoStack();
                layout.Add(row + col, new Layout(x, y, type, sourceno));
            }
            this.Invalidate();
        }

        //add
        public void addLinks(String rowStart, String colStart, String rowEnd, String colEnd)
        {
            addToUndoStack();
            int xStart = Convert.ToInt32(rowStart.Substring(1));
            int yStart = Convert.ToInt32(colStart.Substring(1));
            int xEnd = Convert.ToInt32(rowEnd.Substring(1));
            int yEnd = Convert.ToInt32(colEnd.Substring(1));

            LinkDirections ld = clObject.shortestpath(xStart, yStart + 1, xEnd, yEnd);
            links.Add(linkCnt, ld);

            layout[rowStart + colStart].output.Add(linkCnt);
            layout[rowEnd + colEnd].input.Add(linkCnt);
            linkCnt++;
            this.Invalidate();
        }


        //add
        /* public void DrawLinks(int startx,int starty,int endx,int endy){    
             LinkDirections ld = clObject.shortestpath(startx,starty,endx,endy);
             connectGates(startx,starty, ld.directions, ld.offset);
         }*/
        public void connectGates(int rowS, int colS, int rowE, int colE, char[] path, int[] offset)
        {
            //lasth and v offset keep track of recent offset difference
            int lasthoffset = 0, lastvoffset = 0;
            int curry = rowS * height + start, currx = colS * width + start;
            //Method implement to add connection between gates
            //additional 3 for start,2 for end, + path length ; //4 for start, and 3 arrow points
            Point[] points = new Point[path.Length + 5];
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
            //connecting gate with first point (output)
            Point temp;
            if (true)
            {
                //comp is down
                temp = new Point(currx, curry + offset[0] + (height / 2));
            }
            /*else
            {
                //comp is above
                temp = new Point(currx, curry - offset[0] - (height / 2));
            }*/
            int i = 0;
            points[i++] = new Point(currx - (width / 2), temp.Y);
            points[i++] = temp;
            //rest of the points
            points[i++] = new Point(currx, curry);


            //calculate remaining points
            for (i = 0; i < path.Length; i++)
            {
                //int nextOffset = (i != path.Length - 1 &&
                // (((path[i] == 'L' || path[i] == 'R') && (path[i + 1] == 'U' || path[i + 1] == 'D')) ||
                // ((path[i] == 'U' || path[i] == 'D') && (path[i + 1] == 'L' || path[i + 1] == 'R')))) ? offset[i + 1] : 0;

                int nextOffset = (i != path.Length - 1) ? (offset[i + 1] == 0) ? 0 : offset[i + 1] + 5 : 0;
                //also avoids the small offset difference that gets accumulated till the end
                switch (path[i])
                {
                    case 'L': currx = currx - lasthoffset - width + nextOffset; lasthoffset = nextOffset; break;
                    case 'R': currx = currx - lasthoffset + width + nextOffset; lasthoffset = nextOffset; break;
                    case 'U': curry = curry - lastvoffset - height + nextOffset; lastvoffset = nextOffset; break;
                    case 'D': curry = curry - lastvoffset + height + nextOffset; lastvoffset = nextOffset; break;
                }
                //make the second point void if component is upwards and first point is downwards
                if (i == 0 && i != path.Length - 1 && currx == points[i + 2].X)
                {
                    points[i + 2] = new Point(currx, curry);
                }
                else if (i == (path.Length - 1) && currx == points[i + 2].X)
                {
                    if (points[i + 2].Y < curry)
                    {
                        //component is down
                        points[i + 2] = new Point(currx, curry);
                    }
                    else
                    {
                        //component is up
                        currx = points[i + 2].X;
                        curry = points[i + 2].Y;
                    }
                }
                points[i + 3] = new Point(currx, curry);
            }
            //connect last point with the gate

            if (curry <= ((height * rowE) + start + (height / 2)))
            {
                //comp is down
                points[i + 3] = new Point(currx, curry + (height / 4));
                i++;
                points[i + 3] = new Point(currx + inPinWidth, curry + ((height / 2) - 5));
            }
            else
            {
                //comp is up
                points[i + 3] = new Point(currx, curry - (height / 4));
                i++;
                points[i + 3] = new Point(currx + inPinWidth, curry - ((height / 2) - 5));
            }

            //textBox1.Text += points[i + 3].X;
            /*
            int arrowSize = 0;
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
            */
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
            GrammarBuilder undo = new GrammarBuilder("Undo");
            GrammarBuilder redo = new GrammarBuilder("Redo");
            GrammarBuilder exit = new GrammarBuilder("Exit");

            GrammarBuilder insert = new GrammarBuilder("Insert");
            Choices gates = new Choices(new string[] { "and", "or", "not", "exor", "nor", "nand", "source", "output" });
            Choices columns = new Choices(new string[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" });
            Choices rows = new Choices(new string[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" });
            //Choices orientation = new Choices(new string[] { "left", "right", "up", "down" });
            insert.Append(gates);
            insert.Append("R");
            insert.Append(rows);
            insert.Append("C");
            insert.Append(columns);
            //insert.Append("towards");
            //insert.Append(orientation);

            GrammarBuilder connect = new GrammarBuilder("Connect");
            //connect.Append("output");
            connect.Append("R");
            connect.Append(rows);
            connect.Append("C");
            connect.Append(columns);
            //connect.Append("to");
            //connect.Append("input");
            connect.Append("R");
            connect.Append(rows);
            connect.Append("C");
            connect.Append(columns);

            GrammarBuilder removeGate = new GrammarBuilder("Remove");
            removeGate.Append("gate");
            removeGate.Append("R");
            removeGate.Append(rows);
            removeGate.Append("C");
            removeGate.Append(columns);

            GrammarBuilder removeLink = new GrammarBuilder("Remove");
            removeLink.Append("link");
            removeLink.Append("R");
            removeLink.Append(rows);
            removeLink.Append("C");
            removeLink.Append(columns);
            removeLink.Append("R");
            removeLink.Append(rows);
            removeLink.Append("C");
            removeLink.Append(columns);



            GrammarBuilder droidStart = new GrammarBuilder("droid");


            Grammar _clear_grammar = new Grammar(clear);
            Grammar _insert_grammar = new Grammar(insert);
            Grammar _connect_grammar = new Grammar(connect);
            Grammar _droid_Start = new Grammar(droidStart);
            Grammar _undo = new Grammar(undo);
            Grammar _redo = new Grammar(redo);
            Grammar _exit = new Grammar(exit);
            Grammar _removeGate = new Grammar(removeGate);
            Grammar _removeLink = new Grammar(removeLink);

            recognizer.LoadGrammarAsync(_clear_grammar);
            recognizer.LoadGrammarAsync(_insert_grammar);
            recognizer.LoadGrammarAsync(_connect_grammar);
            recognizer.LoadGrammarAsync(_droid_Start);
            recognizer.LoadGrammarAsync(_undo);
            recognizer.LoadGrammarAsync(_redo);
            recognizer.LoadGrammarAsync(_exit);
            recognizer.LoadGrammarAsync(_removeGate);
            recognizer.LoadGrammarAsync(_removeLink);

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
            string droid = e.Result.Text;
            string[] tokens = droid.Split(' ');
            textBox1.ForeColor = Color.Black;

            if (tokens[0] == "droid")
            {
                textBox1.Text = "Ready!!!. Listening..";

                if (secondThreadForm == null || !secondThreadForm.IsHandleCreated)
                {
                    secondThreadForm = SecondUIThreadForm.Create();
                    secondThreadForm.Focus();
                    droidReady = true;
                }

            }

            else if (tokens[0] == "Insert" && droidReady == true)
            {
                if (tokens.Length != 6)
                { return; }

                string gate = tokens[1];
                string row = "R";
                string column = "C";
                int r = GetNumber(tokens[3]);
                int c = GetNumber(tokens[5]);
                if (r == -1 || c == -1)
                    return;
                if (r > 6)
                {
                    textBox1.Text = "Illegal location R" + r + ", Try Again";
                    textBox1.Enabled = true;
                    textBox1.BackColor = Color.White;
                    textBox1.ForeColor = Color.Red;
                    this.Focus();
                    droidReady = false;
                    droidReady = false;
                    if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                        secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));
                    return;
                }

                if (c > 16)
                {
                    textBox1.Text = "Illegal location C" + c + ", Try Again";
                    textBox1.Enabled = true;
                    textBox1.BackColor = Color.White;
                    textBox1.ForeColor = Color.Red;
                    this.Focus();
                    droidReady = false;
                    if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                        secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));
                    return;
                }

                if (layout.ContainsKey(row + r + column + c))
                {
                    textBox1.Text = "A gate already exits in the slot R" + r + " C" + c;
                    textBox1.Enabled = true;
                    textBox1.BackColor = Color.White;
                    textBox1.ForeColor = Color.Red;
                    this.Focus();
                    droidReady = false;
                    if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                        secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));

                    this.Focus();
                    return;
                }


                row = row + r;
                column = column + c;
                switch (gate)
                {
                    case "source": String source = ((char)sourceAlphabet).ToString(); addGates(gate, row, column, source); sourceAlphabet += 1; break;
                    default: addGates(gate, row, column, ""); break;
                }

                textBox1.Text = "Insert " + gate.ToUpper() + " R" + r + " C" + c;
                droidReady = false;
                historyOfdroids = historyOfdroids + "Insert " + gate.ToUpper() + " R" + r + " C" + c + "\r\n";
                textBox2.Text = historyOfdroids;

                if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                    secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));

                this.Focus();

            }
            else if (tokens[0] == "Connect" && droidReady == true)
            {
                if (tokens.Length != 9)
                { return; }

                String rowS = "R" + GetNumber(tokens[2]);
                String colS = "C" + GetNumber(tokens[4]);
                String rowE = "R" + GetNumber(tokens[6]);
                String colE = "C" + GetNumber(tokens[8]);
                if (GetNumber(tokens[2]) > 6)
                {
                    textBox1.Text = "Illegal location R" + GetNumber(tokens[2]) + ", Try Again";
                    textBox1.Enabled = true;
                    textBox1.BackColor = Color.White;
                    textBox1.ForeColor = Color.Red;
                    this.Focus();
                    if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                        secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));
                    return;
                }

                if (GetNumber(tokens[4]) > 16)
                {
                    textBox1.Text = "Illegal location C" + GetNumber(tokens[4]) + ", Try Again";
                    textBox1.Enabled = true;
                    textBox1.BackColor = Color.White;
                    textBox1.ForeColor = Color.Red;
                    this.Focus();
                    if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                        secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));
                    return;
                }
                if (GetNumber(tokens[6]) > 6)
                {
                    textBox1.Text = "Illegal location R" + GetNumber(tokens[6]) + ", Try Again";
                    textBox1.Enabled = true;
                    textBox1.BackColor = Color.White;
                    textBox1.ForeColor = Color.Red;
                    this.Focus();
                    if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                        secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));
                    return;

                }
                if (GetNumber(tokens[8]) > 16)
                {
                    textBox1.Text = "Illegal location C" + GetNumber(tokens[8]) + ", Try Again";
                    textBox1.Enabled = true;
                    textBox1.BackColor = Color.White;
                    textBox1.ForeColor = Color.Red;
                    this.Focus();
                    if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                        secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));
                    return;

                }

                if (!layout.ContainsKey("R" + GetNumber(tokens[2]) + "C" + GetNumber(tokens[4])))
                {
                    textBox1.Text = "Please add gate in slot R" + GetNumber(tokens[2]) + " C" + GetNumber(tokens[4]) + " before connecting";
                    textBox1.Enabled = true;
                    textBox1.BackColor = Color.White;
                    textBox1.ForeColor = Color.Red;
                    this.Focus();
                    droidReady = false;
                    if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                        secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));

                    this.Focus();
                    return;
                }

                if (!layout.ContainsKey("R" + GetNumber(tokens[6]) + "C" + GetNumber(tokens[8])))
                {
                    textBox1.Text = textBox1.Text = "Please add gate in slot R" + GetNumber(tokens[6]) + " C" + GetNumber(tokens[8]) + " before connecting";
                    textBox1.Enabled = true;
                    textBox1.BackColor = Color.White;
                    textBox1.ForeColor = Color.Red;
                    this.Focus();
                    droidReady = false;
                    if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                        secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));

                    this.Focus();
                    return;
                }


                foreach (int index in links.Keys)
                {
                    LinkDirections temp = links[index];
                    if (temp.x == GetNumber(tokens[2]) && temp.y == GetNumber(tokens[4]) + 1 && temp.destX == GetNumber(tokens[6]) && temp.destY == GetNumber(tokens[8]))
                    {
                        textBox1.Text = "Link R" + GetNumber(tokens[2]) + " C" + GetNumber(tokens[4]) + "R" + GetNumber(tokens[6]) + " C" + GetNumber(tokens[8]) + " already Exists, Choose another one!";
                        textBox1.Enabled = true;
                        textBox1.BackColor = Color.White;
                        textBox1.ForeColor = Color.Red;
                        this.Focus();
                        if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                            secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));
                        return;
                    }

                }
                droidReady = false;
                textBox1.Text = "Connect " + rowS + " " + colS + " " + rowE + " " + colE;
                historyOfdroids = historyOfdroids + "Connect " + rowS + " " + colS + " " + rowE + " " + colE + "\r\n";
                textBox2.Text = historyOfdroids;
                addLinks(rowS, colS, rowE, colE);

                if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                    secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));

                this.Focus();

            }
            else if (tokens[0] == "Clear" && droidReady == true)
            {

                textBox1.Text = "Are you sure you want to clear the screen? Please say yes or no!";
                textBox1.Enabled = true;
                textBox1.BackColor = Color.White;
                textBox1.ForeColor = Color.Red;
                this.Focus();

                SpeechRecognitionEngine recognizerYesNo = new SpeechRecognitionEngine();

                //recognizerYesNo.LoadGrammarCompleted += new EventHandler<LoadGrammarCompletedEventArgs>(LoadGrammarCompleted);
                recognizerYesNo.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognizedForYesNo);
                recognizerYesNo.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(SpeechRejectedForYesNo);
                recognizerYesNo.SetInputToDefaultAudioDevice();

                GrammarBuilder yes = new GrammarBuilder("yes");
                GrammarBuilder no = new GrammarBuilder("no");
                Grammar _yes = new Grammar(yes);
                Grammar _no = new Grammar(no);
                recognizerYesNo.LoadGrammarAsync(_yes);
                recognizerYesNo.LoadGrammarAsync(_no);

                recognizerYesNo.Recognize();

                while (yesNoForRemoveGateOrLink == "invalid")
                {
                    textBox1.Text = "I could not hear you. Are you sure you want to clear the screen? Please say yes or no!";
                    textBox1.BackColor = Color.White;
                    textBox1.ForeColor = Color.Red;
                    this.Focus();
                    recognizerYesNo.Recognize();
                }

                textBox1.Clear();
                textBox1.BackColor = Color.White;
                textBox1.ForeColor = Color.Black;

                if (yesNoForRemoveGateOrLink == "yes")
                {
                    //Clear logic                  
                    clear();
                    textBox1.Text = "Clear";
                    historyOfdroids = historyOfdroids + e.Result.Text + "\r\n";
                    textBox2.Text = historyOfdroids;
                }

                droidReady = false;
                if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                    secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));

                this.Focus();

            }
            else if (tokens[0] == "Undo" && droidReady == true)
            {
                textBox1.Text = e.Result.Text;
                historyOfdroids = historyOfdroids + e.Result.Text + "\r\n";
                textBox2.Text = historyOfdroids;
                droidReady = false;

                //UNDO LOGIC
                if (currStart == currEnd)
                {
                    textBox1.Text = "No previous state exits.";
                }
                else
                    undo();
                if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                    secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));
                this.Focus();
            }
            else if (tokens[0] == "Redo" && droidReady == true)
            {

                textBox1.Text = e.Result.Text;
                historyOfdroids = historyOfdroids + e.Result.Text + "\r\n";
                textBox2.Text = historyOfdroids;
                droidReady = false;

                //REDO LOGIC
                if (currStart2 == currEnd2)
                {
                    textBox1.Text = "No future state exits.";
                }
                else
                    redo();

                if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                    secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));
                this.Focus();
            }
            else if (tokens[0] == "Remove" && tokens[1] == "gate" && droidReady == true)
            {
                if (GetNumber(tokens[3]) > 6)
                {
                    textBox1.Text = "Illegal location R" + GetNumber(tokens[2]) + ", Try Again";
                    textBox1.Enabled = true;
                    textBox1.BackColor = Color.White;
                    textBox1.ForeColor = Color.Red;
                    this.Focus();
                    if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                        secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));
                    return;
                }

                if (GetNumber(tokens[5]) > 16)
                {
                    textBox1.Text = "Illegal location C" + GetNumber(tokens[4]) + ", Try Again";
                    textBox1.Enabled = true;
                    textBox1.BackColor = Color.White;
                    textBox1.ForeColor = Color.Red;
                    this.Focus();
                    if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                        secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));
                    return;
                }


                if (!layout.ContainsKey("R" + GetNumber(tokens[3]) + "C" + GetNumber(tokens[5])))
                {
                    textBox1.Text = "No gate to remove in slot R" + GetNumber(tokens[3]) + " C" + GetNumber(tokens[5]);
                    textBox1.Enabled = true;
                    textBox1.BackColor = Color.White;
                    textBox1.ForeColor = Color.Red;
                    this.Focus();
                    droidReady = false;
                    if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                        secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));

                    this.Focus();
                    return;
                }

                textBox1.Text = "Are you sure you want to remove gate R " + GetNumber(tokens[3]) + " C " + GetNumber(tokens[5]) + "? Please say yes or no!";
                textBox1.Enabled = true;
                textBox1.BackColor = Color.White;
                textBox1.ForeColor = Color.Red;
                this.Focus();

                SpeechRecognitionEngine recognizerYesNo = new SpeechRecognitionEngine();

                //recognizerYesNo.LoadGrammarCompleted += new EventHandler<LoadGrammarCompletedEventArgs>(LoadGrammarCompleted);
                recognizerYesNo.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognizedForYesNo);
                recognizerYesNo.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(SpeechRejectedForYesNo);
                recognizerYesNo.SetInputToDefaultAudioDevice();

                GrammarBuilder yes = new GrammarBuilder("yes");
                GrammarBuilder no = new GrammarBuilder("no");
                Grammar _yes = new Grammar(yes);
                Grammar _no = new Grammar(no);
                recognizerYesNo.LoadGrammarAsync(_yes);
                recognizerYesNo.LoadGrammarAsync(_no);

                recognizerYesNo.Recognize();

                while (yesNoForRemoveGateOrLink == "invalid")
                {
                    textBox1.Text = "I could not hear you. Are you sure you want to remove gate R " + GetNumber(tokens[3]) + " C " + GetNumber(tokens[5]) + "? Please say yes or no!";
                    textBox1.BackColor = Color.White;
                    textBox1.ForeColor = Color.Red;
                    this.Focus();
                    recognizerYesNo.Recognize();
                }

                textBox1.Clear();
                textBox1.BackColor = Color.White;
                textBox1.ForeColor = Color.Black;

                if (yesNoForRemoveGateOrLink == "yes")
                {
                    //remove logic                    
                    removeGates("R" + GetNumber(tokens[3]), "C" + GetNumber(tokens[5]));
                    textBox1.Text = "Remove Gate R " + GetNumber(tokens[3]) + " C " + GetNumber(tokens[5]);
                    historyOfdroids = historyOfdroids + e.Result.Text + "\r\n";
                    textBox2.Text = historyOfdroids;
                }

                droidReady = false;
                if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                    secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));

                this.Focus();

            }
            else if (tokens[0] == "Remove" && tokens[1] == "link" && droidReady == true)
            {
                if (GetNumber(tokens[3]) > 6)
                {
                    textBox1.Text = "Illegal location R" + GetNumber(tokens[2]) + ", Try Again";
                    textBox1.Enabled = true;
                    textBox1.BackColor = Color.White;
                    textBox1.ForeColor = Color.Red;
                    this.Focus();
                    if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                        secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));
                    return;
                }

                if (GetNumber(tokens[5]) > 16)
                {
                    textBox1.Text = "Illegal location C" + GetNumber(tokens[4]) + ", Try Again";
                    textBox1.Enabled = true;
                    textBox1.BackColor = Color.White;
                    textBox1.ForeColor = Color.Red;
                    this.Focus();
                    if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                        secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));
                    return;
                }
                if (GetNumber(tokens[7]) > 6)
                {
                    textBox1.Text = "Illegal location R" + GetNumber(tokens[6]) + ", Try Again";
                    textBox1.Enabled = true;
                    textBox1.BackColor = Color.White;
                    textBox1.ForeColor = Color.Red;
                    this.Focus();
                    if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                        secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));
                    return;

                }
                if (GetNumber(tokens[9]) > 16)
                {
                    textBox1.Text = "Illegal location C" + GetNumber(tokens[8]) + ", Try Again";
                    textBox1.Enabled = true;
                    textBox1.BackColor = Color.White;
                    textBox1.ForeColor = Color.Red;
                    this.Focus();
                    if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                        secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));
                    return;

                }

                bool src = false;
                foreach (int index in links.Keys)
                {
                    LinkDirections temp = links[index];
                    if (temp.x == GetNumber(tokens[3]) && temp.y == GetNumber(tokens[5]) + 1 && temp.destX == GetNumber(tokens[7]) && temp.destY == GetNumber(tokens[9]))
                    {
                        src = true;
                        break;
                    }

                }

                if (src == false)
                {
                    textBox1.Text = "Link R" + GetNumber(tokens[3]) + "C" + GetNumber(tokens[5]) + " R" + GetNumber(tokens[7]) + "C" + GetNumber(tokens[9]) + " does not Exists!";
                    textBox1.Enabled = true;
                    textBox1.BackColor = Color.White;
                    textBox1.ForeColor = Color.Red;
                    this.Focus();
                    if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                        secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));
                    return;
                }



                textBox1.Text = "Are you sure you want to remove link from R" + GetNumber(tokens[3]) + " C " + GetNumber(tokens[5]) + " to R" + GetNumber(tokens[7]) + " C " + GetNumber(tokens[9]) + "? Please say yes or no!";
                textBox1.Enabled = true;
                textBox1.BackColor = Color.White;
                textBox1.ForeColor = Color.Red;
                this.Focus();

                SpeechRecognitionEngine recognizerYesNo = new SpeechRecognitionEngine();

                //recognizerYesNo.LoadGrammarCompleted += new EventHandler<LoadGrammarCompletedEventArgs>(LoadGrammarCompleted);
                recognizerYesNo.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognizedForYesNo);
                recognizerYesNo.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(SpeechRejectedForYesNo);
                recognizerYesNo.SetInputToDefaultAudioDevice();

                GrammarBuilder yes = new GrammarBuilder("yes");
                GrammarBuilder no = new GrammarBuilder("no");
                Grammar _yes = new Grammar(yes);
                Grammar _no = new Grammar(no);
                recognizerYesNo.LoadGrammarAsync(_yes);
                recognizerYesNo.LoadGrammarAsync(_no);

                recognizerYesNo.Recognize();

                while (yesNoForRemoveGateOrLink == "invalid")
                {
                    textBox1.Text = "I could not hear you. Are you sure you want to remove link from R" + GetNumber(tokens[3]) + " C " + GetNumber(tokens[5]) + " to R" + GetNumber(tokens[7]) + " C " + GetNumber(tokens[9]) + "? Please say yes or no!";
                    textBox1.BackColor = Color.White;
                    textBox1.ForeColor = Color.Red;
                    this.Focus();
                    recognizerYesNo.Recognize();
                }

                textBox1.Clear();
                textBox1.BackColor = Color.White;
                textBox1.ForeColor = Color.Black;

                if (yesNoForRemoveGateOrLink == "yes")
                {
                    //remove logic                    
                    removeLinks("R" + GetNumber(tokens[3]), "C" + GetNumber(tokens[5]), "R" + GetNumber(tokens[7]), "C" + GetNumber(tokens[9]));
                    textBox1.Text = "Remove Link R " + GetNumber(tokens[3]) + " C " + GetNumber(tokens[5]) + " R " + GetNumber(tokens[7]) + " C " + GetNumber(tokens[9]);
                    historyOfdroids = historyOfdroids + e.Result.Text + "\r\n";
                    textBox2.Text = historyOfdroids;
                }

                droidReady = false;
                if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                    secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));
                this.Focus();
            }
            else if (tokens[0] == "Exit" && droidReady == true)
            {
                textBox1.Text = "Are you sure you want to Exit? Please say yes or no!";
                textBox1.Enabled = true;
                textBox1.BackColor = Color.White;
                textBox1.ForeColor = Color.Red;
                this.Focus();

                SpeechRecognitionEngine recognizerYesNo = new SpeechRecognitionEngine();

                //recognizerYesNo.LoadGrammarCompleted += new EventHandler<LoadGrammarCompletedEventArgs>(LoadGrammarCompleted);
                recognizerYesNo.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognizedForYesNo);
                recognizerYesNo.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(SpeechRejectedForYesNo);
                recognizerYesNo.SetInputToDefaultAudioDevice();

                GrammarBuilder yes = new GrammarBuilder("yes");
                GrammarBuilder no = new GrammarBuilder("no");
                Grammar _yes = new Grammar(yes);
                Grammar _no = new Grammar(no);
                recognizerYesNo.LoadGrammarAsync(_yes);
                recognizerYesNo.LoadGrammarAsync(_no);

                recognizerYesNo.Recognize();

                while (yesNoForRemoveGateOrLink == "invalid")
                {
                    textBox1.Text = "I could not hear you. Are you sure you want to Exit? Please say yes or no!";
                    textBox1.BackColor = Color.White;
                    textBox1.ForeColor = Color.Red;
                    this.Focus();
                    recognizerYesNo.Recognize();
                }

                textBox1.Clear();
                textBox1.BackColor = Color.White;
                textBox1.ForeColor = Color.Black;

                if (yesNoForRemoveGateOrLink == "yes")
                {
                    Environment.Exit(0);
                }


                //Exit logic

                if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                    secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));
                this.Focus();
            }
            else
            {
                droidReady = false;
                textBox1.Text = "Please say 'droid' before the expression.";
                textBox2.Text = historyOfdroids;
                if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                    secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));
                this.Focus();
            }

        }

        public void SpeechRecognizedForYesNo(object sender, SpeechRecognizedEventArgs e)
        {
            yesNoForRemoveGateOrLink = e.Result.Text;
        }

        public void SpeechRejectedForYesNo(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            yesNoForRemoveGateOrLink = "invalid";
        }

        public void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            //Console.WriteLine("Speech input failed");
            droidReady = false;

            if (secondThreadForm != null && secondThreadForm.IsHandleCreated)
                secondThreadForm.Invoke((Action)(() => secondThreadForm.Close()));

            textBox1.Text = "Failed to recognize input. Please try again, start with the word droid ";
            this.Focus();
        }

        public int GetNumber(string numberString)
        {
            int toReturn = -1;
            switch (numberString)
            {
                case "zero": toReturn = 0; break;
                case "one": toReturn = 1; break;
                case "two": toReturn = 2; break;
                case "three": toReturn = 3; break;
                case "four": toReturn = 4; break;
                case "five": toReturn = 5; break;
                case "six": toReturn = 6; break;
                case "seven": toReturn = 7; break;
                case "eight": toReturn = 8; break;
                case "nine": toReturn = 9; break;
                case "ten": toReturn = 10; break;
                case "eleven": toReturn = 11; break;
                case "twelve": toReturn = 12; break;
                case "thirteen": toReturn = 13; break;
                case "fourteen": toReturn = 14; break;
                case "fifteen": toReturn = 15; break;
                case "sixteen": toReturn = 16; break;
                case "seventeen": toReturn = 17; break;
                case "eighteen": toReturn = 18; break;
                case "nineteen": toReturn = 19; break;
            }

            return toReturn;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*
            addGates("source", "R0", "C4", "A");
            addGates("source", "R2", "C4", "B");
            addGates("or", "R1", "C6", "");
            addLinks("R0", "C4", "R1", "C6");
            addLinks("R2", "C4", "R1", "C6");

            addGates("source", "R3", "C4", "C");
            addGates("source", "R5", "C4", "D");
            addGates("or", "R4", "C6", "");
            addLinks("R3", "C4", "R4", "C6");
            addLinks("R5", "C4", "R4", "C6");


            addGates("and", "R3", "C8", "");
            addLinks("R1", "C6", "R3", "C8");
            addLinks("R4", "C6", "R3", "C8");
            
            addGates("output", "R3", "C11", "");
            addLinks("R3", "C8", "R3", "C11");
            
            addGates("xor", "R5", "C3", "");
            addGates("nand", "R5", "C5", "");
            addLinks("R5", "C3", "R5", "C5");

            addGates("and", "R5", "C0", "");
            addGates("nor", "R4", "C1", "");
            addLinks("R5", "C0", "R4", "C1");

            addGates("source", "R6", "C4", "B");
            addGates("source", "R2", "C0", "A");


            addLinks("R2", "C0", "R5", "C0");
            addLinks("R2", "C0", "R2", "C3");
            addLinks("R3", "C4", "R5", "C3");

            addGates("source", "R1", "C10", "C");
            addLinks("R6", "C8", "R1", "C10");
            addLinks("R3", "C8", "R1", "C10");

            */
            RecognizeSpeech();


            /*addGates("or", "R1", "C0");
            addGates("not", "R6", "C8");
            addLinks("R1", "C0", "R6", "C8");*/
            //removeGates("R0", "C0");
            //removeLinks("R0", "C0", "R3", "C8");
            /*addGates("xor", "R2", "C3");
            addGates("nand", "R3", "C4");
            addLinks("R2", "C3", "R3", "C4");

            addGates("xor", "R5", "C3");
            addGates("nand", "R5", "C5");
            addLinks("R5", "C3", "R5", "C5");

            addGates("and", "R5", "C0");
            addGates("nor", "R4", "C1");
            addLinks("R5", "C0", "R4", "C1");
            
            //failed test 
            /*addGates("xor", "R5", "C9");
            addGates("nand", "R5", "C10");
            addLinks("R5", "C9", "R5", "C10");*/


            //addGates("or", "R0", "C1");
            //addGates("not", "R0", "C2");
            //addGates("xnor", "R0", "C7");
            //addGates("xnor", "R5", "C7");
            //addLinks("R0", "C7", "R5", "C7");
        }

        public void clear()
        {
            addToUndoStack();
            layout = new Dictionary<String, Layout>();
            links = new Dictionary<int, LinkDirections>();
            clObject = new ConnectLink(clObject.m, clObject.n, clObject.maxLinks);
            linkCnt = 0;
        }

        public void removeGates(String row, String col)
        {
            //row and col should be of the format R0 and C1 
            int x = Convert.ToInt32(row.Substring(1));
            int y = Convert.ToInt32(col.Substring(1));
            addToUndoStack();

            for (int index = 0; index < layout[row + col].output.Count; index++)
            {
                removeLink((int)layout[row + col].output[index]);
            }

            for (int index = 0; index < layout[row + col].input.Count; index++)
            {
                removeLink((int)layout[row + col].input[index]);
            }
            layout.Remove(row + col);
            this.Invalidate();
        }
        //add
        /* public void DrawLinks(int startx,int starty,int endx,int endy){    
             LinkDirections ld = clObject.shortestpath(startx,starty,endx,endy);
             connectGates(startx,starty, ld.directions, ld.offset);
         }*/

        public void removeLink(int index)
        {
            String source = "R" + links[index].x + "C" + (links[index].y - 1);
            String dest = "R" + links[index].destX + "C" + links[index].destY;

            layout[source].output.Remove(index);
            layout[dest].input.Remove(index);

            links.Remove(index);
        }

        public void removeLinks(String row, String col, String rowDest, String colDest)
        {
            //row and col should be of the format R0 and C1 
            int x = Convert.ToInt32(row.Substring(1));
            int y = Convert.ToInt32(col.Substring(1)) + 1;
            int xDest = Convert.ToInt32(rowDest.Substring(1));
            int yDest = Convert.ToInt32(colDest.Substring(1));
            addToUndoStack();
            int sol = -1;
            foreach (int index in links.Keys)
            {
                LinkDirections temp = links[index];
                if (temp.x == x && temp.y == y && temp.destX == xDest && temp.destY == yDest)
                {
                    sol = index;
                    break;
                }

            }
            removeLink(sol);

            this.Invalidate();
            this.Focus();
        }

        private void undo()
        {
            removeFromUndoStack();
        }
        private void redo()
        {
            removeFromRedoStack();
        }

        public void addToUndoStack()
        {
            StackData s = new StackData(CloneDictionaryLayout(layout),
                CloneDictionaryLinks(links), new ConnectLink(clObject.m, clObject.n, clObject.maxLinks), linkCnt);
            if (currEnd - currStart + 1 >= 5)
            {
                currStart++;
            }
            arr[(currEnd) % 5] = s;
            currEnd++;
        }
        private void addToUndoStack(StackData s)
        {
            if (currEnd - currStart + 1 >= 5)
            {
                currStart++;
            }
            arr[(currEnd) % 5] = s;
            currEnd++;
        }
        public void removeFromUndoStack()
        {
            if (currEnd > currStart)
            {
                addToRedoStack();
                StackData temp = arr[(currEnd - 1) % 5];
                currEnd--;
                //get the prev state
                if (temp != null)
                {
                    this.layout = CloneDictionaryLayout(temp.layout);
                    this.links = CloneDictionaryLinks(temp.links);
                    this.clObject = new ConnectLink(temp.clObject.m, temp.clObject.n, temp.clObject.maxLinks);
                    this.linkCnt = temp.linkCnt;
                    this.Invalidate();
                }
            }
        }
        private void addToRedoStack()
        {
            StackData s = new StackData(CloneDictionaryLayout(layout),
                CloneDictionaryLinks(links), new ConnectLink(clObject.m, clObject.n, clObject.maxLinks), linkCnt);
            if (currEnd2 - currStart2 + 1 >= 5)
            {
                currStart2++;
            }
            arr2[(currEnd2) % 5] = s;
            currEnd2++;
        }
        private void addToRedoStack(StackData s)
        {
            if (currEnd2 - currStart2 + 1 >= 5)
            {
                currStart2++;
            }
            arr2[(currEnd2) % 5] = s;
            currEnd2++;
        }
        public void removeFromRedoStack()
        {
            if (currEnd2 > currStart2)
            {
                addToUndoStack();
                StackData temp = arr2[(currEnd2 - 1) % 5];
                currEnd2--;

                //get the prev state
                if (temp != null)
                {
                    this.layout = CloneDictionaryLayout(temp.layout);
                    this.links = CloneDictionaryLinks(temp.links);
                    this.clObject = new ConnectLink(temp.clObject.m, temp.clObject.n, temp.clObject.maxLinks);
                    this.linkCnt = temp.linkCnt;
                    this.Invalidate();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            undo();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            redo();
        }

        private static Dictionary<string, Layout> CloneDictionaryLayout(Dictionary<string, Layout> original)
        {
            Dictionary<string, Layout> ret = new Dictionary<string, Layout>(original.Count,
                                                                    original.Comparer);
            foreach (KeyValuePair<string, Layout> entry in original)
            {
                Layout value = entry.Value.copy(entry.Value);
                ret.Add(entry.Key, value);
            }
            return ret;
        }
        public static Dictionary<int, LinkDirections> CloneDictionaryLinks(Dictionary<int, LinkDirections> original)
        {
            Dictionary<int, LinkDirections> ret = new Dictionary<int, LinkDirections>(original.Count,
                                                                    original.Comparer);
            foreach (KeyValuePair<int, LinkDirections> entry in original)
            {
                LinkDirections value = entry.Value.copy(entry.Value);
                ret.Add(entry.Key, value);
            }
            return ret;
        }
    }

    class StackData
    {
        public Dictionary<String, Layout> layout;
        public Dictionary<int, LinkDirections> links;
        public ConnectLink clObject;
        public int linkCnt = 0;
        public StackData(Dictionary<String, Layout> layout1, Dictionary<int, LinkDirections> links1, ConnectLink clObject1, int linkCnt1)
        {
            layout = layout1;
            links = links1;
            clObject = clObject1;
            linkCnt = linkCnt1;
        }
    }
    //Add
    public class LinkDirections
    {
        //To find the position of the gate subtrat 1 from Y
        public int x;
        public int y;
        public int destX;
        public int destY;

        public int[] offset;
        public char[] directions;
        public LinkDirections()
        {
        }

        public LinkDirections(int size, int x, int y, int xd, int yd)
        {
            this.offset = new int[size];
            this.directions = new char[size];
            this.x = x;
            this.y = y;
            this.destX = xd;
            this.destY = yd;
        }
        public LinkDirections copy(LinkDirections input)
        {
            LinkDirections s = new LinkDirections();
            s.x = input.x;
            s.y = input.y;
            s.destX = input.destX;
            s.destY = input.destY;
            s.offset = new int[input.offset.Length];
            for (int i = 0; i < input.offset.Length; i++)
            {
                s.offset[i] = input.offset[i];
            }
            s.directions = new char[input.directions.Length];
            for (int i = 0; i < input.directions.Length; i++)
            {
                s.directions[i] = input.directions[i];
            }
            return s;
        }

    }

    //Add
    public class ConnectLink
    {
        public Dictionary<int, Dictionary<int, int>> graph;
        public int m;
        public int n;
        public int maxLinks;


        public ConnectLink(int m, int n, int max)
        {
            // TODO: Complete member initialization
            this.m = m;
            this.n = n;
            this.maxLinks = max;
            int size = m * n;
            graph = new Dictionary<int, Dictionary<int, int>>();
            for (int i = 0; i < size; i++)
                graph.Add(i, new Dictionary<int, int>());
            for (int i = 0; i < size; i++)
            {

                if (((i + 1) % n) != 0)
                {
                    graph[i].Add(i + 1, max);
                    graph[i + 1].Add(i, max);

                }

                if (i < size - n)
                {
                    graph[i].Add(i + n, max);
                    graph[i + n].Add(i, max);
                }
            }
        }

        public int cordinateToindex(int x, int y)
        {
            return (x * this.n) + y;
        }

        public int[] indesToCoordinate(int coordinate)
        {
            int[] res = new int[2];
            res[0] = coordinate % this.m;
            res[1] = coordinate % this.n;

            return res;
        }

        public LinkDirections shortestpath(int x1, int y1, int x2, int y2)
        {
            int start = cordinateToindex(x1, y1);
            int dest = cordinateToindex(x2, y2);
            int size = this.m * this.n;
            Queue<int> queue = new Queue<int>();
            int[] dist = new int[size];
            int[] prev = new int[size];
            int[] color = new int[size];


            for (int i = 0; i < size; i++)
            {
                dist[i] = int.MaxValue;
                prev[i] = -1;
                color[i] = 0;
            }

            dist[start] = 0;
            color[start] = 1;
            queue.Enqueue(start);

            while (queue.Count != 0)
            {
                int temp = queue.Dequeue();

                foreach (int i in graph[temp].Keys)
                    if (i != temp && graph[temp][i] > 0)
                        if (color[i] == 0)
                        {
                            color[i] = 1;
                            dist[i] = dist[temp] + 1;
                            prev[i] = temp;
                            queue.Enqueue(i);
                        }
                color[temp] = 2;
            }

            int totalLen = dist[dest];
            LinkDirections ld = new LinkDirections(totalLen, x1, y1, x2, y2);
            int next = dest;
            int j = totalLen - 1;
            while (next != start)
            {
                if ((prev[next] + 1) == next)
                    ld.directions[j] = 'R';
                else if (prev[next] == (next + 1))
                    ld.directions[j] = 'L';
                else if (prev[next] > next)
                    ld.directions[j] = 'U';
                else
                    ld.directions[j] = 'D';
                ld.offset[j] = this.maxLinks - this.graph[prev[next]][next];
                j--;
                this.graph[prev[next]][next] -= 1;
                this.graph[next][prev[next]] -= 1;
                next = prev[next];
            }

            return ld;
        }
    }

    //Add
    class Layout
    {
        public int xcord;
        public int ycord;
        public Gates type;
        public String sourceno;
        public ArrayList input;
        public ArrayList output;
        public Layout()
        {
        }

        public Layout(int x, int y, Gates type,String sourceno)
        {
            this.xcord = x;
            this.ycord = y;
            this.type = type;
            this.sourceno = sourceno;
            this.input = new ArrayList();
            this.output = new ArrayList();
        }
        public Layout copy(Layout input)
        {
            Layout n = new Layout();
            n.xcord = input.xcord;
            n.ycord = input.ycord;
            n.type = input.type;
            n.sourceno = input.sourceno;
            n.input = new ArrayList(input.input.Count);
            for (int i = 0; i < input.input.Count; i++)
            {
                n.input.Insert(i, input.input[i]);
            }
            n.output = new ArrayList(input.output.Count);
            for (int i = 0; i < input.output.Count; i++)
            {
                n.output.Insert(i, input.output[i]);
            }
            return n;
        }

    }

    enum Gates { AND, OR, EXOR, NOT, NAND, NOR, SOURCE, OUTPUT };
}
