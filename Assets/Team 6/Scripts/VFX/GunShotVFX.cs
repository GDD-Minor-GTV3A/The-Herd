using UnityEngine;
using System.Collections;

public class SimpleGunshotVFX3D : MonoBehaviour
{
    [Header("Light Settings")]
    public AudioClip gunshotSound;
    public float flashIntensity = 80f;
    public float flashDuration = 0.3f;
    public float offsetDistance = 0.05f;
    
    [Header("Particle Settings")]
    public float particleSphereSize = 0.1f;
    public float smokeSpreadRadius = 0.1f;

    private ParticleSystem muzzleFlash;
    private ParticleSystem smoke;
    private Light flashLight;
    private AudioSource audioSrc;
    private Mesh sphereMesh;

    void Start()
    {
        CreateSharedSphereMesh();

        GameObject vfxOrigin = new GameObject("MuzzleOrigin");
        vfxOrigin.transform.parent = transform;
        vfxOrigin.transform.localPosition = new Vector3(0, 0, offsetDistance);
        vfxOrigin.transform.localRotation = Quaternion.identity;

        muzzleFlash = Create3DParticleSystem("MuzzleFlash", vfxOrigin.transform, Color.yellow, 0.3f, 0.1f, true);
        smoke = Create3DParticleSystem("Smoke", vfxOrigin.transform, new Color(0.3f, 0.3f, 0.3f, 0.5f), 1f, 1.2f, false);

        GameObject lightObj = new GameObject("MuzzleLight");
        lightObj.transform.parent = vfxOrigin.transform;
        lightObj.transform.localPosition = Vector3.zero;
        flashLight = lightObj.AddComponent<Light>();
        flashLight.type = LightType.Point;
        flashLight.color = new Color(1f, 0.8f, 0.6f);
        flashLight.range = 6f;
        flashLight.intensity = 0f;
        flashLight.enabled = false;

        audioSrc = gameObject.AddComponent<AudioSource>();
        audioSrc.playOnAwake = false;
        audioSrc.spatialBlend = 0f;
        if (gunshotSound) audioSrc.clip = gunshotSound;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlayVFX();
        }
    }



    private void CreateSharedSphereMesh()
    {
        GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphereMesh = tempSphere.GetComponent<MeshFilter>().sharedMesh;
        Destroy(tempSphere);
    }

    private ParticleSystem Create3DParticleSystem(string name, Transform parent, Color color, float size, float lifetime, bool isFlash)
    {
        GameObject go = new GameObject(name);
        go.transform.parent = parent;
        go.transform.localPosition = Vector3.zero;

        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime = lifetime;
        main.startSize = particleSphereSize;
        main.startColor = color;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.loop = false;
        main.duration = lifetime;
        main.playOnAwake = false;

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = isFlash ? ParticleSystemShapeType.Cone : ParticleSystemShapeType.Sphere;
        shape.angle = isFlash ? 25f : 0f;
        shape.radius = isFlash ? 0.05f : smokeSpreadRadius;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, isFlash ? 20 : 30) });

        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.Local;
        velocity.speedModifier = isFlash ? 5f : 0.3f;

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Mesh;
        renderer.mesh = sphereMesh; 

        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
            shader = Shader.Find("Unlit/Color");

        Material mat = new Material(shader);
        mat.SetColor("_BaseColor", color);
        mat.renderQueue = 3000;
        mat.enableInstancing = true;

        renderer.material = mat;

        return ps;
    }

    public void PlayVFX()
    {
        if (muzzleFlash) muzzleFlash.Play();
        if (smoke) smoke.Play();
        if (audioSrc && audioSrc.clip) audioSrc.PlayOneShot(audioSrc.clip);
        StartCoroutine(FlashLightCoroutine());
    }

    IEnumerator FlashLightCoroutine()
    {
        flashLight.enabled = true;
        float timer = 0f;
        while (timer < flashDuration)
        {
            timer += Time.deltaTime;
            float t = timer / flashDuration;
            flashLight.intensity = Mathf.Lerp(flashIntensity, 0f, t);
            yield return null;
        }
        flashLight.enabled = false;
    }

    void OnDestroy()
    {
        if (sphereMesh != null)
        {
            Destroy(sphereMesh);
        }
    }
}