using System.Diagnostics.CodeAnalysis;

namespace Akytos.Assertions;

public static class Assert
{
    public static void AreEqual(object? a, object? b, string? message = null)
    {
        if (a != null && !a.Equals(b))
        {
            HandleAssertionFailed(message);
        }
    }
    
    public static void AreNotEqual(object? a, object? b, string? message = null)
    {
        if (a != null && a.Equals(b))
        {
            HandleAssertionFailed(message);
        }
    }
    
    public static void IsTrue([DoesNotReturnIf(false)] bool condition, string? message = null)
    {
        if (!condition)
        {
            HandleAssertionFailed(message);
        }
    }

    public static void IsFalse([DoesNotReturnIf(true)] bool condition, string? message = null)
    {
        if (condition)
        {
            HandleAssertionFailed(message);
        }
    }
    
    public static void IsNull(object? o, string? message = null)
    {
        if (o != null)
        {
            HandleAssertionFailed(message);
        }
    }
    
    public static void IsNotNull([NotNull] object? o, string? message = null)
    {
        if (o == null)
        {
            HandleAssertionFailed(message);
        }
    }

    private static void HandleAssertionFailed(string? message)
    {
#if DEBUG
        throw new AssertionException(message);
#else
        Debug.LogError(message);
#endif
    }
}