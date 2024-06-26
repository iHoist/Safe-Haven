﻿using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    private Camera myCamera;
    private Func<Vector3> GetCameraFollowPositionFunc;
    private Func<float> GetCameraZoomFunc;

    public Transform playerTransform;
    private float zoom = 5f;

    public void Setup(Func<Vector3> GetCameraFollowPositionFunc, Func<float> GetCameraZoomFunc) {
        this.GetCameraFollowPositionFunc = GetCameraFollowPositionFunc;
        this.GetCameraZoomFunc = GetCameraZoomFunc;
    }

    private void Start() {
        Setup(() => playerTransform.position, () => zoom);
        myCamera = transform.GetComponent<Camera>();
    }

    public void SetCameraFollowPosition(Vector3 cameraFollowPosition) {
        SetGetCameraFollowPositionFunc(() => cameraFollowPosition);
    }

    public void SetGetCameraFollowPositionFunc(Func<Vector3> GetCameraFollowPositionFunc) {
        this.GetCameraFollowPositionFunc = GetCameraFollowPositionFunc;
    }

    public void SetCameraZoom(float cameraZoom) {
        SetGetCameraZoomFunc(() => cameraZoom);
    }

    public void SetGetCameraZoomFunc(Func<float> GetCameraZoomFunc) {
        this.GetCameraZoomFunc = GetCameraZoomFunc;
    }


    // Update is called once per frame
    void Update() {
        HandleMovement();
        HandleZoom();
    }

    private void HandleMovement() {
        Vector3 cameraFollowPosition = GetCameraFollowPositionFunc();
        cameraFollowPosition.z = transform.position.z;

        Vector3 cameraMoveDir = (cameraFollowPosition - transform.position).normalized;
        float distance = Vector3.Distance(cameraFollowPosition, transform.position);
        float cameraMoveSpeed = 2f;

        if (distance > 0.1f) {
            Vector3 newCameraPosition = transform.position + cameraMoveDir * distance * cameraMoveSpeed * Time.deltaTime;

            float distanceAfterMoving = Vector3.Distance(newCameraPosition, cameraFollowPosition);

            if (distanceAfterMoving > distance) {
                // Overshot the target
                newCameraPosition = cameraFollowPosition;
            }

            transform.position = newCameraPosition;
        }
    }

    private void HandleZoom() {
        /*if (Input.GetKeyDown(KeyCode.Z)) {
            zoom += 40f;
            if (zoom > 200f) zoom = 200f;
        }
        if (Input.GetKeyDown(KeyCode.X)) {
            zoom -= 40f;
            if (zoom < 40f) zoom = 40f;
        }*/

        float cameraZoom = GetCameraZoomFunc();

        float cameraZoomDifference = cameraZoom - myCamera.orthographicSize;
        float cameraZoomSpeed = 1f;

        myCamera.orthographicSize += cameraZoomDifference * cameraZoomSpeed * Time.deltaTime;

        if (cameraZoomDifference > 0) {
            if (myCamera.orthographicSize > cameraZoom) {
                myCamera.orthographicSize = cameraZoom;
            }
        } else {
            if (myCamera.orthographicSize < cameraZoom) {
                myCamera.orthographicSize = cameraZoom;
            }
        }
    }
}