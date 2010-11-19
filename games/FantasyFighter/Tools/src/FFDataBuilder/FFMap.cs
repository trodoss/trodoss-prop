using System;
using System.Collections;
//****************************************
// FFGameObject
//Copyright (c) 2010 trodoss
//See end of file for terms of use.  
//***************************************
namespace FFDataBuilder {
	public class FFMap : FFGameObject {
		public string FileName;
		public int MusicID;
		public bool HasRandomMonsters;
		public FFMonsterList Monsters;
		public FFEventList Events;
	
		private int maxEventID = 0;
		private int maxMonsterID = 0;
		
		public FFMap(int id, string identifier, string fileName, int musicID, bool hasRandomMonsters) {
			this.ID = id;
			this.Identifier = identifier;
			this.FileName = fileName;
			this.MusicID = musicID;
			this.HasRandomMonsters = hasRandomMonsters;
			this.Monsters = new FFMonsterList();
			this.Events = new FFEventList();
		}
		
		public void AddUnresolvedExitEvent(string identifier, int x, int y, int lineNumber, string mapIdentifier, string eventIdentifier) {
			FFUnresolvedExitEvent thisEvent = new FFUnresolvedExitEvent(maxEventID, identifier, x, y, lineNumber, mapIdentifier, eventIdentifier);
			this.Events.Add(thisEvent);
			maxEventID++;
			thisEvent = null;
		}	

		public void AddTextEvent(string identifier, int x, int y, int textID) {
			FFTextEvent thisEvent = new FFTextEvent(maxEventID, identifier, x, y, textID);
			this.Events.Add(thisEvent);
			maxEventID++;
			thisEvent = null;
		}
		
		public void AddShopEvent(string identifier, int x, int y, int level) {
			FFShopEvent thisEvent = new FFShopEvent(maxEventID, identifier, x, y, level);
			this.Events.Add(thisEvent);
			maxEventID++;
			thisEvent = null;
		}	

		public void AddHealEvent(string identifier, int x, int y) {
			FFHealEvent thisEvent = new FFHealEvent(maxEventID, identifier, x, y);
			this.Events.Add(thisEvent);
			maxEventID++;
			thisEvent = null;
		}	

		public void AddMonsterEvent(string identifier, int x, int y, int monsterID) {
			FFMonsterEvent thisEvent = new FFMonsterEvent(maxEventID, identifier, x, y, monsterID);
			this.Events.Add(thisEvent);
			maxEventID++;
			thisEvent = null;
		}	
		
		public void AddMonster(string identifier, int monsterType, int elementType,  int hitPoints, int gold) {
			FFMonster thisMonster = new FFMonster(maxMonsterID, identifier, monsterType, elementType,  hitPoints, gold);
			this.Monsters.Add(thisMonster);
			maxMonsterID++;
			thisMonster = null;
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
