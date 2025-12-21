using UnityEngine;

public class TimePortal : MonoBehaviour
{
	private TimeTravel timeTravel;
	private Collider portalCollider;
	private Tutorial tutorial;

	// Recorded particle system start values
	private ParticleSystem[] portalParticleSystems;
	private float particleStartLifetime;
	private float particleStartSpeed;
	private Color particleStartColor;

	private void Awake()
	{
		timeTravel = TimeTravel.GetInstance();
		portalCollider = GetComponent<Collider>();
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		tutorial = Tutorial.GetOrCreateInstance();
		// Record particle system start values. Use the first particle system found as the source of truth.
		portalParticleSystems = GetComponentsInChildren<ParticleSystem>();
		if (portalParticleSystems != null && portalParticleSystems.Length >0)
		{
			var main = portalParticleSystems[0].main;

			// startLifetime may be a constant or a range. If it's a range, record the average.
			switch (main.startLifetime.mode)
			{
				case ParticleSystemCurveMode.Constant:
					particleStartLifetime = main.startLifetime.constant;
					break;
				case ParticleSystemCurveMode.TwoConstants:
					particleStartLifetime = (main.startLifetime.constantMin + main.startLifetime.constantMax) *0.5f;
					break;
				default:
					particleStartLifetime = main.startLifetime.constant;
					break;
			}

			// startSpeed may also be a constant or a range. Record the average for ranges.
			switch (main.startSpeed.mode)
			{
				case ParticleSystemCurveMode.Constant:
					particleStartSpeed = main.startSpeed.constant;
					break;
				case ParticleSystemCurveMode.TwoConstants:
					particleStartSpeed = (main.startSpeed.constantMin + main.startSpeed.constantMax) *0.5f;
					break;
				default:
					particleStartSpeed = main.startSpeed.constant;
					break;
			}

			// startColor can be a single color, two colors or a gradient. Pick a representative color.
			var startCol = main.startColor;
			switch (startCol.mode)
			{
				case ParticleSystemGradientMode.Color:
					particleStartColor = startCol.color;
					break;
				case ParticleSystemGradientMode.TwoColors:
					particleStartColor = Color.Lerp(startCol.colorMin, startCol.colorMax,0.5f);
					break;
				case ParticleSystemGradientMode.Gradient:
					if (startCol.gradient != null && startCol.gradient.colorKeys.Length >0)
						particleStartColor = startCol.gradient.colorKeys[0].color;
					else
						particleStartColor = Color.white;
					break;
				default:
					particleStartColor = startCol.color;
					break;
			}
		}

		Enable();
	}

	private void OnTriggerEnter(Collider other)
	{
		// Use GameObject tags to detect the player. Check the collider's GameObject and its root in case the collider
		// belongs to a child object of the player.
		var go = other.gameObject;
		var root = other.transform.root.gameObject;

		if (go.CompareTag("Player") || root.CompareTag("Player"))
		{
			tutorial.OnTimePortalEntered();
			Disable(); // Disable the collider to prevent multiple triggers
			timeTravel.StartTimeTravelToBeginning();
		}
	}

	/// <summary>
	/// Enables the portal. The triggers will be checked and the portal graphic will change to an enabled state.
	/// </summary>
	internal void Enable()
	{
		portalCollider.enabled = true;

		// Restore particle system original values when enabling
		if (portalParticleSystems != null && portalParticleSystems.Length >0)
		{
			foreach (var ps in portalParticleSystems)
			{
				var main = ps.main;
				main.startSpeed = new ParticleSystem.MinMaxCurve(particleStartSpeed);
				main.startLifetime = new ParticleSystem.MinMaxCurve(particleStartLifetime);
				main.startColor = new ParticleSystem.MinMaxGradient(particleStartColor);
			}
		}
	}

	/// <summary>
	/// Disables the portal. The triggers will not be checked and the portal graphic will change to a disabled state.
	/// </summary>
	internal void Disable()
	{
		portalCollider.enabled = false;

		// When disabled, adjust particle systems to slower/fainter effect
		if (portalParticleSystems != null && portalParticleSystems.Length >0)
		{
			foreach (var ps in portalParticleSystems)
			{
				var main = ps.main;
				// Speed to one quarter
				main.startSpeed = new ParticleSystem.MinMaxCurve(particleStartSpeed *0.25f);
				// Increase lifetime by 50%
				main.startLifetime = new ParticleSystem.MinMaxCurve(particleStartLifetime *1.5f);
				// Color to black
				main.startColor = new ParticleSystem.MinMaxGradient(Color.black);
			}
		}
	}
}
