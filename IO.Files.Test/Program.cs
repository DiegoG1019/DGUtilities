using DiegoG.Utilities.Settings;
using System;
using System.ComponentModel;

Settings<MySettings>.Initialize("A", "s");

//Settings<Config>.ApplyEnvironmentVariables();

var x = Settings<MySettings>.Current;
var y = Settings<Config>.Current;

;

public class Config : ISettings
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public string SettingsType => "Nebheka.NotificationNetwork.Config";
    public ulong Version => 0;

    [FromEnvironmentVariable(true, Precedence = false, Required = true)]
    public string[] Tokens { get; set; } = Array.Empty<string>();

    [FromEnvironmentVariable(Precedence = false, Required = true)]
    public string DevKey { get; set; }

    [FromEnvironmentVariable(Precedence = false, Required = true)]
    public double TransactionThreshold { get; set; } = 10_000;
}

public class MySettings : ISettings
{
    public string SettingsType => "asdwws";
    public ulong Version => 1;

    [FromEnvironmentVariable(true)]
    public string[] Meredith { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public MySubSettings MySubSettings { get; init; } = new();

    public MySecondSub MySecondSub { get; init; } = new();
}

public class Abs : ISettingsSection
{
    [FromEnvironmentVariable]
    public string Anth { get; set; }
}

public class MySubSettings : ISettingsSection
{
    [FromEnvironmentVariable(Required = true)]
    public string? Absol { get; set; }

    public Abs Absolute { get; init; } = new();
}

public class MySecondSub : ISettingsSection
{
    [FromEnvironmentVariable(Required = true)]
    public string? Valery { get; set; }
}