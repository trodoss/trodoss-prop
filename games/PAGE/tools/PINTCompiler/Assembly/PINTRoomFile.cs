using System;
using System.Collections;
using System.Text;
using System.IO;
using PINTCompiler.Utilities;
//****************************************
// PINTRoomFile
// 2010 trodoss
//See end of file for terms of use.  
//***************************************

/*
PINT Room file

Each "entry" is 512 bytes in size, which contains all of the script logic for the room

Header:
Bytes: Description
0-1: Backdrop ID
2-21: Event table
[22-39]: Hotspot table

The event table contains 10 entries, each corresponding the absolute location in the room data where an 'event' starts.
Empty entries have '512' (pointing to a value outside the data).

the following are a list of each of these entries:
event entries:
0 = on_load
1 = on_use
2 = on_take
3 = on_look
4 = on_talk
5 = on_give
6 = on_hotspot1
7 = on_hotspot2
8 = on_hotspot3
9 = on_hotspot4

The hotspot table consists of 4 byte entries, with 4 entries
each entry contains:
x position
y position
width
height

Following the header, the next entries are the opcodes themselves.

Immediately following the opcodes are the strings specified in the data (0-terminated).
*/
namespace PINTCompiler.Assembly {
	public class PINTRoomFile {

		private static int EncodeInByte(int number1, int number2) {
			int returnNumber = number1 << 4;
			returnNumber += number2;
			return returnNumber;
		}
		
		public PINTRoomFile() {
		}
		
