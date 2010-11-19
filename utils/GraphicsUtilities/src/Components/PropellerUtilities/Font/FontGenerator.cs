using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
//****************************************
//trodoss 2009-2010 
//See end of file for terms of use.  
//***************************************

//************** N A M E S P A C E ****************************************
namespace PropellerUtilities.Font {
	//*********************************************************************
	// FontGenerator Class
	//*********************************************************************		
	public enum OutputType {AIGeneric, PotatoText, OREText, Binary, TVText}
	/// <summary>
	/// Establish the standard output types (AIGeneric, PotatoText,  OREText,  Binary, TVText)
	/// - AIGeneric format has an 'as viewed' order by default, and includes a special 'header' for redefines
	/// - PotatoText format is a simplified version of the AIGeneric format (leaves out 'as viewed')
	/// -  OREText format is a 2bpp format that inverts the pixel order
	/// - Binary format stores the bits in reverse order, 1 byte per font line
	/// </summary>
	
	public class FontGenerator {
		/// <sumary>
		/// Generate the physical font file based on settings
		/// </sumary>
		public static void Generate(OutputType outputType, string bitmapFileName, string outputFileName){
			Bitmap bitmap;
			
			if ((bitmapFileName == "") || (bitmapFileName == null)) {
				throw new FontException("Bitmap file name not specified");
			}
			
			if ((outputFileName == "") || (outputFileName == null)) {
				throw new FontException("Output file name not specified");
			}		
			
			try {		
				bitmap = new Bitmap( bitmapFileName );
				
				//Validation for formats other than the TV_Text format
				if (outputType != OutputType.TVText) {
					if ((bitmap.Width != 256)) {
						bitmap = null;
						throw new FontException("Source bitmap must be 256 pixels wide.");
					}
					
					if ((bitmap.Height < 8)|| (bitmap.Height > 32)) {
						bitmap = null;
						throw new FontException("Source bitmap must be at least 8 pixels high, and less than 32 pixels high.");
					}
				}
				
			} catch (Exception e) {
				Console.WriteLine(e.ToString());
				throw new FontException("Unable to load source bitmap: " + e.ToString());
			}
			
			switch (outputType) {
				case OutputType.AIGeneric:
					AIGenericWriter.Write(bitmap, outputFileName);
					break;

				case OutputType.PotatoText:
					PotatoTextWriter.Write(bitmap, outputFileName);
					break;					

				case OutputType.OREText:
					ORETextWriter.Write(bitmap, outputFileName);
					break;	
					
				case OutputType.Binary:
					BinaryFormatWriter.Write(bitmap, outputFileName);
					break;
					
				case OutputType.TVText:
					TVTextWriter.Write(bitmap, outputFileName);
					break;

				default:
				throw new FontException("Output type not recognized.");
			}
			if (bitmap != null) bitmap = null;
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
