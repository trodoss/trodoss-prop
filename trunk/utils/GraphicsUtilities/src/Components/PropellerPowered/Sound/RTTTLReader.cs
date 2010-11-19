using System;
using System.Collections;
using System.IO;
//****************************************
//trodoss - 2010
//See end of file for terms of use.  
//***************************************
//Version 1.0 - Initial Release
//************** N A M E S P A C E ****************************************
namespace PropellerPowered.Sound {
	//*********************************************************************
	// RTTTLReader Class
	//*********************************************************************	
	public class RTTTLReader {					
		
		//*********** P U B L I C   F U N C T I O N S  ( M E T H O D S ) ******
		public static Song Read(string inputFileName) {
			Song thisSong = new Song();
			int defaultNoteLength = 0;
			int defaultOctave = 0;
			int beatsPerMinute = 0;
			
			try {
				StreamReader reader = new StreamReader(inputFileName);
				string musicFileData = reader.ReadToEnd();
				reader = null;
				
				string [] musicDataElements = musicFileData.Split(':');
				//skip first element (title), since we cannot use it
									
				//second section includes defaults
				if (musicDataElements.Length > 1) {
					string [] musicDefaultDataSubElements = musicDataElements[1].Split(',');
					//each of the sub elments are divided by an = sign from their value
					foreach (string musicDefaultSubElement in musicDefaultDataSubElements) {
						string [] musicDefaultParameter = musicDefaultSubElement.Split('=');
						if (musicDefaultParameter.Length > 1) {
							switch (musicDefaultParameter[0].ToUpper()) {
								//default note length
								case "D":
									defaultNoteLength = Convert.ToInt32(musicDefaultParameter[1]);
									break;
														
								case "O":
									defaultOctave = Convert.ToInt32(musicDefaultParameter[1]);
									break;

								case "B":
									beatsPerMinute = Convert.ToInt32(musicDefaultParameter[1]);
									break;														
													
								default:
									throw new SoundException("Unrecognized RTTTL command '" + musicDefaultParameter[0] + "'");
							}
						}
					}				
				}
				
				thisSong.BeatsPerMinute = beatsPerMinute;
									
				//third section includes the actual song data
				if (musicDataElements.Length > 2) {
					string [] musicNotesSubElements = musicDataElements[2].Split(',');
					for (int j=0; j < musicNotesSubElements.Length; j++) {
						string musicNoteSubElement = (string) musicNotesSubElements[j];
						bool noteFound = false;

						string octaveString = "";
						string lengthString = "";
						string noteString = "";
											
						foreach (char musicNoteSubCharacter in musicNoteSubElement) {
												
							//handle characters below a specific range
							if (musicNoteSubCharacter < 32) {
								throw new SoundException("Unrecognized character in sub element");
							} else {
								if (musicNoteSubCharacter == 35) noteString += "#";
													
								//numeric digit
								if ((musicNoteSubCharacter > 47) && (musicNoteSubCharacter < 58)) {
														
								//if the note portion has already been found, this is now the octive being specified
								if (noteFound) {
									octaveString += (char) musicNoteSubCharacter;
								} else {
									//otherwise, this is the note length being specified
									lengthString += (char) musicNoteSubCharacter;
								}
							}
													
							//leter (a-h)
							if ((musicNoteSubCharacter > 96) && (musicNoteSubCharacter <= 123)) {
								noteFound = true;
								noteString += (char) musicNoteSubCharacter;
														
							}
													
						}
												
					}
											
					int octaveValue = defaultOctave;
					int lengthNumber = defaultNoteLength;
					int lengthValue = 0;
					int noteValue = 0;
					int eofFlagValue = 0;
											
					//if values were specified for this note, use them
					if (octaveString != "") octaveValue = Convert.ToInt32(octaveString);
					if (lengthString != "") lengthNumber = Convert.ToInt32(lengthString);
	
					//translate the note values to an integer
					switch (noteString) {
						case "p":
							noteValue = 0;
							break;
													
						case "a":
							noteValue = 1;
							break;
													
						case "a#":
							noteValue = 2;
							break;
													
						case "b":
							noteValue = 3;
							break;
													
						case "c":
							noteValue = 4;
							break;
												
						case "c#":
							noteValue = 5;
							break;
												
						case "d":
							noteValue = 6;
							break;
													
						case "d#":
							noteValue = 7;
							break;
												
						case "e":
							noteValue = 8;
							break;
													
						case "f":
							noteValue = 9;
							break;
													
						case "f#":
							noteValue = 10;
							break;
													
						case "g":
							noteValue = 11;
							break;
													
						case "g#":
							noteValue = 12;
							break;									
					}
											
					//translate the length value into integer
					switch (lengthNumber) {
						case (int)64:
							lengthValue = 0;
							break;
											
						case (int)32:
							lengthValue = 1;
							break;
												
						case (int)16:
							lengthValue = 2;
							break;
													
						case (int)8:
							lengthValue = 3;
							break;
													
						case (int)4:
							lengthValue = 4;
							break;

						case (int)2:
							lengthValue = 5;
							break;
													
						case (int)1:
							lengthValue = 6;
							break;
					}
											
					//if this is the last note, mark the EOF byte
					if (j == (musicNotesSubElements.Length-1)) eofFlagValue = 1;
											
					Note thisNote = new Note();
					thisNote.NoteValue = noteValue;
					thisNote.Octave = octaveValue;
					thisNote.Duration = lengthValue;
					thisNote.EOFFlag = eofFlagValue;
					thisSong.Notes.Add(thisNote);
					thisNote = null;
					}
				}
				
			} catch (Exception e){
				throw new SoundException("Error in reading input file " + inputFileName + " - " + e.ToString());
			}
			
			return thisSong;
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
