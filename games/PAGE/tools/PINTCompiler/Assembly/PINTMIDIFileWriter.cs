using System;
using System.Collections;
using System.IO;
using PINTCompiler.Utilities;
//****************************************
//PINTMIDIFileWriter
//2011   trodoss 
//See end of file for terms of use.  
//***************************************

//************** N A M E S P A C E ****************************************
namespace PINTCompiler.Assembly {
	//*********************************************************************
	// PINTMIDIFileWriter Class
	//*********************************************************************	
	public enum MIDIFileState { Valid = 0, FileNotFound = -1, FileEmpty = -2, NotMIDI = -3, NotMIDI0Format = -4 };
	public class PINTMIDIFileWriter {			

		//must be: MIDI 0 (all tracks mixed into 1)
		//must have: < 20 channels

		//documentation from http://www.sonicspot.com/guide/midifiles.html
		
		//Header
		//Offset Length Type Description Value 
		//0x00 4 char[4] chunk ID "MThd" (0x4D546864) 
		//0x04 4 dword chunk size 6 (0x00000006) 
		//0x08 2 word format type 0 - 2 
		//0x10 2 word number of tracks 1 - 65,535 
		//0x12 2 word time division

		//*********** P U B L I C   F U N C T I O N S  ( M E T H O D S ) ******
		/// <sumary>
		/// Main (entrypoint)	
		/// </sumary>
		public void Process(PINTMusicEntryList thisList, CompilationLog thisLog) {
			foreach (PINTMusicEntry thisMusic in thisList) {
				MIDIFileState thisState = Write(thisMusic.FileName, thisMusic.MusicID);
				switch (thisState) {
					case MIDIFileState.Valid:
						thisLog.AddInformation("", 0, "MIDI File: " + thisMusic.FileName + " processed as: " + thisMusic.MusicID +".MID");
						break;
						
					case MIDIFileState.FileNotFound:
						thisLog.AddError("", 0, "File not found: " + thisMusic.FileName);
						break;
	
					case MIDIFileState.FileEmpty:
						thisLog.AddError("", 0, "File empty or invalid format: " + thisMusic.FileName);
						break;	
						
					case MIDIFileState.NotMIDI:
						thisLog.AddError("", 0, "File not MIDI file: " + thisMusic.FileName + " - MIDI files must be in MIDI 0 format");
						break;							

					case MIDIFileState.NotMIDI0Format:
						thisLog.AddError("", 0, "File not in MIDI 0 format: " + thisMusic.FileName);
						break;							
				}
			}
		}
		
		public MIDIFileState Write(string source, int id) {
			MIDIFileState returnState = Validate(source);
			if (returnState == MIDIFileState.Valid) {
				string newFilename = id.ToString() + ".MID";
				File.Copy(source, newFilename, true);
			}
			return returnState;
		}

		public MIDIFileState Validate(string source) {
			MIDIFileState returnState = MIDIFileState.Valid;
			
			if (File.Exists(source)) {
				using (BinaryReader binReader = new BinaryReader(File.Open(source, FileMode.Open))) {
					byte[] fileID = new byte[4];
					int count = binReader.Read(fileID, 0, 4);
					
					if (count > 0) {
						//test to see if this is a MIDI header
						if ((fileID[0] == 'M') && (fileID[1] == 'T') && (fileID[2] == 'h') && (fileID[3] == 'd')) {
							//read past the next 4 bytes (chunk size)
							binReader.Read(fileID, 0, 4);
							
							//examine the format type
							int formatType = binReader.ReadInt16();
							if (formatType == 0) {
								returnState = MIDIFileState.Valid;
							} else {
								returnState = MIDIFileState.NotMIDI0Format;
							}
						} else {
							returnState = MIDIFileState.NotMIDI;
						}
					} else {
						returnState = MIDIFileState.FileEmpty;
					}
				}

			} else {
				returnState = MIDIFileState.FileNotFound;
			}
			return returnState;
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
