using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Dictionary;

namespace RandomArrayProject
{
    public class GridClass
    {
        int[] lookupX = { 0, 0, 1, -1, 1, -1, -1, 1 };
        int[] lookupY = { 1, -1, 0, 0, 1, -1, 1, -1 };
        static Random r = new Random();

        public char[,] Puzzle { get; set; }

        public List<Words> WordList { get; set; }
        public List<Words> Dictionary { get; set; }

        public GridClass()
        {
            InitializeGrid();
        }

        /// <summary>
        /// Initialize the grid array
        /// </summary>
        void InitializeGrid()
        {
            Puzzle = new char[10, 10];

            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    Puzzle[i, j] = ' ';
        }

        /// <summary>
        /// Initialize the word list
        /// </summary>
        void InitializeWordList()
        {
            WordList = new List<Words>();

            if (Dictionary == null)
                Dictionary = ReadDictionary();

            int index = r.Next(0, Dictionary.Count);

            for (int i = 0; i < 10; i++)
            {
                WordList.Add(Dictionary[(index) % 15]);
                index = index + 2;
            }
        }

        /// <summary>
        /// Get words from the dictionary
        /// </summary>
        /// <returns>List</returns>
        List<Words> ReadDictionary()
        {
            int index;
            List<Words> tempList = new List<Words>();
            string path = ConfigurationManager.AppSettings.Get("Dictionary");
            Dictionary<string, string> dictionary = InputText.InputWords(path);

            for (int i = 0; i < 15; i++)
            {
                index = r.Next(0, dictionary.Count);
                tempList.Add(new Words(dictionary.ElementAt(index).Key, dictionary.ElementAt(index).Value));
                dictionary.Remove(dictionary.ElementAt(index).Key);
            }

            return tempList;
        }

        /// <summary>
        /// Generate the puzzle grid
        /// </summary>
        public void GeneratePuzzle()
        {
            int loopCount = 0;

            do
            {
                loopCount++;

                if (loopCount > 25)
                {
                    loopCount = 0;
                    Dictionary = null;
                }

                InitializeWordList();
            } while (!Arrange());
        }

        /// <summary>
        /// Checks if the list of words can be arranged in the puzzle
        /// </summary>
        /// <returns>bool</returns>
        public bool Arrange()
        {
            bool isFit = false;
            if (isArrangable(FindNextWord()))
            {
                completeGrid();
                isFit = true;
            }
            
            return isFit;
        }

        /// <summary>
        /// Checks if the word can be arranged in the grid
        /// </summary>
        /// <param name="word"></param>
        /// <returns>bool</returns>
        private bool isArrangable(Words word)
        {
            if (word == null)
                return true;

            PuzzleState currentState;
            string options = string.Empty;

            if (!string.IsNullOrEmpty(word.Options))
                options = word.Options;
            else if (word.StartIndex == -1)
                options = FindAvaiableOptions(word);

            if (!string.IsNullOrEmpty(options))
            {
                WordList[WordList.IndexOf(word)] = word = Initialize(word, options);
                currentState = SaveState(word);
                Add(word);
                
                if (isArrangable(FindNextWord()))
                    return true;
                else
                {
                    RetractState(currentState);
                    if (isArrangable(word))
                        return true;
                    else
                        return false;
                }
            }            
            return false;
        }

        /// <summary>
        /// Gets the next word to be arranged
        /// </summary>
        /// <returns>Words</returns>
        private Words FindNextWord()
        {
            return WordList.Find(w => string.IsNullOrEmpty(w.Options));
        }

