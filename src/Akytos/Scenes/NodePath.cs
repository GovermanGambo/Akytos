using System.Text.RegularExpressions;

namespace Akytos
{
    public readonly struct NodePath
    {
        private const string FullPathRegex = @"^(\.|\.\/|\/)?([a-zA-Z0-9]+(\/[a-zA-Z0-9]+)*)*(:[a-z]+)*$";
        private const string ConcatenatedNamesRegex = @"[a-zA-Z0-9]+(\/[a-zA-Z0-9]+)*";
        private const string ConcatenatedSubNamesRegex = @"(:[a-z]+)+";
        
        private readonly string m_pathString;

        /// <summary>
        ///     Creates a new NodePath from the specified string.
        /// </summary>
        /// <param name="pathString">A valid node path string</param>
        public NodePath(string pathString)
        {
            var regex = new Regex(FullPathRegex);
            m_pathString = regex.IsMatch(pathString) ? pathString : "";
        }

        public static implicit operator string(NodePath nodePath)
        {
            return nodePath.m_pathString;
        }

        /// <summary>
        ///     True if the path is empty
        /// </summary>
        public bool IsEmpty => string.IsNullOrEmpty(m_pathString);
        /// <summary>
        ///     True if the path is an absolute path from the scene root
        /// </summary>
        public bool IsAbsolute => m_pathString.StartsWith("/");

        /// <summary>
        ///     Gets the name at the specified position in the path.
        /// </summary>
        /// <param name="index">Index to look for.</param>
        /// <returns>The node name at the specified position.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public string GetName(int index)
        {
            string[] splitString = GetConcatenatedNames().Split("/");

            if (index >= splitString.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return splitString[index];
        }

        /// <summary>
        ///     Returns the property part of the node path
        /// </summary>
        /// <returns>NodePath with only the property part of the path</returns>
        public NodePath AsPropertyPath()
        {
            string propertyPath = GetConcatenatedSubNames();
            if (propertyPath != "")
            {
                propertyPath = $":{propertyPath}";
            }
            return new NodePath(propertyPath);
        }

        /// <summary>
        ///     Returns a concatenated list of the node names in the path. 
        /// </summary>
        /// <returns>A string with names split by '/'</returns>
        public string GetConcatenatedNames()
        {
            var regex = new Regex(ConcatenatedNamesRegex);
            var match = regex.Match(m_pathString);

            if (!match.Success)
            {
                return "";
            }
            
            string result = match.Groups[0].Value;

            return result;
        }

        /// <summary>
        ///     Returns a concatenated list of the property names in the path. 
        /// </summary>
        /// <returns>A string with names split by ':'</returns>
        public string GetConcatenatedSubNames()
        {
            var regex = new Regex(ConcatenatedSubNamesRegex);
            var match = regex.Match(m_pathString);

            if (!match.Success)
            {
                return "";
            }
            
            string result = match.Groups[0].Value;

            // Strip first colon
            return result[1..];
        }

        public override int GetHashCode()
        {
            return m_pathString.GetHashCode();
        }
    }
}