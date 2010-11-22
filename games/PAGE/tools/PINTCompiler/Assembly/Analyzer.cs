using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using PINTCompiler.Utilities;
//****************************************
// PINT Compiler Assembler Analyzer
// Performs lexical analysis and data analysis on the code
//
// 2010 - trodoss
//See end of file for terms of use.  
//***************************************
namespace PINTCompiler.Assembly {
	public class Analyzer {

		private static void WriteError(CompilationLog thisLog, string message) {
			thisLog.CanContinue = false;
			thisLog.AddError("", 0, message);
		}
		
		private static void WriteError(CompilationLog thisLog, SourceLine thisLine, string message) {
			thisLog.CanContinue = false;
			thisLog.AddError(thisLine.Source, thisLine.LineNumber, message);
		}
		
		private static void WriteWarning(CompilationLog thisLog, SourceLine thisLine, string message) {
			thisLog.AddWarning(thisLine.Source, thisLine.LineNumber, message);
		}

		private static void WriteInformation(CompilationLog thisLog, string message) {
			thisLog.AddInformation("", 0, message);
		}		
		private static void WriteInformation(CompilationLog thisLog, SourceLine thisLine, string message) {
			thisLog.AddInformation(thisLine.Source, thisLine.LineNumber, message);
		}
				
		private static PINTRoomEntry PreProcessStrings(CompilationLog thisLog, PINTRoomEntry thisRoom) {
			for (int i=0; i< thisRoom.Strings.Count; i++) {
				PINTString thisString = (PINTString) thisRoom.Strings[i];
				//remove any embedded quotes
				thisString.Text = thisString.Text.Replace("\"", "");
				//strip down to just the ASCII we want
				thisString.Text = Regex.Replace(thisString.Text, @"[^\u0020-\u005F]", "");
				
				if (thisString.Text.Length > 20) {
					WriteError(thisLog, thisString.Source, "String size exceeded.  Strings must be 20 characters or less.  String Size: " + thisString.Text.Length);
				} else {
					ASCIIEncoding thisEncoding = new ASCIIEncoding();
					byte[] result = new byte[thisString.Text.Length + 1];
					byte[] encodeBytes = thisEncoding.GetBytes(thisString.Text);
					Array.Copy(encodeBytes,0,result,0,encodeBytes.Length);
					thisString.Bytes = result;
				}
			}
			
			return thisRoom;
		}
		
		private static PINTRoomEntry CalculateStringOffsets(CompilationLog thisLog, PINTRoomEntry thisRoom) {
			
			//to do: go trough each string and calculate their offset from the start of the data, which is used when a string is referenced.
			int stringOffset = thisRoom.Offset + (4 * thisRoom.Commands.Count);
			if (stringOffset > 512) WriteError (thisLog, "Resulting output (before string data is included) would exceed the 512 byte limit for room data.");
			
			for (int i=0; i< thisRoom.Strings.Count; i++) {
				PINTString thisString = (PINTString) thisRoom.Strings[i];
				thisString.Offset = stringOffset;
				
				//add in the string's length to increment the offset to the next entry (accounting for the null at the end)
				stringOffset += thisString.Bytes.Length;
				if (stringOffset > 512) WriteError (thisLog, "Resulting output (with string data included) would exceed the 512 byte limit for room data.");
			}
			
			return thisRoom;
		}
			
		private static PINTRoomEntry CalculateEventTableEntries(PINTRoomEntry thisRoom) {
			int found = -1;
			
			//0 = on_load
			found = thisRoom.FindCommandLocationByLabel("ON_LOAD");
			if (found > -1) thisRoom.EventTableEntries[0] = (found * 4) + thisRoom.Offset;
			found = -1;
			
			//1 = on_use
			found = thisRoom.FindCommandLocationByLabel("ON_USE");
			if (found > -1) thisRoom.EventTableEntries[1] = (found * 4) + thisRoom.Offset;
			found = -1;
			
			//2 = on_take
			found = thisRoom.FindCommandLocationByLabel("ON_TAKE");
			if (found > -1) thisRoom.EventTableEntries[2] = (found * 4) + thisRoom.Offset;
			found = -1;
			
			//3 = on_look
			found = thisRoom.FindCommandLocationByLabel("ON_LOOK");
			if (found > -1) thisRoom.EventTableEntries[3] = (found * 4) + thisRoom.Offset;
			found = -1;

			//4 = on_talk
			found = thisRoom.FindCommandLocationByLabel("ON_TALK");
			if (found > -1) thisRoom.EventTableEntries[4] = (found * 4) + thisRoom.Offset;
			found = -1;
			
			//5 = on_give
			found = thisRoom.FindCommandLocationByLabel("ON_GIVE");
			if (found > -1) thisRoom.EventTableEntries[5] = (found * 4) + thisRoom.Offset;
			found = -1;
			
			//6 = on_hotspot1
			found = thisRoom.FindCommandLocationByLabel("ON_HOTSPOT1");
			if (found > -1) thisRoom.EventTableEntries[6] = (found * 4) + thisRoom.Offset;
			found = -1;
			
			//7 = on_hotspot2
			found = thisRoom.FindCommandLocationByLabel("ON_HOTSPOT2");
			if (found > -1) thisRoom.EventTableEntries[7] = (found * 4) + thisRoom.Offset;
			found = -1;
			
			//8 = on_hotspot3
			found = thisRoom.FindCommandLocationByLabel("ON_HOTSPOT3");
			if (found > -1) thisRoom.EventTableEntries[8] = (found * 4) + thisRoom.Offset;
			found = -1;
			
			//9 = on_hotspot4
			found = thisRoom.FindCommandLocationByLabel("ON_HOTSPOT4");
			if (found > -1) thisRoom.EventTableEntries[9] = (found * 4) + thisRoom.Offset;
			found = -1;	

			return thisRoom;
		}
		
