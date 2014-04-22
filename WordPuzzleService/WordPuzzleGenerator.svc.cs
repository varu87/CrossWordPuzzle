using System.ServiceModel.Activation;
using RandomArrayProject;
using System.Text;

namespace WordPuzzleService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WordPuzzleGenerator : IWordPuzzleGenerator
    {
        GridClass grid = new GridClass();

        /// <summary>
        /// Generate the puzzle grid, solution array and hints
        /// </summary>
        /// <returns>CrossWordPuzzle</returns>
        public CrossWordPuzzle GetPuzzle()
        {            
            grid.GeneratePuzzle();
            CrossWordPuzzle data = new CrossWordPuzzle();
            data.Grid = GenerateGrid();
            data.Check = GenerateSolution();
            data.Meanings = GetHints();
            return data;
        }

        /// <summary>
        /// Generate the unsolved grid
        /// </summary>
        /// <returns>string</returns>
        string GenerateGrid()
        {
            StringBuilder p = new StringBuilder();

            for (int row = 0; row < 10; row++)
                for (int column = 0; column < 10; column++)
                    p.Append(grid.Puzzle[row, column]);

            return p.ToString();
        }

        /// <summary>
        /// Generate the solution array
        /// </summary>
        /// <returns>string[]</returns>
        string[] GenerateSolution()
        {
            string[] s = new string[10];

            for (int i = 0; i < 10; i++)
                s[i] = string.Format("{0},{1}", grid.WordList[i].StartIndex, grid.WordList[i].EndIndex);

            return s;
        }

        /// <summary>
        /// Generate the hints
        /// </summary>
        /// <returns>string[]</returns>
        string[] GetHints()
        {
            string[] h = new string[10];

            for (int i = 0; i < 10; i++)
                h[i] = string.Format("{0}|{1}|{2}", grid.WordList[i].Direction, grid.WordList[i].Word, grid.WordList[i].Meaning);

            return h;
        }
    }
}
