using UnityEngine;
using System.Collections;

public class CameraTabController : MonoBehaviour
{
    [Header("Camera")]
    public Camera mainCamera;
    public Transform cameraTransform;

    [Header("Targets")]
    public Transform equipPoint;
    public Transform battlePoint;

    [Header("FOV")]
    public float equipFOV = 60f;
    public float battleFOV = 45f;

    [Header("Settings")]
    public float moveDuration = 0.8f;
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    Coroutine moveRoutine;

    public void MoveToEquip()
    {
        MoveCamera(equipPoint, equipFOV);
    }

    public void MoveToBattle()
    {
        MoveCamera(battlePoint, battleFOV);
    }

    void MoveCamera(Transform target, float targetFOV)
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveRoutine(target, targetFOV));
    }

    IEnumerator MoveRoutine(Transform target, float targetFOV)
    {
        Vector3 startPos = cameraTransform.position;
        Quaternion startRot = cameraTransform.rotation;
        float startFOV = mainCamera.fieldOfView;

        float time = 0f;

        while (time < moveDuration)
        {
            float t = time / moveDuration;
            float easedT = ease.Evaluate(t);

            cameraTransform.position = Vector3.Lerp(startPos, target.position, easedT);
            cameraTransform.rotation = Quaternion.Slerp(startRot, target.rotation, easedT);
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, easedT);

            time += Time.deltaTime;
            yield return null;
        }

        cameraTransform.position = target.position;
        cameraTransform.rotation = target.rotation;
        mainCamera.fieldOfView = targetFOV;
    }
}
