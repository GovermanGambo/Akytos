using YamlDotNet.Core;
using YamlDotNet.Core.Tokens;

namespace Akytos.Serialization;

public static class ScannerExtensions
    {
        public static string ReadScalar(this Scanner scanner, string key)
        {
            scanner.ReadScalarKey(key);

            return scanner.ReadScalarValue();
        }

        public static void ReadScalarKey(this Scanner scanner, string key)
        {
            // Key
            scanner.Read<Key>();
            // Scalar
            var keyScalar = scanner.Read<Scalar>();
            if (key != keyScalar.Value)
            {
                throw new ArgumentException($"Expected key '{key}' but found '{keyScalar.Value}'");
            }
        }

        public static string ReadScalarValue(this Scanner scanner)
        {
            // Value
            scanner.Read<Value>();
            // Scalar
            var scalar = scanner.Read<Scalar>();

            return scalar.Value;
        }

        public static void ReadDocumentStart(this Scanner scanner)
        {
            // StreamStart
            scanner.MoveNext();
            // DocumentStart
            scanner.MoveNext();
        }

        public static TToken Read<TToken>(this Scanner scanner) where TToken : Token
        {
            if (scanner.Current is not TToken)
            {
                throw new ArgumentException($"Expected token {typeof(TToken)} but found {scanner.Current?.GetType()}");
            }

            var token = scanner.Current;
            
            scanner.MoveNext();

            return (TToken)token;
        }

        public static bool TryRead<TToken>(this Scanner scanner, out TToken? token) where TToken : Token
        {
            if (scanner.Current is not TToken foundToken)
            {
                token = null;
                return false;
            }

            token = foundToken;
            
            scanner.MoveNext();

            return true;
        }
        
        public static bool Peek<TToken>(this Scanner scanner) where TToken : Token
        {
            return scanner.Current is TToken foundToken;
        }
        
        public static void Begin(this Scanner scanner)
        {
            if (scanner.Current == null)
            {
                scanner.MoveNext();
            }
        }
    }