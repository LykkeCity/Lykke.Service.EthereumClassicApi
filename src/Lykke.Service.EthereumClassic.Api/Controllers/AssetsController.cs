using System;
using System.Collections.Immutable;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Assets;
using Lykke.Service.EthereumClassic.Api.Common;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EthereumClassic.Api.Controllers
{
    [Route("/api/assets")]
    public class AssetsController : Controller
    {
        private static readonly AssetResponse AssetResponse;

        private static readonly ImmutableList<AssetResponse> AssetsResponse;


        static AssetsController()
        {
            AssetResponse = new AssetResponse
            {
                Accuracy = Constants.EtcAsset.Accuracy,
                AssetId  = Constants.EtcAsset.AssetId,
                Name     = Constants.EtcAsset.Name
            };

            AssetsResponse = (new[] { AssetResponse })
                .ToImmutableList();
        }

        
        [HttpGet("{assetId}")]
        public IActionResult GetAsset(string assetId)
        {
            if (Constants.EtcAsset.AssetId.Equals(assetId, StringComparison.InvariantCultureIgnoreCase))
            {
                return Ok(AssetResponse);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpGet]
        public IActionResult GetAssetList([FromQuery] int take, [FromQuery] string continuation = "")
        {
            return Ok(new PaginationResponse<AssetResponse>
            {
                Continuation = null,
                Items        = AssetsResponse
            });
        }
    }
}
