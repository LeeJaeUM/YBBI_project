using Unity.Netcode;
using UnityEngine;
using static MapRandomSpawner;

public class MapRpc : MonoBehaviour
{
    [SerializeField] MapRandomSpawner mapRandomSpawner;


    [ClientRpc]
    public void FinishMapSetupClientRpc(bool isShopScene)
    {
        if(!isShopScene)
        {
            mapRandomSpawner.RebuildMapArr();
            mapRandomSpawner.RefreshInspectorForTPID();
            mapRandomSpawner.DisableUnusedTeleporters();
        }
        mapRandomSpawner.NotifyPlayerWallUpdate();
        mapRandomSpawner.RequestSetPlayerPos();

    }


}
