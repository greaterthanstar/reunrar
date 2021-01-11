# ReUnrar
## Recursively extract archives in a directory
The purpose of this .Net Core C# console app is to process a directory with zip, tar, or rar archives
and recursively extract them, preserving paths and overwriting files. It then deletes the archive afterward. 

### Bugs
* Zip files aren't extracting if they contain a file path. This must be something to do with the SharpCompress
package I am using. I probably should file a bug report with them. 

I'm still testing this and you should not use it without testing, as it's set up to delete archives after processing.