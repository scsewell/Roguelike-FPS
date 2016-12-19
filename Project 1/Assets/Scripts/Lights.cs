using UnityEngine;
using System.Collections;

public enum LightMode
{
	Static = 0,
	Sine = 1,		
	RandomFluctuation = 2,
	Flicker = 3,
}

public class Lights : MonoBehaviour
{
	
	public LightMode lightMode = LightMode.Static;
	public float lightDeactivateDistance = 40;
	//public float shadowDeactivateDistance = 35;
	public float maxIntensity = 1;
	public float minIntensity = 1;
	public float intensityChangeRate = 3;
	public int flickerChance = 100;
	public float flickerMinDuration = 5;
	public float flickerMaxDuration = 10;
	
	//private Settings settings;
	private Transform player;
	private Material lightMeshMaterial;
	private Light lightComponent;

	private float targetIntensity;
	private bool isFlickering;
	private int flickeringDuration;
	
	void Start () 
    {
		player = GameObject.FindGameObjectWithTag("Player").transform;

		lightComponent = GetComponent<Light>();
		lightComponent.intensity = maxIntensity;

		if (lightMode == LightMode.Flicker) {
			targetIntensity = Random.Range(minIntensity, maxIntensity);
		}

		lightMeshMaterial = transform.parent.parent.GetComponent<MeshRenderer>().material;
	}

	void Update () 
    {
		if (Vector3.Distance(player.position, GetComponent<Light>().transform.position) > lightDeactivateDistance) {
			lightComponent.enabled = false;
		} else {
			lightComponent.enabled = true;
		}

		if (lightMode == LightMode.Sine) {
			
			lightComponent.intensity = ((maxIntensity - minIntensity) / 2) * Mathf.Sin(Time.time * intensityChangeRate) + ((maxIntensity - minIntensity) / 2) + minIntensity;

		} else if (lightMode == LightMode.RandomFluctuation) {

			if (Mathf.Abs(lightComponent.intensity - targetIntensity) < .1f) {
				targetIntensity = Random.Range(minIntensity, maxIntensity);
			}
			
			lightComponent.intensity = Mathf.Lerp(lightComponent.intensity, targetIntensity, Time.deltaTime * intensityChangeRate);

		} else if (lightMode == LightMode.Flicker) {

			if (!isFlickering && Random.Range(0, flickerChance) < 2) {
				isFlickering = true;
				flickeringDuration = 0;
			} else if (isFlickering) {
				flickeringDuration++;
				lightComponent.intensity = minIntensity;
			}

			if (isFlickering && (flickeringDuration > flickerMinDuration && (int)Random.Range(flickerMinDuration, flickerMaxDuration) == flickeringDuration || flickeringDuration > flickerMaxDuration)) {
				isFlickering = false;
				lightComponent.intensity = maxIntensity;
			}

		}
		lightMeshMaterial.SetColor("_EmissionColor", lightComponent.color * (lightComponent.intensity / maxIntensity));
	}
}
