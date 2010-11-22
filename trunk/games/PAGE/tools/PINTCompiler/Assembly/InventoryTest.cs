using System;
using System.Collections;
//****************************************
// InventoryTest
// 2010 trodoss
//See end of file for terms of use.  
//***************************************
namespace PINTCompiler.Assembly {
	public class InventoryTest : PINTCommand {
		public int ItemID;
		public int NotMetJumpTo;
		public string LabelReference;

		public InventoryTest() {
			this.Opcode = 82;
		}
		
		public InventoryTest(int itemID, int notMetJumpTo) {
			this.Opcode = 82;
			this.ItemID = itemID;
			this.NotMetJumpTo = notMetJumpTo;
		}	
		
		public InventoryTest(int itemID, string labelReference) {
			this.Opcode = 82;
			this.ItemID = itemID;
			this.LabelReference = labelReference;
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
