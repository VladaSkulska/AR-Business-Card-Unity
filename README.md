# AR-Business Card: Digital Gateway

Interactive Augmented Reality (AR) mobile application developed as part of a Software Engineering coursework at Kyiv Polytechnic Institute (KPI).

## Project Description

The "AR-Business Card" project is a mobile solution designed to transform traditional paper business cards into interactive digital experiences. By utilizing Unity and AR Foundation, the application identifies physical markers and overlays detailed 3D models—such as laptops, smartphones, and planets—which users can manipulate in real-time.

## Technical Specifications

* **Engine**: Unity 2022.3+ using Universal Render Pipeline (URP)
* **AR Framework**: AR Foundation supporting ARCore and ARKit
* **Scripting Language**: C# focusing on asynchronous logic and Raycasting
* **Input System**: New Unity Input System with Enhanced Touch for multi-touch gesture processing

## Engineering Features

### Image Tracking and Stabilization
The application implements a Reference Image Library to detect and track physical business cards. It includes custom logic to reduce jitter, ensuring that virtual objects remain stable even during minor camera movements.

### Interactive 3D Manipulation
* **Rotation**: Users can rotate 3D models around the world-space Y-axis using a single touch.
* **Scaling and Translation**: Implementation of pinch-to-zoom functionality and spatial movement using Raycasting to detect collisions with model colliders.

### Billboard UI System
Interface elements, such as social media icons, utilize a Billboard effect. This ensures that 3D buttons always orient themselves toward the user's camera for optimal readability regardless of the viewing angle.

## Repository Architecture

The repository includes the essential source files required to build or modify the project:

* **Assets**: Contains all C# scripts, 3D assets, and AR configurations
* **Packages**: Lists project dependencies and XR plugins
* **ProjectSettings**: Configuration for the Unity engine and platform-specific build parameters
* **Markers**: A dedicated folder containing the 4 business card images used as AR markers for testing

## Installation and Usage

1. **Download**: Navigate to the [Releases](https://github.com/VladaSkulska/AR-Business-Card-Unity/releases) section and download the `build1.apk` file
2. **Install**: Sideload the APK onto an ARCore-compatible Android device
3. **Test**: Launch the application and point the device camera at the images provided in the `/Markers` folder. Scanning directly from a computer screen is supported.
