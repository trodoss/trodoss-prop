using System;
using System.Collections;
using System.IO;
//****************************************
//trodoss - 2010
//See end of file for terms of use.  
//***************************************
//Version 1.0 

// Descrption:
// Object that represents Map data in memory

//************** N A M E S P A C E ****************************************
namespace PropellerPowered.Maps {
	//*********************************************************************
	// Map Class
	//*********************************************************************	
	public class MapPattern {
		public MapCellList Cells;
		public int Count;
		
		public MapPattern() {
			this.Cells = new MapCellList();
			this.Count = 1;
		}
	}
}