using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.XR.VisionOS.Samples.URP
{
    public class SampleUIController : MonoBehaviour
    {
        const string k_ShowPassthroughText = "Show Passthrough";
        const string k_ShowSkyboxText = "Show Skybox";
        const string k_HandTrackingAuthorizationFormat = "Hand Tracking Authorization: {0}";
        const string k_WorldSensingAuthorizationFormat = "World Sensing Authorization: {0}";

        [SerializeField]
        ParticleSystem m_ParticleSystem;

        [SerializeField]
        Camera m_Camera;

        [SerializeField]
        GameObject m_FloorObject;

        [SerializeField]
        Text m_SkyboxToggleText;

        [SerializeField]
        Text m_HandTrackingAuthorizationText;

        [SerializeField]
        Text m_WorldSensingAuthorizationText;

        void Awake()
        {
            UpdateSkyboxToggleText();

#if UNITY_VISIONOS || UNITY_EDITOR
            UpdateAuthorizationText();
#endif
        }

#if UNITY_VISIONOS || UNITY_EDITOR
        void OnEnable()
        {
            VisionOS.AuthorizationChanged += OnAuthorizationChanged;
        }

        void OnDisable()
        {
            VisionOS.AuthorizationChanged += OnAuthorizationChanged;
        }

        void UpdateAuthorizationText()
        {
            var type = VisionOSAuthorizationType.HandTracking;
            var status = VisionOS.QueryAuthorizationStatus(type);
            OnAuthorizationChanged(new VisionOSAuthorizationEventArgs { type = type, status = status });

            type = VisionOSAuthorizationType.WorldSensing;
            status = VisionOS.QueryAuthorizationStatus(type);
            OnAuthorizationChanged(new VisionOSAuthorizationEventArgs { type = type, status = status });
        }

        void OnAuthorizationChanged(VisionOSAuthorizationEventArgs args)
        {
            switch (args.type)
            {
                case VisionOSAuthorizationType.HandTracking:
                    m_HandTrackingAuthorizationText.text = string.Format(k_HandTrackingAuthorizationFormat, args.status);
                    break;
                case VisionOSAuthorizationType.WorldSensing:
                    m_WorldSensingAuthorizationText.text = string.Format(k_WorldSensingAuthorizationFormat, args.status);
                    break;
                // We do not support CameraAccess yet so ignore it
            }
        }
#endif

        public void SetParticleStartSpeed(float speed)
        {
            var mainModule = m_ParticleSystem.main;
            mainModule.simulationSpeed = speed;
        }

        public void ToggleSkybox()
        {
            if (m_Camera.clearFlags == CameraClearFlags.Skybox)
            {
                m_Camera.clearFlags = CameraClearFlags.Color;

                // A clear color with alpha = 0 is required to show passthrough, as well as setting the Metal Immersion Style to Automatic or Mixed
                // You must reset the background color every time, because setting the clear flags to Skybox will override it
                m_Camera.backgroundColor = Color.clear;

                // The floor looks weird and is no longer necessary when passthrough is visible
                if (m_FloorObject != null)
                    m_FloorObject.SetActive(false);
            }
            else
            {
                m_Camera.clearFlags = CameraClearFlags.Skybox;

                if (m_FloorObject != null)
                    m_FloorObject.SetActive(true);
            }

            UpdateSkyboxToggleText();
        }

        void UpdateSkyboxToggleText()
        {
            m_SkyboxToggleText.text = m_Camera.clearFlags == CameraClearFlags.Skybox ? k_ShowPassthroughText : k_ShowSkyboxText;
        }
    }
}
