using System.Reflection;

namespace WS_ClinicService.Core
{
    public static class ApplicationVersion
    {
        /// <summary>Product version from the build (e.g. 0.1.0), without the source revision suffix.</summary>
        public static string Current { get; } = Resolve();

        private static string Resolve()
        {
            var informational = typeof(ApplicationVersion).Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

            return informational?.Split('+')[0] ?? "0.0.0";
        }
    }
}
