using SRTPluginBase;
using System;

namespace SRTPluginProviderRE6
{
    internal class PluginInfo : IPluginInfo
    {
        public string Name => "Game Memory Provider Resident Evil 6";

        public string Description => "A game memory provider plugin for Resident Evil 6.";

        public string Author => "Mysterion_06_ (Pointers & Coding), Squirrelies (Provider of the SRTHost) & TheDementedSalad (Pointers and Moral support)";

        public Uri MoreInfoURL => new Uri("https://github.com/Mysterion06/SRTPluginProviderRE6");

        public int VersionMajor => assemblyFileVersion.ProductMajorPart;

        public int VersionMinor => assemblyFileVersion.ProductMinorPart;

        public int VersionBuild => assemblyFileVersion.ProductBuildPart;

        public int VersionRevision => assemblyFileVersion.ProductPrivatePart;

        private System.Diagnostics.FileVersionInfo assemblyFileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
    }
}
