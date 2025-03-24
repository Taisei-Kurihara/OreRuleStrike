using UnityEngine;

public class Player_Attach : MonoBehaviour
{
    public PlayerSessionData _playerData=null;


    /// <summary>
    /// íœ‚³‚ê‚½
    /// </summary>
    private void OnDestroy()
    {
        if(_playerData != null)
        {
            _playerData.Death = true;
        }
    }
}
