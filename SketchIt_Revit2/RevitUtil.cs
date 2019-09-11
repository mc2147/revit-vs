using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace PW
{
    public class RevitUtil
    {
        /// <summary>
        /// Helper function: find a list of element with given Class, Name and Category (optional). 
        /// </summary>  
        public static IList<Element> FindElements(
          Document rvtDoc,
          Type targetType,
          string targetName,
          Nullable<BuiltInCategory> targetCategory)
        {
            // First, narrow down to the elements of the given type and category 
            var collector = new FilteredElementCollector(rvtDoc).OfClass(targetType);
            if (targetCategory.HasValue)
            {
                collector.OfCategory(targetCategory.Value);
            }

            // Parse the collection for the given names 
            // Using LINQ query here. 
            var elems =
                from element in collector
                where element.Name.Equals(targetName)
                select element;

            // Put the result as a list of element for accessibility. 

            return elems.ToList();
        }

        /// <summary>
        /// Helper function: searches elements with given Class, Name and Category (optional), 
        /// and returns the first in the elements found. 
        /// This gets handy when trying to find, for example, Level. 
        /// e.g., FindElement(_doc, GetType(Level), "Level 1") 
        /// </summary>
        public static Element FindElement(
          Document rvtDoc,
          Type targetType,
          string targetName,
          Nullable<BuiltInCategory> targetCategory)
        {
            // Find a list of elements using the overloaded method. 
            IList<Element> elems = FindElements(rvtDoc, targetType, targetName, targetCategory);

            // Return the first one from the result. 
            if (elems.Count > 0)
            {
                return elems[0];
            }

            return null;
        }

        
    }
}



