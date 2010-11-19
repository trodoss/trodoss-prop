using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;

//****************************************
//trodoss - 2009
//See end of file for terms of use.  
//***************************************
//Version 1.0 - Initial Release

//************** N A M E S P A C E ****************************************
namespace PropellerUtilities.Font {
	//*********************************************************************
	// BinaryFormatWriter Class
	//*********************************************************************	
	public class BinaryFormatWriter {					
		
		//*********** P U B L I C   F U N C T I O N S  ( M E T H O D S ) ******

		public static void Write (Bitmap bitmap, string outputFileName) {						
			try {
				BinaryWriter binaryWriter = new BinaryWriter(File.Open(outputFileName, FileMode.Create));
						
				//the assumption is that each are 8x8 character blocks
				int totalCharacterRows = bitmap.Height / 8;
				int totalFontChars = totalCharacterRows * 32;
								
				for (int characterRow=0; characterRow < totalCharacterRows; characterRow++) {
					for (int characterLineWidth=0; characterLineWidth <32; characterLineWidth++) {

						for (int characterHeight = (characterRow * (int)8); characterHeight < (characterRow * (int)8) + (int)8; characterHeight++) {
							double currentValue = 0;
							int multiplier = 0;
							for (int characterWidth = (characterLineWidth * (int)8); characterWidth < (characterLineWidth * (int)8)+(int)8; characterWidth++) {
												
								Color pixelColor = bitmap.GetPixel(characterWidth, characterHeight);
											
								if ((pixelColor.R == 255) && (pixelColor.G == 255) && (pixelColor.B == 255)) currentValue += Math.Pow(2,multiplier);	
								multiplier++;			
							}
							binaryWriter.Write((byte)currentValue);
						}
					}
				}
				binaryWriter.Close();
				binaryWriter = null;
						
			} catch (Exception e){
				throw new FontException("Error in creating output file " + outputFileName + " - " + e.ToString());
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
