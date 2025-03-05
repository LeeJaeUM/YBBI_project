using Unity.Netcode;
using UnityEngine;

public class PlayerData : NetworkBehaviour
{
    [SerializeField] float _playerDefaultEnergy = 100;
    [SerializeField] string _playerDefaultName = "DefaultName";

    public NetworkVariable<float> _playerEnergy;
    public NetworkVariable<string> _playerName;
    


    void Awake()
    {
        _playerEnergy = new NetworkVariable<float>(_playerDefaultEnergy);
        _playerName = new NetworkVariable<string>(_playerDefaultName);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
