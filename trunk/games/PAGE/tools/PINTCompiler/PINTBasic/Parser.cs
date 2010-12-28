using System;
using System.Collections;
using System.IO;
using PINTCompiler.Utilities;
//****************************************
// PINT Compiler Assembler Parser
// 2010 - trodoss
//See end of file for terms of use.  
//***************************************
namespace PINTCompiler.PINTBasic {
	public class Parser {
		private PINTBasicApplication thisApplication = null;
		private	PINTBasicRoom thisRoom = null;
		private bool canContinue = true;
		private bool endOfSource = false;
		private int i = 0;
		private int currentLine = 0;
		
		private void WriteError(CompilationLog thisLog, SourceLine thisLine, string message) {
			thisLog.CanContinue = false;
			thisLog.AddError(thisLine.Source, thisLine.LineNumber, message);
		}
		
		private void WriteWarning(CompilationLog thisLog, SourceLine thisLine, string message) {
			thisLog.AddWarning(thisLine.Source, thisLine.LineNumber, message);
		}

		private void WriteInformation(CompilationLog thisLog, SourceLine thisLine, string message) {
			thisLog.AddInformation(thisLine.Source, thisLine.LineNumber, message);
		}
			
		public EventType ResolveAsEventType(string thisElement, CompilationLog thisLog, SourceLine thisLine) {
			try {
				
				EventType returnType = (EventType) Enum.Parse(typeof(EventType), thisElement, true);
				return returnType;
			} catch {
				WriteError(thisLog, thisLine, "unable to resolve '"+thisElement+"' as a valid event type");
				return EventType.Undefined;
			}
		}
		
		public SourceLine GetNextLine(SourceLineList lines) {
			if (currentLine < lines.Count) {
				SourceLine thisLine = lines[currentLine];
				currentLine++;
				return thisLine;
			} else {
				endOfSource = true;
				return null;
			}
		}
		
		public PINTBasicExpression ResolveAsExpressionElement (string thisElement, CompilationLog thisLog, SourceLine thisLine) {
			PINTBasicExpression returnExpression = null;
			int thisValue = -1;
			//check first to see if this is a literal
			try {
				thisValue = Convert.ToInt32(thisElement);
				returnExpression = new LiteralExpression(thisValue);
			} catch {
				thisElement = thisElement.ToUpper();
				//try to resolve the element as a constant
				PINTBasicConstant testConstant = thisApplication.Constants.FindByName(thisElement);
				if (testConstant != null) {
					//found as a constant expression
					returnExpression = new ConstantExpression(testConstant.ID);
					testConstant = null;
				} else {
					PINTBasicByte testByte = thisApplication.Variables.FindByName(thisElement);
					if (testByte != null) {
						returnExpression = new GlobalVariableExpression(testByte.ID);
						testByte = null;
					} else {
						testByte = thisRoom.Variables.FindByName(thisElement);
						if (testByte != null) {
							returnExpression = new RoomVariableExpression(testByte.ID);
							testByte = null;
						} else {
							WriteError(thisLog, thisLine, "unable to resolve '"+thisElement+"' as a valid expression element");
						}
					}
				}
				
			}
			return returnExpression;
		}
		
