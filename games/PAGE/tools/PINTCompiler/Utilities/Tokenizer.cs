using System;
using System.Collections;
using System.IO;
//****************************************
// PINT Compiler Assembler Parser
// 2010 - trodoss
//See end of file for terms of use.  
//***************************************
namespace PINTCompiler.Utilities {
	public class Tokenizer {
		public static string[] TokenizeLine(string lineData) {
			ArrayList stringSet = new ArrayList();
			string currentString = "";
			for (int i=0; i<lineData.Length; i++) {
				switch (lineData[i]) {
					case ' ':
						//only write out a string if there is something to write (prevent errors from extra spaces)
						if (currentString != "") stringSet.Add(currentString);
						currentString = "";
						break;
					
					case ',':
					case '(':
					case ')':
						if (currentString != "") stringSet.Add(currentString);
						stringSet.Add(lineData[i].ToString());
						currentString = "";
						break;
					
					case '"':
						currentString += lineData[i];
						i++;
						if (i >= (lineData.Length-1)) break;
						
						while (lineData[i] != '"') {
							currentString += lineData[i];
							i++;
							if (i >= (lineData.Length-1)) break;
						}
						currentString += lineData[i];
						break;
						
					default:
						currentString += lineData[i];
						break;
				}
			}
			if (currentString != "") stringSet.Add(currentString);
			
			string[] returnArray = new string[stringSet.Count];
			for (int j=0; j< stringSet.Count; j++) {
				returnArray[j] = (string)stringSet[j];
			}
			return returnArray;
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
