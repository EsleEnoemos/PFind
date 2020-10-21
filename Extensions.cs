using System;
using System.Collections.Generic;

namespace PFind {
	public static class Extensions {
		public static bool ContainsAll( this string ths, IEnumerable<string> these, StringComparison stringComparison ) {
			foreach( string s in these ) {
				if( ths.IndexOf( s, stringComparison ) == -1 ) {
					return false;
				}
			}
			return true;
		}

		public static string PopFirstHit( this string ths, List<string> these, StringComparison stringComparison ) {
			int minInd = int.MaxValue;
			string firstHit = null;
			foreach( string s in these ) {
				int ind = ths.IndexOf( s, stringComparison );
				if( ind < minInd ) {
					minInd = ind;
					firstHit = s;
				}
			}
			these.Remove( firstHit );
			return firstHit;
		}
	}
}
