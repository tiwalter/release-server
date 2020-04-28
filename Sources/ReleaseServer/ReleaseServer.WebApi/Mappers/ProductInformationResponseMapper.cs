using System.Collections.Generic;
using ReleaseServer.WebApi.Models;

namespace ReleaseServer.WebApi.Mappers
{
    public static class ProductInformationResponseMapper
    {
        public static ProductInformationResponseModel ToProductInfoResponse(this ProductInformationModel productInfo)
        {
            return new ProductInformationResponseModel
            {
                Identifier = productInfo.ProductIdentifier,
                Os = productInfo.Os,
                Version = productInfo.Version.ToString(),
                Architecture = productInfo.HwArchitecture,
                Changelog = productInfo.Changelog,
                ReleaseDate = productInfo.ReleaseDate
            };
        }

        public static ProductInformationListResponseModel ToProductInfoListResponse(this List<ProductInformationModel> productInfoList)
        {
            var retVal = new List<ProductInformationResponseModel>();

            foreach (var productInfo in productInfoList)
            {
                retVal.Add(productInfo.ToProductInfoResponse());
            }
            
            return new ProductInformationListResponseModel
            {
                ProductInformation = retVal,
                Count = retVal.Count
            };
        }

        public static ProductInformationListResponseModel ToProductInfoListResponse(this List<ProductInformationModel> productInfoList, int limit, int offset)
        {
            var retVal = new List<ProductInformationResponseModel>();

            productInfoList.RemoveRange(0, offset);
            int? nextOffset = null;

            int count = 0;
            foreach (var productInfo in productInfoList)
            {
                if (count++ < limit) {
                    retVal.Add(productInfo.ToProductInfoResponse());
                }
                else {
                    nextOffset = limit + offset;
                    break;
                }
            }

            return new ProductInformationListResponseModel
            {
                ProductInformation = retVal,
                Count = count - 1,
                NextOffset = nextOffset
            };
        }
    }
}