		public MethodExpression ResolveAsMethodExpression(string thisElement, string[] elements, CompilationLog thisLog, SourceLine thisLine) {
			MethodExpression returnExpression = null;
			string[] methodElements = thisElement.Split('.');
			
			if (methodElements.Length > 0) {
				switch (methodElements[0].ToUpper()) {
					case "EGO":
						if (methodElements.Length > 1) {
							if (methodElements[1].ToUpper() == "LOAD") {
								//EGO.Load(ix, y, facing)
								if (ExpectedKeyword(elements, "(", methodElements[1].ToUpper(), thisLog, thisLine)) {
									int xValue = ExpectedConstantOrNumber(elements, methodElements[1].ToUpper(), thisLog, thisLine);
									if (xValue > -1) {
										//look for X value
										if (ExpectedKeyword(elements, ",", methodElements[1].ToUpper(), thisLog, thisLine)) {
											//look for Y value
											int yValue = ExpectedConstantOrNumber(elements, methodElements[1].ToUpper(), thisLog, thisLine);
											if (yValue > -1) {
												if (ExpectedKeyword(elements, ",", methodElements[1].ToUpper(), thisLog, thisLine)) {
													//look for facing value
													int facingValue = ExpectedConstantOrNumber(elements, methodElements[1].ToUpper(), thisLog, thisLine);
													if (facingValue > -1) {
														if (ExpectedKeyword(elements, ")", methodElements[1].ToUpper(), thisLog, thisLine)) {
															returnExpression = new EgoLoadExpression(xValue, yValue, facingValue);
														}
													}
												}
											}
										}
									}
								}
							} else {
								WriteError(thisLog, thisLine, "Ego object does not contain the method '"+methodElements[1].ToUpper()+"'");
							}
						}
						break;
						
					case "INVENTORY":
						if (methodElements.Length > 1) {
							switch (methodElements[1].ToUpper()) {
								case "CONTAINS":
									//INVENTORY.Contains(item#)
									if (ExpectedKeyword(elements, "(", methodElements[1].ToUpper(), thisLog, thisLine)) {
										int containsItemID = ExpectedConstantOrNumber(elements, methodElements[1].ToUpper(), thisLog, thisLine);
										if (containsItemID > -1) {
											if (ExpectedKeyword(elements, ")", methodElements[1].ToUpper(), thisLog, thisLine)) {
												returnExpression = new InventoryContainsExpression (containsItemID);
											}
										}
									}
									break;
									
								case "ADD":
									//INVENTORY.Add(item#)
									if (ExpectedKeyword(elements, "(", methodElements[1].ToUpper(), thisLog, thisLine)) {
										int addItemID = ExpectedConstantOrNumber(elements, methodElements[1].ToUpper(), thisLog, thisLine);
										if (addItemID > -1) {
											if (ExpectedKeyword(elements, ")", methodElements[1].ToUpper(), thisLog, thisLine)) {
												returnExpression = new InventoryAddExpression (addItemID);
											}
										}

									}									
									break;
									
								case "REMOVE":
									//INVENTORY.Remove(item#)
									if (ExpectedKeyword(elements, "(", methodElements[1].ToUpper(), thisLog, thisLine)) {
										int removeItemID = ExpectedConstantOrNumber(elements, methodElements[1].ToUpper(), thisLog, thisLine);
										if (removeItemID > -1) {
											if (ExpectedKeyword(elements, ")", methodElements[1].ToUpper(), thisLog, thisLine)) {
												returnExpression = new InventoryRemoveExpression (removeItemID);
											}
										}
									}									
									break;
									
								default:
									WriteError(thisLog, thisLine, "Inventory object does not contain the method '"+methodElements[1].ToUpper()+"'");
									break;
							}
						}					
						break;
						
					case "ROOM":
						if (methodElements.Length > 1) {
							if (methodElements[1].ToUpper() == "LOAD") {
								//Room.Load(room#)
								if (ExpectedKeyword(elements, "(", methodElements[1].ToUpper(), thisLog, thisLine)) {
										int roomID = ExpectedConstantOrNumber(elements, methodElements[1].ToUpper(), thisLog, thisLine);
										if (roomID > -1) {
											if (ExpectedKeyword(elements, ")", methodElements[1].ToUpper(), thisLog, thisLine)) {
												returnExpression = new RoomLoadExpression (roomID);
											}
										}
								}

							} else {
								WriteError(thisLog, thisLine, "Room object does not contain the method '"+methodElements[1].ToUpper()+"'");
							}
						}					
						break;
						
					default:
						//check to see if this is a PIC or a Hotspot reference
						PINTBasicPic thisPic = thisApplication.Pics.FindByName(methodElements[0].ToUpper());
						if (thisPic != null) {
							if (methodElements.Length > 1) {
								switch (methodElements[1].ToUpper()) {
									case "LOAD":
										if (ExpectedKeyword(elements, "(", methodElements[1].ToUpper(), thisLog, thisLine)) {
											int picXValue = ExpectedConstantOrNumber(elements, methodElements[1].ToUpper(), thisLog, thisLine);
											if (picXValue > -1) {
												//look for X value
												if (ExpectedKeyword(elements, ",", methodElements[1].ToUpper(), thisLog, thisLine)) {
													//look for Y value
													int picYValue = ExpectedConstantOrNumber(elements, methodElements[1].ToUpper(), thisLog, thisLine);
													if (picYValue > -1) {
														if (ExpectedKeyword(elements, ")", methodElements[1].ToUpper(), thisLog, thisLine)) {
															returnExpression = new PicLoadExpression(thisPic.ID, picXValue, picYValue);
														}
													}
												}
											}
										}
										break;
										
									case "HIDE":
										if (ExpectedKeyword(elements, "(", methodElements[1].ToUpper(), thisLog, thisLine)) {
											if (ExpectedKeyword(elements, ")", methodElements[1].ToUpper(), thisLog, thisLine)) {
												returnExpression = new PicHideExpression(thisPic.ID);
											}
										}
										break;
								}
							} else {
								WriteError(thisLog, thisLine, "Pic object reference does not contain a method");
							}
						} else {
							PINTBasicHotspot thisHotspot = thisRoom.Hotspots.FindByName(methodElements[0].ToUpper());
							if (thisHotspot != null) {
								if (methodElements.Length > 1) {
									//Hotspot.ContainsEgo() method
									if (methodElements[1].ToUpper() == "CONTAINSEGO") {		
										if (ExpectedKeyword(elements, "(", methodElements[1].ToUpper(), thisLog, thisLine)) {
											if (ExpectedKeyword(elements, ")", methodElements[1].ToUpper(), thisLog, thisLine)) {
												returnExpression = new HotspotContainsEgoExpression(thisHotspot.ID);
											}
										}
									} else {
										WriteError(thisLog, thisLine, "Hotspot object does not contain the method '"+methodElements[1].ToUpper()+"'");
									}
								}
							} else {
								WriteError(thisLog, thisLine, "Unable to resolve '"+methodElements[0].ToUpper()+"' as a valid object reference");
							}
						}
						thisPic = null;
						break;
				}
			}
			
			return returnExpression;
		}
		
