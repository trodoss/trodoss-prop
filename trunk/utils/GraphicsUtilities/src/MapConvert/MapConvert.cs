using System;
using PropellerPowered.Maps;

//****************************************
//MapConvert 1.1
//(c) 2010   trodoss 
//
//See end of file for terms of use.  
//***************************************
//Version 1.0 - Initial Release
//Verision 1.1 - Added options and wildcard support

//************** N A M E S P A C E ****************************************
namespace MapConvert {
	//*********************************************************************
	// MapConvert Class
	//*********************************************************************	
	public class MapConvert {			
		/// <sumary>
		/// application version number
		/// </sumary>
		private static string GetVersion() {
			return "1.1";
		}
		
		//*********** P U B L I C   F U N C T I O N S  ( M E T H O D S ) ******
		/// <sumary>
		/// Main (entrypoint)	
		/// </sumary>
		public static void Main(string[] arguments) {
			string inputFileName = "";
			string inputFormat = "G";
			string outputFormat = "XY";
			string options = "";
			bool IncludeDimensions = true;
			Map.ReductionType Reduction = Map.ReductionType.None;

			if (arguments.Length > 0) {
				inputFileName = arguments[0];
				
				InputType inputType = InputType.GBM;
				OutputType outputType = OutputType.MapScrollXY;
				
				if (arguments.Length > 1) {
					inputFormat = arguments[1].ToUpper();
					
					switch (inputFormat) {							
						case "G":
							inputType = InputType.GBM;
							break;
					}
					
					if (arguments.Length > 2) {
						string outputFileName = arguments[2];
						
						if (arguments.Length > 3) {
							outputFormat = arguments[3].ToUpper();
						}
						
						switch (outputFormat) {
							case "XY":
								outputType = OutputType.MapScrollXY;
								break;													
						}

						if (arguments.Length > 4) {
							options = arguments[4].ToUpper();
							
							foreach (char option in options) {
								switch (option) {
									//omit dimensions
									case 'O':
										IncludeDimensions = false;
										break;
									
									//reduction type RLE
									case 'R':
										Reduction = Map.ReductionType.RLE;
										break;
								}
								
							}
						}
						try {
							PropellerPowered.Maps.MapConverter.Convert(inputType, outputType, inputFileName, outputFileName, IncludeDimensions, Reduction);
						} catch (Exception e) {
							Console.WriteLine("Error in MapConvert v"+GetVersion() +" - " + e.ToString());
						}
					} else {
						Console.WriteLine ("MapConvert v"+GetVersion()+"  - Output filename not specified.");
					}
				} else {
					Console.WriteLine ("MapConvert v"+GetVersion()+"  - Input format not specified.");
				}
			} else {
				Console.WriteLine ("MapConvert v"+GetVersion()+"  - Usage: MapConvert.exe [input_filename] [input_format] [output_filename] ([output_format] [options])");
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
