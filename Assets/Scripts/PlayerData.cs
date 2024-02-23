using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    // In order to <T>ype inside of a NetworkList this need to implement IEquatable
    // To prevent ArgumentException: Type PlayerData is not supported by NetworkVariable, So implement INetworkSerializable

    public ulong clientId;
    public int colorId;

    public bool Equals(PlayerData other)
    {
        return clientId == other.clientId && colorId == other.colorId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        // Here serializing the variables
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
    }
}
