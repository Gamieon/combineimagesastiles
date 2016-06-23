combineimagesastiles is a Windows C# console application that combines multiple image files into a single image as tiles. Each tile within the single image is as large as the largest child image. The program also tries to match the number of rows and columns as best it can.

For a visual description, go to http://gamieon.com/combineimagesastiles/example.png


Download Link: http://gamieon.com/combineimagesastiles/combineimagesastiles.exe


Requirements:
	.NET 4.0 Framework

Usage: combineimagesastiles filemask outputfile

Example 1: combineimagesastiles *.png output.png
Example 2: combineimagesastiles ""d:\tiles\grass*.png"" ""d:\tiles\cmb\grass.png""