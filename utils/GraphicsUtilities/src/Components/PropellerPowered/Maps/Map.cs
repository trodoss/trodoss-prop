using System;
using System.Collections;
using System.IO;
//****************************************
//trodoss - 2010
//See end of file for terms of use.  
//***************************************
//Version 1.1 - Added support for patterns/indexes (for reduction processing)

// Descrption:
// Object that represents Map data in memory

//************** N A M E S P A C E ****************************************
namespace PropellerPowered.Maps {
	//*********************************************************************
	// Map Class
	//*********************************************************************	
	public class Map {					
		public int Rows;
		public int Columns;
		public MapCellList Cells;
		public MapPatternList Patterns;
		public ReductionType Reduction;

		public bool IncludeDimensions;
		
		public enum ReductionType {None, RLE, Pattern}
		
		//*********** P U B L I C   F U N C T I O N S  ( M E T H O D S ) ******
		//constructor
		public Map() {
			this.Cells = new MapCellList();
			this.Reduction = ReductionType.None;
		}
		
		public Map(int rows, int columns) {
			this.Rows = rows;
			this.Columns = columns;
			this.Cells = new MapCellList();
			this.Patterns = new MapPatternList();
			this.IncludeDimensions = true;
			this.Reduction = ReductionType.None;
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
