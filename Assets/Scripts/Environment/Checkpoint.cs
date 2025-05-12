using System.Runtime.CompilerServices;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Vector3 tempVector;

    public Vector3 SavedPlayerPosition => tempVector;

    private void Start()
    {
        tempVector = TryLoadPlayerData();
        if (tempVector != Vector3.zero)
        {
            Transform player = FindAnyObjectByType<CombatControllerPlayer>().gameObject.transform;
            player.position = tempVector;
            Debug.LogWarning("Checkpoint saved data found!");
        }
        else
            Debug.LogWarning("Checkpoint.Start: No saved player position!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tempVector = other.transform.position;
            SavePlayerData(tempVector, other.gameObject);
        }
    }

    private void SavePlayerData(Vector3 playerPos, GameObject player)
    {
        float playerPosX = playerPos.x;
        float playerPosY = playerPos.y;
        float playerPosZ = playerPos.z;

        PlayerPrefs.SetFloat("PlayerX", playerPosX);
        PlayerPrefs.SetFloat("PlayerY", playerPosY);
        PlayerPrefs.SetFloat("PlayerZ", playerPosZ);

        if (player.TryGetComponent<Health>(out Health playerHealth))
        {
            PlayerPrefs.SetFloat("Health", playerHealth.HP);
        }

        if (player.TryGetComponent<UseItem>(out UseItem useItemComponent))
        {
            PlayerPrefs.SetInt("Medkits", useItemComponent.GetMedkitsAmount());
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
        }

        GameTimer gt = FindAnyObjectByType<GameTimer>();
        if (gt != null)
        {
            PlayerPrefs.SetFloat("Timer", gt.TimeClamped);
        }
    }

    private Vector3 TryLoadPlayerData()
    {
        Vector3 temp = new();

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
            // Load health
            if (PlayerPrefs.HasKey("Health") && player.TryGetComponent<Health>(out Health playerHealth))
            {
                playerHealth.SetHPViaCheckpointData(PlayerPrefs.GetFloat("Health"));
            }
            else
            {
                Debug.LogError("Checkpoint load data: can't find Health!");
                return Vector3.zero;
            }

            // Load amount of Medkits
            if (PlayerPrefs.HasKey("Medkits") && player.TryGetComponent<UseItem>(out UseItem useItemComponent))
            {
                useItemComponent.SetMedkitsAmountViaCheckpointData(PlayerPrefs.GetInt("Medkits"));
            }
            else
            {
                Debug.LogError("Checkpoint load data: can't find Medkits!");
                return Vector3.zero;
            }

            // Load kills
            GameplayManager gm = FindAnyObjectByType<GameplayManager>();
            if (gm != null)
            {
                if (PlayerPrefs.HasKey("Rats") && PlayerPrefs.HasKey("Rats") && PlayerPrefs.HasKey("Rats"))
                {
                    int rats = PlayerPrefs.GetInt("Rats");
                    gm.SetRatKillsCounted(rats);

                    int lights = PlayerPrefs.GetInt("Light");
                    gm.SetLightEnemiesKillsCounted(lights);

                    int heavies = PlayerPrefs.GetInt("Heavy");
                    gm.SetShieldEnemiesKillsCounted(heavies);

                }
                else
                {
                    Debug.LogError("Checkpoint load data: can't find Kills!");
                }
                
            }
            else
            {
                Debug.LogError("Checkpoint load data: can't find GameplayManager!");
                return Vector3.zero;
            }

            //Load timer value
            GameTimer gt = FindAnyObjectByType<GameTimer>();
            if (gt != null)
            {
                gt.SetTimerFromCheckpointData(PlayerPrefs.GetFloat("Timer"));
            }
            else
            {
                Debug.LogError("Checkpoint load data: can't find Timer!");
                return Vector3.zero;
            }
        }
        else
        {
            Debug.LogError("NO PLAYER FOUND FOR CHECKPOINT LOAD!");
        }

        return temp;
    }
}
