# ViewpointManipulation

This is the repository for my master's thesis: "Navigation and Manipulation of Multiple Live Viewpoints for Mixed Reality Head-mounted Displays".

The thesis is in [`MasterThesis_ViewPointManipulation_dmaerzendorfer.pdf`](MasterThesis_ViewPointManipulation_dmaerzendorfer.pdf).

## About

The prototype lets a headset user place and control extra live camera viewpoints and look through them on in-world view panels. Three viewpoint techniques are implemented and compared in a user study:

- **Drone cam.** A free-flying camera that the user steers directly (`DroneCamController`).
- **Orbit cam.** A camera that orbits a point of interest (`OrbitCamController`).
- **Hover cam.** Camera navigation close to an object, based on Khan et al., 2005, "HoverCam" (`HoverCamController`).

`ViewManager` and the `ViewModeHandler` classes switch between the modes. `StudyManager` and `StudyDataRecord` run the study and log its data.

## Repository layout

| Path | Contents |
| --- | --- |
| `ViewpointManipulation/` | Unity project |
| `Builds/` | Android APK build for the headset |
| `Calibrations/` | Room calibration files |
| `Latex/` | LaTeX source of the thesis (zipped) |
| `ResearchDiary/` | Research diary and questionnaires |
| `Sources/` | PDFs of the cited papers |

## Getting started

1. Open `ViewpointManipulation/` in **Unity 2022.3.9f1**.
2. Main scenes: `Assets/Scenes/Study.unity`, `PilotStudy.unity` and `Museum.unity`. There are also test scenes for each camera mode (`DroneCam`, `OrbitCam`, `HoverModeTest`).
3. Build for Android, or run it through Quest Link. The project uses the XR Interaction Toolkit 2.5, OpenXR and the Oculus XR plugin.
