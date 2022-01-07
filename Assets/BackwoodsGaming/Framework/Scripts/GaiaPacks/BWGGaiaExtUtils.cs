#if GAIA_PRESENT && UNITY_EDITOR
using Gaia;
using UnityEngine;
using UnityEditor;

namespace BackwoodsGaming.Framework
{
    public class BWGGaiaExtUtils : MonoBehaviour
    {

        #region Helper methods

        /// <summary>
        /// Move the Gaia stamp to/from Gaia stamps folders based on selected mode.
        /// </summary>
        /// <param name="name">Name of the stamp</param>
        /// <param name="mode">Whether we are enabling (1) or disabling (0)</param>
        /// <param name= "gaiaType">The type of stamp being moved</param>
        /// <param name="assetFolderName">The name of the BackwodsGaming folder asset is stored in</param>
        /// <returns></returns>
        public static void MoveBWStamp( string name, BWGGaiaExtConst.BWGStampFeatureType gaiaType, string assetFolderName, int mode )
        {
            string stampOrigin;
            string stampDestination;

            // Either enable (1) to move stamp to Gaia folder or disable (0) to move stamp to package folder.
            if ( mode == 0 )
            {
                //Debug.Log("Disabling " + name + " from Gaia stamps.");
                //Debug.Log("Gaia Type: " + gaiaType);
                //Debug.Log("Asset Folder Name: " + assetFolderName);
                stampOrigin = "Assets/Gaia/Stamps/" + gaiaType + "/";
                stampDestination = assetFolderName;
            }
            else
            {
                //Debug.Log("Enabling " + name + " to Gaia stamps.");
                //Debug.Log("Gaia Type: " + gaiaType);
                //Debug.Log("Asset Folder Name: " + assetFolderName);
                stampOrigin = assetFolderName;
                stampDestination = "Assets/Gaia/Stamps/" + gaiaType + "/";
            }

            AssetDatabase.MoveAsset(stampOrigin + name + ".jpg", stampDestination + name + ".jpg");
            AssetDatabase.MoveAsset(stampOrigin + "Data/" + name + ".bytes", stampDestination + "Data/" + name + ".bytes");

            // Need to add error checking on the success/failure of MoveAsset.  Need to research.
        }


        /// <summary>
        /// Move the Gaia stamp to/from Gaia stamps folders based on where it currently resides.
        /// </summary>
        /// <param name="name">Name of the stamp</param>
        /// <param name= "gaiaType">The type of stamp being moved</param>
        /// <param name="assetFolderName">The name of the BackwodsGaming folder asset is stored in</param>
        /// <returns></returns>
        public static void MoveBWStamp( string name, BWGGaiaExtConst.BWGStampFeatureType gaiaType, string assetFolderName )
        {
            string stampOrigin;
            string stampDestination;

            // Check to see if the stamp is active in Gaia already, if so set the stamp to be disabled.
	        if ( System.IO.File.Exists(Gaia.Utils.GetGaiaAssetPath((GaiaConstants.FeatureType)gaiaType-1, name + ".jpg")) )
            {
                //Debug.Log("Disabling " + name + " from Gaia stamps.");
                //Debug.Log("Gaia Type: " + gaiaType);
                //Debug.Log("Asset Folder Name: " + assetFolderName);
                stampOrigin = "Assets/Gaia/Stamps/" + gaiaType + "/";
                stampDestination = assetFolderName;
            }
            else
            {
                //Debug.Log("Enabling " + name + " to Gaia stamps.");
                //Debug.Log("Gaia Type: " + gaiaType);
                //Debug.Log("Asset Folder Name: " + assetFolderName);
                stampOrigin = assetFolderName;
                stampDestination = "Assets/Gaia/Stamps/" + gaiaType + "/";
            }

            AssetDatabase.MoveAsset(stampOrigin + name + ".jpg", stampDestination + name + ".jpg");
            AssetDatabase.MoveAsset(stampOrigin + "Data/" + name + ".bytes", stampDestination + "Data/" + name + ".bytes");

            // Need to add error checking on the success/failure of MoveAsset.  Need to research.

        }

        public static void PlayBWSession(string stampName, BWGGaiaExtConst.BWGStampFeatureType featureType, string packShortname)
        {
            // Enable stamp if it isn't already enabled
            MoveBWStamp(stampName, featureType, "Assets/BackwoodsGaming/GaiaPacks/Stamp/"+packShortname+"/Stamps/"+featureType+"/", 1);

            // Check to see if a terrain already exists in the scene.  If so, give user option to flatten.
            Terrain t = Gaia.TerrainHelper.GetActiveTerrain();
            if (t != null)
            {
                if (EditorUtility.DisplayDialog("WARNING", "You have an existing terrain in your scene!  Would you like to flatten it or Quit?", "Quit", "Flatten"))
                {
                    Debug.Log("Quit selected? ");
                    return;
                }

                // User chose to flatten terrain.
                GaiaWorldManager wm = new GaiaWorldManager(Terrain.activeTerrains);
                wm.FlattenWorld();
            }

            // Create Gaia root game object if it doesn't already exist in scene hierarchy.
            GameObject gaiaObj = GameObject.Find("Gaia");
            if (gaiaObj == null)
            {
                gaiaObj = new GameObject("Gaia");
            }

            // Check for Session Manager and create if it doesn't exist.
            GaiaSessionManager sessionMgr = null;
            GameObject mgrObj = GameObject.Find("Session Manager");
            if (mgrObj == null)
            {
                mgrObj = new GameObject("Session Manager");
                sessionMgr = mgrObj.AddComponent<GaiaSessionManager>();
                mgrObj.transform.parent = gaiaObj.transform;
            }
            else
            {
                sessionMgr = mgrObj.GetComponent<GaiaSessionManager>();
            }

            // Find and load the saved session for this stamp
            sessionMgr.m_session = AssetDatabase.LoadAssetAtPath<GaiaSession>("Assets/BackwoodsGaming/GaiaPacks/Stamp/"+packShortname+"/Data/GS-"+stampName+".asset");

            // Debug.Log("Session: " + sessionMgr.m_session.m_name);

            // Play the session after user confirms
            if (EditorUtility.DisplayDialog("Playback Session ?", "Are you sure you want to playback this session - this can not be undone ?", "Yes", "No"))
            {
                sessionMgr.PlaySession();
            }
        }

        #endregion
    }
}
#endif