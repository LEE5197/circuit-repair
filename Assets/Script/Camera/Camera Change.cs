using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class CameraChange : MonoBehaviour
{
    public Camera mainCam;
    private bool isSubCamActive = false;
	private LayerMask mainCam_original_layer;

	[SerializeField] private ScriptableRendererData rendererData;
	private ScriptableRendererFeature renderObject;

	private void Awake()
	{
		mainCam_original_layer = mainCam.cullingMask;


		if (rendererData == null) return;
		foreach(var features in rendererData.rendererFeatures)
		{
			if(features.name== "Sub Cam Render Object")
			{
				renderObject = features;
				break;
			}
		}
		renderObject.SetActive(false);
	}

	public void ChangeState()
	{
		isSubCamActive = !isSubCamActive;


		if (isSubCamActive)
		{
			mainCam.cullingMask = LayerMask.GetMask("Interactable", "Power Cable", "Player", "Default");
			renderObject.SetActive(true);
		}
		else
		{
			mainCam.cullingMask = mainCam_original_layer;
			renderObject.SetActive(false);
		}
	}
}
