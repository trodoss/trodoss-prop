using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using PropellerUtilities.Font;
//****************************************
//trodoss - 2010 
//See end of file for terms of use.  
//***************************************

//************** N A M E S P A C E ****************************************
namespace PropellerPowered.Graphics {
	//*********************************************************************
	// GraphicsConverter Class
	//*********************************************************************		
	public enum InputType {GBR}
	/// <summary>
	/// Establish the standard output types ( GBR)
	/// </summary>
	
	public enum OutputType {OREText, Bitmap}
	/// <summary>
	/// Establish the standard output types ( OREText,  Bitmap )
	/// </summary>
	
	public class GraphicsConverter {
		/// <sumary>
		/// Generate the physical font file based on settings
		/// </sumary>
		public static void Convert(InputType inputType, OutputType outputType, string inputFileName, string outputFileName){
			Bitmap bitmap;
			
			if ((inputFileName == "") || (inputFileName == null)) {
				throw new GraphicsException("Input file name not specified");
			}
			
			if ((outputFileName == "") || (outputFileName == null)) {
				throw new FontException("Output file name not specified");
			}		
			
			try {		
				bitmap = new Bitmap(1,1);
				
				switch (inputType) {
					case InputType.GBR:
						bitmap = GBRReader.Read(inputFileName);
						break;
				}
				
			} catch (Exception e) {
				Console.WriteLine(e.ToString());
				throw new FontException("Unable to load file: " + e.ToString());
			}
			
			switch (outputType) {
				case OutputType.OREText:
					ORETextWriter.Write(bitmap, outputFileName);
					break;	
					
				case OutputType.Bitmap:
					bitmap.Save(outputFileName, System.Drawing.Imaging.ImageFormat.Bmp);
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
