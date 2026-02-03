# Augmented Museum – Unity + Vuforia AR

## Project Overview

This project is an Augmented Reality museum application developed with **Unity 6 LTS** and **Vuforia Engine**. Real-world paintings are recognized using image targets, and animated virtual characters are placed **correctly on the real floor beside each artwork**, providing contextual information through speech bubbles.

The project was developed as part of an exam assignment and strictly follows all required constraints regarding image recognition, ground detection, and placement stability.

---

## Platform

- Android (tested on ARCore-supported devices)
- Unity 6 LTS
- Vuforia Engine AR

---

## Exam Requirements

The application satisfies all exam requirements:

- Recognition of paintings using Vuforia Image Targets
- Automatic activation of content when a painting is detected
- Character placement on the real-world floor (no floating)
- Explicit avoidance of walls and vertical surfaces
- Clear visualization of ground detection using Plane Finder and Plane Indicator
- Informational content displayed via animated speech bubbles

---

## Supported Paintings (Image Targets)

Only **high-quality image targets rated 4 or 5 stars by Vuforia** are used in the final version of the application to ensure robust and stable tracking.

The supported paintings are:

- Sandro Botticelli – *The Birth of Venus* (5★)
- El Greco – *View of Toledo* (5★)
- William Holman Hunt – *The Hireling Shepherd* (4★)
- John Constable – *Salisbury Cathedral from the Bishop’s Grounds* (4★)
- Gustave Caillebotte – *Paris Street; Rainy Day* (4★)

Each painting is implemented as an independent Image Target with its own character and informational content.

---

## Core AR Logic and Workflow

### 1. Painting Recognition

- Implemented using **Vuforia Image Target Behaviour**
- Targets are loaded from a Vuforia database
- When a painting is detected:
  - Visual UI feedback is shown
  - Ground plane detection is enabled

---

### 2. Ground Detection

- Implemented using **Vuforia Ground Plane (Plane Finder)**
- A **Default Plane Indicator** is displayed to visualize detected surfaces
- This clearly demonstrates real-time floor estimation as required by the exam

---

### 3. Floor vs Wall Filtering (Critical Placement Logic)

To avoid incorrect placement on walls, doors, or paintings, surface orientation is filtered using the **surface normal** returned by Vuforia hit tests.

- Floors have an upward-facing normal
- Walls and vertical surfaces have sideways-facing normals

Placement logic:

- If the surface normal’s Y component is above a defined threshold, the surface is accepted as a floor
- Otherwise, the surface is ignored

This prevents:

- Characters appearing on paintings
- Characters attaching to walls or doors
- Incorrect placement at steep camera angles

---

### 4. Character Placement and Stability

- Characters are placed on a **Ground Plane Anchor**
- Once placement occurs:
  - The Plane Finder is disabled
  - Ground refinement stops

This guarantees that:

- The character remains stable
- No drifting occurs when the phone is tilted
- The scene can be framed vertically without artifacts

---

### 5. Animated Characters

- Characters are imported and animated using **Mixamo**
- Animator Controllers manage idle and talking animations
- Character scale and alignment are adjusted for realistic real-world proportions

---

### 6. Camera-Facing Characters

- Characters automatically rotate to face the user
- Rotation is constrained to the Y-axis only
- Prevents unnatural tilting and improves readability

---

### 7. Speech Bubble and Text Display

- Informational text is displayed using **TextMeshPro**
- A typewriter-style animation is used
- The animation is triggered when:
  - The painting is detected
  - The character is placed

---

## Scene Structure (Simplified)

ARCamera
ImageTarget_[Painting]
Plane Finder
Ground Plane Stage
└── CharacterAnchor
    └── MuseumGuide_[Artist]
        ├── Character Mesh
        ├── Animator
        └── SpeechBubble (TextMeshPro)

This structure cleanly separates image tracking from ground placement logic.

---

## Known Limitations

- Ground detection quality depends on:
  - Lighting conditions
  - Device sensors (ARCore)
  - Camera angle
- Lower-end Android devices may show minor depth estimation drift

---

## Technologies Used

- Unity 6 LTS
- Vuforia Engine AR
- ARCore (Android)
- Mixamo
- TextMeshPro
- C#


