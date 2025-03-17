using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using MetadataExtractor;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class Beat : ObservableObject
{
    [ObservableProperty]
    private int id;
    [ObservableProperty]
    private string title;
    [ObservableProperty]
    private int? bpm;
    [ObservableProperty]
    private string? genre;
    [ObservableProperty]
    private int? plays;

    [ObservableProperty]
    private bool isVisible;

    [ObservableProperty]
    private string audioUrl;
    [ObservableProperty]
    private string waveFormImageBase64;




}

