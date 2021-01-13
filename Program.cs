using System.Linq;
using System.Runtime.Serialization.Json;
using System;
using SharpCompress;
using System.IO;
using SharpCompress.Readers;
using SharpCompress.Common;
using SharpCompress.IO;
using SharpCompress.Archives.Rar;
using System.Collections.Generic;
using SharpCompress.Readers.Rar;

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

        static void ExtractRars(string file)
        {
            var filename = Path.GetFileNameWithoutExtension(file);
            var rars = Directory.GetFiles(Path.GetDirectoryName(file), filename + ".r??").ToList()
                        .Where(f => !f.EndsWith(".rar")).OrderBy(x => x.ToString()).ToList();
            rars.Insert(0, file);

            var streams = rars.Select(File.OpenRead).ToList();
            Console.WriteLine($"Extracting rar file {file}");
            using (var reader = RarReader.Open(streams))
            {
                while (reader.MoveToNextEntry())
                {
                    reader.WriteEntryToDirectory(Path.GetDirectoryName(file), new ExtractionOptions()
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                    Console.WriteLine($"extracted rar file {reader.Entry.Key}");
                }
            }
            foreach (var stream in streams)
            {
                stream.Dispose();
            }

            foreach (var rar in rars)
            {
                File.Delete(rar);
            }
        }

        static void ExtractRar(string file)
        {
            try {
                //var tr = SharpCompress.Archives.Rar.RarArchive.Open(file);            
                //using (var reader = tr.ExtractAllEntries())
                //{
                //    var options = new ExtractionOptions();
                //    options.ExtractFullPath = true;
                //    options.Overwrite = true;
                //    reader.WriteAllToDirectory(Path.GetDirectoryName(file), options);
                //}
                var filename = Path.GetFileNameWithoutExtension(file);
                var rars = Directory.GetFiles(Path.GetDirectoryName(file), filename + ".r??").ToList()
                            .Where(f => !f.EndsWith(".rar")).OrderBy(x => x.ToString()).ToList();
                //var str = File.OpenRead
                //var reader = RarArchive.Open(file, new ReaderOptions { })
                List<Stream> streams = new List<Stream>();
                streams.Add(File.OpenRead(file));
                foreach(var f in rars)
                {
                    streams.Add(File.OpenRead(f));
                }
                //using (Stream stream = File.OpenRead(file))
                //using (var reader = RarArchive.Open(streams))
                //{
                //    foreach(var entry in reader.Entries)
                //    {
                //        var curDir = Path.GetDirectoryName(file);
                //        Console.WriteLine("-> " + entry.Key);
                //        entry.WriteEntryToDirectory(curDir, new ExtractionOptions()
                //        {
                //            ExtractFullPath = true,
                //            Overwrite = true,
                //        });
                //    }
                //}
                using (var reader = RarArchive.Open(streams))
                {


                    //using (var entries = reader. ExtractAllEntries())
                    //{
                    //    var options = new ExtractionOptions();
                    //    options.ExtractFullPath = true;
                    //    options.Overwrite = true;
                    //    entries.WriteAllToDirectory(Path.GetDirectoryName(file), options);
                    //}
                }
                Console.WriteLine($"extracted rar {file}");
                //delete rar files                
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
                        ExtractRars(file);
                        break;                        
                }
                    
                Console.WriteLine($"extracted archive {file}");                            
            }
            // 2nd pass, assumes we deleted all archives while processing
            if (Directory.EnumerateFiles(dir).ToList()
                .Any(s => extensions.Any(ext => ext == Path.GetExtension(s))))
            {
                ExtractArchives(dir);
            }
            
        }
    }
}
