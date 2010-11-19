using System;
using System.Collections;
using System.IO;
//****************************************
//trodoss - 2010
//See end of file for terms of use.  
//***************************************

//************** N A M E S P A C E ****************************************
namespace PropellerPowered.General {
	//*********************************************************************
	// FontGenerator Class
	//*********************************************************************	
	public class ReaderUtilities {
		//Source: 
		//http://www.developerfusion.com/code/4696/reading-binary-data-in-c/
		
		/// <summary>
		/// Reads data from a stream until the end is reached. The
		/// data is returned as a byte array. An IOException is
		/// thrown if any of the underlying IO calls fail.
		/// </summary>
		/// <param name="stream">The stream to read data from</param>
		/// <param name="initialLength">The initial buffer length</param>
		public static byte[] ReadFully (Stream stream, long initialLength){
			// If we've been passed an unhelpful initial length, just
			// use 32K.
			if (initialLength < 1) {
				initialLength = 32768;
			}
			
			byte[] buffer = new byte[initialLength];
			int read=0;
			
			int chunk;
			while ( (chunk = stream.Read(buffer, read, buffer.Length-read)) > 0){
				read += chunk;
				
				// If we've reached the end of our buffer, check to see if there's
				// any more information
				if (read == buffer.Length){
					int nextByte = stream.ReadByte();
					
					// End of stream? If so, we're done
					if (nextByte==-1){
						return buffer;
					}
					
					// Nope. Resize the buffer, put in the byte we've just
					// read, and continue
					byte[] newBuffer = new byte[buffer.Length*2];
					Array.Copy(buffer, newBuffer, buffer.Length);
					newBuffer[read]=(byte)nextByte;
					buffer = newBuffer;
					read++;
				}
			}
			// Buffer is now too big. Shrink it.
			byte[] ret = new byte[read];
			Array.Copy(buffer, ret, read);
			return ret;
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
