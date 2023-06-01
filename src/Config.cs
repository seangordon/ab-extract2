using System.Configuration;

namespace ab_extract
{

	public class Config
	{
		public ActivationByteString AudibleActivationBytes;
		public bool   ExtractCoverArt;	// Extract Cover Art to a separate File?
        public bool   SplitbyChapters;  // Auto Split the Audio Book into separate Chapter Files?
		public bool   AutoName;			// Generate the output name from Audio Book Title (Album)
        public string CoverArtName;		// Name for the extracted Cover Art
        public bool   M4BAudioBook;		// Rename output files to .m4b for iTunes / Apple eBooks
        public bool   CreateDir;		// Create sub-directory for the extracted files?

        public Config()
		{
			// Load Configuration
			AudibleActivationBytes = ActivationByteString.Parse(ConfigurationManager.AppSettings["ActivationBytes"]);
			ExtractCoverArt = ConfigurationManager.AppSettings["ExtractCoverArt"].ToLower() == "yes";
            AutoName = ConfigurationManager.AppSettings["AutoName"].ToLower() == "yes";
            SplitbyChapters = ConfigurationManager.AppSettings["SplitbyChapters"].ToLower() == "yes";
			CoverArtName = ConfigurationManager.AppSettings["CoverArtName"];
			M4BAudioBook = ConfigurationManager.AppSettings["M4BAudioBook"].ToLower() == "yes"; 
			CreateDir = ConfigurationManager.AppSettings["CreateDir"].ToLower() == "yes";
		}
	}
}
