Font/Graphics utilities for Propeller-based projects (v1.6)

Contributers:
trodoss
matb
Oldbitcollector

Font Generator:
----------------
A command-line application used to create "font" (1bpp or 2bpp characters) to be used with various Propeller-based display objects. 
It converts a black/white (1bpp) or 16 color (2bpp) bitmap image into the Spin source that can be included with Spin applications.

Font Generator is a C# command-line applications (source included) compiled under .NET framework 2.0 (v2.0.50727). 
If you have the Mono runtime installed on Linux, you should be able to it as well. 


Usage: FontGenerator.exe [bitmap_filename] [output_filename] ([format])

Format can be specified follows:
 A = AIGeneric format (8x8, 1bpp) 
 P = Potato_Text format (8x8, 1bpp)
 B = Binary format (8x8, 1bpp)
 O = ORE_Text format (8x8 2bpp)
 T = TV_Text format (32x16, 1bpp)


 
Graphics Convert:
----------------
A command-line application used to convert various graphics format files to output usable by either Font Generator or
Propeller-based display objects.

Graphics Convert is a C# command-line applications (source included) compiled under .NET framework 2.0 (v2.0.50727). 
If you have the Mono runtime installed on Linux, you should be able to it as well.  


Gameboy Tile Designer (v2.2) - Copyright H. Mulder 1999.  See the README.TEXT included with the application for the disclaimer
and terms of use.

 
Usage: GraphicsConvert.exe [input_filename] [input_format] [output_filename] ([output_format]) 

Input Format can be specified as follows:
 G = Gameboy Tile Designer (.GBR) file

Output Format can be specified follows:
 X = generates a 256x32 bitmap
 O = ORE_Text format (8x8 2bpp)



Map Convert/GBMFix:
----------------
Gameboy Map Builder (v1.8) - Copyright H. Mulder 1999.  See the README.TEXT included with the application for the disclaimer
and terms of use.

Map Convert is a C# command-line application (source included) compiled under .NET framework 2.0 (v2.0.50727).
If you have the Mono runtime installed on Linux, you should be able to it as well.  

Usage: MapConvert.exe [input_filename] [input_format] [output_filename] ([output_format]) 

Input Format can be specified as follows:
 G = Gameboy Map Builder (.GBM) file


Output Format can be specified follows:
 XY = 2 Dimensional (X/Y) Scrolling Map output

----

GBMFix is a C# command-line application (source included) compiled under .NET framework 2.0 (v2.0.50727).
If you have the Mono runtime installed on Linux, you should be able to it as well. 

GBMFix is used to correct situations when the absolute path to the tileset file referenced in a .GBM file needs to be 
corrected to use the current directory (or one specified in the tileset_filename parameter).


Usage: GBMFix.exe [input_filename] ([output_filename], [tileset_filename])



Sound Convert
----------------
Sound Convert can convert various sound input formats (primarily RTTTL), and generate output used by sound objects.

Sound Convert is a C# command-line application (source included) compiled under .NET framework 2.0 (v2.0.50727).
If you have the Mono runtime installed on Linux, you should be able to it as well.  

Usage: SoundConvert.exe [input_filename] [input_format] [output_filename] [output_format]

Input Format can be specified as follows:
 R = RTTTL (Ring Tone Text Transfer Language)

Output Format can be specified follows:
 S = Simple Sound (driver) output (source available in the /examples/SimpleSound/ subdirectory)
 
 
 
  
Version history:
1.0 - Original Release
1.1 - Revision to add Potato_Text and Binary format options
1.2 - Revision to add matb's TV_Text format, ORE_Text format, and include example code
1.3 - Revision to add the GraphicsConvert utility
1.4 - Revision to add the PropellerPowered.Maps namespace (for expansion), MapConvert, and GBMFix utilities.
1.5 - Revision to continue work on the PropellerPowered.Sound, and generation of the SoundConvert utility and sample code



To do:
- Build Mono makefile(s) for Linux distribution.


Note:
The example code included is to demonstrate how the Font Generator output is used with the drivers.  For the most recent
version(s) of these drivers, please reference their most recent release in the OBEX, or in the Parallax forums.

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