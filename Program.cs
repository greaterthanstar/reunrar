using System.Linq;
using System.Runtime.Serialization.Json;
using System;
using SharpCompress;
using System.IO;
using SharpCompress.Readers;
using SharpCompress.Common;
using SharpCompress.IO;

namespace reunrar
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ReUnrar");
            var startingDir = Directory.GetCurrentDirectory();            
            ProcessDirectory(startingDir);
        }

        static void ProcessDirectory(string dir)
        {
            ExtractArchives(dir);            
            foreach(var childDirectories in Directory.EnumerateDirectories(dir))
            {
                ProcessDirectory(childDirectories);
            }
        }

        static void ExtractZip(string file)
        {
            try {
                var tr = SharpCompress.Archives.Zip.ZipArchive.Open(file);            
                using (var reader = tr.ExtractAllEntries())
                {
                    var options = new ExtractionOptions();
                    options.ExtractFullPath = true;
                    options.Overwrite = true;
                    reader.WriteAllToDirectory(Path.GetDirectoryName(file), options);
                }
                Console.WriteLine($"extracted zip {file}");
                File.Delete(file);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        static void ExtractRar(string file)
        {
            try {
                var tr = SharpCompress.Archives.Rar.RarArchive.Open(file);            
                using (var reader = tr.ExtractAllEntries())
                {
                    var options = new ExtractionOptions();
                    options.ExtractFullPath = true;
                    options.Overwrite = true;
                    reader.WriteAllToDirectory(Path.GetDirectoryName(file), options);
                }
                Console.WriteLine($"extracted rar {file}");
                //delete rar files
                var filename = Path.GetFileNameWithoutExtension(file);
                foreach(var rar in Directory.EnumerateFiles(Path.GetDirectoryName(file), filename + ".r??"))
                {
                    Console.WriteLine($"Deleting rar file {rar}");
                    File.Delete(rar);
                }                
            }
            catch (Exception ex)
            {
                throw;
            }
        }        

        static void ExtractTar(string file) 
        {
            try {
                using (Stream stream = File.OpenRead(file))
                using (var reader = ReaderFactory.Open(stream))
                {                    
                    while (reader.MoveToNextEntry())
                    {
                        // if (!reader.Entry.IsDirectory)
                        // {
                            var curDir = Path.GetDirectoryName(file);
                            Console.WriteLine(reader.Entry.Key);
                            reader.WriteEntryToDirectory(curDir, new ExtractionOptions()
                            {
                                ExtractFullPath = true,
                                Overwrite = true,                                
                            });
                        //}
                    }
                }
                File.Delete(file);
            }
            catch (Exception ex) 
            {
                throw;
            }            
        }

        static void ExtractArchives(string dir) 
        {
            //var files = Directory.EnumerateFiles(dir, "searchPattern");
            string[] extensions = { ".tar", ".zip", ".rar" };

            foreach (string file in Directory.EnumerateFiles(dir).ToList()
                    .Where(s => extensions.Any(ext => ext == Path.GetExtension(s))))
            {
                var ext = Path.GetExtension(file);
                switch (ext)
                {
                    case ".tar":
                        ExtractTar(file);
                        break;
                    case ".zip":
                        ExtractZip(file);
                        break;
                    case ".rar":
                        ExtractRar(file);
                        break;                        
                }
                    
                Console.WriteLine($"extracted archive {file}");                            
            }
            // 2nd pass, assumes we deleted all archives while processing
            if (Directory.EnumerateFiles(dir).ToList().Any())
            {
                ExtractArchives(dir);
            }
            
        }
    }
}
