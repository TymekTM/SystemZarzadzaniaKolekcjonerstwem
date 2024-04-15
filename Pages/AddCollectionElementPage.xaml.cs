using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;
using SystemZarzadzaniaKolekcjonerstwem.Controls;

namespace SystemZarzadzaniaKolekcjonerstwem.Pages;

public partial class AddCollectionElementPage : ContentPage
{
	private CollectionPage collectionPage;
    private CollectionElement collectionElement = new CollectionElement();
    private bool isEditing = false;

    public AddCollectionElementPage()
	{
		InitializeComponent(); 
	}

	public AddCollectionElementPage(CollectionPage collectionPage)
	{
		InitializeComponent();
		this.collectionPage = collectionPage;
	}

    public AddCollectionElementPage(CollectionPage collectionPage, CollectionElement collectionElement)
    {
        InitializeComponent();
        this.collectionPage = collectionPage;
        this.collectionElement = collectionElement;
        isEditing = true;

        NameEntry.Text = collectionElement.ElementName;
        DescriptionEntry.Text = collectionElement.ElementDescription;
        ValueEntry.Text = collectionElement.ElementValue.ToString();
        DatePicker.Date = DateTime.Parse(collectionElement.ElementDate);
        SoldCheckBox.IsChecked = collectionElement.ElementSold;

        Debug.WriteLine(collectionElement.ElementImage);
        ElementImage.Source = collectionElement.ElementImage;
        ElementImageText.Text = "Image selected: " + Path.GetFileName(collectionElement.ElementImage);
    }

	private async void SelectImage_Clicked(object sender, EventArgs e)
	{
        await SelectImage();
	}

    private async Task SelectImage()
    {
        string elementName = NameEntry.Text;
        if (string.IsNullOrEmpty(NameEntry.Text))
        {
            await DisplayAlert("Error", "Please set element name before you select image!", "OK");
            return;
        }
        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Pick an image",
            FileTypes = FilePickerFileType.Images
        });

        if (result != null)
        {
            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string imagePath = result.FullPath;
                string newFileName = elementName + ".png";

                string newFilePath = Path.Combine(appDataPath, newFileName);

                if (File.Exists(imagePath))
                {
                    int counter = 1;
                    string uniqueFileName = newFileName;
                    while (File.Exists(newFilePath))
                    {
                        uniqueFileName = $"{elementName}_{counter}.png";
                        newFilePath = Path.Combine(appDataPath, uniqueFileName);
                        counter++;
                    }
                    newFileName = uniqueFileName;
                }

                File.Copy(imagePath, newFilePath);

                collectionElement.ElementImage = newFilePath;
                ElementImage.Source = newFilePath;
                ElementImageText.Text = "Image selected: " + result.FileName;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }

	private void AddElement_Clicked(object sender, EventArgs e)
	{
        if(isEditing)
        {
            EditElement();
        }
        else
        {
            AddElement();
        }
	}

    private void EditElement()
    { 
        try
        {
            string elementName = NameEntry.Text;
            string elementDescription = DescriptionEntry.Text;
            string elementValueText = ValueEntry.Text;
            string elementDate = DatePicker.Date.ToString("dd/MM/yyyy");
            bool elementSold = SoldCheckBox.IsChecked;

            if (!CheckElements(elementName, elementDescription, elementDate))
                return;

            if (!CheckValue(elementValueText))
                return;

            float elementValue = float.Parse(elementValueText);

            CollectionElement newCollectionElement = new CollectionElement
            {
                ElementName = elementName,
                ElementDescription = elementDescription,
                ElementValue = elementValue,
                ElementDate = elementDate,
                ElementSold = elementSold,
                ElementImage = collectionElement.ElementImage
            };

            CollectionElement element = collectionPage.CollectionElements.First(element => element.ElementName == collectionElement.ElementName);
            int index = collectionPage.CollectionElements.IndexOf(element);
            Debug.WriteLine(index);

            collectionPage.CollectionElements[index] = newCollectionElement;

            collectionPage.SaveCollection();
            Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
    }
    private async void AddElement()
    {
        try
        {
            string elementName = NameEntry.Text;
            string elementDescription = DescriptionEntry.Text;
            string elementValueText = ValueEntry.Text;
            string elementDate = DatePicker.Date.ToString("dd/MM/yyyy");
            bool elementSold = SoldCheckBox.IsChecked;

            if (!CheckElements(elementName, elementDescription, elementDate))
                return;

            if (!CheckValue(elementValueText))
                return;

            float elementValue = float.Parse(elementValueText);

            collectionElement.ElementName = elementName;
            collectionElement.ElementDescription = elementDescription;
            collectionElement.ElementValue = elementValue;
            collectionElement.ElementDate = elementDate;
            collectionElement.ElementSold = elementSold;

            collectionPage.CollectionElements.Add(collectionElement);
            collectionPage.SaveCollection();
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
    }
	

	private bool CheckElements(string name, string desc, string date)
	{
		if(string.IsNullOrEmpty(name))
		{
            DisplayAlert("Error", "Element name cannot be empty", "OK");
            return false;
        }

		if(string.IsNullOrEmpty(desc))
		{
            DisplayAlert("Error", "Element description cannot be empty", "OK");
            return false;
        }


		if(string.IsNullOrEmpty(date))
		{
            DisplayAlert("Error", "Element date cannot be empty", "OK");
            return false;
        }

		return true;
	}

	private bool CheckValue(string val)
	{
        if (string.IsNullOrEmpty(val))
        {
            DisplayAlert("Error", "Element value cannot be empty", "OK");
            return false;
        }

        string pattern = @"^\d+([.]\d+)?$";
        Regex regex = new Regex(pattern);
        if (!regex.IsMatch(val))
        {
            DisplayAlert("Error", "Element value must be a number", "OK");
            return false;
        }

        float value = float.Parse(val);
		if(value < 0)
		{
            DisplayAlert("Error", "Element value cannot be negative", "OK");
            return false;
        }

        return true;
    }
}