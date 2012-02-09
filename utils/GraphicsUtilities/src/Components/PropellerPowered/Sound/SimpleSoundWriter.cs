using System;
using System.Collections;
using System.IO;

//****************************************
//trodoss - 2012
//See end of file for terms of use.  
//***************************************
//Version 1.0 - Initial Release
//Version 1.1 - Revised to include more RTTTL features, and switch from LONG (4 byte) to WORD (2 byte) data
//              to decrease overall file size
//************** N A M E S P A C E ****************************************
namespace PropellerPowered.Sound {
	//*********************************************************************
	// SimpleSoundWriter Class
	//*********************************************************************	
	public class SimpleSoundWriter {					
		private static int EncodeInByte(int number1, int number2) {
			int returnNumber = number1 << 4;
			returnNumber += number2;
			return returnNumber;
		}		
		//*********** P U B L I C   F U N C T I O N S  ( M E T H O D S ) ******
		public static void Write (PropellerPowered.Sound.Song thisSong, string outputFileName) {						
			Write( thisSong, outputFileName, FileMode.Create);
		}
		public static int Write (PropellerPowered.Sound.Song thisSong, string outputFileName, FileMode mode) {	
			int songLength = 0;
			
			try {
				using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(outputFileName, mode))) {

					//write out beats per minute for the song
					binaryWriter.Write((int) thisSong.BeatsPerMinute);
					
					foreach (Note thisNote in thisSong.Notes) {
						binaryWriter.Write((byte) EncodeInByte((int)thisNote.Pitch, (int)thisNote.Octave));
						binaryWriter.Write((byte) EncodeInByte((int)thisNote.Duration, (int)thisNote.Flag));
					}								
				}			
			} catch (Exception e){
				throw new SoundException("Error in creating output file " + outputFileName + " - " + e.ToString());
			}								
			
			//add in long (4 bytes) for tempo at the beginning of the song
			songLength +=4;
			
			//account for the 2 bytes for each of the note elements
			songLength += (thisSong.Notes.Count * 2);
			
			return songLength;
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
