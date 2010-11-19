using System;
using System.Collections;
using System.IO;

//****************************************
//trodoss - 2010
//See end of file for terms of use.  
//***************************************
//Version 1.0 - Initial Release

//************** N A M E S P A C E ****************************************
namespace PropellerPowered.Maps {
	//*********************************************************************
	// MapScrollXYWriter Class
	//*********************************************************************	
	public class MapScrollXYWriter {					
		
		//*********** P U B L I C   F U N C T I O N S  ( M E T H O D S ) ******

		public static int Write (PropellerPowered.Maps.Map map, string outputFileName) {
			return Write(map, outputFileName, FileMode.Create);
		}
		
		public static int Write (PropellerPowered.Maps.Map map, string outputFileName, FileMode mode) {	
			int fileLength = 0;
			try {
				BinaryWriter binaryWriter = new BinaryWriter(File.Open(outputFileName, mode));

				//only write the map dimensions if specified
				if (map.IncludeDimensions) {
					//write map rows to file
					binaryWriter.Write((short)map.Rows);
					
					//write map columns to file
					binaryWriter.Write((short)map.Columns);
					
					fileLength += 4;
				}
				
				switch (map.Reduction) {
					case Map.ReductionType.RLE:
						foreach (MapPattern thisPattern in map.Patterns) {
							if (thisPattern.Count > 1) {
								byte patternID = Convert.ToByte(thisPattern.Cells[0].TileID + (byte)128);
								byte patternCount = Convert.ToByte(thisPattern.Count);
								binaryWriter.Write((byte)patternID);
								binaryWriter.Write((byte)patternCount);
								fileLength += 2;
							} else {
								binaryWriter.Write((byte)thisPattern.Cells[0].TileID);
								fileLength++;
							}
						}
						break;
						
					case Map.ReductionType.None:
					default:
						//write out the entire contents of the map cells to the file
						foreach (MapCell thisCell in map.Cells) {
							binaryWriter.Write((byte)thisCell.TileID);
							fileLength++;
						}
						break;
						
				}
				
				binaryWriter.Close();
				binaryWriter = null;
						
			} catch (Exception e){
				throw new MapException("Error in creating output file " + outputFileName + " - " + e.ToString());
			}	
			return fileLength;
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
