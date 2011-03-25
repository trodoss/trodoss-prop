using System;
using System.Collections;
using System.IO;
using PINTCompiler.Utilities;
//****************************************
// PINT Compiler Assembler Parser
// 2010 - trodoss
//See end of file for terms of use.  
//***************************************
namespace PINTCompiler.Utilities {
	public class Preprocessor {
		
		private static void WriteError(string source, CompilationLog thisLog, int lineNumber, string message) {
			thisLog.CanContinue = false;
			thisLog.AddError(source, lineNumber, message);
		}
		
		private static void WriteWarning(string source, CompilationLog thisLog, int lineNumber, string message) {
			thisLog.AddWarning(source, lineNumber, message);
		}

		private static void WriteInformation(string source, CompilationLog thisLog, int lineNumber, string message) {
			thisLog.AddInformation(source, lineNumber, message);
		}
		
		public static SourceLineList Preprocess(string fileName, string filePath, CompilationLog thisLog) {
			SourceLineList lines = new SourceLineList();
			
			WriteInformation(fileName, thisLog, 0, "Pre-processing file file \"" + fileName + "\"");
			Hashtable constants = new Hashtable();
			
			using (StreamReader streamReader = new StreamReader(filePath +"\\"+ fileName)) {
				int lineNumber = 0;
				string lineData;
					
				while((lineData = streamReader.ReadLine()) != null) {
					lineNumber++;
					SourceLine thisLine = new SourceLine(fileName, lineNumber, "");
					
					//remove leading/trailing spaces
					lineData = lineData.Trim();
					
					//split data into unique elements to parse
					string[] elements = Tokenizer.TokenizeLine(lineData);			
					
					if (elements.Length > 0) {
						for (int i=0; i<elements.Length; i++) {
						
							string element = elements[i];
							bool canContinue = true;
							
							if (element.Length > 0) {
								//indicates that we have found a comment, so skip the element processing
								//Basic-style comments
								if (element.Substring(0,1) == "'") canContinue = false;
								
								//C-style comments
								if (element.Length > 1) {
									if (element.Substring(0,2) == "//") canContinue = false;
								}
							}
							
							if (canContinue) {
								switch (element.ToUpper()) {
									//non-label commands
									case "#DEFINE":
										if (i+1 < elements.Length) {
											i++;
											string constantName = elements[i].ToUpper();
											
											//check to make sure the name is unique
											if (constants[constantName] != null) {
												WriteError(fileName, thisLog, lineNumber, "constant named '" + constantName + "' has already been defined");
											} else {
												if (i+1 < elements.Length) {
													i++;
													string valueString = elements[i];
													try {
														int constantValue = Convert.ToInt32(valueString);
														constants.Add(constantName, constantValue);
													} catch {
														WriteError(fileName, thisLog, lineNumber, "unable to parse numeric value in constant declaration");
													}
												} else {
													WriteError(fileName, thisLog, lineNumber, "expected numeric value in constant delaration");
												}
											}
										} else {
											WriteError(fileName, thisLog, lineNumber, "identifer expected in constant declaration");
										}
										break;

									case "#INCLUDE":
										if (i+1 < elements.Length) {
											i++;
											string includeFilePath = elements[i];
											//replace quotes or brackets in the include
											includeFilePath = includeFilePath.Replace("\"","");
											includeFilePath = includeFilePath.Replace("<","");
											includeFilePath = includeFilePath.Replace(">","");
											
											SourceLineList includeLines = Preprocessor.Preprocess(includeFilePath, filePath, thisLog);
											if (includeLines.Count > 0) {
												foreach (SourceLine includeLine in includeLines) {
													lines.Add(includeLine);
												}
											}
											includeLines = null;
												
										} else {
											WriteError(fileName, thisLog, lineNumber, "file path expected in include declaration");
										}											
									break;
																											
									default:
										if (element != "") {
											//if this is one of the constants, replace with the constant value
											if (constants[element.ToUpper()] != null) element = constants[element.ToUpper()].ToString();
											
											//store non-comment commands 
											if (thisLine.Code != "") thisLine.Code += " ";
											thisLine.Code += element;
										}
										break;
								}
							}
							if (!canContinue) break;
						}
					}
					if (thisLine.Code != "") {
						lines.Add(thisLine);
					}
					thisLine = null;
				}
			}
			return lines;
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
