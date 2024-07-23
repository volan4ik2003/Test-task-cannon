using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CannonController : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public Transform Cannon;
    private float currentRotationAngle = 0f;
    private float currentPitchAngle = 0f;
    public float maxRotationAngle = 50f;
    public float maxPitchAngle = 30f;

    public float recoilDistance = 0.5f;
    public float recoilDuration = 0.05f;

    private Vector3 originalLocalPosition;
    private bool isRecoiling = false;

    public Slider powerSlider;
    public TextMeshProUGUI sliderValue;
    public Transform firePoint;

    public GameObject projectilePrefab;

    public CameraShaker shaker;

    private float power;

    private void Start()
    {
        originalLocalPosition = Cannon.localPosition;
    }

    private void Update()
    {
        power = powerSlider.value;

        sliderValue.text = Mathf.Round(powerSlider.value).ToString();

        if (Input.GetKey(KeyCode.D))
        {
            float rotationStep = rotationSpeed * Time.deltaTime;

            if (currentRotationAngle + rotationStep > maxRotationAngle)
            {
                rotationStep = maxRotationAngle - currentRotationAngle;
            }

            transform.Rotate(Vector3.up * rotationStep);
            currentRotationAngle += rotationStep;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            float rotationStep = rotationSpeed * Time.deltaTime;

            if (currentRotationAngle - rotationStep < -maxRotationAngle)
            {
                rotationStep = currentRotationAngle + maxRotationAngle;
            }

            transform.Rotate(Vector3.down * rotationStep);
            currentRotationAngle -= rotationStep;
        }

        if (Input.GetKey(KeyCode.W))
        {
            float pitchStep = rotationSpeed * Time.deltaTime;

            if (currentPitchAngle + pitchStep > maxRotationAngle)
            {
                pitchStep = maxRotationAngle - currentPitchAngle;
            }

            Cannon.transform.Rotate(Vector3.right * pitchStep);
            currentPitchAngle += pitchStep;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            float pitchStep = rotationSpeed * Time.deltaTime;

            if (currentPitchAngle - pitchStep < -maxRotationAngle)
            {
                pitchStep = currentPitchAngle + maxRotationAngle;
            }

            Cannon.transform.Rotate(Vector3.left * pitchStep);
            currentPitchAngle -= pitchStep;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isRecoiling)
        {
            shaker.TriggerShake(0.1f, 0.1f);
            StartCoroutine(Recoil());
            Shoot();
        }
    }

    IEnumerator Recoil()
    {
        isRecoiling = true;

        float elapsedTime = 0f;
        Vector3 startLocalPosition = Cannon.localPosition;

        Vector3 recoilOffset = -Cannon.transform.up * recoilDistance;

        while (elapsedTime < recoilDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / recoilDuration;
            Cannon.localPosition = Vector3.Lerp(startLocalPosition, startLocalPosition + recoilOffset, t);
            yield return null;
        }

        Cannon.localPosition = startLocalPosition + recoilOffset;


        elapsedTime = 0f;
        Vector3 endLocalPosition = startLocalPosition;

        while (elapsedTime < recoilDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / recoilDuration;

            Cannon.localPosition = Vector3.Lerp(startLocalPosition + recoilOffset, endLocalPosition, t);
            yield return null;
        }

        Cannon.localPosition = originalLocalPosition;

        isRecoiling = false;
    }

    void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.Initialize(power, firePoint.forward);
    }

}
