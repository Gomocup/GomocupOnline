using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace GomocupOnline.Models
{
    public class GomokuMatchModel
    {
        public string FileName { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string Player1 { get; set; }

        public string Player2 { get; set; }

        public GomokuMove[] Moves { get; set; }

        public int Result { get; set; }

        public GomokuMatchModel(string path)
        {
            FileName = Path.GetFileName(path);

            //string[] lines = File.ReadAllLines(path);

            List<string> lines = new List<string>();

            using (Stream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                StreamReader reader = new StreamReader(stream, Encoding.ASCII);

                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Trim().Length > 0)
                        lines.Add(line);
                }

            }

            var moves = new List<GomokuMove>();

            if (lines.Count > 0)
            {
                string line0 = lines[0].Substring("Piskvorky ".Length);
                int commaPos = line0.IndexOf(',');
                if (commaPos >= 0)
                {
                    line0 = line0.Substring(0, commaPos);
                    string[] size = line0.Split('x');
                    Width = int.Parse(size[0]);
                    Height = int.Parse(size[1]);
                }
                

                for (int i = 1; i < lines.Count - 3; i++)
                {
                    string[] numbers = lines[i].Split(',');
                    if (numbers.Length != 3)
                        break;

                    GomokuMove move = new GomokuMove()
                    {
                        X = int.Parse(numbers[0]),
                        Y = int.Parse(numbers[1]),
                        DurationMS = int.Parse(numbers[2]),
                    };
                    moves.Add(move);
                }

                if( lines.Count > 4 )
                {
                    Player1 = lines[lines.Count - 3];
                    Player2 = lines[lines.Count - 2];
                    string result = lines[lines.Count - 1];
                    Result = int.Parse(result);

                    string pbrain = "pbrain-";
                    string exe = ".exe";
                    string zip = ".zip";

                    if (Player1.ToLower().StartsWith(pbrain))
                    {
                        Player1 = Player1.Substring(pbrain.Length);
                    }
                    if (Player1.ToLower().EndsWith(exe))
                    {
                        Player1 = Player1.Substring(0, Player1.Length - exe.Length);
                    }
                    else if (Player1.ToLower().EndsWith(zip))
                    {
                        Player1 = Player1.Substring(0, Player1.Length - zip.Length);
                    }

                    if (Player2.ToLower().StartsWith(pbrain))
                    {
                        Player2 = Player2.Substring(pbrain.Length);
                    }
                    if (Player2.ToLower().EndsWith(exe))
                    {
                        Player2 = Player2.Substring(0, Player2.Length - exe.Length);
                    }
                    else if (Player2.ToLower().EndsWith(zip))
                    {
                        Player2 = Player2.Substring(0, Player2.Length - zip.Length);
                    }
                }
            }
            Moves = moves.ToArray();

        }
    }

    public class GomokuMove
    {
        public int X { get; set; }

        public int Y { get; set; }

        /// <summary>
        /// trvani tahu v milisekundach
        /// </summary>
        public int DurationMS { get; set; }
    }
}