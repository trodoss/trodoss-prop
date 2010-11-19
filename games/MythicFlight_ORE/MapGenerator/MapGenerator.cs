using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
//****************************************
// MapGenarator v0.7
//
//trodoss 2009-2010
//See end of file for terms of use.  
//***************************************
//Version 0.7 
// - Generate 2bpp (4-color) graphics definitions for the boss data
// - Adjust enemy position data to fit 20x24 display
//
//Version 0.1 - 0.6
// - Supports the directives /s,  /l, and /b
// - Boss movement pattern definition in /b directive
// - Allows the specification of RTTTL file (music) on /s and /l directives
//  - Supports the inclusion/handling of enemies being specified
//   - Supports output in either .spin or in binary files and allows specification of section length
//***********************************************************************
//	Notes (version 0.2 and higher):
//		- When storing the enemies, we assume that no more than 8 will be specified
//		  for any given level.  
//
//		- The current assumtion (8 bytes) for the enemy definition is as follows:
//  			ENEMY_SECTION = defines the section when this enemy activates
//  			ENEMY_ALIVE   = toggles alive/dead
//  			ENEMY_TYPE    = defines enemy type
//  			ENEMY_ACTION  =  defines the current action of the enemy
//  			ENEMY_XPOS    = X position of the enemy
// 			ENEMY_YPOS    = Y position of the enemy
//  			ENEMY_FRAME   = defines animation frame of the enemy
//  			ENEMY_LIFE    = defines how many 'hits' the enemy can take before dying (set by type)
//
//		- The default values for these elemsnts are as follows:
//			ENEMY_SECTION = section index when it is encountered in the data file
//			ENEMY_ALIVE = 1 (1=alive, 0 = dead)
//			ENEMY_TYPE = the value specified when encountered
//			ENEMY_ACTION = defaulted to 0 (different values based on type)
//			ENEMY_XPOS = 23 (screen position when it will 'appear'), 22 for boss characters
//			ENEMY_YPOS = Y value position where encountered in the data file
//			ENEMY_FRAME = 0
//			ENEMY_LIFE = enemies in range 0-7 is 1; 8-? is 10
//
//************** N A M E S P A C E ****************************************
namespace GameUtilities {
	//*********************************************************************
	// MapEnemy Class
	//*********************************************************************	
	public class MapEnemy {
		public int EnemySection;
		public int EnemyAlive;
		public int EnemyType;
		public int EnemyAction;
		public int EnemyXPos;
		public int EnemyYPos;
		public int EnemyFrame;
		public int EnemyLife;

		/// <sumary>
		/// Constructor
		/// </sumary>		
		public MapEnemy(int enemySection, int enemyType, int enemyYPos) {
			this.EnemySection =  enemySection;
			this.EnemyAlive = 1;
			this.EnemyType =  enemyType;
			this.EnemyAction = 0;
			this.EnemyYPos = enemyYPos;
			this.EnemyFrame = 0;
			if (enemyType > 7) { 
				this.EnemyLife = 10;
				//adjusted foreward 1, accounting for 16x16 size of 'boss' characters'
				this.EnemyXPos = 18;
			} else {
				this.EnemyLife = 1;
				this.EnemyXPos = 19;
			}
		}
	}
	//*********************************************************************
	// BossCharacterLine Class
	//*********************************************************************	
	public class BossCharacterLine {
		public string PicString;
		public string BinaryString;
		public int LineValue;

		/// <sumary>
		/// Constructor
		/// </sumary>		
		public BossCharacterLine(string picString, string binaryString) {
			this.PicString =  picString;
			this.BinaryString = binaryString;
			this.LineValue = Convert.ToInt32(binaryString, 2);
		}
	}
	//*********************************************************************
	// BossCharacter Class
	//*********************************************************************
	public class BossCharacter {
		public ArrayList Lines;
		
		/// <sumary>
		/// Constructor
		/// </sumary>			
		public BossCharacter () {
			this.Lines = new ArrayList();
		}
	}
	//*********************************************************************
	// Song Class
	//*********************************************************************
	public class Song {
		public ArrayList Notes;
		public int BeatsPerMinute;
		
