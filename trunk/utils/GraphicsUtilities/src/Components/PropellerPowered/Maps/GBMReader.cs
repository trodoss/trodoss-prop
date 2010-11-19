using System;
using System.Collections;
using System.IO;
using PropellerPowered.General;
//****************************************
//trodoss - 2010
//See end of file for terms of use.  
//***************************************
//Version 1.0 - Initial Release

// Descrption:
// Read the Gameboy Map Builder (GBM) format file and output as map data
//
// Gameboy Map Builder
// Version 1.8
// Copyright H. Mulder 1999

//************** N A M E S P A C E ****************************************
namespace PropellerPowered.Maps {
	//*********************************************************************
	// GBMReader Class
	//*********************************************************************	
	public class GBMReader {	
		// Convert four byte array elements to an int and display it.
		public static int ArrayEntryToInt32( byte[ ] bytes, int index ){
			int value = BitConverter.ToInt32( bytes, index );
			return value;
		}

		public static GBMFile ReadFile(byte[] byteArray, int offset) {
			GBMFile returnFile = null;
			//assume that the file header is 20 bytes long, and that the last 4 bytes contain the length
			int start = offset + 16;
			int fileLength = ArrayEntryToInt32(byteArray, start);
			
			//retain a copy of the header
			byte[] headerInfo = new byte[16];
			Array.Copy(byteArray, offset, headerInfo, 0, 16);
			
			byte[] returnByteArray = null;
			if (fileLength > 0) {
				returnByteArray = new byte[fileLength];
				Array.Copy(byteArray, start+4, returnByteArray, 0, fileLength);
			}
			returnFile = new GBMFile(fileLength, headerInfo, returnByteArray);
			
			//return the contents of the file as a GBMFile object
			return returnFile;		
		}
		
		private static Map ReadMapProperties(GBMFile mapPropertiesFile) {
			Map returnMap = null;
			//skip the first 128 bytes of the file, which are not used
			int offset = 128;
			int mapWidth = ArrayEntryToInt32(mapPropertiesFile.Contents, offset);
			int mapHeight = ArrayEntryToInt32(mapPropertiesFile.Contents, offset+4);
			returnMap = new Map(mapWidth, mapHeight);
			
			return returnMap;
		}
		
		private static MapCellList ReadMapData(GBMFile mapDataFile) {
			MapCellList thisList = new MapCellList();
			//we know each entry is 3 bytes in size,
			int entries = mapDataFile.Contents.Length / 3;
			int pointer = 2;
			for (int i=0; i<entries; i++) {
				MapCell thisCell = new MapCell((int)mapDataFile.Contents[pointer]);
				thisList.Add(thisCell);
				thisCell = null;
				pointer += 3;
			}
			
			return thisList;
		}
		
		//*********** P U B L I C   F U N C T I O N S  ( M E T H O D S ) ******
		public static Map Read(string inputFileName) {
			Map returnMap = new Map();
			try {
				//read the contents of the input file into a byte array
				FileStream fs = File.OpenRead(inputFileName);
				byte[] inputBytes = ReaderUtilities.ReadFully(fs, fs.Length);
				fs = null;
				
				int offset = 4;
				//we read this data to move past the start of the file
				GBMFile fileHeader = ReadFile(inputBytes, offset);
				
				//add to the offset, the previous file's length plus the file header length
				offset += fileHeader.FileLength + 20;
				
				//read the file properties data
				GBMFile fileMapProperties = ReadFile(inputBytes, offset);
				
				//add to the offset, the previous file's length plus the file header length
				offset += fileMapProperties.FileLength + 20;
				
				//read the physical map data
				GBMFile fileMapData = ReadFile(inputBytes, offset);
				
				if (fileMapProperties != null) 	returnMap = ReadMapProperties(fileMapProperties);
				if (fileMapData != null) returnMap.Cells = ReadMapData(fileMapData);
				
			} catch (Exception e){
				throw new MapException("Error in reading input file " + inputFileName + " - " + e.ToString());
			}
			
			return returnMap;
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
