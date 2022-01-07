#if GAIA_PRESENT && UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace BackwoodsGaming.Framework
{
    public class BWGGaiaExtConst : MonoBehaviour
    {
        /// <summary>
        /// Version information
        /// </summary>
        public const string BACKWOODS_PACK_EXTENSION_MAJOR = "1";
        public const string BACKWOODS_PACK_EXTENSION_MINOR = "0.0";
        public const string BACKWOODS_PUBLISHER_URL = "https://www.assetstore.unity3d.com/en/#!/search/page=1/sortby=popularity/query=publisher:15645";

        /// <summary>
        /// Gaia Pack information
        /// </summary>
        public enum BWGGaiaPackType { Stamp, POI }

        public List<string> BWGStampPack = new List<string>()
        {
            "Village Area",
            "Mountain and Volcano",
        };

        public List<string> BWGPOIPack = new List<string>()
        {
            "3DForge Woodland Village",
        };

        public enum BWGStampFeatureType { None, Adhoc, Bases, Hills, Islands, Lakes, Mesas, Mountains, Plains, Rivers, Rocks, Valleys, Villages, Waterfalls};
    }
}
#endif