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
	public class CodeGenerator {
		private PINTBasicApplication thisApplication = null;
		private SourceLineList thisList = null;
		private int lineNumber;
		private int ifCount;
		private int nestLevel;
		private PINTBasicLabelList labelStack;
		
		private void WriteLine(string newSyntax) {
			WriteLine(newSyntax,null);
		}
		
		private void WriteLine(string newSyntax, SourceLine line) {
			SourceLine newLine = null;
			if (line != null) {
				newSyntax = newSyntax.PadRight(40);
				
				string newCode = newSyntax + " ' " + line.Code;
				newLine = new SourceLine(line.Source, lineNumber, newCode);
			} else {
				newLine = new SourceLine("", lineNumber, newSyntax);
			}
			if (thisList != null) thisList.Add(newLine);
			newLine = null;
			lineNumber++;
		}
		
		private void WriteConstants(PINTBasicRoom thisRoom) {
			WriteLine("'constants");
			foreach (PINTBasicConstant thisConstant in thisApplication.Constants) {
				WriteLine("CONST "+ thisConstant.Name +" = " + thisConstant.Value);
			}

			WriteLine("");
			WriteLine("'derived reference constants");
			//write out CONST for the backdrop
			if (thisRoom.Backdrop != null) WriteLine("CONST _BACKDROP = " + thisRoom.Backdrop.ID);
		
			//write out CONST for the Pic resources
			foreach (PINTBasicHotspot roomHotspot in thisRoom.Hotspots) {
				WriteLine("CONST _HOTSPOT_"+ roomHotspot.ID + " = " + roomHotspot.ID);
			}	
	
			//write out CONST for the Pic resources
			foreach (PINTBasicPic globalPic in thisApplication.Pics) {
				WriteLine("CONST _PIC_"+ globalPic.ID + " = " + globalPic.ID);
			}	

			//write out CONST for the Music resources
			foreach (PINTBasicMusic thisMusic in thisApplication.Musics) {
				WriteLine("CONST _MUSIC_"+ thisMusic.ID + " = " + thisMusic.ID);
			}			
			
			//write out CONST for the Text resources
			foreach (PINTBasicText thisText in thisApplication.Texts) {
				WriteLine("CONST _TEXT_"+ thisText.ID +" = " + thisText.ID);
			}			

			//write out CONST for the Item resources
			foreach (PINTBasicItem thisItem in thisApplication.Items) {
					WriteLine("CONST _ITEM_"+ thisItem.ID +" = " + thisItem.ID);
			}				
			WriteLine("");
		}
		
		private void WriteBackdrop(PINTBasicRoom thisRoom) {
			WriteLine("'backdrop");
			if (thisRoom.Backdrop != null) {
				WriteLine("BACKDROP _BACKDROP " + thisRoom.Backdrop.FileName);
			}
			WriteLine("");
		}

		private void WriteHotspots(PINTBasicRoom thisRoom) {
			WriteLine("'hotspots");
			foreach (PINTBasicHotspot roomHotspot in thisRoom.Hotspots) {
				WriteLine("HOTSPOT _HOTSPOT_" + roomHotspot.ID + " " + roomHotspot.X + " " + roomHotspot.Y + " " + roomHotspot.Width + " " + roomHotspot.Height);
			}
			WriteLine("");
		}		

		private void WritePics() {
			WriteLine("'pics");
			foreach (PINTBasicPic globalPic in thisApplication.Pics) {
				WriteLine("PIC _PIC_"+ globalPic.ID + " " + globalPic.FileName);
			}
			
			WriteLine("");
		}	
		
		private void WriteMusics() {
			WriteLine("'musics");
			foreach (PINTBasicMusic thisMusic in thisApplication.Musics) {
				WriteLine("MUSIC _MUSIC_"+ thisMusic.ID + " " + thisMusic.FileName);
			}
			
			WriteLine("");
		}		
		
		private void WriteTexts() {
			WriteLine("'texts");
			foreach (PINTBasicText thisText in thisApplication.Texts) {
				WriteLine("TEXT _TEXT_"+ thisText.ID +" " + thisText.Text);
			}
			WriteLine("");
		}
		
		private void WriteItems() {
			WriteLine("'items");
			foreach (PINTBasicItem thisItem in thisApplication.Items) {
				WriteLine("ITEM _ITEM_"+ thisItem.ID +" " + thisItem.Text);
			}
			WriteLine("");
		}		
		
		private void WriteEvent(PINTBasicEvent thisEvent) {
			WriteLine("'event " + thisEvent.Name);
			//derive a label name from the event name
			string lineLabel = thisEvent.Name + ": ";
			int lineCount = 0;
			int statementsLength = thisEvent.Statements.Count-1;
			int labelIndex = -1;
			nestLevel = 0;
			
			PINTBasicLabel thisLabel = null;
			labelStack = new PINTBasicLabelList();
			bool hasNextLine = true;
			
			foreach (PINTBasicStatement thisStatement in thisEvent.Statements) {
				if (lineCount == statementsLength) hasNextLine = false;
				
				if (lineCount == 0) {
					
					WriteStatement(thisStatement, lineLabel, hasNextLine);
				} else {
					labelIndex = labelStack.FindIndexByID(nestLevel);
					if (labelIndex > -1) {
						thisLabel = (PINTBasicLabel) labelStack[labelIndex];
						WriteStatement(thisStatement, thisLabel.Name + ": ", hasNextLine);
						labelStack.RemoveAt(labelIndex);
						thisLabel = null;
					} else {
						WriteStatement(thisStatement, hasNextLine);
					} 
				}
				lineCount++;
			}
			
			labelIndex = labelStack.FindIndexByID(nestLevel);
			if (labelIndex > -1) {
				thisLabel = (PINTBasicLabel) labelStack[labelIndex];
				WriteLine(thisLabel.Name + ": EVENT_END");
				labelStack.RemoveAt(labelIndex);
				thisLabel = null;			
			} else {
				WriteLine("EVENT_END");	
			} 
			labelStack = null;
			WriteLine("");
		}
		
		private void WriteStatement(PINTBasicStatement thisStatement, bool hasNextLine) {
			WriteStatement(thisStatement, "", hasNextLine);
		}
		
		private void WriteStatement(PINTBasicStatement thisStatement, string lineLabel, bool hasNextLine) {
			string outStatement = lineLabel;
			string currentLabel = "";
			int labelIndex = 0;
			PINTBasicLabel thisLabel = null;
			
			int statementsLength = 0;
			int lineCount = 0;
			bool nestHasNextLine = true;
			
			switch (thisStatement.GetType().Name) {
				case "PINTBasicSay":
					PINTBasicSay thisSay = (PINTBasicSay) thisStatement;
					outStatement += "SAY _TEXT_" + thisSay.TextID;
					WriteLine(outStatement, thisSay.Source);
					thisSay = null;
					break;
					
				case "PINTBasicIf":
					PINTBasicIf thisIf = (PINTBasicIf) thisStatement;
					if (nestLevel == 0) {
						
						if (thisIf.HasElseClause) {
							currentLabel = "_ELSE_" + ifCount;
							labelStack.Add(new PINTBasicLabel(nestLevel, currentLabel));
							labelStack.Add(new PINTBasicLabel(nestLevel, "_ENDIF_"+ifCount));
						} else {
							currentLabel = "_ENDIF_" + ifCount;
							labelStack.Add(new PINTBasicLabel(nestLevel, currentLabel));
						}
						
						
						ifCount++;
						
					}else {
						if (hasNextLine){
						if (thisIf.HasElseClause) {
							currentLabel = "_ELSE_" + ifCount;
							labelStack.Add(new PINTBasicLabel(nestLevel, currentLabel));
							labelStack.Add(new PINTBasicLabel(nestLevel, "_ENDIF_"+ifCount));
						} else {
							currentLabel = "_ENDIF_" + ifCount;
							labelStack.Add(new PINTBasicLabel(nestLevel, currentLabel));
						}						
							
							ifCount++;
						} else {
							//recursively find the parent label to use here, if there is one
							for (int j = nestLevel-1; j>-1; j--) {
								if (labelStack.FindLastIndexByID(j) > -1) {
									currentLabel = labelStack[labelStack.FindLastIndexByID(j)].Name;
									break;
								}
							}
						}
					}
					
					switch (thisIf.Condition.GetType().Name) {
						case "ComparisonExpression":
							ComparisonExpression thisComparisonExpression = (ComparisonExpression) thisIf.Condition;
							outStatement += GenerateComparisonStatement(thisComparisonExpression, currentLabel);
							WriteLine(outStatement, thisIf.Source);
							thisComparisonExpression = null;
							break;
							
						case "MethodComparisonExpression":
							MethodComparisonExpression thisMethodComparisonExpression = (MethodComparisonExpression) thisIf.Condition;
							outStatement += GenerateMethodComparisonStatement(thisMethodComparisonExpression, currentLabel);
							WriteLine(outStatement, thisIf.Source);
							thisMethodComparisonExpression = null;
							break;						
					}			
					
					nestLevel++;
					statementsLength = thisIf.Met.Count-1;
					foreach (PINTBasicStatement ifMetStatement in thisIf.Met) {
						if (lineCount == statementsLength) nestHasNextLine = false;
						
						labelIndex = labelStack.FindIndexByID(nestLevel);
						if (labelIndex > -1) {
							thisLabel = (PINTBasicLabel) labelStack[labelIndex];
							WriteStatement(ifMetStatement, thisLabel.Name + ": ", nestHasNextLine);
							labelStack.RemoveAt(labelIndex);
							thisLabel = null;
						} else {
							WriteStatement(ifMetStatement, nestHasNextLine);
						} 	
						lineCount++;
					}	
					
					if (thisIf.HasElseClause) {
						lineCount = 0;
						statementsLength = thisIf.Else.Count-1;
						
						//find the 'ENDIF' so that we can jump past the 'else' clause
						for (int j = nestLevel-1; j>-1; j--) {
							if (labelStack.FindLastIndexByID(j) > -1) {
								string gotoLabel = labelStack[labelStack.FindLastIndexByID(j)].Name;
								WriteLine("CODE_GOTO "+ gotoLabel);
								thisLabel = null;
								break;
							}
						}						
						
						//int ifLabelIndex = labelStack.FindLastIndexByID(nestLevel-1);
						//if (ifLabelIndex > -1) {
						//	thisLabel = (PINTBasicLabel) labelStack[ifLabelIndex];
						//	WriteLine("CODE_GOTO "+ thisLabel.Name);
						//	thisLabel = null;
						//}
													
						foreach (PINTBasicStatement elseStatement in thisIf.Else) {
							if (lineCount == statementsLength) nestHasNextLine = false;
							
							labelIndex = labelStack.FindIndexByID(nestLevel-1);
							if (labelIndex > -1) {
								thisLabel = (PINTBasicLabel) labelStack[labelIndex];
								WriteStatement(elseStatement, thisLabel.Name + ": ", nestHasNextLine);
								labelStack.RemoveAt(labelIndex);
								thisLabel = null;
							} else {
								WriteStatement(elseStatement, nestHasNextLine);
							} 	
							lineCount++;
						}					
					}
					
					nestLevel--;
					thisIf = null;

					break;
					
				case "PINTBasicAssignment":
					PINTBasicAssignment thisAssignment = (PINTBasicAssignment) thisStatement;
					string[] statements = GenerateVariableStatement(thisAssignment.Assignment);
					outStatement += statements[0];
					WriteLine(outStatement, thisAssignment.Source);
					if (statements[1] != null) WriteLine(statements[1]);
					thisAssignment = null;
					break;		

				case "PINTBasicMethod":
					PINTBasicMethod thisMethod = (PINTBasicMethod) thisStatement;
					outStatement += GenerateMethodStatement(thisMethod.Expression);
					WriteLine(outStatement, thisMethod.Source);
					thisMethod = null;
					break;						
			}
		}
		
		private string GenerateMethodStatement (MethodExpression thisExpression) {
			string returnString = "";
			switch (thisExpression.GetType().Name) {
				case "EgoLoadExpression":
					EgoLoadExpression thisEgoLoad = (EgoLoadExpression) thisExpression;
					returnString = "EGO_LOAD " + thisEgoLoad.X + " " + thisEgoLoad.Y + " " + thisEgoLoad.Facing;
					thisEgoLoad = null;
					break;
				
				case "InventoryAddExpression":
					InventoryAddExpression thisInventoryAdd = (InventoryAddExpression) thisExpression;
					returnString = "INVENTORY_ADD _ITEM_" + thisInventoryAdd.Item.ID ;
					thisInventoryAdd = null;
					break;
							
				case "InventoryRemoveExpression":
					InventoryRemoveExpression thisInventoryRemove = (InventoryRemoveExpression) thisExpression;
					returnString = "INVENTORY_REMOVE _ITEM_" + thisInventoryRemove.Item.ID ;
					thisInventoryRemove = null;
					break;	
					
				case "PicLoadExpression":
					PicLoadExpression thisPicLoad = (PicLoadExpression) thisExpression;
					returnString = "PIC_LOAD _PIC_" + thisPicLoad.ID + " " + thisPicLoad.X + " " + thisPicLoad.Y;
					thisPicLoad = null;
					break;

				case "PicHideExpression":
					PicHideExpression thisPicHide = (PicHideExpression) thisExpression;
					returnString = "PIC_HIDE _PIC_" + thisPicHide.ID;
					thisPicHide = null;				
					break;	

				case "RoomLoadExpression":
					RoomLoadExpression thisRoomLoad = (RoomLoadExpression) thisExpression;
					returnString = "ROOM_LOAD " + thisRoomLoad.RoomID;
					thisRoomLoad = null;				
					break;	

				case "MusicPlayExpression":
					MusicPlayExpression thisMusicPlay = (MusicPlayExpression) thisExpression;
					returnString = "MUSIC_PLAY _MUSIC_" + thisMusicPlay.ID;
					thisMusicPlay = null;				
					break;						
			}
			
			return returnString;
		}
		
		private string[] GenerateVariableStatement(AssignmentExpression thisExpression) {
			string[] returnString = new string[2];
			RoomVariableExpression roomVariableExpression;
			GlobalVariableExpression globalVariableExpression;
			ConstantExpression constantExpression;
			LiteralExpression literalExpression;
			ArithmeticExpression arithmeticExpression;
			
			string leftExpression = "";
			int leftExpressionID = -1;
			
			switch (thisExpression.Left.GetType().Name) {
				case "RoomVariableExpression":
					roomVariableExpression = (RoomVariableExpression) thisExpression.Left;
					leftExpression += (roomVariableExpression.ID + 8);
					leftExpressionID = roomVariableExpression.ID + 8;
					roomVariableExpression = null;
					break;
					
				case "GlobalVariableExpression":
					globalVariableExpression = (GlobalVariableExpression) thisExpression.Left;
					leftExpression += globalVariableExpression.ID;
					leftExpressionID = globalVariableExpression.ID;
					globalVariableExpression = null;
					break;	
			}			
			
			returnString[0] = "VARIABLE_SET " + leftExpression + " ";
			returnString[1] = null;
			
			//looking at the right expression, we can determine what type of statement the assignment will become
			switch (thisExpression.Right.GetType().Name) {
				case "ArithmeticExpression":
					arithmeticExpression = (ArithmeticExpression) thisExpression.Right;
					
					string leftSubExpression = "";
					string rightSubExpression = "";
					bool leftSubIsVariable = false;
					bool rightSubIsVariable = false;
					int leftSubExpressionID = -1;
					switch (arithmeticExpression.Left.GetType().Name) {
						case "RoomVariableExpression":
							roomVariableExpression = (RoomVariableExpression) arithmeticExpression.Left;
							leftSubExpression += (roomVariableExpression.ID + 8) + " ";
							leftSubExpressionID = (roomVariableExpression.ID + 8);
							roomVariableExpression = null;
							leftSubIsVariable = true;
							break;
							
						case "GlobalVariableExpression":
							globalVariableExpression = (GlobalVariableExpression) arithmeticExpression.Left;
							leftSubExpression += globalVariableExpression.ID + " ";
							leftSubExpressionID = globalVariableExpression.ID; 
							globalVariableExpression = null;
							leftSubIsVariable = true;
							break;		

						case "ConstantExpression":
							constantExpression = (ConstantExpression) arithmeticExpression.Left;
							leftSubExpression += thisApplication.Constants.FindByID(constantExpression.ID).Name + " ";
							constantExpression = null;
							break;
							
						case "LiteralExpression":
							literalExpression = (LiteralExpression) arithmeticExpression.Left;
							leftSubExpression += literalExpression.Value + " ";
							literalExpression = null;
							break;				
					}

					switch (arithmeticExpression.Right.GetType().Name) {
						case "RoomVariableExpression":
							roomVariableExpression = (RoomVariableExpression) arithmeticExpression.Right;
							rightSubExpression += (roomVariableExpression.ID + 8);
							roomVariableExpression = null;
							rightSubIsVariable = true;
							break;
							
						case "GlobalVariableExpression":
							globalVariableExpression = (GlobalVariableExpression) arithmeticExpression.Right;
							rightSubExpression += globalVariableExpression.ID ;
							globalVariableExpression = null;
							rightSubIsVariable = true;
							break;		

						case "ConstantExpression":
							constantExpression = (ConstantExpression) arithmeticExpression.Right;
							rightSubExpression += thisApplication.Constants.FindByID(constantExpression.ID).Name ;
							constantExpression = null;
							break;
							
						case "LiteralExpression":
							literalExpression = (LiteralExpression) arithmeticExpression.Right;
							rightSubExpression += literalExpression.Value;
							literalExpression = null;
							break;				
					}	

					string operatorValue = "";
					switch (arithmeticExpression.Operator) {
						case ArithmeticOperator.Add:
							operatorValue += "ADD";
							break;	

						case ArithmeticOperator.Subtract:
							operatorValue += "SUB";
							break;	

						case ArithmeticOperator.Multiply:
							operatorValue += "MUL";
							break;	

						case ArithmeticOperator.Divide:
							operatorValue += "DIV";
							break;								
					}
					
					//determine: if the left is the same as the outer left, then only an 'math' statement is necessary
					//if the left is different, then a set + math will need to be generated
					if (leftExpressionID == leftSubExpressionID) {
						returnString[0] = "VARIABLE_MATH " + leftExpression + " ";
						
						//finish out the "set" statement, and build the "math" statement
						if (rightSubIsVariable) {
							returnString[0] += "V" + operatorValue + " ";
						} else {
							returnString[0] +=  operatorValue + " ";
						}	
						returnString[0] += rightSubExpression;						
					} else {
						//finish out the "set" statement, and build the "math" statement
						if (leftSubIsVariable) {
							returnString[0] += "VARIABLE ";
						} else {
							returnString[0] += "VALUE ";
						}
						returnString[0] += leftSubExpression;
						
						//finish out the "set" statement, and build the "math" statement
						returnString[1] = "VARIABLE_MATH " + leftExpression + " ";
						if (rightSubIsVariable) {
							returnString[1] += "V" + operatorValue + " ";
						} else {
							returnString[1] += operatorValue + " ";
						}		
						returnString[1] += rightSubExpression;						
					}
					break;
					
				case "RoomVariableExpression":
					roomVariableExpression = (RoomVariableExpression) thisExpression.Right;
					returnString[0] += "VARIABLE " + (roomVariableExpression.ID + 8) ;
					roomVariableExpression = null;
					break;
					
				case "GlobalVariableExpression":
					globalVariableExpression = (GlobalVariableExpression) thisExpression.Right;
					returnString[0] += "VARIABLE " + globalVariableExpression.ID;
					globalVariableExpression = null;
					break;
					
				case "ConstantExpression":
					constantExpression = (ConstantExpression) thisExpression.Right;
					returnString[0] += "VALUE " + thisApplication.Constants.FindByID(constantExpression.ID).Name;
					constantExpression = null;
					break;
					
				case "LiteralExpression":
					literalExpression = (LiteralExpression) thisExpression.Right;
					returnString[0] += "VALUE " + literalExpression.Value ;
					literalExpression = null;
					break;
				
			}
			
			return returnString;
		}
		
		private string GenerateComparisonStatement(ComparisonExpression thisExpression, string notEqualLabel) {
			string returnString = "";
			bool isVariableComparison = false;
			RoomVariableExpression roomVariableExpression;
			GlobalVariableExpression globalVariableExpression;
			ConstantExpression constantExpression;
			LiteralExpression literalExpression;
			ItemReferenceExpression itemReferenceExpression;
			
			switch (thisExpression.Left.GetType().Name) {
				case "RoomVariableExpression":
					roomVariableExpression = (RoomVariableExpression) thisExpression.Left;
					returnString += "VARIABLE_TEST " + (roomVariableExpression.ID + 8) + " ";
					roomVariableExpression = null;
					break;
					
				case "GlobalVariableExpression":
					globalVariableExpression = (GlobalVariableExpression) thisExpression.Left;
					returnString += "VARIABLE_TEST "+ globalVariableExpression.ID + " ";
					globalVariableExpression = null;
					break;
					
			}
			
			string rightExpression = "";
			switch (thisExpression.Right.GetType().Name) {
				case "RoomVariableExpression":
					roomVariableExpression = (RoomVariableExpression) thisExpression.Right;
					rightExpression = (roomVariableExpression.ID + 8) + " ";
					isVariableComparison = true;
					roomVariableExpression = null;
					break;
					
				case "GlobalVariableExpression":
					globalVariableExpression = (GlobalVariableExpression) thisExpression.Right;
					rightExpression = globalVariableExpression.ID + " ";
					isVariableComparison = true;
					globalVariableExpression = null;
					break;
					
				case "ConstantExpression":
					constantExpression = (ConstantExpression) thisExpression.Right;
					rightExpression = thisApplication.Constants.FindByID(constantExpression.ID).Name + " ";
					constantExpression = null;
					break;
					
				case "LiteralExpression":
					literalExpression = (LiteralExpression) thisExpression.Right;
					rightExpression = literalExpression.Value + " ";
					literalExpression = null;
					break;
					
				case "ItemReferenceExpression":
					itemReferenceExpression = (ItemReferenceExpression) thisExpression.Right;
					rightExpression = "_ITEM_" + itemReferenceExpression.ID + " ";
					itemReferenceExpression = null;
					break;					
			}
			
			string operatorValue = "";
			if (isVariableComparison) operatorValue += "V";
			switch (thisExpression.Operator) {
				case ComparisonOperator.LessThan:
					operatorValue += "LT";
					break;
					
				case ComparisonOperator.EqualTo:
					operatorValue += "EQ";
					break;		
					
				case ComparisonOperator.GreaterThan:
					operatorValue += "GT";
					break;	
					
				case ComparisonOperator.LessThanOrEqualTo:
					operatorValue += "LE";
					break;	
					
				case ComparisonOperator.GreaterThanOrEqualTo:
					operatorValue += "GE";
					break;				
					
				case ComparisonOperator.NotEqualTo:
					operatorValue += "NE";
					break;						
			}
			operatorValue += " ";
			
			returnString += operatorValue + rightExpression + notEqualLabel;
			
			return returnString;
		}	
		
		public string GenerateMethodComparisonStatement(MethodComparisonExpression thisExpression, string notEqualLabel) {
			string returnString = "";
			
			switch (thisExpression.Left.GetType().Name) {
				case "HotspotContainsEgoExpression":
					HotspotContainsEgoExpression hotspotContainsEgoExpression = (HotspotContainsEgoExpression) thisExpression.Left;
					returnString += "HOTSPOT_TEST _HOTSPOT_" + hotspotContainsEgoExpression.HotspotID + " ";
					hotspotContainsEgoExpression = null;
					break;
					
				case "InventoryContainsExpression":
					InventoryContainsExpression inventoryContainsExpression = (InventoryContainsExpression) thisExpression.Left;
					returnString += "INVENTORY_TEST _ITEM_" + inventoryContainsExpression.Item.ID + " ";
					inventoryContainsExpression = null;
					break;					
	
			}		
			returnString += notEqualLabel;
			
			return returnString;
		}
		
		public SourceLineList Generate(PINTBasicApplication inApplication, CompilationLog thisLog) {
			thisList = new SourceLineList();
			thisApplication = inApplication;
			lineNumber = 1;
			ifCount = 0;
			
			foreach (PINTBasicRoom thisRoom in thisApplication.Rooms) {
				WriteLine("'IL generated by PINTCompiler.PINTBasic.CodeGenerator at " + DateTime.Now);
				WriteLine("'Room ID: " +thisRoom.ID);
				WriteLine("");
				
				WriteConstants(thisRoom);
				WriteBackdrop(thisRoom);
				WriteHotspots(thisRoom);
				WritePics();
				if (thisRoom.ID == 0) WriteMusics();
				WriteTexts();
				if (thisRoom.ID == 0) WriteItems();
				
				foreach (PINTBasicEvent thisEvent in thisRoom.Events) {
					WriteEvent(thisEvent);
				}
			}
			
			return thisList;
		}
	}
}