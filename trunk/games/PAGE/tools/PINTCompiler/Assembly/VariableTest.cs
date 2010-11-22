using System;
using System.Collections;
//****************************************
// VariableTest
// 2010 trodoss
//See end of file for terms of use.  
//***************************************
namespace PINTCompiler.Assembly {
	public enum ComparisonType : int {GT = 0, EQ = 1, LT = 2, GE = 3, LE = 4, NE = 5, VGT = 6, VEQ = 7, VLT = 8, VGE = 9, VLE = 10, VNE = 11, Undefined = -1}

	public class VariableTest : PINTCommand {
		public int VariableID;
		public ComparisonType Comparison;
		public int TestValue;
		public int NotMetJumpTo;
		public string LabelReference;

		public VariableTest() {
			this.Opcode = 4;
			this.Comparison = ComparisonType.EQ;
		}
		
		public VariableTest(int variableID, int testValue, ComparisonType comparison, int notMetJumpTo) {
			this.Opcode = 4;
			this.VariableID = variableID;
			this.Comparison = comparison;
			this.TestValue = testValue;
			this.NotMetJumpTo = notMetJumpTo;
		}

		public VariableTest(int variableID, int testValue, ComparisonType comparison, string labelReference) {
			this.Opcode = 4;
			this.VariableID = variableID;
			this.Comparison = comparison;
			this.TestValue = testValue;
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
