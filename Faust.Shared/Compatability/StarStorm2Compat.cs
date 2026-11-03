namespace Faust.Shared.Compatability
{
    public class StarStorm2Compat
    {
        public const string PluginGUID = "com.TeamMoonstorm.Starstorm2";
        public static bool IsInstalled =>
            BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(PluginGUID);
    }
}
