using System;
using System.Collections;
//****************************************
// PINTBasicApplication
// 2010 trodoss
//See end of file for terms of use.  
//***************************************
namespace PINTCompiler.PINTBasic {
	public class PINTBasicApplication {
		public PINTBasicRoomList Rooms;
		public PINTBasicPicList Pics;
		public PINTBasicByteList Variables;
		public PINTBasicConstantList Constants;
		public PINTBasicItemList Items;
		public PINTBasicTextList Texts;
		public PINTBasicMusicList Musics;
		
		private int maxConstantID;
		private int maxVariableID;
		private int maxPicID;
		private int maxItemID;
		private int maxTextID;
		private int maxMusicID;
		
		public PINTBasicApplication () {
			maxConstantID = 0;
			maxVariableID = 0;
			maxTextID = 0;
			maxItemID = 0;
			maxPicID = 0;
			maxMusicID = 0;
			this.Rooms = new PINTBasicRoomList();
			this.Pics = new PINTBasicPicList();
			this.Variables = new PINTBasicByteList();
			this.Constants = new PINTBasicConstantList();
			this.Texts = new PINTBasicTextList();
			this.Items = new PINTBasicItemList();
			this.Musics = new PINTBasicMusicList();
		}
				
		public void AddConstant(string constantName, int constantValue) {
			this.Constants.Add(new PINTBasicConstant(maxConstantID, constantName, constantValue));
			maxConstantID++;
		}

		public int AddText(string text) {
			int returnID = maxTextID;
			this.Texts.Add(new PINTBasicText(maxTextID, text));
			maxTextID++;
			return returnID;
		}
		
		public bool AddVariable(string variableName) {
			this.Variables.Add(new PINTBasicByte(maxVariableID, variableName));
			maxVariableID++;
			
			//if we have exceeded the number of global variables, then let the compiler know
			if (maxVariableID > 8) {
				return false;
			} else {
				return true;
			}
			
		}
		
		public bool AddPic(string picName, string fileName) {
			this.Pics.Add(new PINTBasicPic(maxPicID, picName, fileName));
			maxPicID++;
			
			//if we have exceeded the number of pics, then let the compiler know
			if (maxPicID > 6) {
				return false;
			} else {
				return true;
			}
			
		}
		
		public bool AddItem(string itemName, string text) {
			this.Items.Add(new PINTBasicItem(maxItemID, itemName, text));
			maxItemID++;
			
			//if we have exceeded the number of items, then let the compiler know
			if (maxItemID > 7) {
				return false;
			} else {
				return true;
			}		
		}	

		public void AddMusic(string musicName, string fileName) {
			this.Musics.Add(new PINTBasicMusic(maxMusicID, musicName, fileName));
			maxMusicID++;		
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
