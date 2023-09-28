using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;

namespace MFT_Expander
{
    class Program
    {
        static void showProgress(int dirs, int files)
        {
            Console.Write("\r{0} / {1}", dirs, files);
        }
        static void removeFile(string path)
        {
            try
            {
                System.Console.ForegroundColor = ConsoleColor.DarkYellow;
                System.Console.WriteLine("Removing {0}", path);
                System.Console.ResetColor();
                File.Delete(path);
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = ConsoleColor.DarkYellow;
                System.Console.WriteLine(ex.Message);
                System.Console.ResetColor();
            }
        }
        static void removePath(string path)
        {
            try
            {
                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine("Removing {0}", path);
                System.Console.ResetColor();
                Directory.Delete(path, true);
            }
            catch ( Exception ex )
            {
                System.Console.ForegroundColor = ConsoleColor.DarkYellow;
                System.Console.WriteLine(ex.Message);
                System.Console.ResetColor();
            }
        }
        static void doExpand(string drive, int dirs, int files)
        {
            int xfiles = 0;
            int xdirs = 0;
            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("Begin Expanding...");
            System.Console.ResetColor();
            string dirname = @drive + "__MFT_EXPANDER__";
            if (Directory.Exists(dirname))
            {
                removePath(dirname);
            }
            System.Console.WriteLine("Legend: directories / files");
            DirectoryInfo di = Directory.CreateDirectory(dirname);
            for (int xd = 0; xd < dirs; xd++)
            {
                var xdir = dirname + "\\Expand_Directory_" + xd;
                if ( Directory.Exists(xdir) )
                {
                    removePath(xdir);
                }
                Directory.CreateDirectory(xdir);
                xdirs++;
                showProgress(xdirs, xfiles);
                for (int xf = 0; xf < files; xf++)
                {
                    var xfile = @xdir + "\\Expand_File_"+xf+".txt";
                    if ( File.Exists(xfile) )
                    {
                        removeFile(xfile);
                    }
                    File.Create(xfile).Close();
                    xfiles++;
                    showProgress(xdirs, xfiles);
                }
            }
            //
            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("\nDone.");
            System.Console.ResetColor();
            int counter = 10;
            while (true)
            {
                Console.Write("\rPause {0} sec for Hardware cache flushing...", counter);
                System.Threading.Thread.Sleep(1000);
                if (counter < 1)
                {
                    System.Console.Write("\n");
                    break;
                }
                counter--;
            }
            removePath(dirname);
        }
        static void showUsage()
        {
            System.Console.WriteLine("\nUsage: mftexpander.exe [DRIVE] [DIRECTORIES] [FILES]");
            System.Console.WriteLine("    1)     mftexpander.exe C:  123 456");
            System.Console.WriteLine("    2)     mftexpander.exe C:\\ 789 1234");
            System.Console.WriteLine("Where:");
            System.Console.WriteLine("    [C:]  is drive letter to expand NTFS MFT");
            System.Console.WriteLine("    [C:\\] is drive letter to expand NTFS MFT");
            System.Console.WriteLine("    [123] is directory count to make");
            System.Console.WriteLine("    [456] is files count to create to each directory\n");
        }
        static void showDriveInfo( DriveInfo drive, ConsoleColor color )
        {
            NumberFormatInfo nfi = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            nfi.NumberGroupSeparator = " ";
            nfi.NumberGroupSizes = new int[] { 3 };
            nfi.NumberDecimalDigits = 0;
            System.Console.ForegroundColor = color;
            System.Console.WriteLine("\n    Drive info:");
            System.Console.WriteLine("        Name                    : {0}", drive.Name);
            System.Console.WriteLine("        Drive Type              : {0}", drive.DriveType);
            System.Console.WriteLine("        Label                   : {0}", drive.VolumeLabel);
            System.Console.WriteLine("        File System             : {0}", drive.DriveFormat);
            System.Console.WriteLine("        Is Ready                : {0}", drive.IsReady);
            System.Console.WriteLine("        Total Space, Gb         : {0}", String.Format(nfi, "{0:N}", drive.TotalSize / 1000000000));
            System.Console.WriteLine("        Free Space, Gb          : {0}", String.Format(nfi, "{0:N}", drive.TotalFreeSpace / 1000000000));
            System.Console.WriteLine("        Avaible Free Space, Gb  : {0}", String.Format(nfi, "{0:N}", drive.AvailableFreeSpace / 1000000000));
            System.Console.ResetColor();
            System.Console.WriteLine("\n");
        }
        static void Main(string[] args)
        {
            // Arguments count
            int argc = args.Length;
            string drive;
            int dirs;
            int files;

            // CHECKPOINT1
            if ( argc == 3 )
            {
                drive = args[0].ToUpper();
                if (!int.TryParse(args[1], out dirs))
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("Error: directories count is not set");
                    System.Console.ResetColor();
                    showUsage();
                    return;
                }
                if (!int.TryParse(args[2], out files))
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("ERROR: files count is not set");
                    System.Console.ResetColor();
                    showUsage();
                    return;
                }
            }
            else
            {
                showUsage();
                return;
            }

            string diskPattern = "([a-zA-Z])\\:";
            Match matchTest = Regex.Match(drive, diskPattern);
            if ( ! matchTest.Success )
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("ERROR: drive pattern is: C:\\");
                System.Console.ResetColor();
                showUsage();
                return;
            }
            // CHECKPOINT1 COMPLETE

            // CHECKPOINT2 - DRIVE
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            List<string> driveList = new List<string>();
            DriveInfo selectedDrive = null;

            // get selected drive
            foreach (DriveInfo a in allDrives)
            {
                // a.Name : C: or C:\ (with trailing slash
                if (a.Name.ToString().Substring(0, 1) == drive || a.Name.ToString() == drive )
                {
                    selectedDrive = a;
                }
            }
                        
            // get all avaible
            foreach (DriveInfo d in allDrives)
            {
                var diskName = d.Name.ToString();
                if (d.IsReady == true && d.DriveType.ToString() == "Fixed" && d.DriveFormat.ToString() == "NTFS" )
                {
                    driveList.Add(diskName.Substring(0, 1));
                }
            }
            // CHECKPOINT2 - DRIVE

            if (driveList.Contains(drive.Substring(0, 1)))
            {
                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine("Init complete.");
                System.Console.WriteLine("Drive {0} is ACCEPTIBLE", drive);
                System.Console.ResetColor();
                // ALL OK! DO JOB
                doExpand(selectedDrive.Name, dirs,files);
            }
            else if (selectedDrive != null)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("Drive {0} is UNACCEPTIBLE because of:", selectedDrive.Name);
                if (selectedDrive.IsReady != true)
                {
                    System.Console.WriteLine("    Drive is not ready", selectedDrive.Name);
                }
                if (selectedDrive.DriveType.ToString() != "Fixed")
                {
                    System.Console.WriteLine("    Drive is not Fixed HDD Partition", selectedDrive.Name);
                }
                if (selectedDrive.DriveFormat.ToString() != "NTFS")
                {
                    System.Console.WriteLine("    Drive is not NTFS Partition", selectedDrive.Name);
                }
                System.Console.ResetColor();
                showDriveInfo(selectedDrive, ConsoleColor.Yellow);
            }
            else
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("Drive {0} is NOT Found in Acceptible Drives List", drive);
                System.Console.ResetColor();
                showUsage();
            }
            System.Console.WriteLine("\nNOW YOU NEED TO DEFRAGMENT YOUR MFT ZONE TO IMPROVE DISK SUBSYSTEM PERFOMANCE.");

            return;
        }
    }
}
