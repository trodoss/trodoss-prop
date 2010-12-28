using System;
using System.Collections;
using System.IO;
using PINTCompiler.Utilities;
//****************************************
// PINT Compiler Assembler Parser
// 2010 - trodoss
//See end of file for terms of use.  
//***************************************
namespace PINTCompiler.Assembly {
	public class Parser {
		
		private static void WriteError(CompilationLog thisLog, SourceLine thisLine, string message) {
			thisLog.CanContinue = false;
			thisLog.AddError(thisLine.Source, thisLine.LineNumber, message);
		}
		
		private static void WriteWarning(CompilationLog thisLog, SourceLine thisLine, string message) {
			thisLog.AddWarning(thisLine.Source, thisLine.LineNumber, message);
		}

		private static void WriteInformation(CompilationLog thisLog,  string message) {
			thisLog.AddInformation("", 0, message);
		}

		private static void WriteInformation(CompilationLog thisLog, SourceLine thisLine, string message) {
			thisLog.AddInformation(thisLine.Source, thisLine.LineNumber, message);
		}
		
		
		private static int ResolveAsConstantOrNumber(PINTRoomEntry thisRoom, CompilationLog thisLog, SourceLine thisLine, string value) {
			int returnValue = -1;
			value = value.Replace(",","");
			try {
				returnValue = Convert.ToInt32(value);
			} catch {
				if (thisRoom.Constants[value] != null) {
					returnValue = Convert.ToInt32(thisRoom.Constants[value]);
				} else {
					WriteError(thisLog, thisLine, "could not parse '" + value + "' as a constant or numeric value in statement; constants must be declared before being referenced.");
				}
			}
			return returnValue;
		}
		
		private static ComparisonType ResolveAsComparisonType(CompilationLog thisLog, SourceLine thisLine, string thisValue) {
			try {
				
				ComparisonType returnType = (ComparisonType) Enum.Parse(typeof(ComparisonType), thisValue, true);
				return returnType;
			} catch {
				WriteError(thisLog, thisLine, "unable to resolve '"+thisValue+"' as a valid comparison type");
				return ComparisonType.Undefined;
			}			
		}

		private static SetType ResolveAsSetType(CompilationLog thisLog, SourceLine thisLine, string thisValue) {
			try {
				
				SetType returnType = (SetType) Enum.Parse(typeof(SetType), thisValue, true);
				return returnType;
			} catch {
				WriteError(thisLog, thisLine, "unable to resolve '"+thisValue+"' as a valid set type");
				return SetType.Undefined;
			}			
		}		

		private static MathType ResolveAsMathType(CompilationLog thisLog, SourceLine thisLine, string thisValue) {
			try {
				
				MathType returnType = (MathType) Enum.Parse(typeof(MathType), thisValue, true);
				return returnType;
			} catch {
				WriteError(thisLog, thisLine, "unable to resolve '"+thisValue+"' as a valid math type");
				return MathType.Undefined;
			}			
		}		
		
