using UnityEngine;

public class PlayerRoleManager : MonoBehaviour
{
    private ProjectileThrower projectileThrower;
    private CoinSucker coinSucker;
    private SupportThrower supportThrower;

    // Added references to the SpriteRenderers for each role
    private SpriteRenderer projectileRenderer;
    private SpriteRenderer coinRenderer;
    private SpriteRenderer supportRenderer;

    // Store the original colors
    private Color originalProjectileColor;
    private Color originalCoinColor;
    private Color originalSupportColor;

    private enum PlayerRole { Thrower, Sucker, Support }
    private PlayerRole currentRole = PlayerRole.Thrower;

    void Awake()
    {
        // Get references to the components on child GameObjects
        projectileThrower = GetComponentInChildren<ProjectileThrower>();
        coinSucker = GetComponentInChildren<CoinSucker>();
        supportThrower = GetComponentInChildren<SupportThrower>();

        // Also get the SpriteRenderers
        projectileRenderer = projectileThrower.GetComponent<SpriteRenderer>();
        coinRenderer = coinSucker.GetComponent<SpriteRenderer>();
        supportRenderer = supportThrower.GetComponent<SpriteRenderer>();

        // Store the original colors
        originalProjectileColor = projectileRenderer.color;
        originalCoinColor = coinRenderer.color;
        originalSupportColor = supportRenderer.color;

        // Debugging to check if all components are found
        //Debug.Log($"ProjectileThrower found: {projectileThrower != null}");
        //Debug.Log($"CoinSucker found: {coinSucker != null}");
        //Debug.Log($"SupportThrower found: {supportThrower != null}");

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
        // Enable the current role and disable others, and update colors
        bool isThrowerActive = (currentRole == PlayerRole.Thrower);
        bool isSuckerActive = (currentRole == PlayerRole.Sucker);
        bool isSupportActive = (currentRole == PlayerRole.Support);

        if (projectileThrower != null) projectileThrower.isThrowerActive = isThrowerActive;
        if (coinSucker != null) coinSucker.isSuckerActive = isSuckerActive;
        if (supportThrower != null) supportThrower.isSupportActive = isSupportActive;

        // Update colors
        projectileRenderer.color = isThrowerActive ? originalProjectileColor : Color.grey;
        coinRenderer.color = isSuckerActive ? originalCoinColor : Color.grey;
        supportRenderer.color = isSupportActive ? originalSupportColor : Color.grey;
    }
}
