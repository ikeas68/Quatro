using System.Windows;

namespace Quatro
{

    public partial class App : Application
    {
    }
}

#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
    // Requis pour 'init' / records sur .NET Framework / netstandard
    internal static class IsExternalInit { }
}
#endif
