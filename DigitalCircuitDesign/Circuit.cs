using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;
using System.Windows.Forms;

namespace DigitalCircuitDesign
{
    class Circuit
    {
        int[] array = new int[5] { 1, 2, 3, 4, 5 };
        public void StartDesign()
        {
            
            //In Process SpeewchRecognizer
            SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();                     

            recognizer.LoadGrammarCompleted += new EventHandler<LoadGrammarCompletedEventArgs>(LoadGrammarCompleted);
            recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognized);
            recognizer.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(SpeechRejected);  
            recognizer.SetInputToDefaultAudioDevice();

            GrammarBuilder clear = new GrammarBuilder("Clear");
 

            GrammarBuilder insert = new GrammarBuilder("Insert");
            Choices gates = new Choices(new string[] { "and", "or", "not", "ex or", "nor", "nand" });
            Choices columns = new Choices(new string[] { "one", "too", "three", "four", "five", "six", "seven", "eight" });
            Choices rows = new Choices(new string[] { "one", "too", "three", "four", "five" });
            Choices orientation = new Choices(new string[] { "left", "right", "up", "down" });
            insert.Append(gates);
            insert.Append(columns);
            insert.Append(rows);
            insert.Append("towards");
            insert.Append(orientation);

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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            

            //recognizer.RecognizeAsync(RecognizeMode.Multiple);
            while (true)
            {
                recognizer.Recognize();
            }
            
 
        }
            
        static void LoadGrammarCompleted(object sender, LoadGrammarCompletedEventArgs e)
        {
            //Console.WriteLine(e.Grammar.Name + " successfully loaded");            
            MessageBox.Show("Done Loading Grammer");
        }

        static void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //Console.WriteLine("Speech recognized: " + e.Result.Text);
            MessageBox.Show(e.Result.Text);
        }

        static void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            //Console.WriteLine("Speech input failed");
            MessageBox.Show("Failed");

        }   
            
    }
}