		private static PINTRoomEntry ResolveReferences(CompilationLog thisLog, PINTRoomEntry thisRoom) {
			//start resolving references
			//********************************** Reference Resolution: *****************************************
			for (int i=0; i< thisRoom.Commands.Count; i++) {
				PINTCommand thisCommand = (PINTCommand) thisRoom.Commands[i];
				string thisName = thisCommand.GetType().Name;
				int testForLabel = -1;
				int absoluteReference = 0;
				switch (thisName) {
					case "VariableTest":	
						VariableTest thisVariableTest = (VariableTest) thisCommand;
						testForLabel = thisRoom.FindCommandLocationByLabel(thisVariableTest.LabelReference);
						if (testForLabel > -1) {
							absoluteReference = (testForLabel * 4) + thisRoom.Offset;
							thisVariableTest.NotMetJumpTo = absoluteReference;
							thisRoom.Commands[i] = thisVariableTest;
						} else {
							WriteError(thisLog, thisVariableTest.Source, "Label '" + thisVariableTest.LabelReference + "' not found.");
						}
						thisVariableTest = null;
					break;
					
					case "HotspotTest":					
						HotspotTest thisHotspotTest = (HotspotTest) thisCommand;
						testForLabel = thisRoom.FindCommandLocationByLabel(thisHotspotTest.LabelReference);
						if (testForLabel > -1) {
							absoluteReference = (testForLabel * 4) + thisRoom.Offset;
							thisHotspotTest.NotMetJumpTo = absoluteReference;
							thisRoom.Commands[i] = thisHotspotTest;
						} else {
							WriteError(thisLog, thisHotspotTest.Source, "Label '" + thisHotspotTest.LabelReference + "' not found.");
						}
						thisHotspotTest = null;
					break;

					case "InventoryTest":	
						InventoryTest thisInventoryTest = (InventoryTest) thisCommand;
						testForLabel = thisRoom.FindCommandLocationByLabel(thisInventoryTest.LabelReference);
						if (testForLabel > -1) {
							absoluteReference = (testForLabel * 4) + thisRoom.Offset;
							thisInventoryTest.NotMetJumpTo = absoluteReference;
							thisRoom.Commands[i] = thisInventoryTest;
						} else {
							WriteError(thisLog, thisInventoryTest.Source, "Label '" + thisInventoryTest.LabelReference + "' not found.");
						}
						thisInventoryTest = null;					
					break;
					
					case "CodeGoto":					
						CodeGoto thisCodeGoTo = (CodeGoto) thisCommand;
						testForLabel = thisRoom.FindCommandLocationByLabel(thisCodeGoTo.LabelReference);
						if (testForLabel > -1) {
							absoluteReference = (testForLabel * 4) + thisRoom.Offset;
							thisCodeGoTo.JumpTo = absoluteReference;
							thisRoom.Commands[i] = thisCodeGoTo;
						} else {
							WriteError(thisLog, thisCodeGoTo.Source, "Label '" + thisCodeGoTo.LabelReference + "' not found.");
						}
						thisCodeGoTo = null;					
					break;
					
					case "Say":
						absoluteReference = -1;
						Say thisSay = (Say) thisCommand;
						absoluteReference = thisRoom.FindTextOffsetByID(thisSay.TextID);
						if (absoluteReference > -1) {
							thisSay.TextPointer = absoluteReference;
							thisRoom.Commands[i] = thisSay;
						} else {
							WriteError(thisLog, thisSay.Source, "Text ID '" + thisSay.TextID + "' not found.");
						}
					break;	
				}
			}
			return thisRoom;
		}		
		
		public static PINTRoomEntry Analyze(PINTRoomEntry thisRoom, CompilationLog thisLog) {
			WriteInformation(thisLog, "Analyzing room ID \"" + thisRoom.RoomID + "\"");
			
			WriteInformation(thisLog,"Pre-processing strings");
			thisRoom = PreProcessStrings(thisLog,  thisRoom);
			
			if (thisRoom.Log.CanContinue) {		
				WriteInformation(thisLog, "Calculating string offsets");
				thisRoom = CalculateStringOffsets(thisLog, thisRoom);
				if (thisRoom.Log.CanContinue) {
					WriteInformation(thisLog, "Calculating event table entries");
					thisRoom = CalculateEventTableEntries(thisRoom);
					
					WriteInformation(thisLog, "Resolving label references");
					thisRoom = ResolveReferences(thisLog, thisRoom);
				}
			}
			return thisRoom;
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
