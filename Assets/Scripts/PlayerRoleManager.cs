using UnityEngine;

public class PlayerRoleManager : MonoBehaviour
{
    private ProjectileThrower projectileThrower;
    private CoinSucker coinSucker;
    private SupportThrower supportThrower;

    private enum PlayerRole { Thrower, Sucker, Support }
    private PlayerRole currentRole = PlayerRole.Thrower;

    void Awake()
    {
        // Get references to the components on child GameObjects
        projectileThrower = GetComponentInChildren<ProjectileThrower>();
        coinSucker = GetComponentInChildren<CoinSucker>();
        supportThrower = GetComponentInChildren<SupportThrower>();

        // Debugging to check if all components are found
        Debug.Log($"ProjectileThrower found: {projectileThrower != null}");
        Debug.Log($"CoinSucker found: {coinSucker != null}");
        Debug.Log($"SupportThrower found: {supportThrower != null}");

        UpdateRole();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CycleRole();
            UpdateRole();
        }
    }

    void CycleRole()
    {
        // Cycle through the roles
        currentRole = currentRole switch
        {
            PlayerRole.Thrower => PlayerRole.Sucker,
            PlayerRole.Sucker => PlayerRole.Support,
            PlayerRole.Support => PlayerRole.Thrower,
            _ => currentRole
        };
    }

    void UpdateRole()
    {
        // Enable the current role and disable others
        if (projectileThrower != null) projectileThrower.isThrowerActive = (currentRole == PlayerRole.Thrower);
        if (coinSucker != null) coinSucker.isSuckerActive = (currentRole == PlayerRole.Sucker);
        if (supportThrower != null) supportThrower.isSupportActive = (currentRole == PlayerRole.Support);
    }
}