		public int ResolveAsConstantOrNumber(string thisElement, CompilationLog thisLog, SourceLine thisLine) {
			int returnValue = -1;
			try {
				returnValue = Convert.ToInt32(thisElement);
			} catch {
				PINTBasicConstant testConstant = thisApplication.Constants.FindByName(thisElement);
				if (testConstant != null) {
					returnValue = testConstant.Value;
				} else {
					WriteError(thisLog, thisLine, "unable to resolve '"+thisElement+"' as a constant or number");
				}
			}
			return returnValue;
		}
		
		public int ExpectedConstantOrNumber(string[] elements, string statementName, CompilationLog thisLog, SourceLine thisLine) {
			int returnValue = -1;
			string thisElement = "";
			
			i++;
			if (i+1 <= elements.Length) {
				thisElement = elements[i].ToUpper();
				returnValue = ResolveAsConstantOrNumber(thisElement, thisLog, thisLine);
			} else {
				WriteError(thisLog, thisLine, "expected constant or number in '" + statementName + "' statement");
			}
			return returnValue;
			
		}
		
		public ComparisonExpression ExpectedComparisonExpression (string[] elements, CompilationLog thisLog, SourceLine thisLine) {
			ComparisonExpression returnExpression  = null;
			string thisElement = "";
			ComparisonOperator thisOperator = ComparisonOperator.EqualTo;

			i++;
			if (i+1 <= elements.Length) {
				thisElement = elements[i];
				PINTBasicExpression leftExpression = ResolveAsExpressionElement(thisElement, thisLog, thisLine);
				if (leftExpression != null) {
					i++;
					if (i+1 <= elements.Length) {
						thisElement = elements[i];
						switch (thisElement) {
							case "<":
								thisOperator = ComparisonOperator.LessThan;
								break;
								
							case "=":
								thisOperator = ComparisonOperator.EqualTo;
								break;

							case ">":
								thisOperator = ComparisonOperator.GreaterThan;
								break;

							case "<=":
								thisOperator = ComparisonOperator.LessThanOrEqualTo;
								break;									
									
							case ">=":
								thisOperator = ComparisonOperator.GreaterThanOrEqualTo;
								break;										

							case "<>":
								thisOperator = ComparisonOperator.NotEqualTo;
								break;		
									
							default:
								WriteError(thisLog, thisLine, "element '"+thisElement+"' is not a valid comparison operator");
								break;
						}

						i++;
						if (i+1 <= elements.Length) {
							thisElement = elements[i];
							PINTBasicExpression rightExpression = ResolveAsExpressionElement(thisElement, thisLog, thisLine);
							if (rightExpression != null) {
								returnExpression = new ComparisonExpression(leftExpression, rightExpression, thisOperator);
							}
						}
							
					} else {
						WriteError(thisLog, thisLine, "missing comparison operator in expression");
					}
				}
			} else {
				WriteError(thisLog, thisLine, "expected comparison expression");
			}
			
			return returnExpression;
		}
		