		/// <sumary>
		/// Constructor
		/// </sumary>			
		public Song () {
			this.Notes = new ArrayList();
		}
	}	
	//*********************************************************************
	// Note Class
	//*********************************************************************
	public class Note {
		public int Octave;
		public int NoteValue;
		public int Duration;
		public int EOFFlag;
	}	
	//*********************************************************************
	// MapGenarator Class
	//*********************************************************************	
	public class MapGenarator {
		/// <sumary>
		/// Retrieves the version text 
		/// </sumary>
		private static string VersionText() { return "v0.7"; }
		/// <sumary>
		/// Convert a 32-bit number to a hex string with ls-byte first
		/// </sumary>
		private static string ToHexString(int num){
			string hex_chr = "0123456789ABCDEF";
			string str = "";
			str +=  hex_chr.Substring((num >> 4) & 0x0F,1) + hex_chr.Substring(num & 0x0F,1);
			return str;
		}
		/// <sumary>
		/// Overload - Convert a char to a hex string with ls-byte first
		/// </sumary>		
		private static string ToHexString(char charAsNum){
			int num = (int)charAsNum;
			return ToHexString(num);
		}		
		/// <sumary>
		/// Write the pattern data to the output file
		/// </sumary>		
		private static string WritePatternData_Text(ArrayList patternData) {
			string patternText = "Background_Pattern_Set  byte ";
			for (int i=0; i< patternData.Count; i++) {
				char[] patternLineArray = (char[])patternData[i];
				
				if (i>0) patternText += "                        byte ";
				
				for (int j=0; j< patternLineArray.Length; j++) {
					if (j> 0) patternText += ", ";
				    patternText += "$" + ToHexString(patternLineArray[j]);
				}
				patternText += "\n";
			}
			patternText += "\n";
			return patternText;
		}
		/// <sumary>
		/// Write the map data to the output file
		/// </sumary>			
		private static string WriteMapData_Text(ArrayList mapData) {
			string mapText = "Background_Level_Map    byte ";
			for (int i=0; i< mapData.Count; i++) {
				int mapDataElement = (int) mapData[i];
				
				if (i > 0) {
					if ((i % 24) == 0) { 
						mapText += "\n                        byte ";
					} else {
						mapText += ", ";
					}
				}

				mapText += "$" + ToHexString(mapDataElement);
				
			}
			mapText += "\n\n";
			return mapText;
		}
		/// <sumary>
		/// Write the enemy data to the output file
		/// </sumary>		
		private static string WriteEnemyData_Text(ArrayList enemyData) {
			string enemyText = "Enemy_Data_Set          byte ";
			for (int i=0; i< enemyData.Count; i++) {
				if (i > 0) enemyText += "                        byte ";
				MapEnemy thisEnemy = (MapEnemy) enemyData[i];
				enemyText += "$" + ToHexString(thisEnemy.EnemySection) + " ";
				enemyText += "$" + ToHexString(thisEnemy.EnemyAlive) + " ";
				enemyText += "$" + ToHexString(thisEnemy.EnemyType) + " ";
				enemyText += "$" + ToHexString(thisEnemy.EnemyAction) + " ";
				enemyText += "$" + ToHexString(thisEnemy.EnemyXPos) + " ";
				enemyText += "$" + ToHexString(thisEnemy.EnemyYPos) + " ";
				enemyText += "$" + ToHexString(thisEnemy.EnemyFrame) + " ";
				enemyText += "$" + ToHexString(thisEnemy.EnemyLife) + "\n";
			}
			
			//pad the rest of the data with 0's if not enough enemies are being specified
			if (enemyData.Count < 8) {
				int padLength = 8 - enemyData.Count;
				for (int j=0; j < padLength; j++) {
						enemyText += "                        byte ";              
					for (int k=0; k<8; k++) {
						enemyText += "$00 ";
					}
					enemyText += "\n";
				}
			}
			enemyText += "\n";
			return enemyText;
		}	
		/// <sumary>
		/// Write the boss data to the output file
		/// </sumary>		
		private static string WriteBossData_Text(ArrayList bossData) {
		    string bossText = "' Note: these character definitions are reversed, as the .redefine\n";
			bossText += "' statement uses the data as-is (does not reverse the data as it would \n";
			bossText += "' in the original font definitions) \n";
			bossText += "Enemy_Boss_Data_Set     word ";
			bool firstLine = true;
			
			foreach (BossCharacter thisCharacter in bossData) {
				foreach (BossCharacterLine thisLine in thisCharacter.Lines) {
					if (firstLine) {
						firstLine = false; 
					} else {
						bossText += "                        word ";
					}
						
					bossText += "%" + thisLine.BinaryString + "   '" + thisLine.PicString + "\n";
				}
				bossText += "\n";	
			}
			
			return bossText;
		}		
		/// <sumary>
		/// Compares pattern line arrays, to see if they hold the same data
		/// </sumary>			
		private static bool ComparePatternLineArrays(char[] storedPatternLineArray, char[] newPatternLineArray) {
			bool equal = true;
			for (int i=0; i< storedPatternLineArray.Length; i++) {
				if (storedPatternLineArray[i] != newPatternLineArray[i]) {
					equal = false;
					break;
				}
			}
			return equal;
		}
		/// <sumary>
		/// Finds the index of the pattern (if previously stored)
		/// </sumary>		
		private static int IndexOfPattern(ArrayList patternData, char[] newPatternLineArray) {
			int foundIndex = -1;
			for (int i=0; i< patternData.Count; i++) {
				char[] storedPatternLineArray = (char[]) patternData[i];
				if (ComparePatternLineArrays(storedPatternLineArray, newPatternLineArray)) {
					foundIndex = i;
					break;
				}
			}
			return foundIndex;
		}
		/// <sumary>
		/// Remove the filename extension (if specified)
		/// </sumary>		
		private static string RemoveFileExtension(string inputFileName) {
			string outputFileName = inputFileName;
			if (inputFileName.IndexOf('.') > -1) {
				string[] filenameExtension = inputFileName.Split('.');
				outputFileName = filenameExtension[0];
			}
			return outputFileName;
		}
		