		public static PINTRoomEntry Parse(int roomID, SourceLineList lines, CompilationLog thisLog) {
			PINTRoomEntry thisRoom = new PINTRoomEntry(roomID, thisLog);
			WriteInformation(thisLog, "Parsing source lines for roomID: " + roomID.ToString());
		
			int lineNumber = 0;
			string lineData;
			
			string thisLabel = "";
			
			foreach (SourceLine thisLine in lines) {
			//while((lineData = streamReader.ReadLine()) != null) {
				lineData = thisLine.Code;
				lineNumber++;
					
				//remove leading/trailing spaces
				lineData = lineData.Trim();
					
				//split data into unique elements to parse
				string[] elements = Tokenizer.TokenizeLine(lineData);			
					
				//used in event parsing
				thisLabel = "";
					
				if (elements.Length > 0) {
					for (int i=0; i<elements.Length; i++) {
					
						string element = elements[i].ToUpper();
						bool canContinue = true;
							
						if (element.Length > 0) {
							//indicates that we have found a comment, so skip the element processing
							if (element.Substring(0,1) == "'") canContinue = false;
						}
							
						if (canContinue) {
							switch (element) {
								//non-label commands
								case "CONST":
									if (i+1 < elements.Length) {
										i++;
										string constantName = elements[i].ToUpper();
										
										//check to make sure the name is unique
										if (thisRoom.Constants[constantName] != null) {
											WriteError(thisLog, thisLine, "constant named '" + constantName + "' has already been defined");
										} else {
											if (i+1 < elements.Length) {
												i++;
												if (elements[i] == "=") {
													if (i+1 < elements.Length) {
														i++;
														string valueString = elements[i];
														try {
															int constantValue = Convert.ToInt32(valueString);
															thisRoom.Constants.Add(constantName, constantValue);
														} catch {
															WriteError(thisLog, thisLine, "unable to parse numeric value in constant declaration");
														}
													} else {
														WriteError(thisLog, thisLine, "expected numeric value in constant delaration");
													}
												} else {
													WriteError(thisLog, thisLine, "expected '=' in constant declaration ");
												}
											} else {
												WriteError(thisLog, thisLine, "expected '=' in constant declaration");
											}
										}
									} else {
										WriteError(thisLog, thisLine, "identifer expected in constant declaration");
									}
									break;

								case "BACKDROP":
									if (i+1 < elements.Length) {
										i++;
										string backdropIDText = elements[i].ToUpper();
										int backdropID = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, backdropIDText);
											if (i+1 < elements.Length) {
											i++;
											string backdropFilePath = elements[i];
											backdropFilePath = backdropFilePath.Replace("\"","");
											thisRoom.Backdrop = new PINTBackdropEntry(backdropID, backdropFilePath);
											
										} else {
											WriteError(thisLog, thisLine, "backdrop file path expected in backdrop declaration");
										}
										
									} else {
										WriteError(thisLog, thisLine, "backdrop identifier expected in backdrop declaration");
									}
								break;
								
								case "HOTSPOT":
									if (i+1 < elements.Length) {
										i++;
										string hotspotIDText = elements[i].ToUpper();
										int hotspotID = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, hotspotIDText);
										if (i+1 < elements.Length) {
											i++;
											string hotspotXText = elements[i].ToUpper();
											int hotspotX = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, hotspotXText);
											if (i+1 < elements.Length) {
												i++;
												string hotspotYText = elements[i].ToUpper();
												int hotspotY = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, hotspotYText);
												if (i+1 < elements.Length) {
													i++;
													string hotspotWText = elements[i].ToUpper();
													int hotspotW = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, hotspotWText);
													if (i+1 < elements.Length) {
														i++;
														string hotspotHText = elements[i].ToUpper();
														int hotspotH = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, hotspotHText);
														
														thisRoom.Hotspots.Add(new PINTHotspotEntry(hotspotID, hotspotX, hotspotY, hotspotW, hotspotH));
													} else {
														WriteError(thisLog, thisLine, "height value expected in hotspot declaration");
													}
												}else {
													WriteError(thisLog, thisLine, "width value expected in hotspot declaration");
												}
											} else {
												WriteError(thisLog, thisLine, "y value expected in hotspot declaration");
											}
										}else{
											WriteError(thisLog, thisLine, "x value expected in hotspot declaration");
										}
									} else {
										WriteError(thisLog, thisLine, "hotspot identifier expected in hotspot declaration");
									}
								break;
								
								case "PIC":
									if (i+1 < elements.Length) {
										i++;
										string picIDText = elements[i].ToUpper();
										int picID = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, picIDText);
										if (i+1 < elements.Length) {
											i++;
											string picFilePath = elements[i];
											picFilePath = picFilePath.Replace("\"","");
											thisRoom.Pics.Add (new PINTPicEntry(picID, picFilePath));
											
										} else {
											WriteError(thisLog, thisLine, "pic file path expected in pic declaration");
										}
										
									} else {
										WriteError(thisLog, thisLine, "pic identifier expected in pic declaration");
									}
								break;									

								case "TEXT":
									if (i+1 < elements.Length) {
										i++;
										string textIDText = elements[i].ToUpper();
										int textID = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, textIDText);
										
