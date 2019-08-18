using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace TestAutomation.Helper.Models
{
    [XmlRoot("MovieInfo")]
    public class MovieInfo
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        public string WikiScreenShotPath { get; set; }

        public List<string> WikiLinks { get; set; }

        public string WikiLink { get; set; }

        public string Directors_Wiki { get; set; }

        public string ImdbScreenShotPath { get; set; }

        public string ImdbLink { get; set; }

        public string Directors_Imdb { get; set; }

        public bool Passed
        {
            get
            {
                var wikiList = !string.IsNullOrEmpty(Directors_Wiki) ? Directors_Wiki.Split(',').ToList() : null;
                var imdbList = !string.IsNullOrEmpty(Directors_Imdb) ?  Directors_Imdb.Split(',').ToList() : null;
              
                if (wikiList != null && imdbList != null && wikiList.Count() == imdbList.Count() && wikiList.Intersect(imdbList, StringComparer.InvariantCultureIgnoreCase).Count() == wikiList.Count())
                {
                    return true;
                }                    

                return false;
            }
        }

        public IEnumerable<string> ToArray()
        {
            return new string[]
            {
                    ReportGenerator.GenerateElementHtml(Name,ReportElementType.Text),
                    ReportGenerator.GenerateElementHtml(WikiLink,ReportElementType.Link,WikiLink),
                    ReportGenerator.GenerateElementHtml(Directors_Wiki,ReportElementType.Table),
                    ReportGenerator.GenerateElementHtml(WikiScreenShotPath,ReportElementType.Image,WikiScreenShotPath),
                    ReportGenerator.GenerateElementHtml(ImdbLink,ReportElementType.Link,ImdbLink),
                    ReportGenerator.GenerateElementHtml(Directors_Imdb,ReportElementType.Table),
                    ReportGenerator.GenerateElementHtml(ImdbScreenShotPath,ReportElementType.Image,ImdbScreenShotPath),
                    ReportGenerator.GenerateElementHtml(Passed.ToString(),ReportElementType.Text),
            };
        }
    }
}
