using System;
using System.IO;
using System.Linq;
using System.Text;
using AAXClean;

namespace ab_extract
{
    // Contains all the information relating to the current Audio Book being processed
	public class ABJob
	{
        public string inputFileName;    // Filename Only
        public string outputFileName;   // Filename Only
        public string coverArtFile;     // Filename Only
        public string inputDir;
        public string outputDir;
        public AaxFile aaxFile;
        public AppleTags appleTags;
        public ChapterInfo chapterInfo;
        public FileInfo OutputFile;
        private string fileExt;

        public string FullOutputPath   { get { return Path.Combine(outputDir, outputFileName); } }
        public string FullCoverArtPath { get  { return Path.Combine(outputDir, coverArtFile);  } }

        public ABJob(string filename, string coverArt, bool m4b)
		{
            // Save original Input Filename
            inputFileName = Path.GetFileName(filename);
            inputDir = Path.GetDirectoryName(filename);
            coverArtFile = Path.GetFileName(coverArt);
            outputDir = inputDir;  // Default is to output files in the same dir

            fileExt = (m4b ? ".m4b" : ".m4a");

            //Set default output filename (input file name with a .m4[a|b] extension)
            outputFileName = Path.ChangeExtension(inputFileName, fileExt);

        }

        public AaxFile OpenAaxFile (ActivationByteString activationByteString)
        {
            aaxFile = new AaxFile(inputFileName, FileAccess.Read, FileShare.Read);
            appleTags = aaxFile.AppleTags;
            chapterInfo = aaxFile.GetChaptersFromMetadata();

            //Set Decryption Bytes
            aaxFile.SetDecryptionKey(activationByteString.Bytes);

            return aaxFile;
        }

        public FileInfo OpenOutputFile()
        {
            OutputFile = new FileInfo(FullOutputPath);
            return OutputFile;
        }

        public DirectoryInfo CreateOutputDir ()
        {
            if ((appleTags.Album is not null) & (appleTags.FirstAuthor is not null))
            {
                // Set the book title
                string bookTitle = TrimTitle(appleTags.Album);

                outputDir = Path.Combine(Path.Combine(inputDir, appleTags.FirstAuthor), bookTitle);
                ///TODO - Check result from directory creation
                DirectoryInfo sub = Directory.CreateDirectory(outputDir);

                return sub;
            }

            return null;
        }

        public string SetOutputFilename (string title)
        {
            outputFileName = Path.ChangeExtension(TrimTitle(title), fileExt);
            return outputFileName;
        }

        private static string TrimTitle(string title)
        {
            string outTitle = title;
            // Some Audiobook titles can be too long for sensible filenames, so
            // trim any titles which are multi-part e.g. contain ':' or '-' 
            // Now check if the titile is an extended one and trim off the 2nd part
            if (title.Contains(':'))
            {
                outTitle = title[..title.IndexOf(':')];
            }
            else if (title.Contains('-'))
            {
                outTitle = title[..title.IndexOf('-')];
            }

            // Trim Title to a max of 32 chars
            if (outTitle.Length > 32)
            {
                outTitle = outTitle.Substring(0, 32);
            }

            // Make sure there are no other invalid characters in the title
            return CleanFileName(outTitle, true);
        }

        public bool SaveCoverArt()
        {
            try
            {
                FileInfo cover = new(FullCoverArtPath);
                FileStream fs = cover.OpenWrite();

                fs.Write(appleTags.Cover, 0, appleTags.Cover.Length);
                fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Saving Cover Art - ", ex.Message);
                return false;
            }
            return true;
        }

        public Stream GetOutputStream()
        {
            if (!OutputFile.Directory.Exists)
                OutputFile.Directory.Create();
            return OutputFile.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        public Stream GetOutputStream(int chapter)
        {
            if (!OutputFile.Directory.Exists)
                OutputFile.Directory.Create();
            return File.Open(Path.Combine(OutputFile.Directory.FullName, $"{chapter:d2} - {OutputFile.Name}"), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        public static string CleanFileName(string filename, bool replace)
        {
            var builder = new StringBuilder();
            var invalid = System.IO.Path.GetInvalidFileNameChars();
            foreach (var cur in filename)
            {
                if (!invalid.Contains(cur))
                {
                    builder.Append(cur);
                }
                else if (replace)
                {
                    builder.Append('_');
                }
            }
            return builder.ToString();
        }
    }
}

