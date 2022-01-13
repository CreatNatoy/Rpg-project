#if GAIA_PRESENT && UNITY_EDITOR

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using BackwoodsGaming.Framework;
using System.Reflection;
using System.Text;
using UnityEditor;

namespace Gaia.GX.BackwoodsGaming
{
    /// <summary>
    /// Gaia Item Pack
    /// </summary>
    public class V004IslandsXtn : MonoBehaviour {

        #region Generic informational methods

        /// <summary>
        /// Returns the publisher name if provided. 
        /// This will override the publisher name in the namespace ie Gaia.GX.PublisherName
        /// </summary>
        /// <returns>Publisher name</returns>
        public static string GetPublisherName()
        {
            return "Backwoods Gaming";
        }

        /// <summary>
        /// Returns the package name if provided
        /// This will override the package name in the class name ie public class PackageName.
        /// </summary>
        /// <returns>Package name</returns>
        public static string GetPackageName()
        {
            return "Vol 4 - Island Stamps Pack";
        }

        #endregion
		
		#region Class Variables
		
		private static List<string> _packContents = new List<string>()
		{
				"Vol4Islands001",
				"Vol4Islands002",
				"Vol4Islands003",
				"Vol4Islands004",
				"Vol4Islands005",
				"Vol4Islands006",
				"Vol4Islands007",
				"Vol4Islands008",
				"Vol4Islands009",
				"Vol4Islands010",
		};
		
		#endregion
		
		public static void GX_About_About()
		{
			EditorUtility.DisplayDialog("About Vol 4 - Island Stamps Pack", "This stamps pack includes a variety of islands, everything from sandbar islands with shallow waters to mountainous islands in the deeps.  This pack is a perfect addition to fill your island hopping needs!", "OK");
		}
		
		
		// Bulk Operations Foldout
		// These operations are for enabling/disabling all stamps from pack
		//
		
		public static void GX_StampBulkOperations_EnableAllStampsFromPack()
		{
			for ( int itemCnt = 0; itemCnt < _packContents.Count; itemCnt++ )
			{
				BWGGaiaExtUtils.MoveBWStamp(_packContents[itemCnt], BWGGaiaExtConst.BWGStampFeatureType.Islands, "Assets/BackwoodsGaming/GaiaPacks/Stamp/V004Islands/Stamps/Islands/", 1);
			}
		}
		
		public static void GX_StampBulkOperations_DisableAllStampsFromPack()
		{
			for ( int itemCnt = 0; itemCnt < _packContents.Count; itemCnt++ )
			{
				BWGGaiaExtUtils.MoveBWStamp(_packContents[itemCnt], BWGGaiaExtConst.BWGStampFeatureType.Islands, "Assets/BackwoodsGaming/GaiaPacks/Stamp/V004Islands/Stamps/Islands/", 0);
			}
		}
		
		
		// Enable/Disable Individual Stamps
		// These operations are for enabling/disabling stamps individually
		//
		
		public static void GX_IndividualStamps_Vol4Islands001Stamp()
		{
			BWGGaiaExtUtils.MoveBWStamp(_packContents[0], BWGGaiaExtConst.BWGStampFeatureType.Islands, "Assets/BackwoodsGaming/GaiaPacks/Stamp/V004Islands/Stamps/Islands/");
		}
		
		public static void GX_IndividualStamps_Vol4Islands002Stamp()
		{
			BWGGaiaExtUtils.MoveBWStamp(_packContents[1], BWGGaiaExtConst.BWGStampFeatureType.Islands, "Assets/BackwoodsGaming/GaiaPacks/Stamp/V004Islands/Stamps/Islands/");
		}
		
		public static void GX_IndividualStamps_Vol4Islands003Stamp()
		{
			BWGGaiaExtUtils.MoveBWStamp(_packContents[2], BWGGaiaExtConst.BWGStampFeatureType.Islands, "Assets/BackwoodsGaming/GaiaPacks/Stamp/V004Islands/Stamps/Islands/");
		}
		
		public static void GX_IndividualStamps_Vol4Islands004Stamp()
		{
			BWGGaiaExtUtils.MoveBWStamp(_packContents[3], BWGGaiaExtConst.BWGStampFeatureType.Islands, "Assets/BackwoodsGaming/GaiaPacks/Stamp/V004Islands/Stamps/Islands/");
		}
		
		public static void GX_IndividualStamps_Vol4Islands005Stamp()
		{
			BWGGaiaExtUtils.MoveBWStamp(_packContents[4], BWGGaiaExtConst.BWGStampFeatureType.Islands, "Assets/BackwoodsGaming/GaiaPacks/Stamp/V004Islands/Stamps/Islands/");
		}
		
