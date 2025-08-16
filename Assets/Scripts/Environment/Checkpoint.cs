using System.Collections;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private float saveAfterRestartTimer = 10f;

    private Vector3 tempVector;
    private bool canSave = false;

    public Vector3 SavedPlayerPosition => tempVector;

    private void Start()
    {
        canSave = false;
        tempVector = TryLoadPlayerData();
        if (tempVector != Vector3.zero)
        {
            Debug.Log($"Saved position: x={tempVector.x}, y={tempVector.y}, z={tempVector.z}");
            Transform player = FindAnyObjectByType<CombatControllerPlayer>().gameObject.transform;
            if (player != null)
            {
                if ( player.gameObject.TryGetComponent<CharacterController>(out CharacterController cc))
                {
                    cc.enabled = false;
                    Debug.Log($"Player initial position: x={player.position.x}, y={player.position.y}, z={player.position.z}");

                    player.position = tempVector;

                    Debug.LogWarning("Checkpoint saved data found!");
                    Debug.Log($"Player new position: x={player.position.x}, y={player.position.y}, z={player.position.z}");
                    cc.enabled = true;
                }
                else
                    Debug.LogError("Checkpoint.Start: NO CHARACTER CONTROLLER ON PLAYER!");
            }
            else
                Debug.LogError("Checkpoint.Start: NO PLAYER ON THE SCENE!");
            
        }
        else
            Debug.LogWarning("Checkpoint.Start: No saved player position!");

        StartCoroutine(WaitUntilNextSaveCoroutine(saveAfterRestartTimer));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.LogWarning("Player entered the checkpoint trigger!");
            Debug.Log($"CP pos: {gameObject.transform.position}, player pos: {other.transform.position}");
            tempVector = other.transform.position;
            SavePlayerData(tempVector, other.gameObject);
        }
    }

    private void SavePlayerData(Vector3 playerPos, GameObject player)
    {
        if (canSave)
        {
            float playerPosX = playerPos.x;
            float playerPosY = playerPos.y;
            float playerPosZ = playerPos.z;

            PlayerPrefs.SetFloat("PlayerX", playerPosX);
            PlayerPrefs.SetFloat("PlayerY", playerPosY);
            PlayerPrefs.SetFloat("PlayerZ", playerPosZ);

            Debug.Log($"SavePlayerData: position saved! Values: X = {playerPosX}, Y = {playerPosY}, Z = {playerPosZ}");

            if (player.TryGetComponent<Health>(out Health playerHealth))
            {
                PlayerPrefs.SetFloat("Health", playerHealth.HP);
                Debug.Log($"SavePlayerData: hp saved! {playerHealth.HP}");
            }

            if (player.TryGetComponent<UseItem>(out UseItem useItemComponent))
            {
                PlayerPrefs.SetInt("Medkits", useItemComponent.GetMedkitsAmount());
                Debug.Log($"SavePlayerData: medkits saved! {useItemComponent.GetMedkitsAmount()}");
            }

            GameplayManager gm = FindAnyObjectByType<GameplayManager>();
            if (gm != null)
            {
                int rats = gm.GetRatKillsCounted();
                PlayerPrefs.SetInt("Rats", rats);

                int lights = gm.GetLightEnemiesKillsCounted();
                PlayerPrefs.SetInt("Light", lights);

                int heavies = gm.GetShieldEnemiesKillsCounted();
                PlayerPrefs.SetInt("Heavy", heavies);

                Debug.Log($"SavePlayerData: kills saved! Values: r={rats}, l={lights}, s={heavies}");
            }
            else
                Debug.LogWarning("Can't find GameplayManager to save kills!");

            GameTimer gt = FindAnyObjectByType<GameTimer>();
            if (gt != null)
            {
                PlayerPrefs.SetFloat("Timer", gt.TimeClamped);
                Debug.Log($"SavePlayerData: timer value saved! {gt.TimeClamped}");
            }
            else
                Debug.LogWarning("Can't find GameTimer to save timer!");

            StartCoroutine(WaitUntilNextSaveCoroutine(saveAfterRestartTimer));
        }
        else
            Debug.LogWarning("Checkpoint's save system is on cooldown!");
    }

    private IEnumerator WaitUntilNextSaveCoroutine(float timer)
    {
        canSave = false;
        yield return new WaitForSeconds(timer);
        canSave = true;
    }

    private Vector3 TryLoadPlayerData()
    {
        Vector3 temp = Vector3.zero;

        // Load world position
        if (PlayerPrefs.HasKey("PlayerX") && PlayerPrefs.HasKey("PlayerY") && PlayerPrefs.HasKey("PlayerZ"))
        {
            temp.x = PlayerPrefs.GetFloat("PlayerX");
            temp.y = PlayerPrefs.GetFloat("PlayerY");
            temp.z = PlayerPrefs.GetFloat("PlayerZ");
        }
        else
        {
            Debug.LogWarning("Checkpoint.TryLoadPlayerData: No saved player position!");
            temp = Vector3.zero;
            return temp;
        }

        GameObject player = FindAnyObjectByType<CombatControllerPlayer>().gameObject;
        if (player != null)
        {
            Debug.Log("Loading saved values...");
            StartCoroutine(GetPlayerComponentValuesCoroutine(player));
        }
        else
        {
            Debug.LogError("NO PLAYER FOUND FOR CHECKPOINT LOAD!");
            temp = Vector3.zero;
            return temp;
        }

        return temp;
    }

    private IEnumerator GetPlayerComponentValuesCoroutine(GameObject playerC)
    {
        yield return null;
        yield return new WaitForSeconds(0.75f); // просто ждём

        // Load health
        if (PlayerPrefs.HasKey("Health") && playerC.TryGetComponent<Health>(out Health playerHealth))
        {
            playerHealth.SetHPViaCheckpointData(PlayerPrefs.GetFloat("Health"));
            Debug.Log($"Saved health restored! Value: {PlayerPrefs.GetFloat("Health")}");
        }
        else
        {
            Debug.LogError("Checkpoint load data: can't find Health!");
        }

        // Load amount of Medkits
        if (PlayerPrefs.HasKey("Medkits") && playerC.TryGetComponent<UseItem>(out UseItem useItemComponent))
        {
            useItemComponent.SetMedkitsAmountViaCheckpointData(PlayerPrefs.GetInt("Medkits"));
            Debug.Log($"Saved medkits amount restored! Value: {PlayerPrefs.GetInt("Medkits")}");
        }
        else
        {
            Debug.LogError("Checkpoint load data: can't find Medkits!");
        }

        // Load kills
        GameplayManager gm = FindAnyObjectByType<GameplayManager>();
        if (gm != null)
        {
            if (PlayerPrefs.HasKey("Rats") && PlayerPrefs.HasKey("Light") && PlayerPrefs.HasKey("Heavy"))
            {
                int rats = PlayerPrefs.GetInt("Rats");
                gm.SetRatKillsCounted(rats);

                int lights = PlayerPrefs.GetInt("Light");
                gm.SetLightEnemiesKillsCounted(lights);

                int heavies = PlayerPrefs.GetInt("Heavy");
                gm.SetShieldEnemiesKillsCounted(heavies);

                Debug.Log($"Saved kills restored! Values: r={rats}, l={lights}, s={heavies}");
            }
            else
            {
                Debug.LogError("Checkpoint load data: can't find all kills!");
            }

        }
        else
        {
            Debug.LogError("Checkpoint load data: can't find GameplayManager!");
        }

        //Load timer value
        GameTimer gt = FindAnyObjectByType<GameTimer>();
        if (gt != null)
        {
            gt.SetTimerFromCheckpointData(PlayerPrefs.GetFloat("Timer"));
            Debug.Log($"Saved Timer value restored! Value: {PlayerPrefs.GetFloat("Timer")}");
        }
        else
        {
            Debug.LogError("Checkpoint load data: can't find Timer!");
        }
        Debug.Log("Loading checkpoint data complete!");
        yield return null;
    }
}
