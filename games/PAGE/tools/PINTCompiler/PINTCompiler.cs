using System;
using System.Collections;
using System.Text;
using System.IO;
using PINTCompiler.Utilities;
using PINTCompiler.Assembly;
using PINTCompiler.PINTBasic;
//****************************************
// PINTCompiler
//
//2010 trodoss
//See end of file for terms of use.  
//***************************************

namespace PINTCompiler {
	//*********************************************************************
	// DataBuilder Class
	//*********************************************************************	
	public class PINTCompiler {
		/// <sumary>
		/// Retrieves the version text 
		/// </sumary>
		private static string VersionText() { return "v0.1"; }
			
			
		//*********** P U B L I C   F U N C T I O N S  ( M E T H O D S ) ******
		/// <sumary>
		/// Main (entrypoint)	
		/// </sumary>
		public static void Main(string[] arguments) {
			string fileName = "";
			bool generateILfile = true;
			
			if (arguments.Length > 0) {
				fileName = arguments[0];
				CompilationLog thisLog = new CompilationLog();
				
				SourceLineList lines = Preprocessor.Preprocess(fileName, thisLog);
				PINTBasic.Parser parser = new PINTBasic.Parser();
				PINTBasicApplication thisApplication = parser.Parse(thisLog, lines);
				parser = null;
				
				if (thisLog.CanContinue) {
					CodeGenerator generator = new CodeGenerator();
					SourceLineList newLines = generator.Generate(thisApplication, thisLog);
					generator = null;
					
					if (generateILfile) WriteILFile(newLines, thisLog, fileName + ".il");
					
					if (thisLog.CanContinue) {
						foreach (PINTBasicRoom sourceRoom in thisApplication.Rooms) {
							PINTRoomEntry thisRoom = Assembly.Parser.Parse(sourceRoom.ID, newLines, thisLog);
							if (thisLog.CanContinue) {
								thisRoom = Assembly.Analyzer.Analyze(thisRoom, thisLog);
								if (thisLog.CanContinue) {
									Assembly.PINTRoomFile thisFile = new Assembly.PINTRoomFile();
									thisFile.Save(thisRoom.RoomID + ".rm", thisRoom, thisLog);
									thisFile = null;
								}
							}
						}
					}
					//foreach (SourceLine thisLine in newLines) {
					//	Console.WriteLine(thisLine.LineNumber + "> " + thisLine.Code);
					//}	

					
				}
				
				/*
				PINTRoomEntry thisEntry = Parser.Parse(fileName, thisLog);
				if (thisEntry.Log.CanContinue) {
					thisEntry = Analyzer.Analyze(thisEntry);
					if (thisEntry.Log.CanContinue) {
						PINTRoomFile thisFile = new PINTRoomFile();
						thisFile.Save("000.rm", thisEntry);
						thisFile = null;
					}
				}
				*/
				
				foreach (CompilationLogEntry entry in thisLog.Entries) {
					Console.WriteLine(entry.Source + "> Line: " + entry.LineNumber + " Message: " + entry.Message);
				}
				
				/*
				
				Console.WriteLine("global variables **************************");
				foreach (PINTBasicByte thisByte in thisApplication.Variables) {
					Console.WriteLine(thisByte.Name);
				}
				
				Console.WriteLine("global PIC resources **************************");
				foreach (PINTBasicPic thisPic in thisApplication.Pics) {
					Console.WriteLine(thisPic.Name + " - FileName: " + thisPic.FileName);
				}
				
				foreach (PINTBasicRoom thisRoom in thisApplication.Rooms) {
					Console.WriteLine("Room informaation ********");
					
					Console.WriteLine("room variables ***********");
					foreach (PINTBasicByte thisRoomByte in thisRoom.Variables) {
						Console.WriteLine(thisRoomByte.Name);
					}
					
					Console.WriteLine("room PiC resources *******");
					foreach (PINTBasicPic thisRoomPic in thisRoom.Pics) {
						Console.WriteLine(thisRoomPic.Name + " - FileName: " + thisRoomPic.FileName);
					}	

					Console.WriteLine("Events *******");
					foreach (PINTBasicEvent thisEvent in thisRoom.Events) {
						Console.WriteLine(thisEvent.Name + " - Statements: " + thisEvent.Statements.Count);
						foreach (PINTBasicStatement thisStatement in thisEvent.Statements) {
							Console.WriteLine(thisStatement.GetType().Name);
						}
					}						
				}
				*/
				
			} else {
				Console.WriteLine("PINTCompiler " + VersionText() + " - no file name specified. ");
			}
		}
		
		public static void WriteILFile(SourceLineList lines, CompilationLog thisLog, string fileName) {
			thisLog.AddInformation("", 0, "Generating IL source file '" + fileName + "'");
			using (StreamWriter writer = new StreamWriter(File.Open(fileName, FileMode.Create))) {
				foreach (SourceLine thisLine in lines) {
					writer.WriteLine(thisLine.Code);
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
