using CommunityToolkit.Maui.Storage;
using System.Diagnostics;
using System.Text;
using System.Threading;
using SystemZarzadzaniaKolekcjonerstwem.Controls;


namespace SystemZarzadzaniaKolekcjonerstwem.Pages;

public partial class MainPage : ContentPage
{
	List<MyCollection> collections = new List<MyCollection>();

	public MainPage()
	{
        try
        {
            InitializeComponent();
            LoadCollections();
            RefreshCollections();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
	}

    protected override void OnDisappearing()
    {
        SaveCollections();
    }
    
    protected override void OnAppearing()
    {
        LoadCollections();
        RefreshCollections();
    }

	private void LoadCollections()
    {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string filePath = Path.Combine(appDataPath, "collections.txt");
        
        Debug.WriteLine(filePath);

        if (File.Exists(filePath))
        {
            collections.Clear();

            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] sections = line.Split(';');
                if(sections.Length > 0)
                {
                    string[] collectionInfo = sections[0].Split(",");
                    if(collectionInfo.Length >= 2)
                    {
                        string collectionName = collectionInfo[0];
                        string collectionImage = collectionInfo[1];
                        MyCollection collection = new MyCollection(collectionName, collectionImage);

                        foreach(string element in sections.Skip(1))
                        {
                            string[] infoRows = element.Split("|");

                            string[] elementInfo = infoRows[0].Split(",");
                            string elementName = elementInfo[0];
                            string elementDescription = elementInfo[1];
                            string elementImage = elementInfo[2];
                            float elementValue = float.Parse(elementInfo[3]);
                            string elementDate = elementInfo[4];
                            bool elementSold = bool.Parse(elementInfo[5]);
                            CollectionElement collectionElement = new CollectionElement(elementName, elementImage, elementDescription, elementValue, elementDate, elementSold);

                            collection.AddElement(collectionElement);
                        }

                        collections.Add(collection);
                    }
                }
            }
       }
    }

	private void SaveCollections()
    {
        Debug.WriteLine("Saving collections");
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string filePath = Path.Combine(appDataPath, "collections.txt");

        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
        }

        File.WriteAllText(filePath, string.Empty);

        foreach (var item in collections)
        {
            string data = item.CollectionName + "," + item.CollectionImage;
            item.GetElements().ForEach(element =>
            {
                data += ";";
                data += element.ElementName + "," + element.ElementDescription + "," + element.ElementImage + "," + element.ElementValue.ToString() + "," + element.ElementDate + "," + element.ElementSold.ToString();
            });

            File.AppendAllText(filePath, data + Environment.NewLine);
        }
    }

	private void RefreshCollections()
	{
        Border AddCollection = CollectionList.FindByName<Border>("AddCollectionCard");
		CollectionList.Children.Clear();
        foreach (MyCollection collection in collections)
		{
            CollectionCard card = new CollectionCard(collection);
            card.RemoveCollectionClicked += OnRemoveCollectionClicked;
            CollectionList.Children.Add(card);
        }
		CollectionList.Children.Add(AddCollection);
    }

	private async void AddCollection_Tapped(object sender, EventArgs e)
	{
        await Navigation.PushAsync(new AddCollectionPage());
    }

    private void OnRemoveCollectionClicked(object sender, EventArgs e)
    {
        if(sender is CollectionCard card)
        {
            MyCollection elementToRemove = collections.First(col => col.CollectionName == card.CollectionName);
            File.Delete(elementToRemove.CollectionImage);
            collections.Remove(elementToRemove);
            RefreshCollections();
            SaveCollections();
        }
    }


}