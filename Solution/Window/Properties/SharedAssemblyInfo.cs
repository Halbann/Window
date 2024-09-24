using System.Reflection;

using Stapler;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle(MetaInfo.ASSEMBLY)]
[assembly: AssemblyProduct(MetaInfo.ASSEMBLY)]
[assembly: AssemblyCopyright("Copyright © Halban 2024, MIT License")]

//[assembly: AssemblyVersion(MetaInfo.STRING)]
//[assembly: AssemblyFileVersion(MetaInfo.STRING)]

//[assembly: KSPAssembly(MetaInfo.ASSEMBLY, MetaInfo.MAJOR, MetaInfo.MINOR, MetaInfo.REVISION)]
[assembly: KSPAssemblyDependency("HarmonyKSP", 1, 0)]

namespace Stapler
{
    static class MetaInfo
    {
        public const string ASSEMBLY = "Stapler";
        //public const int MAJOR = 1;
        //public const int MINOR = 2;
        //public const int REVISION = 0;
        //public const string STRING = "1.2.0";
    }
}
