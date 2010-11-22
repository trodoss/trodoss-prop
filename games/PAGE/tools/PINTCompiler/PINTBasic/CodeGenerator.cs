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
		private int whileCount;
		private int nestLevel;
		private PINTBasicLabelList labelStack;
		
		private void WriteLine(string newSyntax) {
			WriteLine(newSyntax,null);
		}
		
		private void WriteLine(string newSyntax, SourceLine line) {
			SourceLine newLine = null;
			if (line != null) {
				newSyntax = newSyntax.PadRight(40);
				
				string newCode = newSyntax + "' " + line.Code;
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
			foreach (PINTBasicPic globalPic in thisApplication.Pics) {
				WriteLine("CONST _PIC_"+ globalPic.ID + " = " + globalPic.ID);
			}			
			
			//write out CONST for the Text resources
			foreach (PINTBasicText thisText in thisApplication.Texts) {
				WriteLine("CONST _TEXT_"+ thisText.ID +" = " + thisText.ID);
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

		private void WritePics() {
			WriteLine("'pics");
			foreach (PINTBasicPic globalPic in thisApplication.Pics) {
				WriteLine("PIC _PIC_"+ globalPic.ID + " " + globalPic.FileName);
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
		
		
		private void WriteEvent(PINTBasicEvent thisEvent) {
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
						currentLabel = "_ENDIF_" + ifCount;
						ifCount++;
						labelStack.Add(new PINTBasicLabel(nestLevel, currentLabel));
					}else {
						if (hasNextLine){
							currentLabel = "_ENDIF_" + ifCount;
							ifCount++;
							labelStack.Add(new PINTBasicLabel(nestLevel, currentLabel));
						} else {
							//find the parent label to use here, if there is one
							if (labelStack.FindIndexByID(nestLevel-1) > -1) currentLabel = labelStack[labelStack.FindIndexByID(nestLevel-1)].Name;
						}
					}
					
					WriteLine(GenerateComparisonStatement(thisIf.Condition, currentLabel), thisIf.Source);
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
					nestLevel--;
					thisIf = null;

					break;
					
				case "PINTBasicWhile":
					break;
			}
		}
		
		private string GenerateComparisonStatement(ComparisonExpression thisExpression, string notEqualLabel) {
			string returnString = "";
			bool isVariableComparison = false;
			RoomVariableExpression roomVariableExpression;
			GlobalVariableExpression globalVariableExpression;
			ConstantExpression constantExpression;
			LiteralExpression literalExpression;
			
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
		
		
		public SourceLineList Generate(PINTBasicApplication inApplication, CompilationLog thisLog) {
			thisList = new SourceLineList();
			thisApplication = inApplication;
			lineNumber = 1;
			ifCount = 0;
			whileCount = 0;
			
			foreach (PINTBasicRoom thisRoom in thisApplication.Rooms) {
				WriteLine("'IL generated by PINTCompiler.PINTBasic.CodeGenerator at " + DateTime.Now);
				WriteLine("");
				
				WriteConstants(thisRoom);
				WriteBackdrop(thisRoom);
				WritePics();
				WriteTexts();
				
				foreach (PINTBasicEvent thisEvent in thisRoom.Events) {
					WriteEvent(thisEvent);
				}
			}
			
			return thisList;
		}
	}
}