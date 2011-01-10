using System;
using System.Collections;
using System.Text;
using System.IO;
using PINTCompiler.Utilities;
//****************************************
// PINTItemFile
// 2011 trodoss
//See end of file for terms of use.  
//***************************************
/*
PINT Item file

The item file contains 8 4-character entries that constitute
the items possible in the PAGE game.  The entire file should
be 32 bytes in size.

entries that are less than 4 characters are padded with char 0
to reach the 4 character limit.

*/
namespace PINTCompiler.Assembly {
	public class PINTItemFile {
	
		public PINTItemFile() {
		}
		
		public void Save(string saveFileName, PINTItemEntryList thisList, CompilationLog thisLog) {
			thisLog.AddInformation("", 0, "Building item file \"" + saveFileName + "\"");
			using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(saveFileName, FileMode.Create))) {
				thisLog.AddInformation("", 0, "Item count: " + thisList.Count);
				
				//the code is allocate 32 bytes of space
				int bytesLeft = 32;
				
				foreach (PINTItemEntry thisEntry in thisList) {
					binaryWriter.Write(thisEntry.Bytes);
					bytesLeft -= 4;
				}
				
				//pad out the file for any extra code space
				for (int j=0; j< bytesLeft; j++) {
					binaryWriter.Write((byte)0);
				}				
			}
		}
		
	}
}