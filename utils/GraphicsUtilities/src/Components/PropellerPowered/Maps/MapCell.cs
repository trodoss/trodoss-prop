using System;
using System.Collections;
using System.IO;
//****************************************
//trodoss - 2010
//See end of file for terms of use.  
//***************************************
//Version 1.0 - Initial Release

// Descrption:
// Object that represents Map data in memory

//************** N A M E S P A C E ****************************************
namespace PropellerPowered.Maps {
	//*********************************************************************
	// Map Class
	//*********************************************************************	
	public class MapCell {					
		public int TileID;
		public int Attributes;
		
		//*********** P U B L I C   F U N C T I O N S  ( M E T H O D S ) ******
		//constructor
		public MapCell() {
		}

		public MapCell(int tileID) {
			this.TileID = tileID;
		}		

		public MapCell(int tileID, int attributes) {
			this.TileID = tileID;
			this.Attributes = attributes;
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
