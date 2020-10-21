using System;
using System.IO;

namespace PFind {
	class Program {
		#region static void Main( string[] args )
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		static void Main( string[] args ) {
			string envFilter = Environment.GetEnvironmentVariable( "PFINDFILTER" );
			bool showUsage = false;
			if( args.Length < 2 && string.IsNullOrEmpty( envFilter ) ) {
				showUsage = true;
			}
			if( showUsage ) {
				ShowUsage();
				return;
			}
			string pattern = args[ 0 ];
			string filter = envFilter;
			if( args.Length > 1 ) {
				filter = args[ 1 ];
			}
			if( string.IsNullOrEmpty( filter ) ) {
				ShowUsage();
				return;
			}
			Console.WriteLine( "Searching for \"" + pattern + "\" using filter \"" + filter + "\"" + Environment.NewLine );
			bool recursive = false;
			if( args.Length == 3 ) {
				recursive = string.Compare( args[ 2 ], "/R", true ) == 0;
			}
			string[] files = Directory.GetFiles( Environment.CurrentDirectory, filter, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly );
			if( files.Length == 0 ) {
				Console.WriteLine( "No files was found with file filter \"" + filter + "\"" );
				return;
			}
			pattern = pattern.ToLower();
			int hitCount = 0;
			int fileCount = 0;
			foreach( string s in files ) {
				string[] lines = File.ReadAllLines( s );
				bool printName = true;
				for( int i = 0; i < lines.Length; i++ ) {
					string line = lines[ i ];
					if( string.IsNullOrEmpty( line ) ) {
						continue;
					}
					line = line.ToLower();
					int hitIndex = line.IndexOf( pattern );
					if( hitIndex > -1 ) {
						if( printName ) {
							ConsoleColor cc = Console.ForegroundColor;
							Console.ForegroundColor = ConsoleColor.White;
							Console.WriteLine( s.Replace( Environment.CurrentDirectory + "\\", "" ) );
							Console.ForegroundColor = cc;
							printName = false;
							fileCount++;
						}
						string before = lines[ i ].Substring( 0, hitIndex );
						string linePattern = lines[ i ].Substring( hitIndex, pattern.Length );
						string after = lines[ i ].Substring( hitIndex + pattern.Length );
						ConsoleColor fg = Console.ForegroundColor;
						Console.Write( "Line " + (i + 1) + ": " + before );
						Console.ForegroundColor = ConsoleColor.Yellow;
						Console.Write( linePattern );
						Console.ForegroundColor = fg;
						Console.WriteLine( after );
						//Console.WriteLine( "Line " + (i + 1) + ": " + lines[ i ] );
						hitCount++;
					}
				}
				if( !printName ) {
					Console.WriteLine();
				}
			}
			Console.WriteLine( "Found " + hitCount + " hits in " + fileCount + " files, searching a total of " + files.Length + " files" );
		}
		#endregion
		#region private static void ShowUsage()
		/// <summary>
		/// 
		/// </summary>
		private static void ShowUsage() {
			Program p = new Program();
			Console.WriteLine( "PFind (PatternFind) " + p.GetType().Assembly.GetName().Version );
			Console.WriteLine( "PFind <pattern> <file filter> [/R]" );
			Console.WriteLine( "/R: Search recursivly from current directory" );
			Console.WriteLine( "Example: pfind \"Hello world\" *.txt" );
			Console.WriteLine( "Example: pfind \"Hello world\" *.txt /R" );
			Console.WriteLine( "Filter can be omitted if specified by the enviroment variable PFINDFILTER" );
		}
		#endregion
	}
}
