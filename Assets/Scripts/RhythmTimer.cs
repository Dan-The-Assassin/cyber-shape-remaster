using UnityEngine;

public class RhythmTimer : MonoBehaviour
{
    [field: SerializeField] public float Offset { get; private set; } = 0.045f;
    [SerializeField] private float bpm = 135.0f; // default value, change in inspector for each track

    public float Interval { get; private set; }
    public float DspTimeSong { get; private set; }

    public float ClosenessToBeat
    {
        get
        {
            var beatProgress = _songPositionInBeats - Mathf.Floor(_songPositionInBeats);
            return 1.0f - Mathf.Sin(beatProgress);
        }
    }
    private float ClipLength => _audioSource.clip.length;
   
    private AudioSource _audioSource;
    private float _songPosition;
    private float _songPositionInBeats;

    private void Awake()
    {
        Interval = 60.0f / bpm;
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        DspTimeSong = (float)AudioSettings.dspTime;
        _audioSource.Play();
    }

    private void Update()
    {
        _songPosition = (float)(AudioSettings.dspTime - DspTimeSong) - Offset;
        if (_songPosition >= ClipLength)
        {
            DspTimeSong += ClipLength;
            _songPosition -= ClipLength;
        }

        _songPositionInBeats = _songPosition / Interval;
    }

    public bool CheckTime(float leeway) // checks if time since last beat is in the decided time interval for the next beat
    {
        var beatProgress = _songPositionInBeats - Mathf.Floor(_songPositionInBeats);
        return beatProgress < leeway / 2 || beatProgress > 1 - leeway / 2;
    }
}
