using System;
using System.IO;
using System.Collections;
using PropellerPowered.General;
using PropellerPowered.Sound;
//****************************************
//trodoss - 2010 
//See end of file for terms of use.  
//***************************************

//************** N A M E S P A C E ****************************************
namespace PropellerPowered.Sound {
	//*********************************************************************
	// SoundConverter Class
	//*********************************************************************		
	public enum InputType {RTTTL}
	/// <summary>
	/// Establish the standard output types ( RTTTL)
	/// </summary>
	
	public enum OutputType {SimpleSound}
	/// <summary>
	/// Establish the standard output types ( SimpleSound)
	/// </summary>
	
	public class SoundConverter {
		/// <sumary>
		/// Generate the physical font file based on settings
		/// </sumary>
		public static void Convert(InputType inputType, OutputType outputType, string inputFileName, string outputFileName){
			PropellerPowered.Sound.Song song;
			
			if ((inputFileName == "") || (inputFileName == null)) {
				throw new SoundException("Input file name not specified");
			}
			
			if ((outputFileName == "") || (outputFileName == null)) {
				throw new SoundException("Output file name not specified");
			}		
			
			//to do: check to see if wildcard processing is being done
			if (inputFileName.IndexOf('*') > -1) {
				string path = Directory.GetCurrentDirectory();
				DirectoryInfo di = new DirectoryInfo(path);
				FileInfo[] files = di.GetFiles(inputFileName, SearchOption.TopDirectoryOnly);
				ArrayList songLengths = new ArrayList();
				
				foreach (FileInfo file in files) {
					string thisFileName = file.Name;
					int songLength = 0;
					try {		
						song = new PropellerPowered.Sound.Song();
						
						switch (inputType) {
							case InputType.RTTTL:
								song = RTTTLReader.Read(thisFileName);
								break;
						}
						
					} catch (Exception e) {
						throw new SoundException("Unable to load file: " + e.ToString());
					}
					
					switch (outputType) {
						case OutputType.SimpleSound:
							songLength = SimpleSoundWriter.Write(song, outputFileName, FileMode.Append);
							songLengths.Add(songLength);
							break;	
						
						default:
						throw new SoundException("Output type not recognized.");
					}
					if (song != null) song = null;	
				}
				di = null;
				files = null;
				
				//write sound index file
				IndexWriter.Write(songLengths, "sndindex.six");
				
			} else {
				try {		
					song = new PropellerPowered.Sound.Song();
					
					switch (inputType) {
						case InputType.RTTTL:
							song = RTTTLReader.Read(inputFileName);
							break;
					}
					
				} catch (Exception e) {
					throw new SoundException("Unable to load file: " + e.ToString());
				}
				
				switch (outputType) {
					case OutputType.SimpleSound:
						SimpleSoundWriter.Write(song, outputFileName);
						break;	
					

					default:
					throw new SoundException("Output type not recognized.");
				}
				if (song != null) song = null;
			}
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
