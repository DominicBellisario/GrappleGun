using UnityEngine;

public class FlyingEnemySoundLogic : SoundLogic
{
    [SerializeField] AudioClip explodeClip;

    GVar gvar;

    protected override void Start()
    {
        base.Start();
        gvar = GVar.Instance;
    }

    public void PlayPrimeClip()
    {
        audioSource.Play();
    }

    public void PlayExplodeClip()
    {
        // play the explode sound at a random pitch amd half volume
        GameObject newSource = Instantiate(gvar.AudioSourcePrefab, transform.position, Quaternion.identity);
        newSource.GetComponent<AudioSourceLogic>().Constructor(explodeClip, Random.Range(0.9f, 1.1f));
    }
}
