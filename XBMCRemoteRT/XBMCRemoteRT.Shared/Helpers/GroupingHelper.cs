using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Windows.Globalization.Collation;
using XBMCRemoteRT.Models.Common;

namespace XBMCRemoteRT.Helpers
{
    public class GroupingHelper
    {
        public delegate string GetNameDelegate<T>(T item);

        private static string GetTrimmedName(string itemName)
        {
            if (itemName.StartsWith("a ", StringComparison.OrdinalIgnoreCase))
            {
                itemName = itemName.Remove(0, 2);
            }
            else if (itemName.StartsWith("an ", StringComparison.OrdinalIgnoreCase))
            {
                itemName = itemName.Remove(0, 3);
            }
            else if (itemName.StartsWith("the ", StringComparison.OrdinalIgnoreCase))
            {
                itemName = itemName.Remove(0, 4);
            }
            return itemName;
        }
        private static string allLetters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTWXYZ";
        private static string allNumbers = "0123456789";
        
        public static List<ListGroup<T>> GroupList<T>(IEnumerable<T> listToGroup, GetNameDelegate<T> getName, bool sort)
        {
            List<ListGroup<T>> groupedList = new List<ListGroup<T>>();
            CharacterGroupings slg = new CharacterGroupings();
            foreach (CharacterGrouping key in slg)
            {
                if (string.IsNullOrWhiteSpace(key.Label) == false)
                    groupedList.Add(new ListGroup<T>(key.Label));
            }

            foreach (T item in listToGroup)
            {
                string itemName = GetTrimmedName(getName(item));

                string groupLabel = slg.Lookup(itemName);
                if (!string.IsNullOrEmpty(groupLabel))
                    groupedList.Find(t => t.Key == groupLabel).Add(item);
            }

            if (sort)
            {
                foreach (ListGroup<T> group in groupedList)
                {
                    group.Sort((c0, c1) => { return CultureInfo.InvariantCulture.CompareInfo.Compare(GetTrimmedName(getName(c0)), GetTrimmedName(getName(c1))); });
                }
            }

            return groupedList;
        }
    }
}
