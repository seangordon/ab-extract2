using AAXClean;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ab_extract
{
	class Program
	{
        private static readonly TextWriter ConsoleText = Console.Error;
		public static async Task<int> Main(string[] args)
		{

            // Load app config
			Config config = new();

            // Must have 1 argument
            if (args is null || args.Length != 1)
			{
                Console.WriteLine("AB-Extract v2.0 - Decrypt and unpack Audible AudioBooks to Individual Chapters.");
                Console.WriteLine(" -- Configure settings via AB-Extract.exe.config");
                Console.WriteLine(" -- Usage: AB-Extract <AudibleFile.aax>");
				return -1;
			}

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("Error - File Does Not Exist: " + args[0]);
                return 2;
            }

            // Create processing job information
            ABJob abJob = new(args[0], config.CoverArtName, config.M4BAudioBook);

            try
			{
				ReWriteColored(($"Opening Aax file...", ConsoleColor.White));

                //Open source AAX File
                abJob.OpenAaxFile(config.AudibleActivationBytes);

                ReWriteColored(($"Aax file opened successfully\r\n", ConsoleColor.Green));

                // Show Audio Book Info
                ShowTagInfo(abJob.appleTags);

				if (config.SplitbyChapters)
				{
                    ShowChapterInfo(abJob.chapterInfo);
				}

                // Do we need to create a sub-directory for the output files?
                if (config.CreateDir)
                {
                    abJob.CreateOutputDir();
                }

                if (config.AutoName)
                {
                    abJob.SetOutputFilename(abJob.appleTags.Album);
                }

                WriteColoredLine(("\r\nOutput Folder : ", ConsoleColor.Yellow), ($"{abJob.outputDir}", ConsoleColor.Green));
                WriteColoredLine(("Cover Art     : ", ConsoleColor.Yellow), ($"{abJob.coverArtFile}", ConsoleColor.Green));
                WriteColoredLine(("Output File(s): ", ConsoleColor.Yellow), ($"{abJob.outputFileName}\r\n", ConsoleColor.Green));
                WriteColoredLine(("Split Chapters: ", ConsoleColor.Yellow), ($"{(config.SplitbyChapters?"Yes":"No")}\r\n", ConsoleColor.Green));

                // Setup the outputfile
                abJob.OpenOutputFile();

                // First Extract the Album art from the .aax file (this doesn't need decryption since only the audio stream is encrypted
                if (config.ExtractCoverArt)
                {
                    abJob.SaveCoverArt();
                }

                DateTime startTime = DateTime.Now;
				int chNum = 1;
				var operation
					= config.SplitbyChapters
					? abJob.aaxFile.ConvertToMultiMp4aAsync(abJob.chapterInfo, cb => cb.OutputFile = abJob.GetOutputStream(chNum++))
					: abJob.aaxFile.ConvertToMp4aAsync(abJob.GetOutputStream(), null);

				operation.ConversionProgressUpdate += AaxFile_ConversionProgressUpdate;

				await operation;

				var duration = DateTime.Now - startTime;

				ConsoleText.WriteLine();
				WriteColoredLine(
					("\r\nConversion succeeded!", ConsoleColor.Green),
					($"  Total time: {duration:mm\\:ss\\.ff}", ConsoleColor.White));

				return 0;

			}
			catch (Exception ex)
			{
				ConsoleText.WriteLine();
				WriteColoredLine(
					("Error Converting Book", ConsoleColor.Red),
					(": ", ConsoleColor.White),
					(ex.Message, ConsoleColor.DarkRed));

				return await Task.FromResult(-2); ;
			}
		}

		private static void AaxFile_ConversionProgressUpdate(object sender, ConversionProgressEventArgs e)
		{
            ReWriteColored
				(
				("Conversion progress", ConsoleColor.Green),
				($": {e.FractionCompleted * 100:F2}%    ", ConsoleColor.White),
				("average speed", ConsoleColor.Green),
				($" = {(int)e.ProcessSpeed}x", ConsoleColor.White)
				);
		}

        private static void ShowChapterInfo(ChapterInfo chInfo)
        {
            if (chInfo is null)
            {
                WriteColoredLine(("Error reading chapters from metadata", ConsoleColor.Red));
            }
            else
            {
                int maxLen = chInfo.Max(c => c.Title.Length) + 3;

                WriteColoredLine(($"\r\nCHAPTER LIST", ConsoleColor.Magenta));

                WritePad('-', maxLen + 67);
                ConsoleText.WriteLine();

                foreach (var ch in chInfo)
                {
                    WriteColored(($"\"{ch.Title}\"", ConsoleColor.Yellow));
                    WritePad(' ', maxLen - ch.Title.Length);
                    WriteColoredLine(
                        ($"Start", ConsoleColor.Green),
                        ($" = {(int)ch.StartOffset.TotalHours:D2}:{ch.StartOffset:mm\\:ss\\.fff}, ", ConsoleColor.White),
                        ($"End", ConsoleColor.Green),
                        ($" = {(int)ch.EndOffset.TotalHours:D2}:{ch.EndOffset:mm\\:ss\\.fff}, ", ConsoleColor.White),
                        ($"Duration", ConsoleColor.Green),
                        ($" = {ch.Duration:hh\\:mm\\:ss\\.fff}", ConsoleColor.White));
                }

                WritePad('-', maxLen + 67);
                ConsoleText.WriteLine();
            }
        }

        private static void ShowTagInfo(AppleTags abInfo)
        {
            if (abInfo is null)
            {
                WriteColoredLine(("Error audiobook info from metadata", ConsoleColor.Red));
            }
            else
            {
                WriteColoredLine(($"\r\nAUDIOBOOK INFO", ConsoleColor.Magenta));

                WritePad('-', 80);
                ConsoleText.WriteLine();

                if (abInfo.Album is not null) WriteColoredLine(($"Album: ", ConsoleColor.Green), (abInfo.Album, ConsoleColor.White));
                if (abInfo.AlbumArtists is not null) WriteColoredLine(($"Album Artists: ", ConsoleColor.Green), (abInfo.AlbumArtists, ConsoleColor.White));
                if (abInfo.BookCopyright is not null) WriteColoredLine(($"Book Copyright: ", ConsoleColor.Green), (abInfo.BookCopyright, ConsoleColor.White));
                if (abInfo.Copyright is not null) WriteColoredLine(($"Copyright: ", ConsoleColor.Green), (abInfo.Copyright, ConsoleColor.White));
                if (abInfo.Comment is not null) WriteColoredLine(($"Comment: ", ConsoleColor.Green), (abInfo.Comment, ConsoleColor.White));
                if (abInfo.FirstAuthor is not null) WriteColoredLine(($"First Author: ", ConsoleColor.Green), (abInfo.FirstAuthor, ConsoleColor.White));
                if (abInfo.Generes is not null) WriteColoredLine(($"Generes: ", ConsoleColor.Green), (abInfo.Generes, ConsoleColor.White));
                if (abInfo.LongDescription is not null) WriteColoredLine(($"Description: ", ConsoleColor.Green), (abInfo.LongDescription, ConsoleColor.White));
                if (abInfo.Narrator is not null) WriteColoredLine(($"Narrator: ", ConsoleColor.Green), (abInfo.Narrator, ConsoleColor.White));
                if (abInfo.Publisher is not null) WriteColoredLine(($"Publisher: ", ConsoleColor.Green), (abInfo.Publisher, ConsoleColor.White));
                if (abInfo.ReleaseDate is not null) WriteColoredLine(($"Release Date: ", ConsoleColor.Green), (abInfo.ReleaseDate, ConsoleColor.White));
                if (abInfo.Title is not null) WriteColoredLine(($"Title: ", ConsoleColor.Green), (abInfo.Title, ConsoleColor.White));
                if (abInfo.Year is not null) WriteColoredLine(($"Album: ", ConsoleColor.Green), (abInfo.Year, ConsoleColor.White));
            }
        }

        static int lastUpdateLength = 0;

		private static void ReWriteColored(params (string str, ConsoleColor color)[] coloredText)
		{
			int textLen = 0;
			ConsoleText.Write('\r');
			foreach (var (str, color) in coloredText)
			{
				textLen += str.Length;
				Console.ForegroundColor = color;
				ConsoleText.Write(str);
			}
			Console.ResetColor();

			int endPad = Math.Max(0, lastUpdateLength - textLen);
			WritePad(' ', endPad);

			lastUpdateLength = textLen - endPad;
		}
		private static void WriteColoredLine(params (string str, ConsoleColor color)[] coloredText)
		{
			WriteColored(coloredText);
			ConsoleText.WriteLine();
		}

		private static void WriteColored(params (string str, ConsoleColor color)[] coloredText)
		{
			foreach (var (str, color) in coloredText)
			{
				Console.ForegroundColor = color;
				ConsoleText.Write(str);
			}
			Console.ResetColor();
		}

		static void WritePad(char c, int padLen)
		{
			for (int i = 0; i < padLen; i++)
				ConsoleText.Write(c);
		}
	}
}
