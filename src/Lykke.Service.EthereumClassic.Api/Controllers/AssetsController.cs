using System;
using System.Collections.Generic;
using Lykke.Service.BlockchainApi.Contract.Assets;
using Lykke.Service.EthereumClassic.Api.Common;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EthereumClassic.Api.Controllers
{
    [Route("/api/assets")]
    public class AssetsController : Controller
    {
        private static readonly AssetResponse AssetResponse;

        private static readonly IEnumerable<AssetResponse> AssetsResponse;


        static AssetsController()
        {
            AssetResponse = new AssetResponse
            {
                Accuracy = Constants.EtcAsset.Accuracy,
                AssetId  = Constants.EtcAsset.AssetId,
                Name     = Constants.EtcAsset.Name
            };

            AssetsResponse = new[]
            {
                AssetResponse
            };
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
                return NotFound();
            }
        }

        [HttpGet]
        public IActionResult GetAssetList([FromQuery] int take, [FromQuery] string continuation = "")
        {
            return Ok(AssetsResponse);
        }
    }
}
