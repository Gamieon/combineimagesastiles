/**  
Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

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
    /// This program generates a single image made up of a collection of smaller "child" images arranged as tiles.
    /// Each tile within the single image is as large as the largest child image.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Generates a single image made up of a number of collection images arranged as tiles
        /// </summary>
        /// <param name="searchPattern">The search string to match against the names of files in the current path</param>
        /// <param name="outFile">The filename of the generated single image</param>
        static void CombineImagesAsTiles(string searchPattern, string outFile)
        {
            // Gather a list of all the image files to read in
            string searchPath = Path.GetDirectoryName(searchPattern);
            searchPattern = Path.GetFileName(searchPattern);
            string[] tileFilenames = Directory.GetFiles(searchPath, searchPattern);

            // We currently operate under the assumption that every gathered file is a valid
            // image that can be interpreted by a Bitmap object. In an effort to make the resulting
            // image not be width or height heavy, we calculate that the number of tiles per side to
            // be the square root of the gathered file count
            int tilesPerSide = (int)Math.Ceiling(Math.Sqrt((float)tileFilenames.Length));
            
            // Load the content of each file into memory
            List<Bitmap> tiles = new List<Bitmap>();
            foreach (string s in tileFilenames)
            {
                tiles.Add(new Bitmap(s));
            }

            // Calculate the largest width and height of all the images we collected 
            int tileWidth = 0;
            int tileHeight = 0;
            foreach (Bitmap t in tiles)
            {
                tileWidth = Math.Max(tileWidth, t.Width);
                tileHeight = Math.Max(tileHeight, t.Height);
            }

            // Now that we have all the time bitmaps and the dimensions of the output image, we can
            // generate the output image
            using (Bitmap target = new Bitmap(tileWidth * tilesPerSide, tileHeight * tilesPerSide))
            {
                for (int i = 0; i < tiles.Count; i++)
                {
                    int x = (i % tilesPerSide) * tileWidth;
                    int y = (i / tilesPerSide) * tileHeight;

                    using (Graphics g = Graphics.FromImage(target))
                    {
                        g.DrawImage(tiles[i], new Point(x, y));
                    }
                }
                target.Save(outFile);
            }
        }

        /// <summary>
        /// Entry point
        /// </summary>
        /// <param name="args">Command-line arguments</param>
        static void Main(string[] args)
        {
            try
            {
                // Ensure we have two arguments:
                //
                // 1. File mask (e.g. *.png)
                // 2. Output file (e.g. output.png)
                //
                if (args.Length != 2)
                {
                    Console.WriteLine(@"
Usage: combineimagesastiles filemask outputfile

Examples:
    combineimagesastiles *.png output.png
    combineimagesastiles ""d:\tiles\grass*.png"" ""d:\tiles\cmb\grass.png""
");
                }
                else
                {
                    // Do the conversion
                    CombineImagesAsTiles(args[0], args[1]);

                    // All done
                    Console.WriteLine("Done!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
