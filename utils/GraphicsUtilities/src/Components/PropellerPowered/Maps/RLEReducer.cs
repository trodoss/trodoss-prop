using System;
using System.Collections.Generic;
//****************************************
//trodoss - 2010
//See end of file for terms of use.  
//***************************************
//Version 1.0 - Initial Release

//************** N A M E S P A C E ****************************************
namespace PropellerPowered.Maps {
	public class RLEReducer {
		public static Map Reduce (PropellerPowered.Maps.Map map) {	
			Map reducedMap = map;
			
			try {	
				for (int i=0; i< map.Cells.Count; i++) {
					int runLength = 1;
					while ((i + 1 < map.Cells.Count) && (map.Cells[i].TileID == map.Cells[i+1].TileID)) {
						runLength++;
						i++;
					}
					//generate the map pattern for the 'run'
					MapPattern thisPattern = new MapPattern();
					thisPattern.Cells.Add(new MapCell(map.Cells[i].TileID));
					thisPattern.Count = runLength;
					reducedMap.Patterns.Add(thisPattern);
					thisPattern = null;
				}
										
			} catch (Exception e){
				throw new MapException("Error in reducing map data using RLE (Run-Length Encoding) method - " + e.ToString());
			}

			return reducedMap;
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
