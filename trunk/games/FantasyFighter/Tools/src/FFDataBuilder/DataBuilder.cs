using System;
using System.Collections;
using System.Text;
using System.IO;
using PropellerPowered.General;
using PropellerPowered.Sound;
using PropellerPowered.Maps;
//****************************************
// DataBuilder
//
//Copyright (c) 2010 trodoss
//See end of file for terms of use.  
//***************************************

namespace FFDataBuilder {
	//*********************************************************************
	// DataBuilder Class
	//*********************************************************************	
	public class DataBuilder {
		/// <sumary>
		/// Retrieves the version text 
		/// </sumary>
		private static string VersionText() { return "v0.1"; }
		
		private static int EncodeInByte(int number1, int number2) {
			int returnNumber = number1 << 4;
			returnNumber += number2;
			return returnNumber;
		}
	
		private static void GenerateSongs(FFSongList songList) {
			ArrayList songLengths = new ArrayList();
			PropellerPowered.Sound.Song song;
			int songLength = 0;
			
			foreach (FFSong thisSong in songList) {
				songLength = 0;
				song = new PropellerPowered.Sound.Song();
				try {	
					Console.WriteLine(thisSong.ID + " " + thisSong.FileName);
					song = RTTTLReader.Read(thisSong.FileName);
				} catch {
					Console.WriteLine("Unable to load file: " + thisSong.FileName );
				}
				
				songLength = SimpleSoundWriter.Write(song, "ffdata.snd", FileMode.Append);
				songLengths.Add(songLength);				
			}
			
			//write sound index file
			IndexWriter.Write(songLengths, "ffdata.six");
			
		}
		
		private static void GenerateMaps(FFMapList mapList) {
			ArrayList mapLengths = new ArrayList();
			PropellerPowered.Maps.Map map;
			int mapLength = 0;
			
			foreach (FFMap thisMap in mapList) {
				map = new PropellerPowered.Maps.Map();
				try {
					map = GBMReader.Read(thisMap.FileName);
				} catch {
					Console.WriteLine("Unable to load file: " + thisMap.FileName);
				}
				//set the reduction to RLE (Run-length encoding)
				map.IncludeDimensions = false;
				map.Reduction = Map.ReductionType.RLE;
				map = RLEReducer.Reduce(map);
				
				mapLength = MapScrollXYWriter.Write(map, "ffdata.map", FileMode.Append);
				mapLengths.Add(mapLength);
				Console.WriteLine("Map Length: " + mapLength);
			}
			
			//write sound index file
			IndexWriter.Write(mapLengths, "ffdata.mix");			
			
		}
		
