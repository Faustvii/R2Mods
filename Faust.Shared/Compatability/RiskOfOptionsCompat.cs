﻿using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;

namespace Faust.Shared.Compatability
{
    public class RiskOfOptionsCompat
    {
        public const string PluginGUID = "com.rune580.riskofoptions";
        public static bool IsInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(PluginGUID);

        public static void SetModDescription(string description)
        {
            ModSettingsManager.SetModDescription(description);
        }

        public static void AddCheckboxOptions(bool restartRequired = false, params ConfigEntry<bool>[] configEntries)
        {
            foreach (var config in configEntries)
            {
                ModSettingsManager.AddOption(new CheckBoxOption(config, new CheckBoxConfig { restartRequired = restartRequired }));
            }
        }

        public static void AddSliderNumberOptions(bool restartRequired = false, params ConfigEntry<float>[] configEntries)
        {
            foreach (var config in configEntries)
            {
                ModSettingsManager.AddOption(new SliderOption(config, new SliderConfig { min = 0.5f, max = 5f, formatString = "{0:0.#}", restartRequired = restartRequired }));
            }
        }

        public static void AddSliderToPercentageOptionsDecimal(bool restartRequired = false, params ConfigEntry<float>[] configEntries)
        {
            foreach (var config in configEntries)
            {
                ModSettingsManager.AddOption(new StepSliderOption(config, new StepSliderConfig { min = 0f, max = 1f, formatString = "{0:P}", increment = 0.01f, restartRequired = restartRequired }));
            }
        }

        public static void AddDropdownOptions<T>(bool restartRequired = false, params ConfigEntry<T>[] configEntries) where T : System.Enum
        {
            foreach (var config in configEntries)
            {

            }
        }

        public static void AddSliderPercentageOptions(bool restartRequired = false, params ConfigEntry<float>[] configEntries)
        {
            foreach (var config in configEntries)
            {
                ModSettingsManager.AddOption(new SliderOption(config, new SliderConfig { restartRequired = restartRequired }));
            }
        }

        public static void AddStringOptions(bool restartRequired = false, params ConfigEntry<string>[] configEntries)
        {
            foreach (var config in configEntries)
            {
                ModSettingsManager.AddOption(new StringInputFieldOption(config, new InputFieldConfig { restartRequired = restartRequired }));
            }
        }
    }
}
