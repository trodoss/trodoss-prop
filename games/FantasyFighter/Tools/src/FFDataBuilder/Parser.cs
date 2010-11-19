using System;
using System.Collections;
using System.IO;
//****************************************
// FFGameObject
//Copyright (c) 2010 trodoss
//See end of file for terms of use.  
//***************************************
namespace FFDataBuilder {
	public class Parser {

		private static string[] TokenizeLine(string lineData) {
			ArrayList stringSet = new ArrayList();
			string currentString = "";
			for (int i=0; i<lineData.Length; i++) {
				switch (lineData[i]) {
					case ' ':
						stringSet.Add(currentString);
						currentString = "";
						break;
						
					case '"':
						currentString += lineData[i];
						i++;
						while (lineData[i] != '"') {
							currentString += lineData[i];
							i++;
						}
						currentString += lineData[i];
						break;
						
					default:
						currentString += lineData[i];
						break;
				}
			}
			stringSet.Add(currentString);
			
			string[] returnArray = new string[stringSet.Count];
			for (int j=0; j< stringSet.Count; j++) {
				returnArray[j] = (string)stringSet[j];
			}
			return returnArray;
		}
		
		private static void WriteError(FFGame thisGame, int lineNumber, string message) {
			thisGame.CanProcess = false;
			Console.WriteLine("(Error) Line: " + lineNumber + " - " + message);
		}
		
		private static void WriteWarning(int lineNumber, string message) {
			Console.WriteLine("(Warning) Line: " + lineNumber + " - " + message);
		}
		
		private static int ResolveAsConstantOrNumber(FFGame thisGame, int lineNumber, string value) {
			int returnValue = -1;
			value = value.Replace(",","");
			try {
				returnValue = Convert.ToInt32(value);
			} catch {
				if (thisGame.Constants[value] != null) {
					returnValue = Convert.ToInt32(thisGame.Constants[value]);
				} else {
					WriteError(thisGame, lineNumber, "could not parse '" + value + "' as a constant or numeric value in statement; constants must be declared before being referenced.");
				}
			}
			return returnValue;
		}
		