		private static void GenerateEventData(FFMapList mapList) {
			ArrayList eventLengths = new ArrayList();
			
			int eventLength = 0;
			foreach (FFMap thisMap in mapList) {
				try {
					using (BinaryWriter binaryWriter = new BinaryWriter(File.Open("ffdata.evt", FileMode.Append))) {
						//writing header data -- music ID, monsters, and event count
						binaryWriter.Write((byte)thisMap.MusicID);
						eventLength++;
						
						if (thisMap.HasRandomMonsters) {
							binaryWriter.Write((byte)1);
						}else{
							binaryWriter.Write((byte)1);
						}
						eventLength++;
						
						binaryWriter.Write((byte)thisMap.Events.Count);
						eventLength++;
						
						binaryWriter.Write((byte)thisMap.Monsters.Count);
						eventLength++;						
						
						foreach (FFEvent thisEvent in thisMap.Events) {
							binaryWriter.Write((byte)thisEvent.X);
							binaryWriter.Write((byte)thisEvent.Y);
							
							if (thisEvent is FFExitEvent) {
								FFExitEvent thisExitEvent = (FFExitEvent) thisEvent;
							
								//encoding 0 = EXIT_EVENT
								//second = event ID (used when loading a new map, to find the starting x/y for the player)
								binaryWriter.Write((byte)EncodeInByte(0,thisExitEvent.EventID));
								binaryWriter.Write((byte)thisExitEvent.MapID);
							}
							
							if (thisEvent is FFTextEvent) {
								FFTextEvent thisTextEvent = (FFTextEvent) thisEvent;
								//encoding 1 = TEXT_EVENT
								//second = 0 (unused)
								binaryWriter.Write((byte)EncodeInByte(1,0));
								binaryWriter.Write((byte)thisTextEvent.TextID);		
							}
							
							if (thisEvent is FFShopEvent) {
								FFShopEvent thisShopEvent = (FFShopEvent) thisEvent;
								//encoding 2 = SHOP_EVENT
								//second = 0 (unused)
								binaryWriter.Write((byte)EncodeInByte(2,0));
								binaryWriter.Write((byte)thisShopEvent.Level);		
							}							

							if (thisEvent is FFHealEvent) {
								FFHealEvent thisHealEvent = (FFHealEvent) thisEvent;
								//encoding 3 = HEAL_EVENT
								//second = 0 (unused)
								binaryWriter.Write((byte)EncodeInByte(3,0));
								binaryWriter.Write((byte)0);		
							}	

							if (thisEvent is FFMonsterEvent) {
								FFMonsterEvent thisMonsterEvent = (FFMonsterEvent) thisEvent;
								//encoding 4 = MONSTER_EVENT
								//second = 0 (unused)
								binaryWriter.Write((byte)EncodeInByte(4,0));
								binaryWriter.Write((byte) thisMonsterEvent.MonsterID);		
							}							
								
							eventLength +=4;
						}
						
						foreach (FFMonster thisMonster in thisMap.Monsters) {
							binaryWriter.Write((byte)EncodeInByte(thisMonster.MonsterType,thisMonster.ElementType));
							binaryWriter.Write((byte)thisMonster.HitPoints);
							binaryWriter.Write((short)thisMonster.Gold);
							eventLength +=4;							
						}
						
						eventLengths.Add(eventLength);
						eventLength = 0;
					}
				} catch {
					Console.WriteLine("Unable to write ffdata.evt data");
				}
			}
			
			//write sound index file
			IndexWriter.Write(eventLengths, "ffdata.eix");				
		}
		
		private static void WriteSharedStringTable(FFGame thisGame) {
			ArrayList entryLengths = new ArrayList();
			int entryLength = 0;
			ASCIIEncoding encoding = new ASCIIEncoding();
			try {
				using (BinaryWriter binaryWriter = new BinaryWriter(File.Open("ffdata.sst", FileMode.Append))) {
					foreach (FFSharedString thisString in thisGame.Strings) {
						byte[] stringData = encoding.GetBytes(thisString.Text);
						binaryWriter.Write(stringData);
						binaryWriter.Write((byte)0);
						entryLength = stringData.Length + 1;
						entryLengths.Add(entryLength);
						entryLength = 0;									
					}
				}
			} catch {
				Console.WriteLine("Unable to write ffdata.sst data");
			}
			encoding = null;
			
			//write shared string index file
			IndexWriter.Write(entryLengths, "ffdata.ssx");	
		}
		
		private static void WriteItemData(FFGame thisGame) {
			try {
				using (BinaryWriter binaryWriter = new BinaryWriter(File.Open("ffdata.itm", FileMode.Append))) {
					foreach (FFItem thisItem in thisGame.Items) {
					
						binaryWriter.Write((byte)EncodeInByte(thisItem.Level, thisItem.ItemType));
						binaryWriter.Write((byte)thisItem.Element);
						binaryWriter.Write((short)thisItem.Cost);							
					}
				}
			} catch {
				Console.WriteLine("Unable to write ffdata.itm data");
			}
		}
		
		//*********** P U B L I C   F U N C T I O N S  ( M E T H O D S ) ******
		/// <sumary>
		/// Main (entrypoint)	
		/// </sumary>
		public static void Main(string[] arguments) {
			string fileName = "";
			
			if (arguments.Length > 0) {
				fileName = arguments[0];
				FFGame thisGame = Parser.Parse(fileName);
				if (thisGame.CanProcess) {
					//generate song data
					GenerateSongs(thisGame.Songs);
					
					//generate map data
					GenerateMaps(thisGame.Maps);
					
					GenerateEventData(thisGame.Maps);
					
					WriteSharedStringTable(thisGame);
					
					WriteItemData(thisGame);
				}
			} else {
				Console.WriteLine("FFDataBuilder " + VersionText() + " - no file name specified. ");
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
