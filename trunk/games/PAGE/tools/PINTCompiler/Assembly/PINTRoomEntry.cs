using System;
using System.Collections;
using PINTCompiler.Utilities;
//****************************************
// PINTRoomEntry
// 2010 trodoss
//See end of file for terms of use.  
//***************************************
namespace PINTCompiler.Assembly {
	public class PINTRoomEntry {
		public PINTBackdropEntry Backdrop;

		public PINTHotspotEntryList Hotspots;
		public PINTCommandList Commands;
		public PINTStringList Strings;
		public Hashtable Constants;
		public PINTPicEntryList Pics;
		public PINTItemEntryList Items;
		public int RoomID;
		public CompilationLog Log;
		
		public int Offset;
		
		public int[] EventTableEntries;

		public PINTRoomEntry( int roomID, CompilationLog log) {
			this.Constants = new Hashtable();
			this.Hotspots = new PINTHotspotEntryList();
			this.Commands = new PINTCommandList();
			this.Strings = new PINTStringList();
			this.Backdrop = new PINTBackdropEntry();
			this.Pics = new PINTPicEntryList();
			this.Items = new PINTItemEntryList();
			this.RoomID = roomID;
			this.Log = log;
			
			this.EventTableEntries = new int[10];
			
			//assume the static header size
			this.Offset = 36;
		}
		
		public int FindCommandLocationByLabel(string label) {
			int returnLoc = -1;
			for (int i=0; i< Commands.Count; i++) {
				PINTCommand thisCommand = (PINTCommand) Commands[i];
				if (thisCommand.Label == label) {
					returnLoc = i;
					break;
				}
			}	
			return returnLoc;
		}
		
		public int FindTextOffsetByID(int textID) {
			int returnOffset = -1;
			foreach (PINTString thisString in this.Strings) {
				if (thisString.StringID == textID) {
					returnOffset = thisString.Offset;
					break;
				}
			}
			return returnOffset;
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
