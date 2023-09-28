# MFTExpander

Expand NTFS MFT Zone by creating directories and files (and removes it at the end)

# Usage
```
Usage: mftexpander.exe [DRIVE] [DIRECTORIES] [FILES]
Usage: mftexpander.exe C:\     789           1234
```
Where:
* [C:\] is drive letter to expand NTFS MFT
* [123] is directory count to make
* [456] is files count to create to each directory

# NOTE:

After the expansion of the zone of the MFT, you need a software system to defragment the MFT zone.

# Writen with Microsoft Visual Studio 2015 Community. Uses .Net Framework 4.6+
