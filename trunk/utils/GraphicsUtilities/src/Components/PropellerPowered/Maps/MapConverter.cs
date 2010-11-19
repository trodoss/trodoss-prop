using System;
using System.Collections;
using PropellerPowered.Maps;
//****************************************
//trodoss - 2010 
//See end of file for terms of use.  
//***************************************

//************** N A M E S P A C E ****************************************
namespace PropellerPowered.Maps {
	//*********************************************************************
	// MapConverter Class
	//*********************************************************************		
	public enum InputType {GBM}
	/// <summary>
	/// Establish the standard output types ( GBM)
	/// </summary>
	
	public enum OutputType {MapScrollXY, MapScrollX, MapScrollY}
	/// <summary>
	/// Establish the standard output types ( MapScrollXY, MapScrollX, MapScrollY)
	/// </summary>
	
	public class MapConverter {
		/// <sumary>
		/// Generate the physical font file based on settings
		/// </sumary>
		public static void Convert(InputType inputType, OutputType outputType, string inputFileName, string outputFileName, bool IncludeDimensions, Map.ReductionType Reduction){
			PropellerPowered.Maps.Map map;
			
			if ((inputFileName == "") || (inputFileName == null)) {
				throw new MapException("Input file name not specified");
			}
			
			if ((outputFileName == "") || (outputFileName == null)) {
				throw new MapException("Output file name not specified");
			}		
			
			try {		
				map = new PropellerPowered.Maps.Map();
				
				switch (inputType) {
					case InputType.GBM:
						map = GBMReader.Read(inputFileName);
						break;
				}
				
				map.IncludeDimensions = IncludeDimensions;
				map.Reduction = Reduction;
				
				switch (map.Reduction) {
					case Map.ReductionType.RLE:
						map = RLEReducer.Reduce(map);
						break;
						
					case Map.ReductionType.None:
					default:
						break;
				}
				
			} catch (Exception e) {
				throw new MapException("Unable to load file: " + e.ToString());
			}
			
			switch (outputType) {
				case OutputType.MapScrollXY:
					MapScrollXYWriter.Write(map, outputFileName);
					break;	
				

				default:
				throw new MapException("Output type not recognized.");
			}
			if (map != null) map = null;
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