		public static FFGame Parse(string fileName) {
			FFGame thisGame = new FFGame();
			
			using (StreamReader streamReader = new StreamReader(fileName)) {
				int lineNumber = 0;
				string lineData;
				
				string mapIdentifier = "";
				int currentMapID = -1;
					
				while((lineData = streamReader.ReadLine()) != null) {
					lineNumber++;
					
					//remove leading/trailing spaces
					lineData = lineData.Trim();
					
					//split data into unique elements to parse
					string[] elements = TokenizeLine(lineData);			
					
					//used in event parsing
					int xNumericValue = -1;
					int yNumericValue = -1;
					string xStringValue = "";
					string yStringValue = "";					
					
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
									case "CONST":
										if (i+1 < elements.Length) {
											i++;
											string constantName = elements[i].ToUpper();
											
											//check to make sure the name is unique
											if (thisGame.Constants[constantName] != null) {
												WriteError(thisGame, lineNumber, "constant named '" + constantName + "' has already been defined");
											} else {
												if (i+1 < elements.Length) {
													i++;
													if (elements[i] == "=") {
														if (i+1 < elements.Length) {
															i++;
															string valueString = elements[i];
															try {
																int constantValue = Convert.ToInt32(valueString);
																thisGame.Constants.Add(constantName, constantValue);
															} catch {
																WriteError(thisGame, lineNumber, "unable to parse numeric value in constant declaration");
															}
														} else {
															WriteError(thisGame, lineNumber, "expected numeric value in constant delaration");
														}
													} else {
														WriteError(thisGame, lineNumber, "expected '=' in constant declaration ");
													}
												} else {
													WriteError(thisGame, lineNumber, "expected '=' in constant declaration");
												}
											}
										} else {
											WriteError(thisGame, lineNumber, "identifer expected in constant declaration");
										}
										break;

									case "TEXT":
										if (i+1 < elements.Length) {
											i++;
											if (elements[i].Substring(0,1) == "(") {
												elements[i] = elements[i].Replace("(", "");
												
												if (elements[i].Substring(0,1) == "\"") {
													string textEntry = elements[i].Replace("\"", "");
													string textIDValue = "";
													int textEntryID = -1;
													switch (textEntry.Substring(textEntry.Length-1, 1)) {
														case ",":
															if (i+1 < elements.Length) {
																i++;
																textIDValue = elements[i].ToUpper();
																textIDValue = textIDValue.Replace(")", "");
																textEntryID = ResolveAsConstantOrNumber(thisGame, lineNumber, textIDValue);
															}
															break;
																
														case ")":
															break;
													}
													textEntry = textEntry.Replace(textEntry.Substring(textEntry.Length-1, 1), "");
													textEntry = textEntry.ToUpper();
													
													if (textEntry.Length > 15) {
														WriteWarning(lineNumber, "text greater than 15 characters; entry truncated");
														textEntry = textEntry.Substring(0,15);
													}
													thisGame.AddString(textEntry, textEntryID);
																
												} else {
													WriteError(thisGame, lineNumber, "expected string literal (text) in text declaration");
												}
											} else {
												WriteError(thisGame, lineNumber, "expected '(' in text declaration");
											}
										}
										break;
										
									case "ITEM":
										if (i+1 < elements.Length) {
											i++;
											string itemIdentifier = elements[i].ToUpper();
											string levelText = "";
											int levelNumber = 0;
											string itemTypeText = "";
											int itemTypeNumber = 0;
											string elementText = "";
											int elementNumber = 0;
											string costText = "";
											int costNumber = 0;
											
											//check to make sure that the identifier has not already been used
											if (thisGame.Items.FindByIdentifier(itemIdentifier) != null) {
												WriteError(thisGame, lineNumber, "item with identifier '" + itemIdentifier + "' has already been defined");
											} else {
												if (i+1 < elements.Length) {
													i++;
													if (elements[i].Substring(0,1) == "(") {
														elements[i] = elements[i].Replace("(", "");
														levelText = elements[i].ToUpper();
														
														if (levelText.Substring(levelText.Length-1, 1) == ",") {
															levelText = levelText.Replace(",","");
															levelNumber = ResolveAsConstantOrNumber(thisGame, lineNumber, levelText);
															
															if (i+1 < elements.Length) {
																i++;
																itemTypeText = elements[i].ToUpper();
																if (itemTypeText.Substring(itemTypeText.Length-1, 1) == ",") {
																	itemTypeText = itemTypeText.Replace(",","");
																	itemTypeNumber = ResolveAsConstantOrNumber(thisGame, lineNumber, itemTypeText);

																	if (i+1 < elements.Length) {
																		i++;	
																		elementText = elements[i].ToUpper();
																		if (elementText.Substring(elementText.Length-1, 1) == ",") {
																			elementText = elementText.Replace(",","");
																			elementNumber = ResolveAsConstantOrNumber(thisGame, lineNumber, elementText);

																			if (i+1 < elements.Length) {
																				i++;		
																				costText = elements[i].ToUpper();
																				if (costText.Substring(costText.Length-1, 1) == ")") {
																					costText = costText.Replace(")","");
																					costNumber = ResolveAsConstantOrNumber(thisGame, lineNumber, costText);
																					
																					thisGame.AddItem(itemIdentifier, levelNumber, itemTypeNumber, elementNumber, costNumber);
																				} else {
																					WriteError(thisGame, lineNumber, "expected ')' in item declaration");
																				}
																			} else {
																				WriteError(thisGame, lineNumber, "expected cost in item declaration");
																			}
																		} else {
																			WriteError(thisGame, lineNumber, "expected ',' in item declaration");
																		}
																	} else {
																		WriteError(thisGame, lineNumber, "expected element type in item declaration");
																	}
																} else {
																	WriteError(thisGame, lineNumber, "expected ',' in item declaration");
																}
															} else {
																WriteError(thisGame, lineNumber, "expected item type in item declaration");
															}
														} else {
															WriteError(thisGame, lineNumber, "expected ',' in item declaration");
														}
																							
													} else {
														WriteError(thisGame, lineNumber, "expected '(' in item declaration");
													}
												}
											}
										} else {
											WriteError(thisGame, lineNumber, "identifier expected in item declaration");
										}	
										break;
										
									case "SONG":
										if (i+1 < elements.Length) {
											i++;
											string songIdentifier = elements[i].ToUpper();
											
											//check to make sure that the identifier has not already been used
											if (thisGame.Songs.FindByIdentifier(songIdentifier) != null) {
												WriteError(thisGame, lineNumber, "song with identifier '" + songIdentifier + "' has already been defined");
											} else {
												if (i+1 < elements.Length) {
													i++;
													if (elements[i].Substring(0,1) == "(") {
														elements[i] = elements[i].Replace("(", "");
														
														if (elements[i].Substring(0,1) == "\"") {
															string songFileName = elements[i].Replace("\"", "");
															string songTypeValue = "";
															int songType = -1;
															switch (songFileName.Substring(songFileName.Length-1, 1)) {
																case ",":
																	if (i+1 < elements.Length) {
																		i++;
																		songTypeValue = elements[i].ToUpper();
																		songTypeValue = songTypeValue.Replace(")", "");
																		songType = ResolveAsConstantOrNumber(thisGame, lineNumber, songTypeValue);
																	}
																	break;
																
																case ")":
																	break;	
															}
															songFileName = songFileName.Replace(songFileName.Substring(songFileName.Length-1, 1), "");
															thisGame.AddSong(songIdentifier, songFileName, songType);
																
														} else {
															WriteError(thisGame, lineNumber, "expected string literal (filename) in song declaration");
														}
													} else {
														WriteError(thisGame, lineNumber, "expected '(' in song declaration");
													}
												}
											}
										} else {
											WriteError(thisGame, lineNumber, "identifier expected in song declaration");
										}	
										break;
										
									case "MAP":
										if (i+1 < elements.Length) {
											i++;
											mapIdentifier = elements[i].ToUpper();	
											
											//check to make sure that the identifier has not already been used
											if (thisGame.Maps.FindByIdentifier(mapIdentifier) != null) {
												WriteError(thisGame, lineNumber, "map with identifier '" + mapIdentifier + "' has already been defined");
											} else {
												if (i+1 < elements.Length) {
													i++;
													if (elements[i].Substring(0,1) == "(") {
														elements[i] = elements[i].Replace("(", "");
														
														if (elements[i].Substring(0,1) == "\"") {
															string mapFileName = elements[i].Replace("\"", "");
															string songIdentifierFind = "";
															int songID = -1;
															if (mapFileName.Substring(mapFileName.Length-1, 1) == ",") {
																mapFileName = mapFileName.Replace(",","");
																if (i+1 < elements.Length) {
																	i++;
																	songIdentifierFind = elements[i].ToUpper();
																	if (songIdentifierFind.Substring(songIdentifierFind.Length-1, 1) == ",") {
																		songIdentifierFind = songIdentifierFind.Replace(",","");
																		
																		FFSong testSong = thisGame.Songs.FindByIdentifier(songIdentifierFind);
																		if (testSong != null) {
																			songID = testSong.ID;
																			if (i+1 < elements.Length) {
																				i++;
																				string randomMonsterFlag = elements[i].ToUpper();
																				int randomMonsterFlagValue = -1;
																				if (randomMonsterFlag.Substring(randomMonsterFlag.Length-1, 1) == ")") {
																					randomMonsterFlag = randomMonsterFlag.Replace(")","");
												
																					randomMonsterFlagValue = ResolveAsConstantOrNumber(thisGame, lineNumber, randomMonsterFlag);

																					bool hasRandomMonsters = false;
																					if (randomMonsterFlagValue == 1) hasRandomMonsters = true;
																					currentMapID = thisGame.AddMap(mapIdentifier, mapFileName, songID, hasRandomMonsters);
																																										
																				} else {
																					WriteError(thisGame, lineNumber, "expected ')' in map declaration");
																				}
																			} else {
																				WriteError(thisGame, lineNumber, "expected value for random monster flag");
																			}
																			
																		} else {
																			WriteError(thisGame, lineNumber, "song identifier '" + songIdentifierFind + "' not found; songs must be declared before being referenced");
																		}
																		testSong = null;
																		
																	} else {
																		WriteError(thisGame, lineNumber, "expected ',' in map declaration");
																	}
																} else {
																	WriteError(thisGame, lineNumber, "expected song identifier in map declaration");
																}				
															} else {
																WriteError(thisGame, lineNumber, "expected ',' in map declaration");
															}
														} else {
															WriteError(thisGame, lineNumber, "expected string literal (filename) in map declaration");
														}
													} else {
														WriteError(thisGame, lineNumber, "expected '(' in map declaration");
													}
												} else {
													WriteError(thisGame, lineNumber, "expected '(' in map declaration");
												}
											}
										} else {
											WriteError(thisGame, lineNumber, "identifier expected in map declaration");
										}
										break;
									
									case "TEXT_EVENT":
										if (i+1 < elements.Length) {
											i++;
											string textEventIdentifier = elements[i].ToUpper();	
											xNumericValue = -1;
											yNumericValue = -1;
											xStringValue = "";
											yStringValue = "";
											string text = "";
											int textID = 0;
											
											//check to make sure that the identifier has not already been used
											if (thisGame.Maps[currentMapID].Events.FindByIdentifier(textEventIdentifier) != null) {
												WriteError(thisGame, lineNumber, "event with identifier '" + textEventIdentifier + "' has already been defined for map '" + mapIdentifier + "'");
											} else {	
												if (i+1 < elements.Length) {
													i++;
													if (elements[i].Substring(0,1) == "(") {
														elements[i] = elements[i].Replace("(", "");
														xStringValue = elements[i].ToUpper();
														
														xNumericValue = ResolveAsConstantOrNumber(thisGame, lineNumber, xStringValue);
														if (xNumericValue > -1) {
															if (xStringValue.Substring(xStringValue.Length-1, 1) == ",") {
																if (i+1 < elements.Length) {
																	i++;	
																	yStringValue = elements[i].ToUpper();
																	if (yStringValue.Substring(yStringValue.Length-1, 1) == ",") {
																		yNumericValue = ResolveAsConstantOrNumber(thisGame, lineNumber, yStringValue);

																		if (yNumericValue > -1) {
																			if (i+1 < elements.Length) {
																				i++;
																				
																				text = elements[i].ToUpper();
																				if (text.Substring(text.Length-1, 1) == ")") {
																					text = text.Replace(")","");
																					text = text.Replace("\"", "");
																					
																					//trim down to size, if bigger than 15 characters
																					if (text.Length > 15) {
																						WriteWarning(lineNumber, "text greater than 15 characters; entry truncated");
																						text = text.Substring(0,15);
																					}
																					
																					//add to the shared strings for the game
																					textID = thisGame.AddString(text);
																					
																					//add text event to the map events
																					thisGame.Maps[currentMapID].AddTextEvent(textEventIdentifier, xNumericValue, yNumericValue, textID); 
																					
																				} else {
																					WriteError(thisGame, lineNumber, "expected ')' in event declaration");
																				}
																			} else {
																				WriteError(thisGame, lineNumber, "expected map identifier in event declaration");
																			}																			
																		}
																	} else {
																		WriteError(thisGame, lineNumber, "expected ',' in event declaration");
																	}
																} else {
																	WriteError(thisGame, lineNumber, "expected y value in event declaration");
																}
															} else {
																WriteError(thisGame, lineNumber, "expected ',' in event declaration");
															}
														}
													} else {
														WriteError(thisGame, lineNumber, "expected '(' in event declaration");
													}
												} else {
													WriteError(thisGame, lineNumber, "identifier expected in event declaration");
												}
											}
										}
										break;
										
									case "SHOP_EVENT":
										if (i+1 < elements.Length) {
											i++;
											string shopEventIdentifier = elements[i].ToUpper();	
											xNumericValue = -1;
											yNumericValue = -1;
											xStringValue = "";
											yStringValue = "";
											string levelTextValue = "";
											int levelNumericValue = 0;
											
											//check to make sure that the identifier has not already been used
											if (thisGame.Maps[currentMapID].Events.FindByIdentifier(shopEventIdentifier) != null) {
												WriteError(thisGame, lineNumber, "event with identifier '" + shopEventIdentifier + "' has already been defined for map '" + mapIdentifier + "'");
											} else {	
												if (i+1 < elements.Length) {
													i++;
													if (elements[i].Substring(0,1) == "(") {
														elements[i] = elements[i].Replace("(", "");
														xStringValue = elements[i].ToUpper();
														
														xNumericValue = ResolveAsConstantOrNumber(thisGame, lineNumber, xStringValue);
														if (xNumericValue > -1) {
															if (xStringValue.Substring(xStringValue.Length-1, 1) == ",") {
																if (i+1 < elements.Length) {
																	i++;	
																	yStringValue = elements[i].ToUpper();
																	if (yStringValue.Substring(yStringValue.Length-1, 1) == ",") {
																		yNumericValue = ResolveAsConstantOrNumber(thisGame, lineNumber, yStringValue);

																		if (yNumericValue > -1) {
																			if (i+1 < elements.Length) {
																				i++;
																				
																				levelTextValue = elements[i].ToUpper();
																				if (levelTextValue.Substring(levelTextValue.Length-1, 1) == ")") {
																					levelTextValue = levelTextValue.Replace(")","");
																					levelNumericValue = ResolveAsConstantOrNumber(thisGame, lineNumber, levelTextValue);
																					
																					//add text event to the map events
																					thisGame.Maps[currentMapID].AddShopEvent(shopEventIdentifier, xNumericValue, yNumericValue, levelNumericValue); 
																					
																				} else {
																					WriteError(thisGame, lineNumber, "expected ')' in event declaration");
																				}
																			} else {
																				WriteError(thisGame, lineNumber, "expected level (numeric value) in event declaration");
																			}																			
																		}
																	} else {
																		WriteError(thisGame, lineNumber, "expected ',' in event declaration");
																	}
																} else {
																	WriteError(thisGame, lineNumber, "expected y value in event declaration");
																}
															} else {
																WriteError(thisGame, lineNumber, "expected ',' in event declaration");
															}
														}
													} else {
														WriteError(thisGame, lineNumber, "expected '(' in event declaration");
													}
												} else {
													WriteError(thisGame, lineNumber, "identifier expected in event declaration");
												}
											}
										}
										break;										

									case "HEAL_EVENT":
										if (i+1 < elements.Length) {
											i++;
											string healEventIdentifier = elements[i].ToUpper();	
											xNumericValue = -1;
											yNumericValue = -1;
											xStringValue = "";
											yStringValue = "";
											
											//check to make sure that the identifier has not already been used
											if (thisGame.Maps[currentMapID].Events.FindByIdentifier(healEventIdentifier) != null) {
												WriteError(thisGame, lineNumber, "event with identifier '" + healEventIdentifier + "' has already been defined for map '" + mapIdentifier + "'");
											} else {	
												if (i+1 < elements.Length) {
													i++;
													if (elements[i].Substring(0,1) == "(") {
														elements[i] = elements[i].Replace("(", "");
														xStringValue = elements[i].ToUpper();
														
														xNumericValue = ResolveAsConstantOrNumber(thisGame, lineNumber, xStringValue);
														if (xNumericValue > -1) {
															if (xStringValue.Substring(xStringValue.Length-1, 1) == ",") {
																if (i+1 < elements.Length) {
																	i++;	
																	yStringValue = elements[i].ToUpper();
																	if (yStringValue.Substring(yStringValue.Length-1, 1) == ")") {
																		yStringValue = yStringValue.Replace(")","");
																		yNumericValue = ResolveAsConstantOrNumber(thisGame, lineNumber, yStringValue);

																		//add text event to the map events
																		thisGame.Maps[currentMapID].AddHealEvent(healEventIdentifier, xNumericValue, yNumericValue); 
																	} else {
																		WriteError(thisGame, lineNumber, "expected ',' in event declaration");
																	}
																} else {
																	WriteError(thisGame, lineNumber, "expected y value in event declaration");
																}
															} else {
																WriteError(thisGame, lineNumber, "expected ',' in event declaration");
															}
														}
													} else {
														WriteError(thisGame, lineNumber, "expected '(' in event declaration");
													}
												} else {
													WriteError(thisGame, lineNumber, "identifier expected in event declaration");
												}
											}
										}
										break;										
										
									case "EXIT_EVENT":
										if (i+1 < elements.Length) {
											i++;
											string exitEventIdentifier = elements[i].ToUpper();	
											xNumericValue = -1;
											yNumericValue = -1;
											xStringValue = "";
											yStringValue = "";
											string eventMapIdentifier = "";
											string eventEventIdentifier = "";
											
											//check to make sure that the identifier has not already been used
											if (thisGame.Maps[currentMapID].Events.FindByIdentifier(exitEventIdentifier) != null) {
												WriteError(thisGame, lineNumber, "event with identifier '" + exitEventIdentifier + "' has already been defined for map '" + mapIdentifier + "'");
											} else {	
												if (i+1 < elements.Length) {
													i++;
													if (elements[i].Substring(0,1) == "(") {
														elements[i] = elements[i].Replace("(", "");
														xStringValue = elements[i].ToUpper();
														
														xNumericValue = ResolveAsConstantOrNumber(thisGame, lineNumber, xStringValue);
														if (xNumericValue > -1) {
															if (xStringValue.Substring(xStringValue.Length-1, 1) == ",") {
																if (i+1 < elements.Length) {
																	i++;	
																	yStringValue = elements[i].ToUpper();
																	if (yStringValue.Substring(yStringValue.Length-1, 1) == ",") {
																		yNumericValue = ResolveAsConstantOrNumber(thisGame, lineNumber, yStringValue);

																		if (yNumericValue > -1) {
																			if (i+1 < elements.Length) {
																				i++;
																				eventMapIdentifier = elements[i].ToUpper();
																				if (eventMapIdentifier.Substring(eventMapIdentifier.Length-1, 1) == ",") {
																					eventMapIdentifier = eventMapIdentifier.Replace(",","");

																					if (i+1 < elements.Length) {
																						i++;
																						eventEventIdentifier = elements[i].ToUpper();
																						if (eventEventIdentifier.Substring(eventEventIdentifier.Length-1, 1) == ")") {
																							eventEventIdentifier = eventEventIdentifier.Replace(")","");
																							
																							thisGame.Maps[currentMapID].AddUnresolvedExitEvent(exitEventIdentifier, xNumericValue, yNumericValue, lineNumber, eventMapIdentifier, eventEventIdentifier); 
																						} else {
																							WriteError(thisGame, lineNumber, "expected ')' in event declaration");
																						}
																						
																					} else {
																						WriteError(thisGame, lineNumber, "expected event identifier in event declaration");
																					}
																					
																				} else {
																					WriteError(thisGame, lineNumber, "expected ',' in event declaration");
																				}
																			} else {
																				WriteError(thisGame, lineNumber, "expected map identifier in event declaration");
																			}																			
																		}
																	} else {
																		WriteError(thisGame, lineNumber, "expected ',' in event declaration");
																	}
																} else {
																	WriteError(thisGame, lineNumber, "expected y value in event declaration");
																}
															} else {
																WriteError(thisGame, lineNumber, "expected ',' in event declaration");
															}
														}
													} else {
														WriteError(thisGame, lineNumber, "expected '(' in event declaration");
													}
												} else {
													WriteError(thisGame, lineNumber, "identifier expected in event declaration");
												}
											}
										}
										break;
										
									case "MONSTER_EVENT":
										if (i+1 < elements.Length) {
											i++;
											string monsterEventIdentifier = elements[i].ToUpper();	
											xNumericValue = -1;
											yNumericValue = -1;
											xStringValue = "";
											yStringValue = "";
											string monsterIdentifierString = "";
											
											//check to make sure that the identifier has not already been used
											if (thisGame.Maps[currentMapID].Events.FindByIdentifier(monsterEventIdentifier) != null) {
												WriteError(thisGame, lineNumber, "event with identifier '" + monsterEventIdentifier + "' has already been defined for map '" + mapIdentifier + "'");
											} else {	
												if (i+1 < elements.Length) {
													i++;
													if (elements[i].Substring(0,1) == "(") {
														elements[i] = elements[i].Replace("(", "");
														xStringValue = elements[i].ToUpper();
														
														xNumericValue = ResolveAsConstantOrNumber(thisGame, lineNumber, xStringValue);
														if (xNumericValue > -1) {
															if (xStringValue.Substring(xStringValue.Length-1, 1) == ",") {
																if (i+1 < elements.Length) {
																	i++;	
																	yStringValue = elements[i].ToUpper();
																	if (yStringValue.Substring(yStringValue.Length-1, 1) == ",") {
																		yNumericValue = ResolveAsConstantOrNumber(thisGame, lineNumber, yStringValue);

																		if (yNumericValue > -1) {
																			if (i+1 < elements.Length) {
																				i++;
																				
																				monsterIdentifierString = elements[i].ToUpper();
																				if (monsterIdentifierString.Substring(monsterIdentifierString.Length-1, 1) == ")") {
																					monsterIdentifierString = monsterIdentifierString.Replace(")","");
																					
																					FFMonster thisMonster = thisGame.Maps[currentMapID].Monsters.FindByIdentifier(monsterIdentifierString);
																					if (thisMonster != null) {
																						//add text event to the map events
																						thisGame.Maps[currentMapID].AddMonsterEvent(monsterEventIdentifier, xNumericValue, yNumericValue, thisMonster.ID); 	
																					} else {
																						WriteError(thisGame, lineNumber, "monster with identifier '"+monsterIdentifierString+"' not found in current map definition; monster entities must be declared prior to being referenced");
																					}
																				
																				} else {
																					WriteError(thisGame, lineNumber, "expected ')' in event declaration");
																				}
																			} else {
																				WriteError(thisGame, lineNumber, "expected level (numeric value) in event declaration");
																			}																			
																		}
																	} else {
																		WriteError(thisGame, lineNumber, "expected ',' in event declaration");
																	}
																} else {
																	WriteError(thisGame, lineNumber, "expected y value in event declaration");
																}
															} else {
																WriteError(thisGame, lineNumber, "expected ',' in event declaration");
															}
														}
													} else {
														WriteError(thisGame, lineNumber, "expected '(' in event declaration");
													}
												} else {
													WriteError(thisGame, lineNumber, "identifier expected in event declaration");
												}
											}
										}
										break;											
										
									case "MONSTER":
										if (i+1 < elements.Length) {
											i++;
											string monsterIdentifier = elements[i].ToUpper();	
											string monsterTypeString = "";
											string elementTypeString = "";
											string hitPointString = "";
											string goldString = "";
											int monsterTypeNumber = -1;
											int elementTypeNumber = -1;
											int hitPointNumber = -1;
											int goldNumber = -1;
											
											//check to make sure that the identifier has not already been used
											if (thisGame.Maps[currentMapID].Events.FindByIdentifier(monsterIdentifier) != null) {
												WriteError(thisGame, lineNumber, "monster with identifier '" + monsterIdentifier + "' has already been defined for map '" + mapIdentifier + "'");
											} else {	
												if (i+1 < elements.Length) {
													i++;
													if (elements[i].Substring(0,1) == "(") {
														elements[i] = elements[i].Replace("(", "");
														monsterTypeString = elements[i].ToUpper();
														monsterTypeNumber = ResolveAsConstantOrNumber(thisGame, lineNumber, monsterTypeString);
														
														if (monsterTypeNumber > -1) {
															if (monsterTypeString.Substring(monsterTypeString.Length-1, 1) == ",") {
																if (i+1 < elements.Length) {
																	i++;	
																	elementTypeString = elements[i].ToUpper();
																	if (elementTypeString.Substring(elementTypeString.Length-1, 1) == ",") {
																		elementTypeString = elementTypeString.Replace(",","");
																		elementTypeNumber = ResolveAsConstantOrNumber(thisGame, lineNumber, elementTypeString);

																		if (i+1 < elements.Length) {
																			i++;
																			
																			hitPointString = elements[i].ToUpper();
																			if (hitPointString.Substring(hitPointString.Length-1, 1) == ",") {
																				hitPointString = hitPointString.Replace(",","");
																				hitPointNumber = ResolveAsConstantOrNumber(thisGame, lineNumber, hitPointString);

																				if (i+1 < elements.Length) {
																					i++;

																					goldString = elements[i].ToUpper();
																					if (goldString.Substring(goldString.Length-1, 1) == ")") {
																						goldString = goldString.Replace(")","");
																						goldNumber = ResolveAsConstantOrNumber(thisGame, lineNumber, goldString);
																						
																						thisGame.Maps[currentMapID].AddMonster(monsterIdentifier, monsterTypeNumber, elementTypeNumber, hitPointNumber, goldNumber);
																						
																					} else {
																						WriteError(thisGame, lineNumber, "expected ',' in monster declaration");
																					}
																				} else {
																					WriteError(thisGame, lineNumber, "expected gold value in monster declaration");
																				}
																				
																			} else {
																				WriteError(thisGame, lineNumber, "expected ',' in monster declaration");
																			}						
																		} else {
																			WriteError(thisGame, lineNumber, "expected hitpoints in monster declaration");
																		}
																	} else {
																		WriteError(thisGame, lineNumber, "expected ',' in monster declaration");
																	}
																} else {
																	WriteError(thisGame, lineNumber, "expected element value in monster declaration");
																}
															} else {
																WriteError(thisGame, lineNumber, "expected ',' in monster declaration");
															}
														}
													} else {
														WriteError(thisGame, lineNumber, "expected '(' in monster declaration");
													}
												} else {
													WriteError(thisGame, lineNumber, "identifier expected in monster declaration");
												}
											}
										}
										break;										
																				
									case "END":
										canContinue = false;
										break;
									
									default:
										if (element != "") {
											WriteError(thisGame, lineNumber, "unrecognized command '" + element + "'");
											canContinue = false;
										}
										break;
								}
							}
							if (!canContinue) break;
						}
					}
				}
				
			}
			
			//start resolving references
			int foundID = -1;
			for (int k=0; k< thisGame.Maps.Count; k++) {
				for (int l=0; l< thisGame.Maps[k].Events.Count; l++) {
					
					//****************** Exit Event resolution **********************
					if (thisGame.Maps[k].Events[l] is FFUnresolvedExitEvent) {
						FFUnresolvedExitEvent thisUnresolvedExitEvent = (FFUnresolvedExitEvent) thisGame.Maps[k].Events[l];
						
						FFMap thisMap = thisGame.Maps.FindByIdentifier(thisUnresolvedExitEvent.MapIdentifier);
						if (thisMap != null) {
							foundID = -1;
							foreach (FFEvent testEvent in thisMap.Events) {
								if (testEvent.Identifier ==  thisUnresolvedExitEvent.MapEventIdentifiers) {
									foundID = testEvent.ID;
									break;
								}
							}
							if (foundID > -1) {
								FFExitEvent thisExitEvent = new FFExitEvent(thisUnresolvedExitEvent.ID, thisUnresolvedExitEvent.Identifier, thisUnresolvedExitEvent.X, thisUnresolvedExitEvent.Y, thisMap.ID, foundID);
								
								//replace with the resolved exit event
								thisGame.Maps[k].Events[l] = thisExitEvent;
								
							} else {
								WriteError(thisGame, thisUnresolvedExitEvent.LineNumber, "referenced event identifier '" + thisUnresolvedExitEvent.MapEventIdentifiers + "' not found in map '"+ thisUnresolvedExitEvent.MapIdentifier+ "'");
							}							
						} else {
							WriteError(thisGame, thisUnresolvedExitEvent.LineNumber, "referenced map identifier '" + thisUnresolvedExitEvent.MapIdentifier + "' not found");
						}
					}			
				}
			}
			return thisGame;
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
