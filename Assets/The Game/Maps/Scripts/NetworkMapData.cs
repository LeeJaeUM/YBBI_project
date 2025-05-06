using Unity.Netcode;
using Unity.Collections;
using UnityEngine;

public struct NetWorkMapData : INetworkSerializable
{
    public Enums.RoomType roomType;

    public bool canConnectUp;
    public bool canConnectDown;
    public bool canConnectLeft;
    public bool canConnectRight;

    public FixedString32Bytes tpUpID;
    public FixedString32Bytes tpDownID;
    public FixedString32Bytes tpLeftID;
    public FixedString32Bytes tpRightID;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref roomType);
        serializer.SerializeValue(ref canConnectUp);
        serializer.SerializeValue(ref canConnectDown);
        serializer.SerializeValue(ref canConnectLeft);
        serializer.SerializeValue(ref canConnectRight);

        serializer.SerializeValue(ref tpUpID);
        serializer.SerializeValue(ref tpDownID);
        serializer.SerializeValue(ref tpLeftID);
        serializer.SerializeValue(ref tpRightID);
    }
}