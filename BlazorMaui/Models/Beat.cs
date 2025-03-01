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
    private Guid guid;
    [ObservableProperty]
    private string title;
    [ObservableProperty]
    private int bpm;
    [ObservableProperty]
    public string genre;
    [ObservableProperty]
    public int plays;

    [ObservableProperty]
    public string audioUrl;

    public Beat()
    {
        guid = Guid.NewGuid();
    }


}

