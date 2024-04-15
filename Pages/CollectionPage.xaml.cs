namespace SystemZarzadzaniaKolekcjonerstwem.Pages;

using CommunityToolkit.Maui.Storage;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.Text;
using SystemZarzadzaniaKolekcjonerstwem.Controls;
public partial class CollectionPage : ContentPage
{
	public string CollectionName { get; set; }
    public string CollectionImage { get; set; }
    public List<CollectionElement> CollectionElements { get; set; } = new List<CollectionElement>();
	public CollectionPage()
	{
        Debug.WriteLine("CollectionPage constructor");
		InitializeComponent();
	}

    override protected void OnAppearing()
    {
        base.OnAppearing();
        if(CollectionElements.Count > 0)
        {
            RefreshCollectionElements();
        }
    }

    override protected void OnDisappearing()
    {
        base.OnDisappearing();
    }

	public CollectionPage(string collectionName, string collectionImage, List<CollectionElement> collectionElements)
	{
        try
        {
            InitializeComponent();
            CollectionName = collectionName;
            CollectionImage = collectionImage;
            foreach(CollectionElement element in collectionElements)
            {
                CollectionElement newElement = new CollectionElement(element.ElementName, element.ElementImage, element.ElementDescription, element.ElementValue, element.ElementDate, element.ElementSold);
                CollectionElements.Add(newElement);
            }

            foreach(CollectionElement element in CollectionElements)
            {
                element._CollectionPage = this;
            } 
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
    }

    private void RefreshCollectionElements()
    {
        Border AddElement = elementsLayout.FindByName<Border>("AddElementCard");
        elementsLayout.Children.Clear();
        foreach(CollectionElement element in CollectionElements)
        {
            if (!element.ElementSold)
            {
                element.RemoveElementClicked += OnRemoveElementClicked;
                elementsLayout.Children.Add(element);
            }
        }
        foreach(CollectionElement element in CollectionElements)
        {
            if(element.ElementSold)
            {
                element.RemoveElementClicked += OnRemoveElementClicked;
                elementsLayout.Children.Add(element);
            }
        }
        elementsLayout.Children.Add(AddElement);

    }

    private void OnRemoveElementClicked(object sender, EventArgs e)
    {
        if(sender is CollectionElement elementToRemove)
        {
            File.Delete(elementToRemove.ElementImage);
            CollectionElements.Remove(elementToRemove);
            RefreshCollectionElements();
        }
        SaveCollection();
    }

    private async void AddElement_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddCollectionElementPage(this));
        RefreshCollectionElements();
    }

    public void SaveCollection()
    {
        Debug.WriteLine("Saving collection");
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string filePath = Path.Combine(appDataPath, "collections.txt");

        string[] lines = File.ReadAllLines(filePath);
        string data = CollectionName + "," + CollectionImage;
        foreach (CollectionElement element in CollectionElements)
        {
            Debug.WriteLine("Saving element " + element.ElementName + " " + element.ElementDescription);
            data += ";" + element.ElementName + "," + element.ElementDescription + "," + element.ElementImage + "," + element.ElementValue + "," + element.ElementDate + "," + element.ElementSold;
        }

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith(CollectionName))
            {
                lines[i] = data;
                break;
            }
        }

        foreach(string line in lines)
        {
            Debug.WriteLine(line);
        }

        File.WriteAllLines(filePath, lines);
    }

}