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
        /// <param name="parameters">The object containing bitmaps and other values that define how the generation will take place</param>
        static void CombineImagesAsTiles(CombineParameters parameters)
        {
            // Now that we have all the time bitmaps and the dimensions of the output image, we can
            // generate the output image
            using (Bitmap output = new Bitmap(parameters.tileWidth * parameters.tilesPerRow, parameters.tileHeight * parameters.tilesPerColumn))
            {
                using (Graphics g = Graphics.FromImage(output))
                {
                    int renderedTileCount = 0;
                    foreach (Bitmap tile in parameters.tiles)
                    {
                        int x = (renderedTileCount % parameters.tilesPerRow) * parameters.tileWidth;
                        int y = (renderedTileCount / parameters.tilesPerRow) * parameters.tileHeight;

                        // Render the base tile
                        g.DrawImage(tile, parameters.GetTilePosition(renderedTileCount++));

                        // Render a 90, 180 and 270 degree rotated version if necessary
                        if (parameters.buildCardinals)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                tile.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                g.DrawImage(tile, parameters.GetTilePosition(renderedTileCount++));
                            }
                        }
                    }
                }
                output.Save(parameters.outputPathName);
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
                if (args.Length < 2)
                {
                    Console.WriteLine(@"
Usage: combineimagesastiles filemask outputfile [options]
    Options:
        -buildcardinals     Renders four versions of each tile rotated 90 degrees apart

Examples:
    combineimagesastiles *.png output.png
    combineimagesastiles ""d:\tiles\grass*.png"" ""d:\tiles\cmb\grass.png"" -buildcardinals
");
                }
                else
                {
                    // Do the conversion
                    CombineParameters parameters = new CombineParameters(args);
                    CombineImagesAsTiles(parameters);

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
