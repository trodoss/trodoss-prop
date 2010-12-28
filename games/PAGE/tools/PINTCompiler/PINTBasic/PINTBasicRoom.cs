using System;
using System.Collections;
//****************************************
// PINTBasicRoom
// 2010 trodoss
//See end of file for terms of use.  
//***************************************
namespace PINTCompiler.PINTBasic {
	public class PINTBasicRoom : PINTBasicObject {
		public PINTBasicBackdrop Backdrop;
		public PINTBasicHotspotList Hotspots;
		public PINTBasicByteList Variables;
		public PINTBasicEventList Events;
		
		private int maxVariableID;
		private int maxHotspotID;
		
		public PINTBasicRoom(int id, string name) {
			this.ID = id;
			this.Name = name;
			maxVariableID = 0;
			maxHotspotID = 0;
			this.Variables = new PINTBasicByteList();
			this.Hotspots = new PINTBasicHotspotList();
			this.Events = new PINTBasicEventList();
		}
		
		public bool AddVariable(string variableName) {
			this.Variables.Add(new PINTBasicByte(maxVariableID, variableName));
			maxVariableID++;
			//if we have exceeded the number of global variables, then let the compiler know
			if (maxVariableID > 8) {
				return false;
			} else {
				return true;
			}
		}	

		public bool AddHotspot(string hotspotName, int x, int y, int w, int h) {
			this.Hotspots.Add(new PINTBasicHotspot(maxHotspotID, hotspotName, x, y, w, h));
			maxHotspotID++;
			//if we have exceeded the number of global variables, then let the compiler know
			if (maxHotspotID > 4) {
				return false;
			} else {
				return true;
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
