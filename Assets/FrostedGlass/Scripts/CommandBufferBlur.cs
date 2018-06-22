using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CommandBufferBlur : MonoBehaviour
{
    [SerializeField]
    Shader _Shader;

    Material _Material = null;

    Camera _Camera = null;
    CommandBuffer _CommandBuffer = null;

    Vector2 _ScreenResolution = Vector2.zero;
    float _OldFov = 0;
    float _OldStepDist = 0;

    public float blurStepDistance = 0.1f;

    public void Cleanup()
    {
        if (!Initialized)
            return;

        _Camera.RemoveCommandBuffer(CameraEvent.AfterSkybox, _CommandBuffer);
        _CommandBuffer = null;
        Object.DestroyImmediate(_Material);
    }

    public void OnEnable()
    {
        Cleanup();
        Initialize();
    }

    public void OnDisable()
    {
        Cleanup();
    }

    public bool Initialized
    {
        get { return _CommandBuffer != null; }
    }

    void Initialize()
    {
        if (Initialized)
            return;

        if (!_Material)
        {
            _Material = new Material(_Shader);
            _Material.hideFlags = HideFlags.HideAndDontSave;
        }

        _Camera = GetComponent<Camera>();

        _CommandBuffer = new CommandBuffer();
        _CommandBuffer.name = "Blur screen";

        int numIterations = 4;

        float stepSize = blurStepDistance / _Camera.fieldOfView;
        Vector2 offsets = new Vector2(stepSize * Screen.height / Screen.width, stepSize);

        

        for (int i = 0; i < numIterations; ++i)
        {
            int screenCopyID = Shader.PropertyToID("_ScreenCopyTexture");
            _CommandBuffer.GetTemporaryRT(screenCopyID, -1, -1, 0, FilterMode.Bilinear);
            _CommandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, screenCopyID);

            int curSize = (int)Mathf.Pow(2,i);
            int curW = Screen.width/curSize;
            int curH = Screen.height/curSize;

            int blurredID = Shader.PropertyToID("_Grab" + i + "_Temp1");
            int blurredID2 = Shader.PropertyToID("_Grab" + i + "_Temp2");
            _CommandBuffer.GetTemporaryRT(blurredID, curW, curH, 0, FilterMode.Bilinear);
            _CommandBuffer.GetTemporaryRT(blurredID2, curW, curH, 0, FilterMode.Bilinear);

            _CommandBuffer.Blit(screenCopyID, blurredID);
            _CommandBuffer.ReleaseTemporaryRT(screenCopyID);

            _CommandBuffer.SetGlobalVector("offsets", new Vector4(offsets.x*curSize, 0, 0, 0));
            _CommandBuffer.Blit(blurredID, blurredID2, _Material);
            _CommandBuffer.SetGlobalVector("offsets", new Vector4(0, offsets.y*curSize, 0, 0));
            _CommandBuffer.Blit(blurredID2, blurredID, _Material);

            _CommandBuffer.SetGlobalTexture("_GrabBlurTexture_" + i, blurredID);
        }

        _Camera.AddCommandBuffer(CameraEvent.AfterSkybox, _CommandBuffer);

        _ScreenResolution = new Vector2(Screen.width, Screen.height);
        _OldFov = _Camera.fieldOfView;
		_OldStepDist = blurStepDistance;
    }

    void OnPreRender()
    {
        if ((_ScreenResolution != new Vector2(Screen.width, Screen.height)) || (_Camera.fieldOfView != _OldFov) || (_OldStepDist != blurStepDistance))
            Cleanup();

        Initialize();
    }
}
