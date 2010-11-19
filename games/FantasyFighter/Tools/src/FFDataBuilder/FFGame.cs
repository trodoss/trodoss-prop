using System;
using System.Collections;
//****************************************
// FFGameObject
//Copyright (c) 2010 trodoss
//See end of file for terms of use.  
//***************************************
namespace FFDataBuilder {
	public class FFGame {
		public Hashtable Constants;
		public FFMapList Maps;
		public FFSongList Songs;
		public FFSharedStringList Strings;
		public FFItemList Items;
		public bool CanProcess;
		
		private int maxSongID = 3;
		private int maxMapID = 0;
		private int maxItemID = 0;
		
		public FFGame() {
			this.Constants = new Hashtable();
			this.Maps = new FFMapList();
			this.Songs = new FFSongList();
			this.Strings = new FFSharedStringList();
			this.Items = new FFItemList();
			this.CanProcess = true; 
		}
		
		public void AddSong (string identifier, string filename, int type) {
			FFSong.SongType songType = FFSong.SongType.Normal;
			switch (type) {
				case 0:
					songType = FFSong.SongType.Main;
					break;
					
				case 1:
					songType = FFSong.SongType.Battle;
					break;
					
				case 2:
					songType = FFSong.SongType.Victory;
					break;
				
				case -1:
					songType = FFSong.SongType.Normal;
					break;
			}
			FFSong addSong;
			
			if (type == -1) {
				addSong = new FFSong(maxSongID, identifier, filename, songType);
				maxSongID++;
			} else {
				//we are overriding the numbering sequence, putting this one in a specific location
				addSong = new FFSong(type, identifier, filename, songType);
			}
			this.Songs.Add(addSong);
			addSong = null;
				
		}
		public void AddItem(string identifier, int level, int itemType, int element, int cost) {
			FFItem thisItem = new FFItem(maxItemID, identifier, level, itemType, element, cost);
			this.Items.Add(thisItem);
			maxItemID++;
			thisItem = null;
		}
		
		public int AddMap(string identifier, string fileName, int musicID, bool hasRandomMonsters) {
			int thisMapID = maxMapID;
			FFMap thisMap = new FFMap(maxMapID, identifier, fileName, musicID, hasRandomMonsters);
			this.Maps.Add(thisMap);
			maxMapID++;
			thisMap = null;
			return thisMapID;
		}
		
		public int AddString(string text) {
			int thisStringID = 0;
			FFSharedString foundString = this.Strings.FindByText(text);
			if (foundString == null) {	
				thisStringID = GetMaxStringID() + 1;
				FFSharedString thisString = new FFSharedString(thisStringID, text);
				this.Strings.Add(thisString);
				thisString = null;
			} else {
				thisStringID = foundString.ID;
			}
			return thisStringID;
		}
		
		public void AddString(string text, int thisStringID) {
			FFSharedString foundString = this.Strings.FindByText(text);
			if (foundString == null) {
				FFSharedString thisString = new FFSharedString(thisStringID, text);
				this.Strings.Add(thisString);
				thisString = null;
			}
		}
		
		public int GetMaxStringID() {
			int maxStringID = 0;
			foreach (FFSharedString thisString in this.Strings) {
				if (thisString.ID > maxStringID) maxStringID = thisString.ID;
			}
			return maxStringID;
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
