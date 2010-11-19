using System;
using System.Collections;
using System.IO;
using PropellerPowered.General;
using PropellerPowered.Maps;
//****************************************
//GBMFix
//(c) 2010   trodoss 
//See end of file for terms of use.  
//***************************************
//Version 1.0 - Initial Release

//************** N A M E S P A C E ****************************************
namespace GBMFix {
	//*********************************************************************
	// GBMFix Class
	//*********************************************************************	
	public class GBMFix {			
		/// <sumary>
		/// application version number
		/// </sumary>
		private static string GetVersion() {
			return "1.0";
		}
		
		//*********** P U B L I C   F U N C T I O N S  ( M E T H O D S ) ******
		/// <sumary>
		/// Main (entrypoint)	
		/// </sumary>
		public static void Main(string[] arguments) {
			string inputFileName = "";
			string outputFileName = "";
			string tilesetFileName = "";
			
			string currentFilePath =  Directory.GetCurrentDirectory();

			if (arguments.Length > 0) {
				inputFileName = arguments[0];
				
			if (arguments.Length > 1) {
				outputFileName = arguments[1];
			} else {
				outputFileName = "fixed_" + inputFileName;
			}
			
			if (arguments.Length > 2) tilesetFileName = arguments[2];
			

			try {
					ArrayList fileList = ReadGBMFile(inputFileName, currentFilePath, tilesetFileName);
					WriteGBMFile(fileList, outputFileName);
				} catch (Exception e) {
					Console.WriteLine("Error in GBMFix v"+GetVersion() +" - " + e.ToString());
				}
			} else {
				Console.WriteLine ("GBMFix v"+GetVersion()+"  - Usage: GBMFix.exe [input_filename] ([output_filename] [tileset_filename]) ");
			}
		}
		
		public static ArrayList ReadGBMFile (string inputFileName, string currentFilePath, string tilesetFileName) {
			ArrayList fileContents = new ArrayList();
			
			//read the contents of the input file into a byte array
			FileStream fs = File.OpenRead(inputFileName);
			byte[] inputBytes = ReaderUtilities.ReadFully(fs, fs.Length);
			fs = null;
				
			int offset = 4;
			//we read this data to move past the start of the file
			GBMFile fileHeader = GBMReader.ReadFile(inputBytes, offset);
			fileContents.Add(fileHeader);
				
			//add to the offset, the previous file's length plus the file header length
			offset += fileHeader.FileLength + 20;
				
			//read the file properties data
			GBMFile fileMapProperties = GBMReader.ReadFile(inputBytes, offset);
			GBMFile fileFix = FixMapPropertiesFile(fileMapProperties, currentFilePath, tilesetFileName);
			fileContents.Add(fileFix);
			
			//add to the offset, the previous file's length plus the file header length
			offset += fileMapProperties.FileLength + 20;
				
			//process through the remaining 9 files included
			for (int i=0; i < 8; i++) {
				//read the physical map data
				GBMFile fileUnidentified1 = GBMReader.ReadFile(inputBytes, offset);
				fileContents.Add(fileUnidentified1);

				//add to the offset, the previous file's length plus the file header length
				offset += fileUnidentified1.FileLength + 20;
				fileUnidentified1 = null;
			}
			
			return fileContents;
		}
		
		public static int Search(byte[] byteArray, int startPosition, int length, byte searchFor) {
			int end = startPosition + length;
			int foundPosition = -1;
			for (int i=startPosition; i<end; i++) {
				if (byteArray[i] == searchFor) {
					foundPosition = i;
					break;
				}
			}
			return foundPosition;
		}	
		
		public static GBMFile FixMapPropertiesFile (GBMFile inputFile, string currentFilePath, string tilesetFileName) {
			//there are 140 bytes of header information preceeding the filename
			byte[] headerInfo = new byte[140];
			Array.Copy(inputFile.Contents, 0, headerInfo, 0, 140);
			
			int nullCharPosition = Search(inputFile.Contents, 140, inputFile.Contents.Length-140, (byte)0);
			int tailLength = inputFile.Contents.Length - nullCharPosition;
			
			//if no overriding tileset filename is being specified, derive the filename from the current contents
			if (tilesetFileName == "") {

				int lastBackslashPosition = 140;
				//work backwards to get the filename (until a backslash is found)
				for (int i = (140 + nullCharPosition); i > 140; i--) {
					if (inputFile.Contents[i] == (byte)92) {
						lastBackslashPosition = i;
						break;
					}
				}

				int fileNameLength = (nullCharPosition - lastBackslashPosition) -1;
				byte[] derivedFilenameInfo = new byte[fileNameLength];
				//copy into a byte buffer, and convert to a string for later use
				System.Buffer.BlockCopy (inputFile.Contents, lastBackslashPosition + 1, derivedFilenameInfo, 0, fileNameLength);
				tilesetFileName = System.Text.ASCIIEncoding.ASCII.GetString(derivedFilenameInfo);
				
			}
						
			System.Text.ASCIIEncoding  encoding = new System.Text.ASCIIEncoding();
			string fullFileName = currentFilePath + "\\" + tilesetFileName;
			byte[] filePathInfo = encoding.GetBytes(fullFileName);
			encoding = null;
			
			//data that represents the rest of the information in the file
			byte[] tailInfo = new byte[tailLength];
			System.Buffer.BlockCopy (inputFile.Contents, nullCharPosition, tailInfo, 0, tailLength);
			
			byte[] concat = new byte[headerInfo.Length + filePathInfo.Length + tailInfo.Length];

			//concatonate the contents into a single array
			System.Buffer.BlockCopy (headerInfo, 0, concat, 0, headerInfo.Length);
			System.Buffer.BlockCopy (filePathInfo, 0, concat, headerInfo.Length, filePathInfo.Length);
			System.Buffer.BlockCopy (tailInfo, 0, concat, headerInfo.Length + filePathInfo.Length, tailInfo.Length);

			GBMFile newFile = new GBMFile(concat.Length, inputFile.Header, concat);
			
			return newFile;
		}
		
		public static void WriteGBMFile(ArrayList fileList, string outputFileName) {
		    using(BinaryWriter binWriter = new BinaryWriter(File.Open(outputFileName, FileMode.Create))) {
				binWriter.Write('G');
				binWriter.Write('B');
				binWriter.Write('O');
				binWriter.Write('1');
				foreach (GBMFile thisFile in fileList) {
					binWriter.Write(thisFile.Header);
					binWriter.Write(thisFile.FileLength);
					if (thisFile.Contents != null) binWriter.Write(thisFile.Contents);
				}
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
