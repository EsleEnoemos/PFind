using System;
using System.Collections.Generic;
using System.IO;

namespace PFind {
	class Program {
		static void Main( string[] args ) {
			string envFilter = Environment.GetEnvironmentVariable( "PFINDFILTER" );
			bool showUsage = args.Length < 2 && string.IsNullOrEmpty( envFilter );
			if( showUsage ) {
				ShowUsage();
				return;
			}
			//string pattern = args[ 0 ];
			List<string> patterns = new List<string>();
			bool recursive = false;
			int patternCount = 0;
			if( string.Equals( args[ args.Length - 1 ], "/R" ) ) {
				patternCount = args.Length - 2;
				recursive = true;
			} else {
				patternCount = args.Length - 1;
			}
			for( int i = 0; i < patternCount; i++ ) {
				patterns.Add( args[ i ] );
			}
			string filter = envFilter;
			if( string.IsNullOrEmpty( filter ) ) {
				filter = args[ patternCount ];
			}
			if( string.IsNullOrEmpty( filter ) ) {
				ShowUsage();
				return;
			}
			if( patterns.Count == 1 ) {
				Console.WriteLine( $"Searching for \"{patterns[ 0 ]}\" using filter \"{filter}\"{Environment.NewLine}" );
			} else {
				Console.Write( "Searching for " );
				ConsoleColor foregroundColor = Console.ForegroundColor;
				for( int i = 0; i < patterns.Count; i++ ) {
					string pattern = patterns[ i ];
					Console.Write( "\"" );
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.Write( pattern );
					Console.ForegroundColor = foregroundColor;
					Console.Write( "\"" );
					if( (i + 1) < patterns.Count ) {
						Console.ForegroundColor = ConsoleColor.Green;
						Console.Write( " AND " );
						Console.ForegroundColor = foregroundColor;
					}
				}
				Console.WriteLine( $" using filter \"{filter}\"" );
			}
			string[] files = Directory.GetFiles( Environment.CurrentDirectory, filter, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly );
			if( files.Length == 0 ) {
				Console.WriteLine( $"No files was found with file filter \"{filter}\"" );
				return;
			}
			//pattern = pattern.ToLower();
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
					//line = line.ToLower();
					if( !line.ContainsAll( patterns, StringComparison.InvariantCultureIgnoreCase ) ) {
						continue;
					}
					hitCount++;
					List<string> tmpPatterns = new List<string>( patterns );
					bool printLine = true;
					do {
						string pattern = line.PopFirstHit( tmpPatterns, StringComparison.InvariantCultureIgnoreCase );
						int hitIndex = line.IndexOf( pattern, StringComparison.InvariantCultureIgnoreCase );
						if( printName ) {
							ConsoleColor cc = Console.ForegroundColor;
							Console.ForegroundColor = ConsoleColor.White;
							Console.WriteLine( s.Replace( Environment.CurrentDirectory + "\\", "" ) );
							Console.ForegroundColor = cc;
							printName = false;
							fileCount++;
						}
						string before = line.Substring( 0, hitIndex );
						string linePattern = line.Substring( hitIndex, pattern.Length );
						string after = line.Substring( hitIndex + pattern.Length );
						ConsoleColor fg = Console.ForegroundColor;
						if( printLine ) {
							Console.Write( $"Line {(i + 1)}: " );
							printLine = false;
						}
						Console.Write( before );
						Console.ForegroundColor = ConsoleColor.Yellow;
						Console.Write( linePattern );
						Console.ForegroundColor = fg;
						if( tmpPatterns.Count == 0 ) {
							Console.WriteLine( after );
						} else {
							line = after;
						}
						//Console.WriteLine( "Line " + (i + 1) + ": " + lines[ i ] );
					} while( tmpPatterns.Count > 0 );
				}
				if( !printName ) {
					Console.WriteLine();
				}
			}
			Console.WriteLine( "Found " + hitCount + " hits in " + fileCount + " files, searching a total of " + files.Length + " files" );
		}
		private static void ShowUsage() {
			Program p = new Program();
			Console.WriteLine( "PFind (PatternFind) " + p.GetType().Assembly.GetName().Version );
			Console.WriteLine( "PFind <pattern> [<pattern2>] ... [<patternN>] <file filter> [/R]" );
			Console.WriteLine( "/R: Search recursivly from current directory" );
			Console.WriteLine( "Example: pfind \"Hello world\" *.txt" );
			Console.WriteLine( "Example: pfind \"Hello world\" *.txt /R" );
			Console.WriteLine( "Example: pfind Hello world *.txt /R" );
			Console.WriteLine( "Filter can be omitted if specified by the enviroment variable PFINDFILTER" );
		}
	}
}
