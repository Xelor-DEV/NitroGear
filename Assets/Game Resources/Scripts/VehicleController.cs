using UnityEngine;
using System.Collections;
public class VehicleController : MonoBehaviour
{
    // Parámetros de velocidad y rotación
    public float acceleration = 10f;
    public float maxSpeed = 50f;
    public float rotationSpeed = 100f;
    public float driftRotationSpeed = 150f;
    public float driftFactor = 0.95f;
    public float normalTurnFactor = 0.8f; // Factor de giro normal
    public float driftTurnFactor = 1.5f;  // Factor de giro durante drift
    public float deceleration = 5f;

    // Turbo settings
    public float turboMultiplier = 1.5f;
    public float turboDuration = 3f;
    private int turboCount = 3;
    private bool isTurboActive = false;

    // Variables internas
    private float currentSpeed = 0f;
    private float currentRotation = 0f;
    private bool isDrifting = false;

    private void Update()
    {
        // Movimiento del vehículo
        HandleMovement();

        // Turbo
        if (Input.GetKeyDown(KeyCode.Space) && turboCount > 0 && !isTurboActive)
        {
            StartCoroutine(ActivateTurbo());
        }

        // Drift
        if (Input.GetKey(KeyCode.LeftShift))
        {
            StartDrift();
        }
        else
        {
            EndDrift();
        }
    }

    private void HandleMovement()
    {
        // Aceleración y velocidad
        float inputVertical = Input.GetAxis("Vertical");
        currentSpeed += inputVertical * acceleration * Time.deltaTime;

        // Limitar la velocidad
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        // Rotación
        float inputHorizontal = Input.GetAxis("Horizontal");
        if (isDrifting)
        {
            currentRotation = inputHorizontal * driftRotationSpeed * Time.deltaTime;
        }
        else
        {
            currentRotation = inputHorizontal * rotationSpeed * Time.deltaTime;
        }

        // Aplicar la rotación del vehículo
        transform.Rotate(0f, currentRotation, 0f);

        // Aplicar la velocidad al vehículo con un leve deslizamiento si está derrapando
        Vector3 forwardMove = transform.forward * currentSpeed * Time.deltaTime;
        if (isDrifting)
        {
            // Deslizamiento lateral
            forwardMove += transform.right * inputHorizontal * driftTurnFactor * Time.deltaTime;
        }

        // Movimiento final del vehículo
        transform.position += forwardMove;

        // Frenado progresivo
        if (inputVertical == 0)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, deceleration * Time.deltaTime);
        }
    }

    private void StartDrift()
    {
        isDrifting = true;
    }

    private void EndDrift()
    {
        isDrifting = false;
    }

    private IEnumerator ActivateTurbo()
    {
        isTurboActive = true;
        turboCount--;
        float originalMaxSpeed = maxSpeed;
        maxSpeed *= turboMultiplier;

        yield return new WaitForSeconds(turboDuration);

        maxSpeed = originalMaxSpeed;
        isTurboActive = false;
    }

    public void ResetTurboCount()
    {
        turboCount = 3; // Restablecer los turbos al comenzar una nueva ronda
    }
}
