using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using PropellerPowered.General;
//****************************************
//trodoss - 2010
//See end of file for terms of use.  
//***************************************
//Version 1.1 
//  - changed to reference PropellerPowered.General assembly

// Descrption:
// Read the Gameboy Tile Designer (GBR) format file and output a bitmap
//
// Gameboy Tile Designer
// Version 2.2
// Copyright H. Mulder 1999

//************** N A M E S P A C E ****************************************
namespace PropellerPowered.Graphics {
	//*********************************************************************
	// GBRReader Class
	//*********************************************************************	
	public class GBRReader {					
	
		//*********** P U B L I C   F U N C T I O N S  ( M E T H O D S ) ******
		public static Bitmap Read (string inputFileName) {						
			try {
				Bitmap workBitmap = new Bitmap(256, 32, PixelFormat.Format32bppArgb);
				
				//read the contents of the input file into a byte array
				FileStream fs = File.OpenRead(inputFileName);
				byte[] inputBytes = ReaderUtilities.ReadFully(fs, fs.Length);
				fs = null;
						
				//the assumption is that each are 8x8 character blocks
				int totalCharacterRows = workBitmap.Height / 8;
				int totalFontChars = totalCharacterRows * 32;
				
				//assume we are skipping over the header (180 bytes)
				int pointer = 180;
						
				//pre-set colors for processing
				Color pixelBlack =Color.FromArgb(0,0,0); 			
				Color pixelWhite =Color.FromArgb(255,255,255); 
				Color pixelBlue =Color.FromArgb(0,0,255); 
				Color pixelRed =Color.FromArgb(255,0,0);
				
				int currentCharacter = 0;
				for (int characterRow=0; characterRow < totalCharacterRows; characterRow++) {
					for (int characterLineWidth=0; characterLineWidth <32; characterLineWidth++) {

						for (int characterHeight = (characterRow * (int)8); characterHeight < (characterRow * (int)8) + (int)8; characterHeight++) {
							for (int characterWidth = (characterLineWidth * (int)8); characterWidth < (characterLineWidth * (int)8)+(int)8; characterWidth++) {
								switch (inputBytes[pointer]) {		
									case 1:
										workBitmap.SetPixel(characterWidth, characterHeight, pixelWhite);
										break;

									case 2:
										workBitmap.SetPixel(characterWidth, characterHeight, pixelRed);
										break;

									case 3:
										workBitmap.SetPixel(characterWidth, characterHeight, pixelBlue);
										break;	

									default:
										workBitmap.SetPixel(characterWidth, characterHeight, pixelBlack);
										break;
								}
								pointer++;												
							}
							currentCharacter++;
						}
					}
				}
				return workBitmap;
				
			} catch (Exception e){
				throw new GraphicsException("Error in reading input file " + inputFileName + " - " + e.ToString());
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
