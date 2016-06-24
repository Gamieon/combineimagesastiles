using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace combineimagesastiles
{
    /// <summary>
    /// Contains information translated from command-line parameters
    /// for the image combining functionality to use
    /// </summary>
    class CombineParameters
    {
        /// <summary>
        /// The collection of tiles read from files that satisfy the
        /// search pattern. If any one file fails to load, the program
        /// will throw an exception and the single image will not be
        /// generated
        /// </summary>
        public readonly Bitmap[] tiles;
        
        /// <summary>
        /// The file path name to the single image
        /// </summary>
        public readonly string outputPathName;

        /// <summary>
        /// True if for every tile we should render four tiles onto the
        /// single image; each one rotated ninety degrees from the previous
        /// </summary>
        public readonly bool buildCardinals;

        /// <summary>
        /// The width of a tile in the single image
        /// </summary>
        public readonly int tileWidth;

        /// <summary>
        /// The height of a tile in the single image
        /// </summary>
        public readonly int tileHeight;

        /// <summary>
        /// The number of tiles per row in the single image
        /// </summary>
        public readonly int tilesPerRow;

        /// <summary>
        /// The number of tiles per column in the single image
        /// </summary>
        public readonly int tilesPerColumn;

        /// <summary>
        /// Gather all of the filenames that satisfy a search pattern
        /// </summary>
        /// <param name="searchPattern">The file path and search pattern</param>
        /// <returns>The filenames which satisfy the search pattern</returns>
        private static string[] GetTileFilenames(string searchPattern)
        {
            string searchPath = Path.GetDirectoryName(searchPattern);
            searchPattern = Path.GetFileName(searchPattern);
            return Directory.GetFiles(searchPath, searchPattern);
        }

        /// <summary>
        /// Load the contents of a collection of files as bitmaps
        /// </summary>
        /// <param name="tileFilenames">The files to load from</param>
        /// <returns>The loaded bitmap contents</returns>
        private static Bitmap[] LoadTiles(string[] tileFilenames)
        {
            List<Bitmap> tileList = new List<Bitmap>();
            foreach (string s in tileFilenames)
            {
                tileList.Add(new Bitmap(s));
            }
            return tileList.ToArray();
        }

        /// <summary>
        /// Gets the position of a tile
        /// </summary>
        /// <param name="ordinal">The zero-based ordinal of the tile in the image</param>
        /// <returns>The tile position</returns>
        public Point GetTilePosition(int ordinal)
        {
            return new Point((ordinal % tilesPerRow) * tileWidth, (ordinal / tilesPerRow) * tileHeight);
        }

        /// <summary>
        /// Construction
        /// </summary>
        /// <param name="args">Command-line arguments</param>
        public CombineParameters(string[] args)
        {
            // Fail if there aren't enough arguments
            if (args.Length < 2)
            {
                throw new ArgumentOutOfRangeException("args.Length", args.Length, "Too few input arguments");
            }

            // Look for optional arguments
            buildCardinals = args.Contains("-buildcardinals");

            // Gather a list of all the tile filenames
            string[] tileFilenames = GetTileFilenames(args[0]);
            outputPathName = args[1];

            // Load each tile into memory
            tiles = LoadTiles(tileFilenames);

            // Determine the width and height of a tile in the single image
            tileWidth = tiles.Max(t => t.Width);
            tileHeight = tiles.Max(t => t.Height);

            // Determine the max numbre of tiles per row and column based on
            // how the user wants to do the conversion
            if (buildCardinals)
            {
                // We're rotating each tile 90 degrees and rendering them all onto the
                // image; the easiest way to read the single image is one row per tile
                // (which puts us at four tiles per row: 0, 90, 180, 270)
                tilesPerRow = 4;
                tilesPerColumn = tiles.Length;
            }
            else
            {
                // Try to fit all the tiles into a square as best we can
                tilesPerRow = tilesPerColumn = (int)Math.Ceiling(Math.Sqrt((float)tiles.Length));
            }
        }
    }
}
