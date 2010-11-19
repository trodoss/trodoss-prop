using System;
using PropellerPowered.Sound;
//****************************************
//SoundConvert 1.0
//(c) 2010   trodoss 
//
//See end of file for terms of use.  
//***************************************
//Version 1.0 - Initial Release

//************** N A M E S P A C E ****************************************
namespace SoundConvert {
	//*********************************************************************
	// SoundConvert Class
	//*********************************************************************	
	public class SoundConvert {			
		/// <sumary>
		/// application version number
		/// </sumary>
		private static string GetVersion() {
			return "1.0";
		}
		
		//*********** P U B L I C   F U N C T I O N S  ( M E T H O D S ) ******
		/// <sumary>
		/// Main (entrypoint)	
		/// </sumary>
		public static void Main(string[] arguments) {
			string inputFileName = "";
			string inputFormat = "R";
			string outputFormat = "S";

			if (arguments.Length > 0) {
				inputFileName = arguments[0];
				
				InputType inputType = InputType.RTTTL;
				OutputType outputType = OutputType.SimpleSound;
				
				if (arguments.Length > 1) {
					inputFormat = arguments[1].ToUpper();
					
					switch (inputFormat) {							
						case "R":
							inputType = InputType.RTTTL;
							break;
					}
					
					if (arguments.Length > 2) {
						string outputFileName = arguments[2];
						
						if (arguments.Length > 3) {
							outputFormat = arguments[3].ToUpper();
						}
						
						switch (outputFormat) {
							case "S":
								outputType = OutputType.SimpleSound;
								break;							
								
						}

						try {
							PropellerPowered.Sound.SoundConverter.Convert(inputType, outputType, inputFileName, outputFileName);
						} catch (Exception e) {
							Console.WriteLine("Error in SoundConvert v"+GetVersion() +" - " + e.ToString());
						}
					} else {
						Console.WriteLine ("SoundConvert v"+GetVersion()+"  - Output filename not specified.");
					}
				} else {
					Console.WriteLine ("SoundConvert v"+GetVersion()+"  - Input format not specified.");
				}
			} else {
				Console.WriteLine ("SoundConvert v"+GetVersion()+"  - Usage: SoundConvert.exe [input_filename] [input_format] [output_filename] ([output_format]) ");
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
