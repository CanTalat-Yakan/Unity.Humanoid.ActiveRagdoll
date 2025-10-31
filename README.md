# Unity Essentials

This module is part of the Unity Essentials ecosystem and follows the same lightweight, editor-first approach.
Unity Essentials is a lightweight, modular set of editor utilities and helpers that streamline Unity development. It focuses on clean, dependency-free tools that work well together.

All utilities are under the `UnityEssentials` namespace.

```csharp
using UnityEssentials;
```

## Installation

Install the Unity Essentials entry package via Unity's Package Manager, then install modules from the Tools menu.

- Add the entry package (via Git URL)
  - Window → Package Manager
  - "+" → "Add package from git URL…"
  - Paste: `https://github.com/CanTalat-Yakan/UnityEssentials.git`

- Install or update Unity Essentials packages
  - Tools → Install & Update UnityEssentials
  - Install all or select individual modules; run again anytime to update

---

# Humanoid Active Ragdoll

> Quick overview: Physics-driven “active ragdoll” that follows an animation using a simple PD controller and ConfigurableJoints. It can temporarily weaken on collisions and then recover, giving natural, reactive motion.

A compact, dependency-free system for reactive humanoids: a ragdoll (“slave”) tracks an animated hierarchy (“master”) via PD forces and joint slerp drives, weakens on contact for believable impacts, and smoothly regains strength—ideal for physics-augmented characters and dynamic hit reactions.

![screenshot](Documentation/Screenshot.png)

## Features
- Animation-following controller
  - PD control for position and rotation alignment per ragdoll limb
  - Uses ConfigurableJoint slerpDrive and force application at each Rigidbody’s center of mass
- Collision-aware strength modulation
  - Weakens while colliding; smoothly regains strength when free
  - LayerMask to exclude collisions that should not cause weakening
- Simple state machine
  - Following Animation, Losing Strength, Gaining Strength, Dead
  - Context menu actions to Die, Come Alive, or Reset Forces
- Tunable physics
  - Gravity toggle, drag/ang. drag, max angular velocity, joint damping
  - Global limits for max force and max joint torque
- Lightweight, dependency-free
  - No Animator rig constraints required; works with any matching transform hierarchies

## Requirements
- Unity Editor 6000.0+ (per package manifest)
- Two matching humanoid hierarchies:
  - Master (animated/kinematic) hierarchy
  - Slave (ragdoll) hierarchy with Rigidbody + ConfigurableJoint per limb (hips/root without joint)
- Matching transform counts and ordering between master and slave (one-to-one)
- Correct physics setup (layers, colliders, mass, joints)

Note: This repository currently ships with an Editor-only assembly definition (`UnityEssentials.HumanoidActiveRagdoll.Editor.asmdef`). For runtime builds, move the scripts to a `Runtime/` assembly or create a runtime asmdef and adjust references accordingly.

## Usage
1) Prepare your character
   - Create/keep your animated character (Master). The hips/root transform should be the master root.
   - Create a ragdoll (Slave) with Rigidbody + ConfigurableJoint on each limb (hips/root should not have a joint).
   - Ensure the master and slave hierarchies have the same transforms in the same order.

2) Set up the controller
   - Add a new parent GameObject (e.g., `ActiveRagdoll`) and add `HumanoidSetUp` to it.
   - Assign `masterRoot` (static/animated hierarchy root) and `slaveRoot` (ragdoll hips/root).
   - Set `dontLooseStrengthLayerMask` to the layers that should NOT cause weakening (e.g., Player/Own body).
   - Add `AnimationFollowing` and `SlaveController` to the same parent (the scripts auto-find each other via the parent `HumanoidSetUp`).

3) Tune parameters
   - In `AnimationFollowing`: adjust `PForce`, `DForce`, `maxForce`, `maxJointTorque`, `useGravity`, and joint damping.
   - In `SlaveController`: set how quickly strength is lost/recovered, minimum force/torque at contact, and dead time.

4) Play and test
   - Enter Play Mode; the ragdoll follows the master animation.
   - Cause collisions to see temporary weakening; strength returns smoothly when free.
   - Use the `SlaveController` context menu (gear icon) to try `Die`, `Come alive`, and `Reset forces`.

## How It Works
- Initialization
  - Collects transforms from master and slave; validates a 1:1 mapping.
  - Caches rigidbodies, configurable joints, and precomputes joint-space rotations.
- Per-physics-step following
  - Computes a target world center-of-mass for each limb based on the master.
  - Applies a PD force toward that target and updates the joint’s `targetRotation` for orientation.
  - Clamps by `maxForce` and `maxJointTorque` and applies optional gravity/drag settings.
- Collision-driven strength
  - Each limb auto-adds a `CollisionDetector` that increments/decrements a shared collision counter.
  - `SlaveController` reduces force/torque coefficients while colliding (down to configured minimums) and lerps them back afterward.
  - `Die/Come alive` flip an `isAlive` flag that disables joint torque until revived.

## Notes and Limitations
- Hierarchy constraints
  - Master and slave must have identical transform counts/order; otherwise following is aborted with a warning.
  - The ragdoll root (hips) must not have a ConfigurableJoint; limbs should.
- Physics tuning is content-dependent; realistic results require correct masses, limits, and joint settings.
- Per-limb profiles exist internally but aren’t exposed in the inspector in this version.
- Editor-only by default
  - The included asmdef is Editor-only. To use in a player build, move scripts to a runtime assembly definition and recompile.

## Files in This Package
- `Editor/HumanoidSetUp.cs` – References to master/slave roots, layer mask, and component wiring
- `Editor/AnimationFollowing.cs` – PD-based animation following (forces + joint target rotations)
- `Editor/CollisionDetector.cs` – Collision counting per limb, with layer mask filtering
- `Editor/SlaveController.cs` – State machine (follow/lose/gain/dead), strength interpolation, context menu
- `Editor/UnityEssentials.HumanoidActiveRagdoll.Editor.asmdef` – Editor assembly definition

## Tags
unity, unity-editor, ragdoll, active-ragdoll, humanoid, physics, configurable-joint, pd-controller, animation, runtime-physics
