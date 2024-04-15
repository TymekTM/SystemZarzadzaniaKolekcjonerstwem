using CommunityToolkit.Maui.Storage;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using SystemZarzadzaniaKolekcjonerstwem.Pages;

namespace SystemZarzadzaniaKolekcjonerstwem.Controls;

public partial class CollectionCard : ContentView
{
    public event PropertyChangedEventHandler PropertyChanged;

    public event EventHandler RemoveCollectionClicked;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        if (propertyName == nameof(CollectionElements))
        {
            CollectionElementsSizeText = $"Collection size: {(CollectionElements?.Count ?? 0)}";
        }
    }

    public static readonly BindableProperty CollectionNameProperty = 
        BindableProperty.Create(nameof(CollectionName), typeof(string), typeof(CollectionCard), string.Empty);

    public static readonly BindableProperty CollectionImageProperty = 
        BindableProperty.Create(nameof(CollectionImage), typeof(string), typeof(CollectionCard), string.Empty);
    
    public static BindableProperty CollectionElementsSizeTextProperty =
        BindableProperty.Create(nameof(CollectionElementsSizeText), typeof(string), typeof(CollectionCard), string.Empty);
    public string CollectionName
    {
        get => (string)GetValue(CollectionNameProperty);
        set => SetValue(CollectionNameProperty, value);
    }
    public string CollectionImage
    {
        get => (string)GetValue(CollectionImageProperty);
        set => SetValue(CollectionImageProperty, value);
    }

    private List<CollectionElement> _collectionElements;

    public List<CollectionElement> CollectionElements
    {
        get => _collectionElements;
        set
        {
            if (_collectionElements != value)
            {
                _collectionElements = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CollectionElementsSizeText));
            }
        }
    }
    public string CollectionElementsSizeText
    {
        get => $"Collection size: {(CollectionElements?.Count ?? 0)}";
        set => SetValue(CollectionElementsSizeTextProperty, value);
    }
    
    public CollectionCard()
	{
		InitializeComponent();
    }

    public CollectionCard(MyCollection collection)
	{
        InitializeComponent();
        CollectionName = collection.CollectionName;
        CollectionImage = collection.CollectionImage;
        CollectionElements = collection.GetElements();
    }

    private void CollectionCard_Tapped(object sender, TappedEventArgs e)
    {
        Navigation.PushAsync(new CollectionPage(CollectionName, CollectionImage, CollectionElements));
    }


    private void Edit_Clicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Edit clicked");
        Navigation.PushAsync(new AddCollectionPage(CollectionName, CollectionImage, CollectionElements));
    }

    private void Delete_Clicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Delete clicked");
        RemoveCollectionClicked?.Invoke(this, EventArgs.Empty);
    }
}