        /// <summary>
        /// Finds existing start indices or directions for a word
        /// </summary>
        /// <param name="currentWord"></param>
        /// <returns>string</returns>
        private string FindAvaiableOptions(Words currentWord)
        {
            int randomDirection, wordCounter, xIndex, yIndex;
            bool isFit;
            StringBuilder options = new StringBuilder();
            StringBuilder direction;

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    direction = new StringBuilder();
                    randomDirection = 0;

                    while (randomDirection < 8)
                    {
                        if (x + currentWord.Word.Length * lookupX[randomDirection] >= -1 && x + currentWord.Word.Length * lookupX[randomDirection] <= 10 &&
                            y + currentWord.Word.Length * lookupY[randomDirection] >= -1 && y + currentWord.Word.Length * lookupY[randomDirection] <= 10)
                        {
                            xIndex = x;
                            yIndex = y;
                            wordCounter = 0;
                            isFit = true;

                            while (wordCounter < currentWord.Word.Length)
                            {
                                if (Puzzle[xIndex, yIndex].Equals(' ') || Puzzle[xIndex, yIndex].Equals(currentWord.Word.Substring(wordCounter, 1)))
                                {
                                    xIndex = xIndex + lookupX[randomDirection];
                                    yIndex = yIndex + lookupY[randomDirection];
                                }
                                else
                                {
                                    isFit = false;
                                    break;
                                }
                                wordCounter++;
                            }
                        }
                        else
                            isFit = false;

                        if (isFit)
                            direction.Append(randomDirection);

                        randomDirection++;
                    }

                    if (direction.Length > 0)
                    {
                        if (options.Length > 0)
                            options.Append("|");

                        options.Append(x).Append(y).Append(",").Append(direction);
                    }
                }
            }
            return options.ToString();
        }

        /// <summary>
        /// Set the index and direction of the word
        /// </summary>
        /// <param name="word"></param>
        /// <param name="options"></param>
        /// <returns>Words</returns>
        private Words Initialize(Words word, string options)
        {
            int startIndex = -1;
            int direction = -1;
            int endIndex = -1;

            if (!string.IsNullOrEmpty(options))
            {
                string[] optionArray = options.Split('|');
                int currentOptionIndex = r.Next(optionArray.Length);
                string currentOption = optionArray[currentOptionIndex];
                StringBuilder o = new StringBuilder();

                int oIndex = options.IndexOf(currentOption);
                int oLength = currentOption.Length;

                string i = currentOption.Split(',')[0];
                string d = currentOption.Split(',')[1];
                string dir = d.Substring(r.Next(d.Length), 1);

                if (d.Length > 1)
                {
                    d = d.Replace(dir, "");
                    options = options.Replace(currentOption, i + "," + d);
                }
                else
                {
                    for (int c = 0; c < optionArray.Length; c++)
                    {
                        if (c == currentOptionIndex)
                            continue;

                        if (o.Length > 0)
                            o.Append("|");

                        o.Append(optionArray[c]);
                    }
                    options = o.ToString();
                }

                startIndex = Convert.ToInt32(i);
                direction = Convert.ToInt32(dir);
                endIndex = Convert.ToInt32((startIndex / 10 + lookupX[direction] * (word.Word.Length - 1)).ToString() + 
                    (startIndex % 10 + lookupY[direction] * (word.Word.Length - 1)).ToString());
            }

            word.Direction = direction;
            word.StartIndex = startIndex;
            word.EndIndex = endIndex;
            word.Options = options;

            return word;
        }

        /// <summary>
        /// Save the current puzzle state
        /// </summary>
        /// <param name="word"></param>
        /// <returns>PuzzleState</returns>
        private PuzzleState SaveState(Words word)
        {
            StringBuilder overlaps = new StringBuilder();
            int count = 0;
            int indexX = word.StartIndex / 10;
            int indexY = word.StartIndex % 10;

            while (count < word.Word.Length)
            {
                if (overlaps.Length > 0)
                    overlaps.Append(",");
                if (!Puzzle[indexX, indexY].Equals(' '))
                {
                    overlaps.Append(indexX).Append(indexY);
                }

                indexX = indexX + lookupX[word.Direction];
                indexY = indexY + lookupY[word.Direction];
                count++;
            }

            return new PuzzleState(word.StartIndex, word.Direction, word.Word.Length, overlaps.ToString());
        }

        /// <summary>
        /// Reverts to the saved state
        /// </summary>
        /// <param name="currentState"></param>
        private void RetractState(PuzzleState currentState)
        {
            int indexX = currentState.StartIndex / 10;
            int indexY = currentState.StartIndex % 10;
            int length = currentState.Length;
            int direction = currentState.Direction;
            string currentIndex = string.Empty;
            int count = 0;

            while (count < length)
            {
                currentIndex = indexX.ToString() + indexY.ToString();
                if (currentState.Overlaps.IndexOf(currentIndex) < 0)                
                    Puzzle[indexX, indexY] = ' ';                
                indexX = indexX + lookupX[direction];
                indexY = indexY + lookupY[direction];
                count++;
            }
        }

        /// <summary>
        /// Adds the current word to the puzzle
        /// </summary>
        /// <param name="word"></param>
        private void Add(Words word)
        {
            int indexX = word.StartIndex / 10;
            int indexY = word.StartIndex % 10;
            int currentWordLength = word.Word.Length;
            int direction = word.Direction;
            int count = 0;

            while (count < currentWordLength)
            {
                Puzzle[indexX, indexY] = word.Word.Substring(count, 1).ToCharArray()[0];
                indexX = indexX + lookupX[direction];
                indexY = indexY + lookupY[direction];
                count++;
            }
        }

        /// <summary>
        /// Fills the puzzle with random letters
        /// </summary>
        private void completeGrid()
        {
            int index, indexX, indexY, intAlpha, count;

            index = r.Next(0, 99);
            intAlpha = 0;
            count = 0;

            while (count < 100)
            {
                indexX = index / 10;
                indexY = index % 10;

                if (Puzzle[indexX, indexY].Equals(' '))
                    Puzzle[indexX, indexY] = char.ConvertFromUtf32((intAlpha % 26) + 97).ToCharArray()[0];

                index = (index + 3) % 100;
                intAlpha++;
                count++;
            }
        }
    }

    public class PuzzleState
    {
        public PuzzleState(int i, int d, int l, string o)
        {
            StartIndex = i;
            Direction = d;
            Length = l;
            Overlaps = o;
        }

        public int StartIndex { get; set; }

        public int Direction { get; set; }

        public int Length { get; set; }

        public string Overlaps { get; set; }
    }
}
