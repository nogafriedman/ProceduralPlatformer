using System.Collections.Generic;
using UnityEngine;

public class ParticlePool : MonoBehaviour
{
    public static ParticlePool Instance { get; private set; }

    [Header("Prefab & Pool")]
    [SerializeField] private ParticleSystem particlePrefab;
    [SerializeField, Min(0)] private int initialSize = 8;

    private readonly Queue<ParticleSystem> pool = new Queue<ParticleSystem>();

    void Awake()
    {
        // Debug.Log($"[ParticlePool] Awake | prefab={(particlePrefab ? particlePrefab.name : "null")}, initialSize={initialSize}");

        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < initialSize; i++)
        {
            var ps = CreateInstance();
            ps.gameObject.SetActive(false);
            pool.Enqueue(ps);
        }
    }

    ParticleSystem CreateInstance()
    {
        var ps = Instantiate(particlePrefab, transform);
        ConfigureCommon(ps);

        var ret = ps.GetComponent<_ReturnToBurstPool>();
        if (!ret) ret = ps.gameObject.AddComponent<_ReturnToBurstPool>();
        ret.Init(this);
        return ps;
    }

    void ConfigureCommon(ParticleSystem ps)
    {
        var main = ps.main;
        main.loop = false;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    ParticleSystem Get()
    {
        var ps = pool.Count > 0 ? pool.Dequeue() : CreateInstance();
        ConfigureCommon(ps);
        ps.transform.SetParent(transform, true);
        ps.gameObject.SetActive(true);
        return ps;
    }

    public void Release(ParticleSystem ps)
    {
        if (!ps) return;
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.transform.SetParent(transform, false);
        ps.transform.localPosition = Vector3.zero;
        ps.transform.localRotation = Quaternion.identity;
        ps.transform.localScale = Vector3.one;
        ps.gameObject.SetActive(false);
        pool.Enqueue(ps);
    }

    // ---- SPAWN APIS ----

    public ParticleSystem PlayBurstWorld(Vector3 position, Quaternion rotation)
    {
        var ps = Get();
        // Debug.Log($"[ParticlePool] PlayBurstWorld | pos={position}, prefab={particlePrefab?.name}, active={ps.gameObject.activeSelf}");
        var main = ps.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        ps.transform.SetParent(transform, true);
        ps.transform.SetPositionAndRotation(position, rotation);

        ps.Clear(true);
        ps.Play(true);
        return ps;
    }

    public ParticleSystem PlayBurstAttached(Transform anchor, Vector3 localOffset = default)
    {
        if (!anchor) return null;

        var ps = Get();
        var main = ps.main;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.scalingMode     = ParticleSystemScalingMode.Local;

        ps.transform.SetParent(anchor, false);
        ps.transform.localPosition = localOffset;
        ps.transform.localRotation = Quaternion.identity;
        ps.transform.localScale    = Vector3.one;

        ps.Clear(true);
        ps.Play(true);
        return ps;
    }

}

sealed class _ReturnToBurstPool : MonoBehaviour
{
    ParticlePool pool;
    ParticleSystem ps;

    public void Init(ParticlePool p)
    {
        pool = p;
        ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    void OnParticleSystemStopped()
    {
        pool.Release(ps);
    }
}
