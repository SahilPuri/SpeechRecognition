using System;
using System.Collections;
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
        static String imageFolder = "C:\\Users\\Sail\\Documents\\GitHub\\SpeechRecognition\\DigitalCircuitDesign\\images\\";

        /*Start of State of the System*/
        Dictionary<String,Layout> layout = new Dictionary<String,Layout>();
        Dictionary<int, LinkDirections> links = new Dictionary<int, LinkDirections>();
        ConnectLink clObject;
        int linkCnt = 0;
        /*End of State of the System*/
        
        public Form1()
        {
            InitializeComponent();
            clearScreen();
            clObject = new ConnectLink(nHeight, nWidth,2);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            DrawGrid();
            DrawComponents();
            //RecognizeSpeech();
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
            layout = new Dictionary<String, Layout>();
            links = new Dictionary<int, LinkDirections>();
            linkCnt = 0;
        }
        public void DrawComponents()
        {
            foreach(LinkDirections ld in links.Values){
                connectGates(ld.x, ld.y, ld.directions, ld.offset);
            }
            foreach (Layout lay in layout.Values)
            {
                int x = start + lay.ycord * height + 1;
                int y = start + lay.xcord * width + error;

                switch (lay.type)
                {
                    case Gates.AND:
                        addImage(x, y, "and.jpg");
                        break;
                    case Gates.OR:
                        addImage(x, y, "or.jpg");
                        break;
                    case Gates.NAND:
                        addImage(x, y, "nand.jpg");
                        break;
                    case Gates.NOR:
                        addImage(x, y, "nor.jpg");
                        break;
                    case Gates.NOT:
                        addImage(x, y, "not.jpg");
                        break;
                    case Gates.EXOR:
                        addImage(x, y, "xor.jpg");
                        break;
                    /*case "XN":
                        addImage(x, y, imageFolder + "xnor.jpg");
                        break;*/
                }
            }
        }
        
      
        public void addImage(int x, int y, String image)
        {
            String imageLocation = imageFolder + image;
            PictureBox box = new PictureBox();
            box.Location = new Point(x, y);
            box.Image = Image.FromFile(imageLocation);
            box.Size = new Size(width - 1, height - error);
            box.Parent = this;
        }

        //replace
        public void addGates(String gate, String row, String col)
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
                    //case "xnor": array[x, y] = "XN"; break;
                }
                layout.Add(row+col, new Layout(x, y, type));
            }
            this.Invalidate();
        }

        //add
        public void addLinks(String rowStart, String colStart, String rowEnd, String colEnd)
        {
            int xStart = Convert.ToInt32(rowStart.Substring(1));
            int yStart = Convert.ToInt32(colStart.Substring(1));
            int xEnd = Convert.ToInt32(rowEnd.Substring(1));
            int yEnd = Convert.ToInt32(colEnd.Substring(1));
            LinkDirections ld = clObject.shortestpath(xStart, yStart + 1, xEnd, yEnd);
            links.Add(linkCnt,ld);
            layout[rowStart + colStart].output.Add(linkCnt);
            layout[rowEnd + colEnd].input.Add(linkCnt);
        }

          
        //add
       /* public void DrawLinks(int startx,int starty,int endx,int endy){    
            LinkDirections ld = clObject.shortestpath(startx,starty,endx,endy);
            connectGates(startx,starty, ld.directions, ld.offset);
        }*/
        public void connectGates(int rowS, int colS, char[] path, int[] offset)
        {
            //lasth and v offset keep track of recent offset difference
            int lasthoffset=0, lastvoffset=0;
            int curry = rowS * height + start, currx = colS * width + start;
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
            addGates("nor", "R5", "C8");
            addLinks("R0", "C0", "R5", "C8");
            //addGates("or", "R0", "C1");
            //addGates("not", "R0", "C2");
            //addGates("xor", "R0", "C3");
            //addGates("nand", "R0", "C4");
            
            //addGates("xnor", "R0", "C10");
            //addGates("xnor", "R7", "C19");
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

        public LinkDirections(int size, int x, int y, int xd, int yd)
        {
            this.offset = new int[size];
            this.directions = new char[size];
            this.x = x;
            this.y = y;
            this.destX = xd;
            this.destY = yd;
        }
    }

    //Add
    public class ConnectLink
    {
        Dictionary<int, Dictionary<int, int>> graph;
        private int m;
        private int n;
        private int maxLinks;


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
            LinkDirections ld = new LinkDirections(totalLen,x1,y1,x2,y2);
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
        public ArrayList input;
        public ArrayList output;

        public Layout(int x, int y, Gates type)
        {
            this.xcord = x;
            this.ycord = y;
            this.type = type;
            this.input = new ArrayList();
            this.output = new ArrayList();
        }
    }

    enum Gates{AND,OR,EXOR,NOT,NAND,NOR};
}