		public MethodComparisonExpression ExpectedMethodComparisonExpression(string[] elements, CompilationLog thisLog, SourceLine thisLine) {
			MethodComparisonExpression returnExpression  = null;
			string thisElement = "";
			
			i++;
			if (i+1 <= elements.Length) {
				thisElement = elements[i];
				MethodExpression leftExpression = ResolveAsMethodExpression(thisElement, elements, thisLog, thisLine);
				if (leftExpression != null) {	
					//for now, force a value for 'right'
					returnExpression = new MethodComparisonExpression(leftExpression, new LiteralExpression(1));	
				} else {
					WriteError(thisLog, thisLine, "expected method expression");
				}
			} else {
				WriteError(thisLog, thisLine, "expected method expression");
			}
			return returnExpression;
		}
		
		public PINTBasicExpression ExpectedArithmeticExpression (string[] elements, CompilationLog thisLog, SourceLine thisLine) {
			PINTBasicExpression returnExpression  = null;
			string thisElement = "";
			ArithmeticOperator thisOperator = ArithmeticOperator.Add;

			i++;
			if (i+1 <= elements.Length) {
				thisElement = elements[i];
				PINTBasicExpression leftExpression = ResolveAsExpressionElement(thisElement, thisLog, thisLine);
				if (leftExpression != null) {
					i++;
					if (i+1 <= elements.Length) {
						thisElement = elements[i];
						switch (thisElement) {
							case "+":
								thisOperator = ArithmeticOperator.Add;
								break;
								
							case "-":
								thisOperator = ArithmeticOperator.Subtract;
								break;

							case "*":
								thisOperator = ArithmeticOperator.Multiply;
								break;

							case "/":
								thisOperator = ArithmeticOperator.Divide;
								break;									
									
									
							default:
								WriteError(thisLog, thisLine, "element '"+thisElement+"' is not a valid arithmetic operator");
								break;
						}

						i++;
						if (i+1 <= elements.Length) {
							thisElement = elements[i];
							PINTBasicExpression rightExpression = ResolveAsExpressionElement(thisElement, thisLog, thisLine);
							if (rightExpression != null) {
								returnExpression = new ArithmeticExpression(leftExpression, rightExpression, thisOperator);
							}
						}
							
					} else {
						WriteError(thisLog, thisLine, "missing arithmetic operator in expression");
					}
				}
			} else {
				WriteError(thisLog, thisLine, "expected arithmetic expression");
			}
			
			return returnExpression;
		}		
		
		//overload, for single parameter objects
		public string[] ExpectedObjectAssignment (string[] elements, string statementName, CompilationLog thisLog, SourceLine thisLine) {
			return ExpectedObjectAssignment(elements, statementName, 1, thisLog, thisLine);
		}
		
		public string[] ExpectedObjectAssignment (string[] elements, string statementName, int numberOfParameters, CompilationLog thisLog, SourceLine thisLine) {
			string[] returnAssignment = new string[4];
			if (ExpectedKeyword(elements, "(", statementName, thisLog, thisLine)) {		
				for  (int count = 0; count < numberOfParameters; count++) {
					i++;
					if (i+1 <= elements.Length) {
						returnAssignment[count] = elements[i];
						if ((count+1) < numberOfParameters) {
							//check for the close parenthesis
							ExpectedKeyword(elements, ",", statementName, thisLog, thisLine);
						} else {
							//check for the close parenthesis
							ExpectedKeyword(elements, ")", statementName, thisLog, thisLine);
						}
					}
				}
			}
			return returnAssignment;
		}
		
		public bool ExpectedKeyword (string[] elements, string expectedElement, string statementName, CompilationLog thisLog, SourceLine thisLine) {
			i++;
			if (i+1 <= elements.Length) {
				if (elements[i].ToUpper() != expectedElement) {
					WriteError(thisLog, thisLine, "expected keyword '"+expectedElement+"' instead of '"+elements[i].ToUpper()+"' in "+statementName+" statement");
					return false;
				} else {
					return true;
				}
			} else {
				WriteError(thisLog, thisLine, "expected keyword '"+expectedElement+"' in "+statementName+" statement");
				return false;
			}
		}
		