		public static void GX_IndividualStamps_Vol4Islands006Stamp()
		{
			BWGGaiaExtUtils.MoveBWStamp(_packContents[5], BWGGaiaExtConst.BWGStampFeatureType.Islands, "Assets/BackwoodsGaming/GaiaPacks/Stamp/V004Islands/Stamps/Islands/");
		}
		
		public static void GX_IndividualStamps_Vol4Islands007Stamp()
		{
			BWGGaiaExtUtils.MoveBWStamp(_packContents[6], BWGGaiaExtConst.BWGStampFeatureType.Islands, "Assets/BackwoodsGaming/GaiaPacks/Stamp/V004Islands/Stamps/Islands/");
		}
		
		public static void GX_IndividualStamps_Vol4Islands008Stamp()
		{
			BWGGaiaExtUtils.MoveBWStamp(_packContents[7], BWGGaiaExtConst.BWGStampFeatureType.Islands, "Assets/BackwoodsGaming/GaiaPacks/Stamp/V004Islands/Stamps/Islands/");
		}
		
		public static void GX_IndividualStamps_Vol4Islands009Stamp()
		{
			BWGGaiaExtUtils.MoveBWStamp(_packContents[8], BWGGaiaExtConst.BWGStampFeatureType.Islands, "Assets/BackwoodsGaming/GaiaPacks/Stamp/V004Islands/Stamps/Islands/");
		}
		
		public static void GX_IndividualStamps_Vol4Islands010Stamp()
		{
			BWGGaiaExtUtils.MoveBWStamp(_packContents[9], BWGGaiaExtConst.BWGStampFeatureType.Islands, "Assets/BackwoodsGaming/GaiaPacks/Stamp/V004Islands/Stamps/Islands/");
		}
		
		
		// Playback Individual Default Sessions
		// These operations are for playing back individual area Gaia sessions
		//      using the default Gaia resources files
		//
		
		public static void GX_PlaySessions_Vol4Islands001Session()
		{
			BWGGaiaExtUtils.PlayBWSession("Vol4Islands001", BWGGaiaExtConst.BWGStampFeatureType.Islands, "V004Islands");
		}
		
		public static void GX_PlaySessions_Vol4Islands002Session()
		{
			BWGGaiaExtUtils.PlayBWSession("Vol4Islands002", BWGGaiaExtConst.BWGStampFeatureType.Islands, "V004Islands");
		}
		
		public static void GX_PlaySessions_Vol4Islands003Session()
		{
			BWGGaiaExtUtils.PlayBWSession("Vol4Islands003", BWGGaiaExtConst.BWGStampFeatureType.Islands, "V004Islands");
		}
		
		public static void GX_PlaySessions_Vol4Islands004Session()
		{
			BWGGaiaExtUtils.PlayBWSession("Vol4Islands004", BWGGaiaExtConst.BWGStampFeatureType.Islands, "V004Islands");
		}
		
		public static void GX_PlaySessions_Vol4Islands005Session()
		{
			BWGGaiaExtUtils.PlayBWSession("Vol4Islands005", BWGGaiaExtConst.BWGStampFeatureType.Islands, "V004Islands");
		}
		
		public static void GX_PlaySessions_Vol4Islands006Session()
		{
			BWGGaiaExtUtils.PlayBWSession("Vol4Islands006", BWGGaiaExtConst.BWGStampFeatureType.Islands, "V004Islands");
		}
		
		public static void GX_PlaySessions_Vol4Islands007Session()
		{
			BWGGaiaExtUtils.PlayBWSession("Vol4Islands007", BWGGaiaExtConst.BWGStampFeatureType.Islands, "V004Islands");
		}
		
		public static void GX_PlaySessions_Vol4Islands008Session()
		{
			BWGGaiaExtUtils.PlayBWSession("Vol4Islands008", BWGGaiaExtConst.BWGStampFeatureType.Islands, "V004Islands");
		}
		
		public static void GX_PlaySessions_Vol4Islands009Session()
		{
			BWGGaiaExtUtils.PlayBWSession("Vol4Islands009", BWGGaiaExtConst.BWGStampFeatureType.Islands, "V004Islands");
		}
		
		public static void GX_PlaySessions_Vol4Islands010Session()
		{
			BWGGaiaExtUtils.PlayBWSession("Vol4Islands010", BWGGaiaExtConst.BWGStampFeatureType.Islands, "V004Islands");
		}
	}
}
#endif
