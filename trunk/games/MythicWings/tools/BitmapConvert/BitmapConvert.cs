using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
//****************************************
//BitmapConvert
//***************************************
//Version 1.0 - Initial Release

//************** N A M E S P A C E ****************************************
namespace BitmapConvert {
	//*********************************************************************
	// BitmapConvert Class
	//*********************************************************************	
		public class BitmapConvert {		
		/// <sumary>
		/// Retrieves the version text 
		/// </sumary>
		private static string VersionText() { return "v1.0"; }
		
		private static bool isNumber (string inString) {
			bool isValid = true;
			try {
				int test = Convert.ToInt32(inString);
			} catch {
				isValid = false;
			}
			return isValid;
		}
		
		private static void ShowHelp() {
				Console.WriteLine("");
				Console.WriteLine("Usage: BitmapConvert.exe [source_path] [output_path] ([element_witdh] [element_height])");
				Console.WriteLine("[source_path] - Source bitmap file path");
				Console.WriteLine("[output_path] - Output file file path");
				Console.WriteLine("[element_witdh]  - (Optional) Element width (8 by defalut)");
				Console.WriteLine("[element_height] - (Optional) Element height (8 by default)");
				Console.WriteLine("");
				Console.WriteLine("Additional options:");
				Console.WriteLine("");
				Console.WriteLine("/? - Show command line help information");
		}
	
