using DiegoG.Utilities.Settings;
using System;
using System.ComponentModel;

Environment.SetEnvironmentVariable("Anth", "asdw");
Environment.SetEnvironmentVariable("Absol", "asdw");
Environment.SetEnvironmentVariable("Valery", "asdw");

Settings<MySettings>.Initialize("A", "s");

var x = Settings<MySettings>.Current;

;

public class MySettings : ISettings
{
    public string SettingsType { get; }
    public ulong Version { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public MySubSettings MySubSettings { get; init; } = new();

    public MySecondSub MySecondSub { get; init; } = new();
}

public class Abs : ISettingsSection
{
    [FromEnvironmentVariable(nameof(Anth), Required = true)]
    public string Anth { get; set; }
}

public class MySubSettings : ISettingsSection
{
    [FromEnvironmentVariable(nameof(Absol), Required = true)]
    public string? Absol { get; set; }

    public Abs Absolute { get; init; } = new();
}

public class MySecondSub : ISettingsSection
{
    [FromEnvironmentVariable(nameof(Valery), Required = true)]
    public string? Valery { get; set; }
}