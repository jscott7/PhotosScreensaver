# PhotosScreensaver

This project was created both as an exercise in creating a windows C#/WPF screensaver and also to solve the problem with the old photos screensaver where it used a fixed random number seed.
This uses a seed based on time, which while not truly random will display a sufficiently differing set of pictures on each invocation.

There are two supported ways of discovering images to show:

1. All images in a directory tree from a configurable root.
2. The images in a random directory with a size greater than 100, from the configurable root

A random choice of the above can also be used.

A label is displayed giving information for each image. There is currently a single implementation of this. Given a URI that contains a year, print the year to final folder
  
  e.g. file://C:/Temp/Photos/2019/November/London/img_01020.jpg
 
 gives:

   2019/November/London

To install on a client machine:

1.  Copy the .scr file from the bin\Release folder to convenient location on your C: drive.
2.  Right click the .scr file.
3.  Select Install.

To uninstall delete the .scr file.

Command-line Arguments:
	<no args>  Display screen saver  
	"/s"	   Display screen saver  
	"/c"	   Show settings window  
	"/p"       Preview  