		public string ExpectedIdentifier (string[] elements, string statementName, CompilationLog thisLog, SourceLine thisLine) {
			string returnIdentifier = null;
			i++;
			if (i+1 <= elements.Length) {
				returnIdentifier = elements[i].ToUpper();
			} else {
				WriteError(thisLog, thisLine, "expected identifier in "+statementName+" statement");
			}
			return returnIdentifier;
		}
		
		public string ExpectedStringLiteral(string[] elements, string statementName, CompilationLog thisLog, SourceLine thisLine) {
			string returnLiteral = null;
			i++;
			if (i+1 <= elements.Length) {
				returnLiteral = elements[i].ToUpper();
			} else {
				WriteError(thisLog, thisLine, "expected string literal in "+statementName+" statement");
			}
			return returnLiteral;
		}		

		public string ExpectedObject (string[] elements, string statementName, CompilationLog thisLog, SourceLine thisLine) {
			string returnObjectName = null;
			i++;
			if (i+1 <= elements.Length) {
				returnObjectName = elements[i].ToUpper();
				switch (returnObjectName) {
					case "BACKDROP":
					case "BYTE":	
					case "HOTSPOT":
					case "PIC":
						break;
															
					default:
						WriteError(thisLog, thisLine, "object type '" +returnObjectName+ "' not recognized in "+statementName+" statement");
						returnObjectName = null;
						break;					
				}
			} else {
				WriteError(thisLog, thisLine, "expected object type in "+statementName+" statement");
			}
			return returnObjectName;
		}
		
		public PINTBasicStatementList ParseStatements(CompilationLog thisLog, SourceLineList lines, string terminatorStatement) {
			PINTBasicStatementList returnList = new PINTBasicStatementList();
			bool terminatorReached = false;
			if (canContinue) {
				while (!endOfSource) {
//				for (statementLine = currentLine; statementLine<lines.Count; statementLine++) {
					SourceLine thisLine = GetNextLine(lines);//lines[statementLine];
					if (thisLine != null) {
						string[] elements = Tokenizer.TokenizeLine(thisLine.Code);
						canContinue = true;
						
						if (elements.Length > 0) {
							for (i=0; i<elements.Length; i++) {
								string element = elements[i].ToUpper();
								
								if (element.Length > 0) {
									//indicates that we have found a comment, so skip the element processing
									if (element.Substring(0,1) == "'") canContinue = false;
								}
									
								if (canContinue) {
									switch (element) {
										//********* Statement Parsing *************************************************************************************
										case "SAY":
											if (ExpectedKeyword(elements, "(", element, thisLog, thisLine)) {
												
												//parse out the say text
												string thisSayText = ExpectedStringLiteral(elements, element, thisLog, thisLine);
												
												if (thisSayText != null) {
													if (ExpectedKeyword(elements, ")", element, thisLog, thisLine)) {
														int foundID = thisApplication.Texts.FindIndexByText(thisSayText);
														if (foundID == -1) foundID = thisApplication.AddText(thisSayText);
														returnList.Add(new PINTBasicSay(thisLine, foundID));
													}
												}
											}
											break;
											
										case "IF":
											if (ExpectedKeyword(elements, "(", element, thisLog, thisLine)) {
												ComparisonExpression thisIfCondition = null;
												MethodComparisonExpression thisMethodIfCondition = null;
												if (elements[i+1].IndexOf('.') > -1) {
													thisMethodIfCondition = ExpectedMethodComparisonExpression(elements, thisLog, thisLine);
												} else {
													thisIfCondition = ExpectedComparisonExpression (elements, thisLog, thisLine);
												}
												if (ExpectedKeyword(elements, ")", element, thisLog, thisLine)) {
													if (ExpectedKeyword(elements, "THEN", element, thisLog, thisLine)) {
														PINTBasicIf thisIf = null;
														if (thisIfCondition != null) thisIf = new PINTBasicIf(thisLine, thisIfCondition);
														if (thisMethodIfCondition != null) thisIf = new PINTBasicIf(thisLine, thisMethodIfCondition);
														thisIf.Met = ParseStatements(thisLog, lines, "IF");
														returnList.Add(thisIf);
														thisIf = null;
													}
												}
											}
											break;
											
										//special case: 'END' (terminator) statements*****
										case "END":
											//look at the next keyword, and determine what action needs to be taken
											i++;
											if (i+1 <= elements.Length) {
												if (elements[i].ToUpper() == terminatorStatement) {
													terminatorReached = true;
													break;
													
												} else {
													WriteError(thisLog, thisLine, "unexpected END "+elements[i].ToUpper()+" statement");
												}	
											}
											break;
										
										default:										
											//process assignments or method invocation, and only at the beginning of a line
											if (i == 0 ) {
												//indication of an object method invocation
												if (element.IndexOf('.') > -1) {
													MethodExpression thisMethod = ResolveAsMethodExpression(element, elements, thisLog, thisLine);
													if (thisMethod != null) {
														returnList.Add (new PINTBasicMethod(thisLine, thisMethod));
														canContinue = false;
													}
												} else {	
													PINTBasicExpression thisLeft = ResolveAsExpressionElement(element, thisLog, thisLine);
													if (thisLeft != null) {		
														if (ExpectedKeyword(elements, "=", element, thisLog, thisLine)) {
															PINTBasicExpression thisRight = null;
															
															if (elements.Length > 3){
																thisRight = ExpectedArithmeticExpression (elements, thisLog, thisLine);
															} else {
																i++;
																if (i+1 <= elements.Length) {	
																	element = elements[i].ToUpper();
																	thisRight = ResolveAsExpressionElement(element, thisLog, thisLine);
																} else {
																	WriteError(thisLog, thisLine, "expected expession in assignment statement");
																}
															}
															
															if (thisRight != null) {
																AssignmentExpression thisAssignmentExpression = new AssignmentExpression(thisLeft, thisRight);
																returnList.Add(new PINTBasicAssignment(thisLine, thisAssignmentExpression));
																canContinue = false;
															} else {
																WriteError(thisLog, thisLine, "expected expession in assignment statement");
															}												
														} 											
													}																				
												}
											}
											break;
									}
								}
								if (terminatorReached) break;
							}
						}
						if (terminatorReached) break;
					}
				}
			}
			if (!terminatorReached) {
				WriteError(thisLog, lines[lines.Count-1], "expected END "+terminatorStatement+" statement");
			}
			return returnList;
		}
		