		//*********** P U B L I C   F U N C T I O N S  ( M E T H O D S ) ******
		/// <sumary>
		/// Main (entrypoint)	
		/// </sumary>
		public static void Main(string[] arguments) {
			if (arguments.Length > 0) {
				bool continueProcessing = true;
				ArrayList patternData = new ArrayList();
				ArrayList mapData = new ArrayList();
				ArrayList enemyData = new ArrayList();
				ArrayList songData = new ArrayList();
				
				ArrayList bossFiles = new ArrayList();
				ArrayList bossMovements = new ArrayList();
				ArrayList bossData = new ArrayList();
				ArrayList bossMovementData = new ArrayList();
				ArrayList musicFiles = new ArrayList();
				
				string outputFileName = "";
				string fileName = "";
				int sectionLength = 0;
				int screenBlockSize = 0;
				int levelBlockSize = 0;
				int currentPatternIndex = 0;
				string outputType = "";
				 
				fileName = arguments[0];
				outputFileName = RemoveFileExtension(fileName);
				
				try {
					if (arguments.Length > 1) {
						sectionLength = Convert.ToInt32(arguments[1]);
					} else {
						//assume a default length
						sectionLength = 16;
					}
				} catch {
					Console.WriteLine ("Error - If specified, section length should be a numeric value.");
				}
				
				try {
					if (arguments.Length > 2) {
						screenBlockSize = Convert.ToInt32(arguments[2]);
					} else {
						//assume a default size
						screenBlockSize = 24;
					}
				} catch {
					Console.WriteLine ("Error - If specified, screen block size should be a numeric value.");
				}	

				try {
					if (arguments.Length > 3) {
						levelBlockSize = Convert.ToInt32(arguments[3]);
					} else {
						//assume a default size
						levelBlockSize = 192;
					}
				} catch {
					Console.WriteLine ("Error - If specified, level block size should be a numeric value.");
				}				
				
				if (arguments.Length > 4) {
					outputType = arguments[4].ToUpper();
					switch (outputType) {
						case "BIN":
							break;
						
						case "SPIN":
							outputFileName += ".spin";
							break;
						
						default:
							Console.WriteLine ("Error - If specified, output type must be either 'bin' or 'spin'");
							continueProcessing = false;
							break;
					}
				} else {
					outputType = "BIN";
				}
				
				if (continueProcessing) {
					int lineNumber = 0;
					int processedLineNumber = 0;
					int relativeLineNumber = 0;
					int maximumRelativeLineNumber = 0;
					int blockCount = 0;
					
					using (StreamReader streamReader = new StreamReader(fileName)) {
						string lineData;
						bool isCommand = false;
						while((lineData = streamReader.ReadLine()) != null) {
							//if this is not a comment -- process
							lineNumber++;
							isCommand = false;
							
							//to do: read the first character (  / indicates a command, ! indicates comment)
							//      /s - screen
							//     /l  - level (resets the enemy index, so that they are 0-based for the level)
							switch (lineData.Substring(0,1)) {
								case "/":
								   //we know that we are processing a new block, so increase the count
								   blockCount++;
								   
								   switch (lineData.Substring(1,1).ToUpper()) {
								         case "S":
											 if (lineData.Length > 2) {
											    string thisScreenInformation = lineData.Substring(3, (lineData.Length - 3));
												string [] screenComponents = thisScreenInformation.Split (' ');
												if (screenComponents.Length > 0) {
													string thisScreenFileName = screenComponents[0];
													if (thisScreenFileName != "") musicFiles.Add(thisScreenFileName);
												}
											}
										    isCommand = true;
										    relativeLineNumber = 0;
                                            maximumRelativeLineNumber = screenBlockSize;											
                                            break;

                                         case "L":
											 if (lineData.Length > 2) {
											    string thisLevelInformation = lineData.Substring(3, (lineData.Length - 3));
												string [] levelComponents = thisLevelInformation.Split (' ');
												if (levelComponents.Length > 0) {
													string thisLevelFileName = levelComponents[0];
													if (thisLevelFileName != "") musicFiles.Add(thisLevelFileName);
												}						
											}		
										    isCommand = true;
										    relativeLineNumber = 0;
											maximumRelativeLineNumber = levelBlockSize;
                                            break;
											
										case "B":
											isCommand = true;
											string thisBossInformation = lineData.Substring(3, (lineData.Length - 3));
											string [] bossComponents = thisBossInformation.Split(' ');
											if (bossComponents.Length > 0) {
												string thisFileName = bossComponents[0];
												if (thisFileName != "")	bossFiles.Add(thisFileName);
											}
											
											if (bossComponents.Length > 1) {
												string[] thisMovementArray = bossComponents[1].Split(',');
												bossMovements.Add(thisMovementArray);
											}
											break;
								   }
								   break;
								   
								case "!":
								   isCommand = true;
								   break;
							}
							
							if (!isCommand) {
								char[] newPatternLineArray = lineData.ToCharArray();

								if (newPatternLineArray.Length > sectionLength) {
									Console.WriteLine ("Error in line data (line  "+lineNumber+") - lines must be no longer than " + sectionLength);
									continueProcessing = false;
									break;
								} else {
									if (newPatternLineArray.Length == sectionLength) {
										if (patternData.Count > 0) {
										
											processedLineNumber++;
											relativeLineNumber++;
											if ((relativeLineNumber - 1) <= maximumRelativeLineNumber) {
												//reverse the array for use in generating data (corrects coordinates)
												Array.Reverse(newPatternLineArray);	
												
												//search for any 'enemy' data that may be specified, and process
												for(int i=0; i< sectionLength; i++) {
													int elementValue = (int)newPatternLineArray[i];
													if ((elementValue >= 48) && (elementValue <= 63)) {
														//replace the element in the newPatternLineArray with space (32)
														newPatternLineArray[i] = (char)32;
														
														MapEnemy newEnemy = new MapEnemy(relativeLineNumber - 1, elementValue - 48, i);
														enemyData.Add(newEnemy);
														newEnemy = null;
													}
												}
																				
												int searchPatternIndex = IndexOfPattern(patternData, newPatternLineArray);
												
												//not found in the stored pattern table, so add to the pattern table
												if (searchPatternIndex == -1) {
													currentPatternIndex++;
													patternData.Add(newPatternLineArray);
													mapData.Add(currentPatternIndex);
												} else {
													//otherwise, we only add to the map data with the index to the pattern
													mapData.Add(searchPatternIndex);
												}
											} else {
												Console.WriteLine ("Warning: (line  "+lineNumber+"): maximum block length (" + maximumRelativeLineNumber + ") exceeded on block " + blockCount + " -- skipped");	
											}
											
										} else {
											//assume that this is the first entry -- add into the array(s)
											patternData.Add(newPatternLineArray);
											mapData.Add(currentPatternIndex);
										}
									} else {
										Console.WriteLine ("Warning: line  "+lineNumber+" is not " + sectionLength + " characters long -- skipped");	
									}
								}
							} 
						}

						if (bossFiles.Count > 0) {
							int [] xpos = new int[8];
							int [] ypos = new int[8];
							xpos[0] = 0;
							ypos[0] = 0;
							xpos[1] = 8;
							ypos[1] = 0;
							xpos[2] = 0;
							ypos[2] = 8;
							xpos[3] = 8;
							ypos[3] = 8;
							xpos[4] = 16;
							ypos[4] = 0;
							xpos[5] = 24;
							ypos[5] = 0;
							xpos[6] = 16;
							ypos[6] = 8;
							xpos[7] = 24;
							ypos[7] = 8;
							
							BossCharacterLine thisLine;
							BossCharacter thisCharacter;
							
							foreach (string bossFileName in bossFiles) {
								bool continueProcessingBitmap = true;
								Console.WriteLine("Processing boss (image) file: " + bossFileName);
								Bitmap bitmap = new Bitmap( bossFileName );
								if ((bitmap.Width != 32)) {
									Console.WriteLine("Error - source bitmap must be 32 pixels wide.");
									continueProcessingBitmap = false;								
								}
								if ((bitmap.Height != 16)) {
									Console.WriteLine("Error - source bitmap must be 16 pixels tall.");
									continueProcessingBitmap = false;								
								}

								if (continueProcessingBitmap) {
									
									for (int i = 0; i < 8; i++) {
									    thisCharacter = new BossCharacter();										
										string binaryString = "";
										string picString = "";
										
										for (int row = ypos[i]; row < ypos[i] + 8; row++) {
											//read these backwards because they are inverted for display
											for (int col = 7+xpos[i]; col >= xpos[i]; col--) {
												Color pixelColor = bitmap.GetPixel(col, row);
														
												if ((pixelColor.R == 255) && (pixelColor.G == 255) && (pixelColor.B == 255)) {
													//pixel is white (color 1)
													picString += "*";
													binaryString += "01";													
												} else {
													if ((pixelColor.R == 255) && (pixelColor.G == 0) && (pixelColor.B == 0)) {
														//pixel is red (color 2)
														picString += "$";
														binaryString += "10";										
													} else {
														if ((pixelColor.R == 0) && (pixelColor.G == 0) && (pixelColor.B == 255)) {
															//pixel is blue (color 3)
															picString += "#";
															binaryString += "11";										
														} else {
															//pixel is black (color 0)
															picString +=  ".";
															binaryString += "00";
														}
													}
												}				
											}
											thisLine = new BossCharacterLine(picString, binaryString);
											thisCharacter.Lines.Add(thisLine);
											binaryString = "";
											picString = "";
										}
										bossData.Add(thisCharacter);
										thisCharacter = null;
									}
								}
								bitmap = null;
							}	
							
							//loop through each of the defined boss characters and define their movement
							for (int i=0; i< bossMovements.Count; i++) {
								if (bossMovements[i] != null) {
									int thisMovement = -1;
									string[] thisBossMovements = (string[]) bossMovements[i];
									foreach (string thisBossMovement in thisBossMovements) {
										thisMovement = -1;
										switch (thisBossMovement) {
											case "N":
												thisMovement = 1;
												break;
												
											case "NE":
												thisMovement = 2;
											    break;
												
											case "E":
												thisMovement = 3;
											    break;
												
											case "SE":
												thisMovement = 4;
											    break;
												
											case "S":
												thisMovement = 5;
											    break;
												
											case "SW":
												thisMovement = 6;
											    break;
												
											case "W":
												thisMovement = 7;	
											    break;
												
											case "NW":
												thisMovement = 8;
											    break;
											
											default:
											    Console.WriteLine("Warning: Unrecognized command '" + thisBossMovement + "' in boss movement; ignored.");
												break;
										}
										if (thisMovement > -1) bossMovementData.Add(thisMovement);
									}
								}
							}
						}
						
						if (musicFiles.Count > 0) {
							StreamReader objReader;
							foreach (string musicFileName in musicFiles) {
								int defaultNoteLength = 0;
								int defaultOctave = 0;
								int beatsPerMinute = 0;
						
								Console.WriteLine("Processing music file (RTTTL format) file: " + musicFileName);
								try {
									objReader = new StreamReader(musicFileName);
									string musicFileData = objReader.ReadToEnd();
									objReader = null;
									
									string [] musicDataElements = musicFileData.Split(':');
									//skip first element (title), since we cannot use it
									
									//second section includes defaults
									if (musicDataElements.Length > 1) {
										string [] musicDefaultDataSubElements = musicDataElements[1].Split(',');
										//each of the sub elments are divided by an = sign from their value
										foreach (string musicDefaultSubElement in musicDefaultDataSubElements) {
											string [] musicDefaultParameter = musicDefaultSubElement.Split('=');
											if (musicDefaultParameter.Length > 1) {
												switch (musicDefaultParameter[0].ToUpper()) {
													//default note length
													case "D":
														defaultNoteLength = Convert.ToInt32(musicDefaultParameter[1]);
														break;
														
													case "O":
														defaultOctave = Convert.ToInt32(musicDefaultParameter[1]);
														break;

													case "B":
														beatsPerMinute = Convert.ToInt32(musicDefaultParameter[1]);
														break;														
													
													default:
														Console.WriteLine("Warning - unrecognized command '" + musicDefaultParameter[0] + "' - skipped");
														break;
												}
											}
										}
										
									}
									Song thisSong = new Song();
									thisSong.BeatsPerMinute = beatsPerMinute;
									
									//third section includes the actual song data
									if (musicDataElements.Length > 2) {
										string [] musicNotesSubElements = musicDataElements[2].Split(',');
										for (int j=0; j < musicNotesSubElements.Length; j++) {
											string musicNoteSubElement = (string) musicNotesSubElements[j];
											bool noteFound = false;

											string octaveString = "";
											string lengthString = "";
											string noteString = "";
											
											foreach (char musicNoteSubCharacter in musicNoteSubElement) {
												
												//handle characters below a specific range
												if (musicNoteSubCharacter < 32) {
													Console.WriteLine("Warning - unrecognized character in sub element - skipped");
												} else {
													if (musicNoteSubCharacter == 35) noteString += "#";
													
													//numeric digit
													if ((musicNoteSubCharacter > 47) && (musicNoteSubCharacter < 58)) {
														
														//if the note portion has already been found, this is now the octive being specified
														if (noteFound) {
															octaveString += (char) musicNoteSubCharacter;
														} else {
															//otherwise, this is the note length being specified
															lengthString += (char) musicNoteSubCharacter;
														}
													}
													
													//leter (a-h)
													if ((musicNoteSubCharacter > 96) && (musicNoteSubCharacter <= 123)) {
														noteFound = true;
														noteString += (char) musicNoteSubCharacter;
														
													}
													
												}
												
											}
											
											int octaveValue = defaultOctave;
											int lengthNumber = defaultNoteLength;
											int lengthValue = 0;
											int noteValue = 0;
											int eofFlagValue = 0;
											
											//if values were specified for this note, use them
											if (octaveString != "") octaveValue = Convert.ToInt32(octaveString);
											if (lengthString != "") lengthNumber = Convert.ToInt32(lengthString);
	
											//translate the note values to an integer
											switch (noteString) {
												case "p":
													noteValue = 0;
													break;
													
												case "a":
													noteValue = 1;
													break;
													
												case "a#":
													noteValue = 2;
													break;
													
												case "b":
													noteValue = 3;
													break;
													
												case "c":
													noteValue = 4;
													break;
													
												case "c#":
													noteValue = 5;
													break;
												
												case "d":
													noteValue = 6;
													break;
													
												case "d#":
													noteValue = 7;
													break;
													
												case "e":
													noteValue = 8;
													break;
													
												case "f":
													noteValue = 9;
													break;
													
												case "f#":
													noteValue = 10;
													break;
													
												case "g":
													noteValue = 11;
													break;
													
												case "g#":
													noteValue = 12;
													break;									
											}
											
											//translate the length value into integer
											switch (lengthNumber) {
												case (int)64:
													lengthValue = 0;
													break;
												
												case (int)32:
													lengthValue = 1;
													break;
												
												case (int)16:
													lengthValue = 2;
													break;
													
												case (int)8:
													lengthValue = 3;
													break;
													
												case (int)4:
													lengthValue = 4;
													break;

												case (int)2:
													lengthValue = 5;
													break;
													
												case (int)1:
													lengthValue = 6;
													break;
											}
											
											//if this is the last note, mark the EOF byte
											if (j == (musicNotesSubElements.Length-1)) eofFlagValue = 1;
											
											Note thisNote = new Note();
											thisNote.NoteValue = noteValue;
											thisNote.Octave = octaveValue;
											thisNote.Duration = lengthValue;
											thisNote.EOFFlag = eofFlagValue;
											thisSong.Notes.Add(thisNote);
											thisNote = null;
											//Console.WriteLine("Info - note: " + noteValue + " octave:" + octaveValue + " length:" + lengthValue);	
										}
									}
									songData.Add(thisSong);
									thisSong = null;
									
								} catch {
									Console.WriteLine ("Unable to find/process file: " + musicFileName);
									objReader = null;
								}
							}
						}
						
						Console.WriteLine("Total lines: "+lineNumber+"  Map lines processed - " + (processedLineNumber + 1) + " (" +(currentPatternIndex + 1) + " distinct patterns - enemies added: "+ enemyData.Count +")");
						if (bossData.Count > 0) Console.WriteLine("Total boss characters added: "+ bossData.Count);
						//if (musicData.Count > 0) Console.WriteLine("Total music files added: "+ musicData.Count);
					}

					if (continueProcessing) {
						try {
								
							switch (outputType) {
								case "SPIN":
									string patternDataString = WritePatternData_Text(patternData);
									string mapDataString = WriteMapData_Text(mapData);
									string enemyDataString = WriteEnemyData_Text(enemyData);
									string bossDataString = "";
									if (bossData.Count > 0) bossDataString = WriteBossData_Text(bossData);
										
									StreamWriter streamWriter = new StreamWriter(outputFileName);
									streamWriter.WriteLine("' Map generated by MapGenerator "+VersionText()+" [.spin output]");
									streamWriter.WriteLine("'    include this in the DAT section of your code");
									streamWriter.Write(patternDataString);
									streamWriter.Write(mapDataString);
									streamWriter.Write(enemyDataString);
									if (bossDataString != "") streamWriter.Write(bossDataString);
									streamWriter.WriteLine("' End of data");
									streamWriter.Close();
									streamWriter = null;
									break;
									
								case "BIN":
									BinaryWriter binaryWriter;
									using (FileStream patternFileStream = new FileStream(outputFileName + ".pat", FileMode.CreateNew)) {
										binaryWriter = new BinaryWriter(patternFileStream);
										foreach (char[] storedPatternArray in patternData) {
											foreach (char patternChar in storedPatternArray) {
												binaryWriter.Write((byte) patternChar);
											}
										}
										binaryWriter.Close();
										binaryWriter = null;
									}
									using (FileStream mapFileStream = new FileStream(outputFileName + ".map", FileMode.CreateNew)) {
										binaryWriter = new BinaryWriter(mapFileStream);
										foreach (int mapIndex in mapData) {
											binaryWriter.Write((byte) mapIndex);
										}
										binaryWriter.Close();
										binaryWriter = null;
									}
									
									using (FileStream mapFileStream = new FileStream(outputFileName + ".emy", FileMode.CreateNew)) {
										binaryWriter = new BinaryWriter(mapFileStream);
										foreach (MapEnemy thisEnemy in enemyData) {
											binaryWriter.Write((byte) thisEnemy.EnemySection);
											binaryWriter.Write((byte) thisEnemy.EnemyAlive);
											binaryWriter.Write((byte) thisEnemy.EnemyType);
											binaryWriter.Write((byte) thisEnemy.EnemyAction);
											binaryWriter.Write((byte) thisEnemy.EnemyXPos);
											binaryWriter.Write((byte) thisEnemy.EnemyYPos);
											binaryWriter.Write((byte) thisEnemy.EnemyFrame);
											binaryWriter.Write((byte) thisEnemy.EnemyLife);
										}
										//pad for fixed file size
										if (enemyData.Count < 8) {
											int padCount = (8 - enemyData.Count) * 8;
											for (int i = 0; i< padCount; i++) {
												binaryWriter.Write((byte)0);
											}
										}
										binaryWriter.Close();
										binaryWriter = null;
									}
									
									if (bossData.Count > 0) {
										using (FileStream bossFileStream = new FileStream(outputFileName + ".bos", FileMode.CreateNew)) {
											binaryWriter = new BinaryWriter(bossFileStream);
											foreach (BossCharacter thisCharacter in bossData) {
												foreach (BossCharacterLine thisLine in thisCharacter.Lines) {
													binaryWriter.Write((short) thisLine.LineValue);
												}
											}
											binaryWriter.Close();
											binaryWriter = null;
										}

										using (FileStream bossFileStream = new FileStream(outputFileName + ".bmv", FileMode.CreateNew)) {
											binaryWriter = new BinaryWriter(bossFileStream);
											foreach (int thisMovement in bossMovementData) {
													binaryWriter.Write((byte) thisMovement);
											}
											binaryWriter.Close();
											binaryWriter = null;
										}
									}
									
									if (songData.Count > 0) {
										using (FileStream songFileStream = new FileStream(outputFileName + ".mus", FileMode.CreateNew)) {
											binaryWriter = new BinaryWriter(songFileStream);
											foreach (Song thisSong in songData) {
												binaryWriter.Write((int) thisSong.BeatsPerMinute);
												foreach (Note thisNote in thisSong.Notes) {
													binaryWriter.Write((byte) thisNote.NoteValue);
													binaryWriter.Write((byte) thisNote.Octave);
													binaryWriter.Write((byte) thisNote.Duration);
													binaryWriter.Write((byte) thisNote.EOFFlag);
												}
											}
											binaryWriter.Close();
											binaryWriter = null;
										}

										using (FileStream songIndexFileStream = new FileStream(outputFileName + ".mix", FileMode.CreateNew)) {
											binaryWriter = new BinaryWriter(songIndexFileStream);
											int startPosition = 0;
											
											//start of song x is the length of x-1
											for (int i=0; i< songData.Count - 1; i++) {
												Song thisSong = (Song) songData[i];
												//add in long (4 bytes) for tempo at the beginning of the song
												startPosition +=4;
												
												//account for the 4 bytes for each of the note elements
												startPosition += (thisSong.Notes.Count * 4);
												
												//write out length (in bytes) of the entry
												binaryWriter.Write((int) startPosition);
											}
											binaryWriter.Close();
											binaryWriter = null;
										}
										
									}
									break;
							}
								
								
						} catch (Exception e){
							Console.WriteLine ("Error in creating output file(s) - " + e.ToString());
						}
					}
				}
			} else { 
				Console.WriteLine ("MapGenerator " + VersionText());
				Console.WriteLine ("Usage: MapGenerator.exe [input_filename] ([section_length]) ([screen_block_size]) ([level_block_size]) ([output_type])");
				Console.WriteLine (" [section_length] - length of each section (16 assumed, if nothing specified).");
				Console.WriteLine (" [screen_block_size] - number of sections that compose a screen (24 assumed, if nothing specified).");
				Console.WriteLine (" [level_block_size] - number of sections that compose a level (192 assumed, if nothing specified).");
				Console.WriteLine (" [output_type] may be defined as either 'spin' or 'bin' ('bin' assumed, if nothing specified).");
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
