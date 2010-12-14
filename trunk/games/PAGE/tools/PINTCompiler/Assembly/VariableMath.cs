using System;
using System.Collections;
//****************************************
// PINTRoomEntry
// 2010 trodoss
//See end of file for terms of use.  
//***************************************
namespace PINTCompiler.Assembly {
	public enum MathType : int {Add = 0, Sub = 1, Mul = 2, Div = 3, VAdd = 4, VSub = 5, VMul = 6, VDiv = 7, Undefined = -1}
	
	public class VariableMath : PINTCommand {
		public int VariableID;
		public MathType Type;
		public int NewValue;

		public VariableMath() {
			this.Opcode = 3;
			this.Type = MathType.Add;
		}
		
		public VariableMath(int variableID, int newValue, MathType type) {
			this.Opcode = 3;
			this.Type = type;
			this.VariableID = variableID;
			this.NewValue = newValue;
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