		public void Save(string saveFileName, PINTRoomEntry thisRoom, CompilationLog thisLog) {
			
			thisLog.AddInformation("", 0, "Building room file \"" + saveFileName + "\"");
			using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(saveFileName, FileMode.Create))) {
					
					//write out the backdrop (6400 bytes)
					thisLog.AddInformation("", 0, "Adding backdrop \"" + thisRoom.Backdrop.FilePath + "\" to room file");
					PINTBitmapWriter.Write(thisRoom.Backdrop.FilePath, binaryWriter, 80, 80);
					
					//write out any PIC (128 bytes) entries (max 6 = 768 bytes)
					thisLog.AddInformation("", 0, "Pic count: " + thisRoom.Pics.Count);
					foreach (PINTPicEntry thisPic in thisRoom.Pics) {
						thisLog.AddInformation("", 0, "Adding pic \"" + thisPic.FilePath + "\" to room file");
						PINTBitmapWriter.Write(thisPic.FilePath, binaryWriter, 8, 16);
					}
					
					//pad out the file for any missing pics
					for (int k=0; k < (6 - thisRoom.Pics.Count) * 128; k++) {
						binaryWriter.Write((byte)0);
					}					
			
					//the code is allocate 512 bytes of space
					int bytesLeft = 512;
					
					foreach (int thisEntry in thisRoom.EventTableEntries) {
						binaryWriter.Write((short)thisEntry);
						bytesLeft -=2;
					}
					
					thisLog.AddInformation("", 0, "Hotspot count: " + thisRoom.Hotspots.Count);
					foreach (PINTHotspotEntry thisHotspot in thisRoom.Hotspots) {
						binaryWriter.Write((byte)thisHotspot.X);
						binaryWriter.Write((byte)thisHotspot.Y);
						binaryWriter.Write((byte)thisHotspot.Width);
						binaryWriter.Write((byte)thisHotspot.Height);
						bytesLeft -= 4;
					}
					//pad out the file for any missing hotspots
					for (int i=0; i< (4-thisRoom.Hotspots.Count); i++) {
						binaryWriter.Write((int)0);
						bytesLeft -= 4;
					}
					
					thisLog.AddInformation("", 0, "Command count: " + thisRoom.Commands.Count);
					foreach(PINTCommand thisCommand in thisRoom.Commands) {
						string thisName = thisCommand.GetType().Name;
						switch (thisName) {
						
							case "VariableSet":
								VariableSet variableSet = (VariableSet)thisCommand;
								binaryWriter.Write(variableSet.Opcode);
								binaryWriter.Write((byte)variableSet.VariableID);	
								binaryWriter.Write((byte)Convert.ToInt32(variableSet.Type));								
								binaryWriter.Write((byte)variableSet.NewValue); 
								variableSet = null;
								break;	
								
							case "VariableMath":
								VariableMath variableMath = (VariableMath)thisCommand;
								binaryWriter.Write(variableMath.Opcode);
								binaryWriter.Write((byte)variableMath.VariableID);	
								binaryWriter.Write((byte)Convert.ToInt32(variableMath.Type));								
								binaryWriter.Write((byte)variableMath.NewValue); 
								variableMath = null;
								break;									
								
							case "VariableTest":
								VariableTest variableTest = (VariableTest)thisCommand;
								binaryWriter.Write(variableTest.Opcode);
								binaryWriter.Write((byte)EncodeInByte(variableTest.VariableID, (int)variableTest.Comparison));			
								binaryWriter.Write((byte)variableTest.TestValue); 
								binaryWriter.Write((byte)variableTest.NotMetJumpTo);
								variableTest = null;
								break;									
								
							case "CodeGoto":
								CodeGoto codeGoto = (CodeGoto)thisCommand;
								binaryWriter.Write(codeGoto.Opcode);
								binaryWriter.Write((byte)0); //filler
								binaryWriter.Write((byte)codeGoto.JumpTo);			
								binaryWriter.Write((byte)0); //filler
								variableTest = null;
								break;								
								
							case "RoomLoad":
								RoomLoad roomLoad = (RoomLoad)thisCommand;
								binaryWriter.Write(roomLoad.Opcode);
								binaryWriter.Write((byte)0); //filler
								binaryWriter.Write((short)roomLoad.RoomID);			
								variableTest = null;
								break;	
								
							case "HotspotTest":
								HotspotTest hotspotTest = (HotspotTest)thisCommand;
								binaryWriter.Write(hotspotTest.Opcode);
								binaryWriter.Write((byte)0); //filler
								binaryWriter.Write((byte)hotspotTest.HotspotID);			 
								binaryWriter.Write((byte)hotspotTest.NotMetJumpTo);
								hotspotTest = null;
								break;	

							case "PicLoad":
								PicLoad picLoad = (PicLoad)thisCommand;
								binaryWriter.Write(picLoad.Opcode);
								binaryWriter.Write((byte)picLoad.PicID);			
								binaryWriter.Write((byte)picLoad.X);
								binaryWriter.Write((byte)picLoad.Y);
								picLoad = null;
								break;

							case "PicHide":
								PicHide picHide = (PicHide)thisCommand;
								binaryWriter.Write(picHide.Opcode);
								binaryWriter.Write((byte)0);
								binaryWriter.Write((byte)picHide.PicID);			
								binaryWriter.Write((byte)0);
								picHide = null;
								break;									
								
							case "Say":
								Say sayCommand = (Say)thisCommand;
								binaryWriter.Write(sayCommand.Opcode);
								binaryWriter.Write((byte)0); //filler								
								binaryWriter.Write((short)sayCommand.TextPointer);			
								sayCommand = null;
								break;	
								
							case "EgoLoad":
								EgoLoad egoLoad = (EgoLoad)thisCommand;
								binaryWriter.Write(egoLoad.Opcode);
								binaryWriter.Write((byte)egoLoad.X);			
								binaryWriter.Write((byte)egoLoad.Y);
								binaryWriter.Write((byte)egoLoad.Facing);
								egoLoad = null;
								break;	

							case "InventoryAdd":
								InventoryAdd inventoryAdd = (InventoryAdd)thisCommand;
								binaryWriter.Write(inventoryAdd.Opcode);
								binaryWriter.Write((byte)0);
								binaryWriter.Write((byte)inventoryAdd.ItemID);			
								binaryWriter.Write((byte)0);
								inventoryAdd = null;
								break;									

							case "InventoryTest":
								InventoryTest inventoryTest = (InventoryTest)thisCommand;
								binaryWriter.Write(inventoryTest.Opcode);
								binaryWriter.Write((byte)0);
								binaryWriter.Write((byte)inventoryTest.ItemID);			
								binaryWriter.Write((byte)inventoryTest.NotMetJumpTo);
								inventoryTest = null;
								break;																	
								
							case "InventoryRemove":
								InventoryRemove inventoryRemove = (InventoryRemove)thisCommand;
								binaryWriter.Write(inventoryRemove.Opcode);
								binaryWriter.Write((byte)0);
								binaryWriter.Write((byte)inventoryRemove.ItemID);			
								binaryWriter.Write((byte)0);
								inventoryRemove = null;
								break;	
								
							case "MusicPlay":
								MusicPlay musicPlay = (MusicPlay)thisCommand;
								binaryWriter.Write(musicPlay.Opcode);
								binaryWriter.Write((byte)0);
								binaryWriter.Write((byte)musicPlay.MusicID);			
								binaryWriter.Write((byte)0);
								musicPlay = null;
								break;									
								
							case "EventEnd":
								EventEnd eventEndCommand = (EventEnd) thisCommand;
								binaryWriter.Write((byte)eventEndCommand.Opcode);
								binaryWriter.Write((byte)0);
								binaryWriter.Write((byte)0);
								binaryWriter.Write((byte)0);	
								eventEndCommand = null;								
								break;									
						}
						bytesLeft -= 4;	
					}
					
					thisLog.AddInformation("", 0, "String count: " + thisRoom.Strings.Count);
					foreach (PINTString thisString in thisRoom.Strings) {
						binaryWriter.Write(thisString.Bytes);
						bytesLeft -= thisString.Bytes.Length;
					}
					
					//pad out the file for any extra code space
					for (int j=0; j< bytesLeft; j++) {
						binaryWriter.Write((byte)0);
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
