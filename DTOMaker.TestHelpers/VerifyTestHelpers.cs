using System.Runtime.CompilerServices;
using VerifyTests;

namespace DTOMaker.TestHelpers
{
    public static class VerifyTestHelpers
    {
#if NET5_0_OR_GREATER
    [ModuleInitializer()]
    public static void Init() => VerifierSettings.FixNewlinesOnRead();

    /// <summary>
    /// On .NET versions 5.0 or greater this method is a no-op as the module initializer,
    /// VerifyTestHelpers.Init(), will have already run.
    /// </summary>
    public static void EnsureInitialized() { }
#else
        private static volatile bool _initialized = false;

        /// <summary>
        /// On .NET versions less than 5.0 this method ensures that the VerifySettings.FixNewlinesOnRead() 
        /// method is called only once.
        /// </summary>
        public static void EnsureInitialized()
        {
            if (!_initialized)
            {
                _initialized = true;
                VerifierSettings.FixNewlinesOnRead();
            }
        }
#endif
    }
}
