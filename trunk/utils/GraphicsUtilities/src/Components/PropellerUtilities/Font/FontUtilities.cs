using System;
using System.Collections;
//****************************************
//trodoss - 2009 
//See end of file for terms of use.  
//***************************************

//************** N A M E S P A C E ****************************************
namespace PropellerUtilities.Font {
	//*********************************************************************
	// FontGenerator Class
	//*********************************************************************	
	public class FontUtilities {
		/// <sumary>
		/// Convert a 32-bit number to a hex string with ls-byte first
		/// </sumary>
		public static string ToHexString(int num){
			string hex_chr = "0123456789ABCDEF";
			string str = "";
			str +=  hex_chr.Substring((num >> 4) & 0x0F,1) + hex_chr.Substring(num & 0x0F,1);
			return str;
		}

		/// <sumary>
		/// Reverse an input string
		/// </sumary>		
		public static string Reverse(string str) {
			int len = str.Length;
			char[] arr = new char[len];

			for (int i = 0; i < len; i++){
				arr[i] = str[len - 1 - i];
			}

			return new string(arr);
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
