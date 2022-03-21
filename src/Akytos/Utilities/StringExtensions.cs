using System.Text.RegularExpressions;

namespace Akytos.Utilities;

public static class StringExtensions
{
    public static string SplitCamelCase( this string str )
    {
        string formattedName = str.Replace("m_", "");

        string withCase = formattedName[0].ToString().ToUpper() + formattedName[1..];
        
        return Regex.Replace( 
            Regex.Replace( 
                withCase, 
                @"(\P{Ll})(\P{Ll}\p{Ll})", 
                "$1 $2" 
            ), 
            @"(\p{Ll})(\P{Ll})", 
            "$1 $2" 
        );
    }

}