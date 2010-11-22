using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
//****************************************
//GBMFix
//(c) 2010   trodoss 
//See end of file for terms of use.  
//***************************************
//Version 1.0 - Initial Release

//************** N A M E S P A C E ****************************************
namespace PINTCompiler.Assembly {
	//*********************************************************************
	// PINTBitmapWriter Class
	//*********************************************************************	
	public class PINTBitmapWriter {			

		//*********** P U B L I C   F U N C T I O N S  ( M E T H O D S ) ******
		/// <sumary>
		/// Main (entrypoint)	
		/// </sumary>
		public static void Write(string source, BinaryWriter binaryWriter, int xWidth, int yWidth) {
			Bitmap bitmap = new Bitmap(source);
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
			for (int y=0; y<= (yWidth-1); y++) {
				for (int x=0; x<= (xWidth-1); x++) {
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