		private static void ProcessFile(string bitmapFileName, string outputFileName, int xWidth, int yHeight) {
			Bitmap bitmap = new Bitmap(bitmapFileName);
			
			//determine the number of rows and columns to process in the image file
			int totalRows = bitmap.Height / yHeight;
			int totalCols = bitmap.Width / xWidth;

			using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(outputFileName, FileMode.Create))) {	
				for (int currentRow = 0; currentRow < totalRows; currentRow++) {
					for (int currentCol = 0; currentCol < totalCols; currentCol++) {
						int currentY = currentRow * yHeight;
						int currentX = currentCol * xWidth;
						Write(bitmap, binaryWriter, xWidth, yHeight, currentX, currentY);
					}
				}
			}
			bitmap = null;
		}
		
		//*********** P U B L I C   F U N C T I O N S  ( M E T H O D S ) ******	
		public static void Main(string[] arguments) {
			bool showHelp = false;
			string bitmapFileName = "";
			string outputFileName = "";
			string elementWidthString = "";
			string elementHeightString = "";
			int elementWidth = 8;
			int elementHeight = 8;
			bool canContinue = true;
			
			//examine each of the arguments and see what is being requested
			if (arguments.Length > 0) {
				foreach (string thisArgument in arguments) {
					if (thisArgument.Length > 0) {
						if (thisArgument.Substring(0,1) == "/") {
								if (thisArgument.Length > 1) {
									switch (thisArgument.Substring(1,1).ToLower()) {
										case "?":
											showHelp = true;
											break;
											
										default:
											if (!isNumber(thisArgument)) canContinue = false;
											break;
									}
								} else {
									canContinue = false;
								}
						} else {
								if (bitmapFileName == "") {
									bitmapFileName = thisArgument;
								} else {
									if (outputFileName == "") {
										outputFileName = thisArgument;
									} else {
										if (elementWidthString == "") {
											elementWidthString = thisArgument;
										} else {
											if (elementHeightString == "") {
												elementHeightString = thisArgument;
											} else {
												//more than that, and it is an error
												canContinue = false;
											}
										}
									}
								}
						}
					}
					
					if (!canContinue) {
						Console.WriteLine("BitmapConvert " + VersionText() + " - invalid argument '" + thisArgument + "' ");
						ShowHelp();
						break;
					}	
				}					
			}
			
			//if we have passed the filename/parameter checks, try to compile the files with the specified settings
			if (showHelp) {
				ShowHelp();
			} else {
				if (canContinue) {
					if (bitmapFileName == "") {
						canContinue = false;
						Console.WriteLine("BitmapConvert " + VersionText() + " - bitmap filename not specified");
						ShowHelp();
					}
					
					if (canContinue) {
						if (outputFileName  == "") {
							canContinue = false;
							Console.WriteLine("BitmapConvert " + VersionText() + " - output filename not specified");
							ShowHelp();
						}
					}
					
					if (canContinue) {
						if (elementWidthString  != "") {
							try {
								elementWidth = Convert.ToInt32(elementWidthString);
							} catch {
								canContinue = false;
								Console.WriteLine("BitmapConvert " + VersionText() + " - width value specified is not a number");
								ShowHelp();							
							}
						}
					}

					if (canContinue) {
						if (elementHeightString  != "") {
							try {
								elementHeight = Convert.ToInt32(elementHeightString);
							} catch {
								canContinue = false;
								Console.WriteLine("BitmapConvert " + VersionText() + " - height value specified is not a number");
								ShowHelp();							
							}
						}
					}
					
					if (canContinue) {
						ProcessFile(bitmapFileName, outputFileName, elementWidth, elementHeight);
					}
				}
			}			
		}
			
		
		public static void Write(Bitmap bitmap, BinaryWriter binaryWriter, int xWidth, int yHeight, int startX, int startY) {
		
			byte[] clut = new byte[16];
			clut[0] = 2;
			clut[1] = 3;
			//clut[2] = 61;
			clut[2] = 92;
			clut[3] = 140;
			clut[4] = 171;
			clut[5] = 204;
			clut[6] = 251;
			//clut[7] = 27;
			clut[7] = 60;
			clut[8] = 7;
			clut[9] = 6;
			//clut[10] = 62;
			clut[10] = 94;
			clut[11] = 142;
			clut[12] = 173;
			clut[13] = 206;
			clut[14] = 253;
			//clut[15] = 28;		
			clut[15] = 62;		
			int currentValue = 0;
			Color pixelColor = bitmap.GetPixel(0, 0);
			for (int y=startY; y<= (startY + yHeight)-1; y++) {
				for (int x=startX; x<= (startX + xWidth)-1; x++) {
					currentValue = 0;
					pixelColor = bitmap.GetPixel(x, y);
					
					if (pixelColor.R == 0) {
						if (pixelColor.G == 0) {
							if (pixelColor.B == 0) currentValue = 0;
							if (pixelColor.B == 128) currentValue = 6;
							if (pixelColor.B == 255) currentValue = 14;
						}
						
						if (pixelColor.G == 128) {
							if (pixelColor.B == 0) currentValue = 4;
							if (pixelColor.B == 128) currentValue = 5;
						}
						
						if (pixelColor.G == 255) {
							if (pixelColor.B == 0) currentValue = 12;
							if (pixelColor.B == 255) currentValue = 13;						
						}
					}
					
					if (pixelColor.R == 128) {
						if (pixelColor.G == 0) {
							if (pixelColor.B == 0) currentValue = 2;
							if (pixelColor.B == 128) currentValue = 7;
						}
						if (pixelColor.G == 128) {
							if (pixelColor.B == 0) currentValue = 3;
							if (pixelColor.B == 128) currentValue = 1;
						}						
					}
					
					if (pixelColor.R == 192) {
						if (pixelColor.B == 192) {
							if (pixelColor.G == 192) currentValue = 9;
						}
					}
			
					if (pixelColor.R == 255) {
						if (pixelColor.G == 0) {
							if (pixelColor.B == 0) currentValue = 10;
							if (pixelColor.B == 255) currentValue = 15;
						}
						if (pixelColor.G == 255) {
							if (pixelColor.B == 0) currentValue = 11;
							if (pixelColor.B == 255) currentValue = 8;
						}

					}
					
					binaryWriter.Write(clut[currentValue]);				
					
				}
			}
		}		
	}
}
/*
+------------------------------------------------------------------------------------------------------------------------------+
                                                   TERMS OF USE: MIT License                                                                                                              
+------------------------------------------------------------------------------------------------------------------------------
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation     
files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy,    
modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software
is furnished to do so, subject to the following conditions:                                                                   
                                                                                                                              
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
                                                                                                                              
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE          
WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR         
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,   
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                         
+------------------------------------------------------------------------------------------------------------------------------+
*/
