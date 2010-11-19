using System;
using PropellerUtilities.Font;

//****************************************
//Font Generator 1.3
//
//2008-2010   trodoss (Alex Schafer), mattb (Mathew Boorman)
//See end of file for terms of use.  
//***************************************
//Version 1.2 - Included contributions by mattb for TV_Text format
//Version 1.1 - Changed to allow multiple output types
//Version 1.0 - Initial Release

//************** N A M E S P A C E ****************************************
namespace FontGenerator {
	//*********************************************************************
	// FontGenerator Class
	//*********************************************************************	
	public class FontGenarator {			
		/// <sumary>
		/// application version number
		/// </sumary>
		private static string GetVersion() {
			return "1.3";
		}
		
		//*********** P U B L I C   F U N C T I O N S  ( M E T H O D S ) ******
		/// <sumary>
		/// Main (entrypoint)	
		/// </sumary>
		public static void Main(string[] arguments) {
			if (arguments.Length > 0) {
			
				string fileName = arguments[0];
				string format = "A";
				OutputType type = OutputType.AIGeneric;
				
				if (arguments.Length > 1) {
					string outputFileName = arguments[1];
					
					if (arguments.Length > 2) {
						format = arguments[2].ToUpper();
					}
					
					switch (format) {
						case "A":
							type = OutputType.AIGeneric;
							break;
							
						case "P":
							type = OutputType.PotatoText;
							break;
						
						case "B":
							type = OutputType.Binary;
							break;
							
						case "O":
							type = OutputType.OREText;
							break;							
							
						case "T":
							type = OutputType.TVText;
							break;
					}

					try {
						PropellerUtilities.Font.FontGenerator.Generate(type, fileName, outputFileName);
					} catch (Exception e) {
						Console.WriteLine("Error in FontGnerator v"+GetVersion() +" - " + e.ToString());
					}
				} else {
					Console.WriteLine ("FontGenerator v"+GetVersion()+"  - Output filename not specified.");
				}
			} else {
				Console.WriteLine ("FontGenerator v"+GetVersion()+"  - Usage: FontGenerator.exe [bitmap_filename] [output_filename] ([format])");
				Console.WriteLine("Format can be specified as 'A' (AIGeneric) 'P' (Potato_Text) 'B' (Binary) 'O' (ORE_Text) or 'T' (TV_Text) - 'A' is used when no value specified");
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
