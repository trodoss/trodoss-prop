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
		private static string VersionText() { return "v0.4"; }
			
			
		//*********** P U B L I C   F U N C T I O N S  ( M E T H O D S ) ******
		/// <sumary>
		/// Main (entrypoint)	
		/// </sumary>
		public static void Main(string[] arguments) {
			bool generateILfile = false;
			bool showHelp = false;
			bool canContinue = true;
			int errorLevel = 1;
			string sourceFilePath = "";
			string outputFilePath = "";
			
			if (arguments.Length > 0) {
				CompilationLog thisLog = new CompilationLog();
								
				//examine the rest of the arguments being specified to determine what flags need to be set
				if (arguments.Length > 0) {
					foreach (string thisArgument in arguments) {
	
						if (thisArgument.Length > 0) {
							if (thisArgument.Substring(0,1) == "/") {
								if (thisArgument.Length > 1) {
									switch (thisArgument.Substring(1,1).ToLower()) {
										case "i":
											generateILfile = true;
											break;
											
										case "e":
											if (thisArgument.Length > 2) {
												string errorLevelText = thisArgument.Substring(2,1);
												try {
													errorLevel = Convert.ToInt32(errorLevelText);
													if ((errorLevel < 0) || (errorLevel > 2)) canContinue = false;
												} catch {
													canContinue = false;
												}
											} else {
												canContinue = false;
											}
											break;
											
										case "?":
											showHelp = true;
											break;
											
										default:
											canContinue = false;
											break;
									}
								} else {
									canContinue = false;
								}
							} else {
								if (sourceFilePath == "") {
									sourceFilePath = thisArgument;
								} else {
									if (outputFilePath == "") {
										outputFilePath = thisArgument;
									} else {
										//more than that, and it is an error
										canContinue = false;
									}
								}
							}
						} else {
							canContinue = false;
						}
						
						if (!canContinue) {
							Console.WriteLine("PINTCompiler " + VersionText() + " - invalid argument '" + thisArgument + "' ");
							ShowHelp();
							break;
						}
					}
				}
				
				//if we have passed the filename/parameter checks, try to compile the files with the specified settings
				if (showHelp) {
					ShowHelp();
				} else {
					if (canContinue) {
						string sourceRootPath = "";
						string outputRootPath = "";
						
						if (!Path.IsPathRooted(sourceFilePath)) {
							sourceRootPath = Directory.GetCurrentDirectory();
							sourceRootPath += "\\" + Path.GetDirectoryName(sourceFilePath);
						} else {
							sourceRootPath = Path.GetDirectoryName(sourceFilePath);
						}

						if (!Path.IsPathRooted(outputFilePath)) {
							outputRootPath = Directory.GetCurrentDirectory();
							if (outputFilePath != "") outputRootPath += "\\" + Path.GetDirectoryName(outputFilePath);
						} else {
							outputRootPath = Path.GetDirectoryName(outputRootPath);
						}					
						
						//determine the filenames for the files being compiled
						DirectoryInfo di = new DirectoryInfo(sourceRootPath);
						FileInfo[] fileInfo = di.GetFiles(Path.GetFileName(sourceFilePath));
				
						//compile each of the  file(s) being specified
						foreach (FileInfo fi in fileInfo) {
							CompileFile (fi.Name, sourceRootPath, outputRootPath + "\\", generateILfile, thisLog);
							if (!thisLog.CanContinue) break;
						}
					}
					
					//list out the log entries from compilation
					ListLogEntries(thisLog, errorLevel);
				}
								
			} else {
				Console.WriteLine("PINTCompiler " + VersionText() + " - no file name specified. ");
				ShowHelp();
			}
		}
		
		public static void CompileFile(string fileName, string sourceFilePath, string outputFilePath, bool generateILFile, CompilationLog thisLog) {
				SourceLineList lines = Preprocessor.Preprocess(fileName, sourceFilePath, thisLog);
				PINTBasic.Parser parser = new PINTBasic.Parser();
				PINTBasicApplication thisApplication = parser.Parse(thisLog, sourceFilePath, lines);
				parser = null;
				
				if (thisLog.CanContinue) {
					CodeGenerator generator = new CodeGenerator();
					SourceLineList newLines = generator.Generate(thisApplication, thisLog);
					generator = null;
								
					if (generateILFile) WriteILFile(newLines, thisLog, outputFilePath + fileName + ".il");
					
					if (thisLog.CanContinue) {
						foreach (PINTBasicRoom sourceRoom in thisApplication.Rooms) {
							PINTRoomEntry thisRoom = Assembly.Parser.Parse(sourceRoom.ID, newLines, thisLog);
							if (thisLog.CanContinue) {
								thisRoom = Assembly.Analyzer.Analyze(thisRoom, thisLog);
								if (thisLog.CanContinue) {
									Assembly.PINTRoomFile thisFile = new Assembly.PINTRoomFile();
									thisFile.Save(outputFilePath + thisRoom.RoomID + ".RM", thisRoom, thisLog);
									thisFile = null;
									
									//special processing for roomID 0 (startup room) - generate the item file and process MIDI files
									if (thisRoom.RoomID == 0) {
										Assembly.PINTItemFile thisItemFile = new Assembly.PINTItemFile();
										thisItemFile.Save(outputFilePath + "0.IT", thisRoom.Items, thisLog);
										thisItemFile = null;
										
										Assembly.PINTMIDIFileWriter thisMIDIWriter = new Assembly.PINTMIDIFileWriter();
										thisMIDIWriter.Process(thisRoom.Musics, outputFilePath, thisLog);
										thisMIDIWriter = null;
									}
								}
							}
						}
					}			
				}		
		}
		
		public static void ListLogEntries(CompilationLog thisLog, int errorLevel) {
			foreach (CompilationLogEntry entry in thisLog.Entries) {
				bool showEntry = false;
				if (entry.Level == CompilationLog.ErrorLevel.Error) showEntry = true;
				if ((errorLevel > 0) && (entry.Level == CompilationLog.ErrorLevel.Warning)) showEntry = true;
				if ((errorLevel > 1) && (entry.Level == CompilationLog.ErrorLevel.Information)) showEntry = true;
				
				if (showEntry) {					
					string errorText = "";
					
					if (entry.Source != "") errorText += entry.Source + ">";
					if (entry.LineNumber > 0) {
						errorText += " Line: " + entry.LineNumber + " - " + entry.Message;
					} else {
						if (errorText != "") errorText += " ";
						errorText += entry.Message;
					}
						
					Console.WriteLine(errorText);
				}
			}
		}
		
		public static void ShowHelp() {
			Console.WriteLine("");
			Console.WriteLine("Usage: PINTCompiler.exe [source_path] ([output_path] [options])");
			Console.WriteLine("[source_path] - Source file path");
			Console.WriteLine("[output_path] - (Optional) Output file path");
			Console.WriteLine("");
			Console.WriteLine("Additional options:");
			Console.WriteLine("");
			Console.WriteLine("/? - Show command line help information");
			Console.WriteLine("/e - Error level:");
			Console.WriteLine("       0 = Errors Only");
			Console.WriteLine("       1 = Warnings/Errors [default]");
			Console.WriteLine("       2 = Info/Warnings/Errors");
			Console.WriteLine("/i - Generate intermediate listing(s)");
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
