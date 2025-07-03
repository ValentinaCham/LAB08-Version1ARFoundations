using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MultitargetManager : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager aRTrackedImageManager;
    [SerializeField] private GameObject[] aRModelsToPlace;
    private Dictionary<string, GameObject> aRModels = new Dictionary<string, GameObject>();
    private Dictionary<string, bool> modelState = new Dictionary<string, bool>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (var model in aRModelsToPlace)
        {
            GameObject newARModel = Instantiate(model, Vector3.zero, Quaternion.identity);
            newARModel.name = model.name;
            aRModels.Add(model.name, newARModel);
            modelState.Add(model.name, false);
        }
    }

    private void OnEnable()
    {
        aRTrackedImageManager.trackedImagesChanged += ImageFound;
    }

    private void OnDisable()
    {
        aRTrackedImageManager.trackedImagesChanged -= ImageFound;
    }

    private void ImageFound(ARTrackedImagesChangedEventArgs eventData)
    {
        foreach (var trackedImage in eventData.added)
        {
            ShowModel(trackedImage);
        }
        foreach (var trackedImage in eventData.updated)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                ShowModel(trackedImage);
            }
            else if (trackedImage.trackingState == TrackingState.Limited)
            {
                HideModel(trackedImage);
            }
        }
    }

    private void ShowModel(ARTrackedImage trackedImage) {
        bool isModelActive = modelState[trackedImage.referenceImage.name];
        if (!isModelActive)
        {
            GameObject aRModel = aRModels[trackedImage.referenceImage.name];
            aRModel.transform.position = trackedImage.transform.position;
            aRModel.SetActive(true);
            modelState[trackedImage.referenceImage.name] = true;
        }
        else
        {
            GameObject aRModel = aRModels[trackedImage.referenceImage.name];
            aRModel.transform.position = trackedImage.transform.position;
        }
    }

    private void HideModel(ARTrackedImage trackedImage)
    {
        bool isModelActive = modelState[trackedImage.referenceImage.name];
        if (isModelActive)
        {
            GameObject aRModel = aRModels[trackedImage.referenceImage.name];
            aRModel.SetActive(false);
            modelState[trackedImage.referenceImage.name] = false;
        }
    }
}
