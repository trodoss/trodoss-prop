using System;
using System.Collections;
//****************************************
//trodoss - 2012
//See end of file for terms of use.  
//***************************************
//Version 1.0 - Initial Release
//************** N A M E S P A C E ****************************************
namespace PropellerPowered.Sound {
	//*********************************************************************
	// Note Class
	//*********************************************************************
	public enum PitchType:int {Pause=0, A=1, ASharp=2, B=3, C=4, CSharp=5, D=6, DSharp=7, E=8, F=9, FSharp=10, G=11, GSharp=12}
	public enum DurationType:int {None=-1, Whole=7, Half=6, Quarter=5, Eighth=4, Sixteenth=3, ThirtySecond=2, SixtyFourth=1, OneHundredTwentyEighth=0}
	public enum FlagType:int {None=0, Dotted=1, EndOfSong = 9}
	
	public class Note {
		public int Octave;
		public PitchType Pitch;
		public DurationType Duration;
		public FlagType Flag;
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