										if (i+1 < elements.Length) {
											i++;
											string text = elements[i].ToUpper();
											PINTString thisString = new PINTString(textID, text);
											thisString.Source = thisLine;
											thisRoom.Strings.Add(thisString);
											thisString = null;
										} else {
											WriteError(thisLog, thisLine, "text data expected in text declaration");
										}
									} else {
										WriteError(thisLog, thisLine, "text identifier expected in text declaration");
									}
								break;	
									
								case "VARIABLE_SET":
									if (i+1 < elements.Length) {
										i++;
										string variableIDText = elements[i].ToUpper();
										int variableID = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, variableIDText);

										if (i+1 < elements.Length) {
											i++;
											string variableSetTypeText = elements[i].ToUpper();
											SetType setType = ResolveAsSetType(thisLog, thisLine, variableSetTypeText);
										
											if (i+1 < elements.Length) {
												i++;
												string variableValueText = elements[i].ToUpper();
												int variableValue = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, variableValueText);
												
												VariableSet thisVariableSet = new VariableSet(variableID, variableValue, setType);
												if (thisLabel != "") thisVariableSet.Label = thisLabel;
												thisVariableSet.Source = thisLine;
												thisRoom.Commands.Add(thisVariableSet);
												thisVariableSet = null;
											} else {
												WriteError(thisLog, thisLine, "variable value expected in variable set declaration");
											}
										} else {
											WriteError(thisLog, thisLine, "variable set type expected in variable set declaration");
										}
									} else {
										WriteError(thisLog, thisLine, "variable identifier expected in variable set declaration");
									}
								break;
								
								case "VARIABLE_MATH":
									if (i+1 < elements.Length) {
										i++;
										string variableIDText = elements[i].ToUpper();
										int variableID = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, variableIDText);

										if (i+1 < elements.Length) {
											i++;
											string variableSetTypeText = elements[i].ToUpper();
											MathType mathType = ResolveAsMathType(thisLog, thisLine, variableSetTypeText);
										
											if (i+1 < elements.Length) {
												i++;
												string variableValueText = elements[i].ToUpper();
												int variableValue = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, variableValueText);
												
												VariableMath thisVariableMath = new VariableMath(variableID, variableValue, mathType);
												if (thisLabel != "") thisVariableMath.Label = thisLabel;
												thisVariableMath.Source = thisLine;
												thisRoom.Commands.Add(thisVariableMath);
												thisVariableMath = null;
											} else {
												WriteError(thisLog, thisLine, "variable value expected in variable math declaration");
											}
										} else {
											WriteError(thisLog, thisLine, "variable math type expected in variable math declaration");
										}
									} else {
										WriteError(thisLog, thisLine, "variable identifier expected in variable math declaration");
									}
								break;								
								
								case "VARIABLE_TEST":
									if (i+1 < elements.Length) {
										i++;
										string testVariableIDText = elements[i].ToUpper();
										int testVariableID = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, testVariableIDText);

										if (i+1 < elements.Length) {
											i++;
											string testVariableComparisonType = elements[i].ToUpper();
											ComparisonType testType = ResolveAsComparisonType(thisLog, thisLine, testVariableComparisonType);
										
										
											if (i+1 < elements.Length) {
												i++;
												string testVariableValueText = elements[i].ToUpper();
												int testVariableValue = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, testVariableValueText);
													if (i+1 < elements.Length) {
													i++;
													string testNotMetJumpToText = elements[i].ToUpper();
																									
													VariableTest thisVariableTest = new VariableTest(testVariableID, testVariableValue, testType, testNotMetJumpToText);
													thisVariableTest.Source = thisLine;
													if (thisLabel != "") thisVariableTest.Label = thisLabel;
													thisRoom.Commands.Add(thisVariableTest);
													thisVariableTest = null;
												} else {
													WriteError(thisLog, thisLine, "jump to label expected in variable test declaration");
												}
											} else {
												WriteError(thisLog, thisLine, "variable value expected in variable test declaration");
											}
										} else {
											WriteError(thisLog, thisLine, "variable comparison type expected in variable test declaration");
										}
									} else {
										WriteError(thisLog, thisLine, "variable identifier expected in variable test declaration");
									}
								break;	

								case "CODE_GOTO":
									if (i+1 < elements.Length) {
										i++;
										string goToJumpToText = elements[i].ToUpper();
										
										CodeGoto thisGoto = new CodeGoto(goToJumpToText);
										if (thisLabel != "") thisGoto.Label = thisLabel;
										thisGoto.Source = thisLine;
										thisRoom.Commands.Add(thisGoto);
										thisGoto = null;
									} else {
										WriteError(thisLog, thisLine, "jump to label not specified in code goto declaration");
									}
									break;

								case "ROOM_LOAD":
									if (i+1 < elements.Length) {
										i++;
										string roomIDText = elements[i].ToUpper();
										int thisRoomID = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, roomIDText);
										
										RoomLoad thisRoomLoad = new RoomLoad(thisRoomID);
										if (thisLabel != "") thisRoomLoad.Label = thisLabel;
										thisRoomLoad.Source = thisLine;
										thisRoom.Commands.Add(thisRoomLoad);
										thisRoomLoad = null;
									} else {
										WriteError(thisLog, thisLine, "room identifier not specified in room load declaration");
									}
									break;		
									
								case "HOTSPOT_TEST":
									if (i+1 < elements.Length) {
										i++;
										string hotspotIDText = elements[i].ToUpper();
										int hotspotID = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, hotspotIDText);

										if (i+1 < elements.Length) {
											i++;
											
											string hotspotNotMetJumpToText = elements[i].ToUpper();
											
											HotspotTest thisHotspotTest = new HotspotTest(hotspotID, hotspotNotMetJumpToText);
											if (thisLabel != "") thisHotspotTest.Label = thisLabel;
											thisHotspotTest.Source = thisLine;
											thisRoom.Commands.Add(thisHotspotTest);
											thisHotspotTest = null;
										} else {
											WriteError(thisLog, thisLine, "hotspot not met jump to label not specified in hotspot test declaration");
										}
									} else {
										WriteError(thisLog, thisLine, "hotspot identifier not specified in hotspot test declaration");
									}
									break;		

								case "PIC_LOAD":
									if (i+1 < elements.Length) {
										i++;
										string loadPicIDText = elements[i].ToUpper();
										int loadPicID = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, loadPicIDText);
										
										if (i+1 < elements.Length) {
											i++;
											string loadPicXText = elements[i].ToUpper();
											int loadPicX = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, loadPicXText);

											if (i+1 < elements.Length) {
												i++;
												string loadPicYText = elements[i].ToUpper();
												int loadPicY = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, loadPicYText);
												
												PicLoad thisPicLoad = new PicLoad(loadPicID, loadPicX, loadPicY);
												thisPicLoad.Source = thisLine;
												if (thisLabel != "") thisPicLoad.Label = thisLabel;
												thisRoom.Commands.Add(thisPicLoad);
												thisPicLoad = null;
											} else {
												WriteError(thisLog, thisLine, "y position value not specified in pic load declaration");
											}
										} else {
											WriteError(thisLog, thisLine, "x position value not specified in pic load declaration");
										}
									} else {
										WriteError(thisLog, thisLine, "pic identifier not specified in pic load declaration");
									}
									break;	
									
								case "PIC_HIDE":
									if (i+1 < elements.Length) {
										i++;
										string hidePicIDText = elements[i].ToUpper();
										int hidePicID = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, hidePicIDText);
										
										PicHide thisPicHide = new PicHide(hidePicID);
										if (thisLabel != "") thisPicHide.Label = thisLabel;
										thisPicHide.Source = thisLine;
										thisRoom.Commands.Add(thisPicHide);
										thisPicHide = null;
									} else {
										WriteError(thisLog, thisLine, "pic identifier not specified in pic hide declaration");
									}
								break;									
																
								case "SAY":
									if (i+1 < elements.Length) {
										i++;
										string sayTextIDText = elements[i].ToUpper();
										int sayTextID = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, sayTextIDText);
											
										Say thisSay = new Say(sayTextID);
										if (thisLabel != "") thisSay.Label = thisLabel;
										thisSay.Source = thisLine;
										thisRoom.Commands.Add(thisSay);
										thisSay = null;
									} else {
										WriteError(thisLog, thisLine, "text identifier expected in say declaration");
									}
									break;		

								case "EGO_LOAD":
									if (i+1 < elements.Length) {
										i++;
										string loadEgoXText = elements[i].ToUpper();
										int loadEgoX = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, loadEgoXText);
										
										if (i+1 < elements.Length) {
											i++;
											string loadEgoYText = elements[i].ToUpper();
											int loadEgoY = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, loadEgoYText);

											if (i+1 < elements.Length) {
												i++;
												string loadEgoFacingText = elements[i].ToUpper();
												int loadEgoFacing = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, loadEgoFacingText);
												
												EgoLoad thisEgoLoad = new EgoLoad(loadEgoX, loadEgoY, loadEgoFacing);
												thisEgoLoad.Source = thisLine;
												if (thisLabel != "") thisEgoLoad.Label = thisLabel;
												thisRoom.Commands.Add(thisEgoLoad);
												thisEgoLoad = null;
											} else {
												WriteError(thisLog, thisLine, "facing value not specified in ego load declaration");
											}
										} else {
											WriteError(thisLog, thisLine, "y position value not specified in ego load declaration");
										}
									} else {
										WriteError(thisLog, thisLine, "x position value not specified in ego load declaration");
									}
									break;	
									
								case "INVENTORY_ADD":
									if (i+1 < elements.Length) {
										i++;
										string addInventoryIDText = elements[i].ToUpper();
										int addInventoryID = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, addInventoryIDText);
										
										InventoryAdd thisInventoryAdd = new InventoryAdd(addInventoryID);
										if (thisLabel != "") thisInventoryAdd.Label = thisLabel;
										thisInventoryAdd.Source = thisLine;
										thisRoom.Commands.Add(thisInventoryAdd);
										thisInventoryAdd = null;
									} else {
										WriteError(thisLog, thisLine, "inventory identifier expected in inventory add declaration");
									}
									break;	

								case "INVENTORY_TEST":
									if (i+1 < elements.Length) {
										i++;
										string testInventoryIDText = elements[i].ToUpper();
										int testInventoryID = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, testInventoryIDText);
										
										if (i+1 < elements.Length) {
											i++;
											string testNotMetJumpToText = elements[i].ToUpper();
		
											InventoryTest thisInventoryTest = new InventoryTest(testInventoryID, testNotMetJumpToText);
											thisInventoryTest.Source = thisLine;
											if (thisLabel != "") thisInventoryTest.Label = thisLabel;
											thisRoom.Commands.Add(thisInventoryTest);
											thisInventoryTest = null;
												
										} else {
											WriteError(thisLog, thisLine, "not met jump to value expected in inventory test declaration");
										}
									} else {
										WriteError(thisLog, thisLine, "inventory identifier expected in inventory test declaration");
									}
									break;										

								case "INVENTORY_REMOVE":
									if (i+1 < elements.Length) {
										i++;
										string removeInventoryIDText = elements[i].ToUpper();
										int removeInventoryID = ResolveAsConstantOrNumber(thisRoom, thisLog, thisLine, removeInventoryIDText);
										
										InventoryRemove thisInventoryRemove = new InventoryRemove(removeInventoryID);
										if (thisLabel != "") thisInventoryRemove.Label = thisLabel;
										thisInventoryRemove.Source = thisLine;
										thisRoom.Commands.Add(thisInventoryRemove);
										thisInventoryRemove = null;
									} else {
										WriteError(thisLog, thisLine, "inventory identifier expected in inventory remove declaration");
									}
									break;										
									
								case "EVENT_END":											
									EventEnd thisEventEnd = new EventEnd();
									if (thisLabel != "") thisEventEnd.Label = thisLabel;
									thisEventEnd.Source = thisLine;
									thisRoom.Commands.Add(thisEventEnd);
									thisEventEnd = null;
									break;	
									
								default:
									if (element != "") {
										if (element.Substring(element.Length-1, 1) == ":") {
											//trim and store the label
											thisLabel = element.Substring(0, element.Length-1);
										} else {
											WriteError(thisLog, thisLine, "unrecognized command '" + element + "'");
											canContinue = false;
										}
									}
									break;
							}
						}
						if (!canContinue) break;
					}
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
