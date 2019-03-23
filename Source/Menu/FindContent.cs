// COPYRIGHT 2019 by the Open Rails project.
// 
// This file is part of Open Rails.
// 
// Open Rails is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Open Rails is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Open Rails.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json; // Added Assembly reference to Menu project
using Newtonsoft.Json.Serialization; // For debugging. Requires NewstonSoft.Json v12.0
using System.IO;

namespace ORTS
{
    public class FindContent
    {
        //CJ

        // First time Menu.exe runs, it visits www.openrails.org to get a list of publishers.
        // From that we go out to the publishers to get lists of their products and also any images and videos.

        // There are opportunities for caching lots of these activities:
        // - Menu.exe can save images and videos locally.
        // - The website can provide a single JSON file of all the publishers' product lists.
        // - The website can run an overnight script to visit each publisher and update the list of products if they have changed.
        // - That file can include a modification date, so that Menu.exe knows whether to update the local copy.

        private class ContentList
        {
            public string Context { get; set; }         // "@context": "http://schema.org"
            public string Type { get; set; }            // "@type": "ItemList"
            public string Name { get;  set; }            // "name": "[Full Name]"
            public string Description { get; set; }     // "description": "[Short Description]"
            public List<Publisher> PublisherList { get; set; } 
        }

        private class Publisher
        {
            public string Type { get; set; }            // "@type": "Organization"
            public string Name { get; set; }            // "name": "[Full Name]"
            public string Url { get; set; }             // "url": "[URL for a product page]",
            public string Logo { get; set; }            // "logo": "[URL for an image] TO DO"
            public List<Product> ProductList { get; set; } 
        }

        private class Product
        {
            public string Type { get; set; }            // "@type": "SoftwareApplication"
            public string Identifier { get; set; }      // see PATH below. Excludes characters /\:*?"<>|

            // Market Information
            public string ContentType { get; set; }     // mini-route, route, trainset, consist, sound, activity, asset
            public string Name { get; set; }            // full product name, e.g. "BNSF Scenic Route"
            public string Description { get; set; }     // optional
            public string Url { get; set; }             // optional: link to a webpage about the product
            public string ThumbnailUrl { get; set; }    // optional: link to a thumbnail image 320x180 pixels
            public string Logo { get; set; }            // "logo": "[URL for an image] TO DO"
            public ContentLocation ContentLocation { get; set; } // optional
            public string TemporalCoverage { get; set; } // optional: era depicted by content, eg "1950-1960"
            public string Gauge { get; set; }           // optional: e.g. standard, narrow, broad
            public bool IsAccessibleForFree { get; set; } // false for paid content
            public Offers Offers { get; set; }          // optional

            // Install Information
            public int? SoftwareVersion { get; set; }   // optional: integer starting from 1, used to highlight new versions
            public string CopyrightHolder { get; set; } // optional
            public int? CopyrightYear { get; set; }     // optional: integer eg 2018
            public string License { get; set; }         // optional: link to license text. If missing, copyright is assumed
            
            // TODO
            //public AttributionList AttributionList { get; set; }
            //public SoftwareRequirementList SoftwareRequirementList { get; set; }

            public string DownloadUrl { get; set; }     // link from which self-installer or Zip is downloaded
            public int? StorageRequirement { get; set; }// optional: size of installation in MB
            public string installerType { get; set; }   // e.g. "Zip", "Exe", "generic"	
        }

        private class ContentLocation
        {
            public string Type { get; set; }            // "@type": "Place"
            public string Name { get; set; }            // name of country e.g. USA
            public string Identifier { get; set; }      // 2-letter ISO 3166-1 alpha-2 country code. e.g. us 
        }

        private class Offers
        {
            public string Type { get; set; }            // "@type": "Offer"
            public decimal Price { get; set; }          // // decimals are permitted
            public string PriceCurrency { get; set; }   // using ISO 4217 format, e.g. "USD"
        }

        public FindContent()
        {   
            var json = new StreamReader(@"si-content.json", Encoding.UTF8).ReadToEnd();
            var contentList = JsonConvert.DeserializeObject<ContentList>(json); 
            
            // For debugging JSON, uncomment next 3 statements instead of statement above
            //ITraceWriter traceWriter = new MemoryTraceWriter();
            //var contentList = JsonConvert.DeserializeObject<ContentList>(json, 
            //    new JsonSerializerSettings { TraceWriter = traceWriter });
            //Console.WriteLine(traceWriter);


        }
    }
}
