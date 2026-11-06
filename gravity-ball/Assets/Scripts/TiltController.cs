
using UnityEngine;
using TMPro;

public class TiltController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float sensitivity = 1f;
    [SerializeField] private float deadzone = 0.05f;
    
    [SerializeField] private TMP_Text accelerationText;
	[SerializeField] private TMP_Text speedText;
	[SerializeField] private TMP_Text directionText;
	[SerializeField] private TMP_Text stateText;

    private Camera mainCamera;
    private float objectRadius;

    private void Start()
    {
        mainCamera = Camera.main;

        // Radius des Kreises ermitteln
        objectRadius = GetComponent<SpriteRenderer>()
            .bounds.extents.x;
    }

    private void Update()
    {
		// Accelerometer-Daten auslesen
		Vector3 acceleration = Input.acceleration;

		// Bewegungsrichtung bestimmen
		Vector2 movement = new Vector2(
			acceleration.x,
			acceleration.y
		);

		// Kleine Sensorbewegungen ignorieren
		if (movement.magnitude < deadzone)
		{
			movement = Vector2.zero;
		}

		// Steuerungswerte begrenzen
		movement = Vector2.ClampMagnitude(movement, 1f);
		
		// Aktuelle Bewegungsgeschwindigkeit berechnen
		float currentSpeed = movement.magnitude * maxSpeed * sensitivity;

		// Bewegungsrichtung bestimmen
		string direction = "Stillstand";

		if (movement.magnitude > 0)
		{
			direction = "";

			if (movement.x > 0.1f) direction += "Rechts ";
			if (movement.x < -0.1f) direction += "Links ";
			if (movement.y > 0.1f) direction += "Oben ";
			if (movement.y < -0.1f) direction += "Unten ";
		}

		// Bewegungsstatus bestimmen
		string state = "Stillstand";

		if (currentSpeed > 0.1f && currentSpeed < 2f)
			state = "Langsam";
		else if (currentSpeed >= 2f && currentSpeed < 4f)
			state = "Mittel";
		else if (currentSpeed >= 4f)
			state = "Schnell";

		// UI aktualisieren
		accelerationText.text =
			$"Acceleration: X {acceleration.x:F2} | Y {acceleration.y:F2}";

		speedText.text = $"Speed: {currentSpeed:F2}";
		directionText.text = $"Direction: {direction}";
		stateText.text = $"State: {state}";

		// Bewegung anwenden
		transform.Translate(
			movement * sensitivity * maxSpeed * Time.deltaTime
		);
		
        // Bildschirmgrenzen berechnen
        Vector3 minBounds = mainCamera.ViewportToWorldPoint(
            new Vector3(0, 0, 0)
        );

        Vector3 maxBounds = mainCamera.ViewportToWorldPoint(
            new Vector3(1, 1, 0)
        );

        // Position begrenzen
        float x = Mathf.Clamp(
            transform.position.x,
            minBounds.x + objectRadius,
            maxBounds.x - objectRadius
        );

        float y = Mathf.Clamp(
            transform.position.y,
            minBounds.y + objectRadius,
            maxBounds.y - objectRadius
        );

        transform.position = new Vector3(
            x,
            y,
            transform.position.z
        );
    }
}

