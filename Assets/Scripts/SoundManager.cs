using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Source")]
    public AudioSource audioSource;
    public AudioSource musicSource;

    [Header("Game Sounds")]
    public AudioClip pickupSound;
    public AudioClip placeSound;
    public AudioClip cookSound;
    public AudioClip serveSound;
    public AudioClip[] dialogueSound;
    public AudioClip BlenderSound;
    public AudioClip deathSound;
    public AudioClip MainMenuSound;
    public AudioClip idleSound;
    //public AudioClip walking;
    [Header("Nick Reviews")]
    public AudioClip[] nickReviews;
    int currentNickIndex = 0;
    int currentDialogueIndex = 0;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayIdleSound(float volume) {
        musicSource.clip = idleSound;
        musicSource.volume = volume;
        musicSource.loop = true;
        musicSource.Play();
    }
    public void ResetDialogue()
    {
        currentDialogueIndex = 0;
        currentNickIndex = 0;
    }
    public void PlayNickReview()
    {
        if (nickReviews.Length == 0) return;

        audioSource.PlayOneShot(nickReviews[currentNickIndex]);

        currentNickIndex++;

        if (currentNickIndex >= nickReviews.Length)
            currentNickIndex = 0; // loops back to first clip
    }
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void PlayPickup()
    {
        PlaySound(pickupSound);
    }

    public void PlayMainMenu()
    {
        PlaySound(MainMenuSound);
       
    }   

    public void PlayPlace()
    {
        PlaySound(placeSound);
    }

    public void PlayCook()
    {
        PlaySound(cookSound);
    }

    public void PlayServe()
    {
        PlaySound(serveSound);
    }

    public void PlayDialogue()
    {
        if (dialogueSound.Length == 0) return;

        audioSource.PlayOneShot(dialogueSound[currentDialogueIndex]);

        currentDialogueIndex++;

        if (currentDialogueIndex >= dialogueSound.Length)
            currentDialogueIndex = 0; // loops back to first clip
    }

    public void PlayDeath()
    {
        PlaySound(deathSound);
    }
    public void PlayBlender()
    {
        PlaySound(BlenderSound);
    }
    public void PlayNickReview1()
    {
        PlaySound(BlenderSound);
    }
}