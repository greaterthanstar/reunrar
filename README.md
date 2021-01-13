# ReUnrar
## Recursively extract archives in a directory
The purpose of this .Net Core C# console app is to process a directory with zip, tar, or rar archives
and recursively extract them, preserving paths and overwriting files. It then deletes the archive afterward. 

### Bugs
* Not set up to process file.part01.rar, file.part02.rar style archives. 

I'm still testing this and you should not use it without testing, as it's set up to delete archives after processing.
