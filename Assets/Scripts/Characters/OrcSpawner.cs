using System;
using UC;
using UC.Interaction;
using UnityEngine;
using UnityEngine.AI;

// Sends the orcs in once started (the Orcs/Start Spawning action), wave after wave, each with its own numbers: count
// orcs, one every interval (the first firstDelay after starting), never more than maxAlive at once, walking at speed
// times the prefab's; each at one of the spawn points (not the one nearest the actor when there's a choice). When a
// wave is all dead the next one comes wavePause later, after onWaveCleared (the director's "More of them!"); when the
// last is, it runs onCleared (the director's next line, the next target). The actions run with the actor as the
// context's trigger object. It waits while gameplay is on hold. Sounds: waveStartSound as a wave's first orc comes,
// waveClearedSound when a wave is all dead.
public class OrcSpawner : ActionRunner
{
    [Serializable]
    public class Wave
    {
        [Min(1)]
        public int      count = 5;
        [Min(1), Tooltip("Never more than this many alive at once")]
        public int      maxAlive = 4;
        [Min(0.1f), Tooltip("Seconds between orcs")]
        public float    interval = 2.8f;
        [Min(0.1f), Tooltip("Times the prefab's walking speed")]
        public float    speed = 1.0f;
    }

    [SerializeField]
    private Orc             orcPrefab;
    [SerializeField]
    private Transform[]     spawnPoints;
    [SerializeField]
    private Wave[]          waves = { new Wave(), new Wave() };
    [SerializeField, Min(0.0f)]
    private float           firstDelay = 1.0f;
    [SerializeField, Min(0.0f), Tooltip("Seconds between a wave dying and the next one coming")]
    private float           wavePause = 3.0f;
    [SerializeField]
    private SoundDef        waveStartSound;
    [SerializeField]
    private SoundDef        waveClearedSound;
    [SerializeReference]
    private GameAction[]    onWaveCleared;
    [SerializeReference]
    private GameAction[]    onCleared;

    public int wave { get; private set; }
    public int waveCount => (waves != null) ? (waves.Length) : (0);
    public int spawned { get; private set; }
    public int alive => CountAlive();
    public bool running { get; private set; }
    public bool cleared { get; private set; }

    int     spawnedThisWave;
    float   timer;

    public void Begin()
    {
        if ((running) || (cleared) || (waveCount == 0)) return;
        running = true;
        timer = firstDelay;
    }

    void Update()
    {
        if ((!running) || (Gameplay.isHeld)) return;

        var current = waves[wave];
        int living = CountAlive();
        if ((spawnedThisWave >= current.count) && (living == 0))
        {
            if (waveClearedSound != null) waveClearedSound.Play();
            if (wave + 1 < waveCount)
            {
                wave++;
                spawnedThisWave = 0;
                timer = wavePause;
                Run(onWaveCleared);
            }
            else
            {
                running = false;
                cleared = true;
                Run(onCleared);
            }
            return;
        }

        timer -= Time.deltaTime;
        if ((timer > 0.0f) || (spawnedThisWave >= current.count) || (living >= current.maxAlive)) return;

        var point = PickSpawnPoint();
        if (point == null) return;
        if ((spawnedThisWave == 0) && (waveStartSound != null)) waveStartSound.Play();
        var orc = Instantiate(orcPrefab, point.position, point.rotation);
        var agent = orc.GetComponent<NavMeshAgent>();
        if (agent != null) agent.speed *= current.speed;
        spawned++;
        spawnedThisWave++;
        timer = current.interval;
    }

    void Run(GameAction[] actions)
    {
        if ((actions == null) || (actions.Length == 0)) return;

        var actor = FindAnyObjectByType<Player>();
        var context = new ActionContext
        {
            triggerGameObject = (actor != null) ? (actor.gameObject) : (null),
            targetGameObject = gameObject,
            runner = this
        };
        GameAction.RunActions(actions, context);
    }

    int CountAlive()
    {
        int n = 0;
        foreach (var orc in Orc.all) if (!orc.isDead) n++;
        return n;
    }

    Transform PickSpawnPoint()
    {
        if ((spawnPoints == null) || (spawnPoints.Length == 0)) return null;
        if (spawnPoints.Length == 1) return spawnPoints[0];

        var actor = FindAnyObjectByType<Player>();
        int nearest = -1;
        if (actor != null)
        {
            float best = float.MaxValue;
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                float d = Vector3.Distance(spawnPoints[i].position, actor.transform.position);
                if (d < best) { best = d; nearest = i; }
            }
        }
        int pick;
        do { pick = UnityEngine.Random.Range(0, spawnPoints.Length); } while (pick == nearest);
        return spawnPoints[pick];
    }
}