		public PINTBasicApplication Parse(CompilationLog thisLog, SourceLineList lines) {
			thisApplication = new PINTBasicApplication();
			thisRoom = null;
			canContinue = true;
			i=0;
			while (!endOfSource) {
			//for (currentLine=0; currentLine<lines.Count; currentLine++) {
			
			//foreach (SourceLine thisLine in lines) {
				SourceLine thisLine = GetNextLine(lines);//lines[currentLine];
				if (thisLine != null) {
					string[] elements = Tokenizer.TokenizeLine(thisLine.Code);
					canContinue = true;
					if (elements.Length > 0) {
						for (i=0; i<elements.Length; i++) {
							string element = elements[i].ToUpper();
							
							if (element.Length > 0) {
								//indicates that we have found a comment, so skip the element processing
								if (element.Substring(0,1) == "'") canContinue = false;
							}
								
							if (canContinue) {
								switch (element) {
									//global non-label commands
									case "CONST":
										string constantName = ExpectedIdentifier(elements, element, thisLog, thisLine);
										
										if (constantName != null) {				
											//check to make sure the name is unique
											if (thisApplication.Constants.FindByName(constantName) != null) {
												WriteError(thisLog, thisLine, "constant named '" + constantName + "' has already been defined");
											} else {
												if (ExpectedKeyword(elements, "=", element, thisLog, thisLine)) {
													i++;
													if (i+1 <= elements.Length) {
														string valueString = elements[i];
														try {
															int constantValue = Convert.ToInt32(valueString);
															thisApplication.AddConstant(constantName, constantValue);
														} catch {
															WriteError(thisLog, thisLine, "unable to parse numeric value in constant declaration: value = '"+valueString+"'");
														}											
													} else {
														WriteError(thisLog, thisLine, "expected value in constant declaration");
													}
												}
											}
										}
										break;									
										
									case "DIM":
										string dimIdentifier = ExpectedIdentifier(elements, element, thisLog, thisLine);
										if (dimIdentifier != null) {
											if (ExpectedKeyword(elements, "AS", element, thisLog, thisLine)) {
												string thisObjectName = ExpectedObject(elements, element, thisLog, thisLine);											
												string[] parameters = null;
												switch (thisObjectName) {
												
													case "BACKDROP":
														parameters = ExpectedObjectAssignment(elements, element, thisLog, thisLine);
														if (parameters[0] != null) {
															if (thisRoom != null) {
																if (thisRoom.Backdrop == null) {
																	thisRoom.Backdrop = new PINTBasicBackdrop(0, dimIdentifier, parameters[0]);
																} else {
																	WriteError(thisLog, thisLine, "backdrop resources cannot be defined more than once in a Room object");
																}
															} else {
																WriteError(thisLog, thisLine, "backdrop resources cannot be defined outside a Room object");
															}
														}
														break;
														
													case "HOTSPOT":
														parameters = ExpectedObjectAssignment(elements, element, 4, thisLog, thisLine);
														if (parameters[0] != null) {
															if (thisRoom != null) {
																if (!thisRoom.AddHotspot(dimIdentifier, Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]), Convert.ToInt32(parameters[2]), Convert.ToInt32(parameters[3]))) WriteError(thisLog, thisLine, "maximum number Hotspot object variables exceeded.");
															} else {
																WriteError(thisLog, thisLine, "hotspot resources cannot be defined outside a Room object");
															}
														}
														break;														
		
													case "PIC":
														parameters = ExpectedObjectAssignment(elements, element, thisLog, thisLine);
														if (parameters[0] != null) {
															if (!thisApplication.AddPic(dimIdentifier, parameters[0])) WriteError(thisLog, thisLine, "maximum number of Pic resources exceeded.");
														}
														break;
														
													case "BYTE":
														if (thisRoom != null) {
															if (!thisRoom.AddVariable(dimIdentifier)) WriteError(thisLog, thisLine, "maximum number Room object variables exceeded.");
														} else {
															if (!thisApplication.AddVariable(dimIdentifier)) WriteError(thisLog, thisLine, "maximum number of global variables exceeded.");
														}
														break;
												}
											}
										}									
										break;
										
										
									case "ROOM":
										string roomIdentifier = ExpectedIdentifier(elements, element, thisLog, thisLine);
										int roomID = ResolveAsConstantOrNumber(roomIdentifier, thisLog, thisLine);
										if (roomID > -1) {
											if (thisRoom == null) {
												thisRoom = new PINTBasicRoom(roomID, roomIdentifier);
											} else {
												WriteError(thisLog, thisLine, "Room object declaration without an END ROOM statement for the previous Room object");
											}
										}
										break;
										
									case "EVENT":
										string eventIdentifier = ExpectedIdentifier(elements, element, thisLog, thisLine);
										EventType thisType = ResolveAsEventType(eventIdentifier, thisLog, thisLine);
										if (thisType != EventType.Undefined) {
											if (thisRoom != null) {
												if (thisRoom.Events.FindByName(eventIdentifier) == null) {
													PINTBasicEvent thisEvent = new PINTBasicEvent((int)thisType, eventIdentifier);
													thisEvent.Statements = ParseStatements(thisLog, lines, "EVENT");
													thisRoom.Events.Add(thisEvent);
													thisEvent = null;
												} else {
													WriteError(thisLog, thisLine, "event with the identifier '" +eventIdentifier+ "' has already been defined for Room '"+thisRoom.Name+"'");
												}
											} else {
												WriteError(thisLog, thisLine, "event '"+eventIdentifier+"' cannot be defined outside of a Room object");
											}
										}
										break;
														
									//special case: 'END' (terminator) statements*****
									case "END":
										//look at the next keyword, and determine what action needs to be taken
										i++;
										if (i+1 <= elements.Length) {
											switch(elements[i].ToUpper()) {
												case "ROOM":
												if (thisRoom != null) {
													thisApplication.Rooms.Add(thisRoom);
													thisRoom = null;
												} else {
													WriteError(thisLog, thisLine, "unexpected END ROOM statement");
												}
												break;
												
												default:
													WriteError(thisLog, thisLine, "unexpected END "+elements[i].ToUpper()+" statement");
													break;
											}
											
										}
										break;
										
									default:
										WriteError(thisLog, thisLine, "unrecognized statement or keyword '"+element+"' outside of an Event");
										break;
									
								}
							}
						}
					}
				}
			}
			
			return thisApplication;
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
