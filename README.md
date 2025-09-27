
# The Herd

[![Discord](https://img.shields.io/badge/Discord-5865F2?style=flat&logo=discord&logoColor=white)](https://discord.gg/yDpBzZGCCA)
[![Hanze](https://img.shields.io/badge/Hanze-EE7F00?style=flat&logo=hanze&logoColor=white)](https://www.hanze.nl/en)
[![Unity](https://img.shields.io/badge/Unity6-FFFFFF?style=flat&logo=unity&logoColor=black)](https://unity.com/releases/unity-6)
[![Blender](https://img.shields.io/badge/blender-EA7600?style=flat&logo=blender&logoColor=white)](https://www.blender.org/)
[![Adobe](https://img.shields.io/badge/adobe-ED2224?style=flat&logo=adobe&logoColor=white)](https://www.adobe.com/)

This project is made with Unity in `Unity 6000.0.58f1`.

A top down horror game where you are a Shepherd herding sheep across the mountains during the old Yugoslavia. The sheep, your dog and you go through fields and villages while traveling the mountain. As you go the people and world around you becomes more and more cryptid. Your goal is to find a place to settle and avoid the war currently happening in your country.

## Table of Contents

1. [Overview](#overview)
    - [Getting Started](#getting-started)
    - [Project Basics](#project-basics)
2. [Best Practices](#best-practices)
    - [Project Structure](#project-structure)
    - [Coding Standards](#coding-standards)
3. [GitHub](#github)
    - [Project](#project)
    - [Rules](#rules)
    - [Teams](#teams)
    - [Pull Requests](#pull-requests)
4. [Support](#support)

## Overview

This repository holds the project `The Herd` for the Project Game Design and Development minor.

### Getting Started

It is recommended to use a linear branch structure and avoid as many ramifications as possible. Every team has its own branch to work with and can use it as their main branch, meaning it is possible to create other branches out of it. It is ***not*** recommended to fork the repository in order to avoid merging conflicts.

#### Steps

- Make sure `Unity 6000.0.58f1` is installed
- Clone this repo on your computer
- In the Unity Hub click the *Add* button and navigate to the folder where your project is located
- Make sure you are in the right branch
- Open your project
- Enjoy

### Project Basics

Contains the following:

| Name | Description |
| --- | --- |
| [Assembly Definition](https://docs.unity3d.com/6000.0/Documentation/Manual/assembly-definition-files.html) | Organize and optimize script compilation |
| [Cinemachine](https://docs.unity3d.com/6000.2/Documentation/Manual/com.unity.cinemachine.html) | Advanced camera system |
| [Input System](https://docs.unity3d.com/6000.0/Documentation/Manual/com.unity.inputsystem.html) | Flexible, event-driven input handling across devices |
| [TextMeshPro](https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/manual/index.html) | High-quality text rendering and formatting |
| [Universal RP](https://docs.unity3d.com/6000.0/Documentation/Manual/urp/urp-introduction.html) | Efficient rendering pipeline for various platforms |

## Best Practices

### Project Structure

The project is structured and organized to be beneficial and readable for you and your team.

Everything is organized inside the `Asset/*your-team*/` folder, keeping your files well organized. Each team will have its personal folder. This is mainly done to prevent confusion between folders when importing a package and avoid conflicts between teams' assets and scripts.

Keep your folder organized! Create folders by type, so it's easy to find any file needed.

**Benefits:**

- Files are separated from packages, keeping everything organized
- A consistent project structure aid readability and help reducing time searching for some file
- The project structure has consistency in presentation regardless of team location and language, or individual programmers
- This helps connecting everything together for the final product

### Coding Standards

Coding standards define a programming style and helps you keep everything organized and clean. The Tech Department has already prepared a detailed document, so please refer to it for any information.

This project has to follows:

- Naming conventions
- Formatting and Indentation
- Use of namespaces
- Comments
- Design Patterns

Writing clean and consistent code give benefits after just a short amount of work. This helps with implementation of new features, maintenance, minimizing the amount of work when debugging and gives a clean communication between different programmers.

> [!TIP]
> Assembly definitions will be used for the main branch and therefore for the final project. Teams can still create their own, as long as it doesn't interfere with each other's work.

> [!CAUTION]
> Use of Layers and Tags inside of Unity can be really dangerous because of the amount of teams working with it! If you need to use one of those please contact Integration team first.

## GitHub

Here are more information about the features this template offers. Follow those guidelines if you want to understand better how the project works and how to modify it.

### Project

This repository is meant for the entire minor project. Everything from the demo to the final project is combined in the `main` branch. Major updates will receive a version tag along with a release package.

When merging a `Team branch` into `main` the Integration Team could move your work into the game main folder in order to connect everyone's' work for the final game.

### Rules

Those are the rules everyone must follow for this repository:

1. It is **NOT** allowed to work on the `main` branch
2. Only Integration Team is allowed to modify and work on `main`
3. Every team has their own `Team branch` and need to treat it as their main branch
4. Team members can make new branches from their `Team branch`
5. Pull requests to the `main branch` are allowed only from a `Team branch`
6. Every commit **must** have a title and an exhaustive description
7. Don't use Layers and Tags before consulting Integration Team first

> [!NOTE]
> Contact Integration Team for further questions

### Teams

We recommend Teams to create personal branches from their `Team branch` and merge their work into it before creating a pull request.

It is also up to each team to make sure they are up to date with `main` and notify other teams when their pull request gets accepted and merged into the main project.

### Pull Requests

As per [Rules](#rules), pull requests to the `main` branch are accepted only from the `Team branch`. If any error occur the Integration team can start a discussion inside the pull request, highliting the problem and clarify what needs to be changed. Discussions may also start because of unclear requests or code. Pull requests can be declined when something doesn't follow departments guidelines or may cause issues with the whole project.

Pull requests may also be redirected to another branch whenever Integration team suspects something may not work as intended when merging or for testing that all features works as intended without dirtying the `main` branch.

## Support

If you have any suggestions for improving this repository or you want to report something, feel free to open an issue here on GitHub or with the ticket system on Discord. The Integration Team will try to make sure to answer you within 1 business day.

Eventually you can also contact the Integration Team directly on Discord for any more urgent issues.

> [!NOTE]
> This is a living document and may be updated in the future
