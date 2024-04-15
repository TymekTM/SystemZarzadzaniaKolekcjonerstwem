using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemZarzadzaniaKolekcjonerstwem.Controls
{
    public class MyCollection
    {
        public string CollectionName { get; set; }
        public string CollectionImage { get; set; }

        public List<CollectionElement> collectionElements = new List<CollectionElement>();

        public MyCollection(string collectionName, string collectionImage)
        {
            CollectionName = collectionName;
            CollectionImage = collectionImage;
        }
        
        public MyCollection(string collectionName, string collectionImage, List<CollectionElement> collectionElements) : this(collectionName, collectionImage)
        {
            foreach(CollectionElement element in collectionElements)
            {
                CollectionElement newElement = new CollectionElement
                {
                    ElementName = element.ElementName,
                    ElementImage = element.ElementImage,
                    ElementDescription = element.ElementDescription,
                    ElementDate = element.ElementDate,
                    ElementValue = element.ElementValue,
                    ElementSold = element.ElementSold
                };
            }
        }

        public void AddElement(CollectionElement element)
        {
            collectionElements.Add(element);
        }

        public void RemoveElement(CollectionElement element)
        {
            collectionElements.Remove(element);
        }

        public void RemoveElement(int index)
        {
            collectionElements.RemoveAt(index);
        }

        public void ClearElements()
        {
            collectionElements.Clear();
        }

        public List<CollectionElement> GetElements()
        {
            return collectionElements;
        }
    }
}
