using System.Collections.Generic;

namespace ReleaseServer.WebApi.Models
{
    public class ProductInformationListResponseModel
    {
        public List<ProductInformationResponseModel> ProductInformation { get; set; }

        public int Count { get; set; }

        public int? NextOffset { get; set; }
    }
    
    public class ProductInformationResponseModel
    {
        public string Identifier { get; set; }
        
        public string Version { get; set;  }

        public string Os { get; set; }
        
        public string Architecture { get; set; }

        public string Changelog { get; set; }

        public string ReleaseDate { get; set; }
    }
}