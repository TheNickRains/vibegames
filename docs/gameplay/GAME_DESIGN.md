# VibeMod Game Design

## Overview

VibeMod is a physics-based multiplayer online game inspired by Garry's Mod but with ultra-realistic graphics and focused game modes. The game combines the creative sandbox elements of Garry's Mod with structured gameplay in the form of Hide and Seek and Infection game modes, all set in detailed environments inspired by Call of Duty maps.

## Core Pillars

1. **Physics-Based Interaction**: Everything in the world can be manipulated, picked up, thrown, and used.
2. **Ultra-Realistic Graphics**: Utilizing Unity's HDRP for next-generation visuals.
3. **Structured Game Modes**: Focused gameplay experiences within the sandbox environment.
4. **Social Gameplay**: Emphasis on player interaction and communication.
5. **Accessibility**: Free-to-play model with non-intrusive monetization.

## Game Modes

### Hide and Seek

A classic game of hide and seek reimagined in detailed, interactive environments.

#### Rules:
- Players are divided into two roles: **Hiders** and **Seekers**
- Initially, there is one Seeker, and all other players are Hiders
- Hiders have a preparation phase (30 seconds) to find hiding spots before the Seeker is released
- Seekers must find and tag all Hiders within the time limit (5 minutes)
- Hiders can pick up and move objects to create better hiding spots or distractions
- When a Hider is found, they become a Seeker and join the hunt
- If all Hiders are found before the time limit, Seekers win
- If any Hiders remain unfound when time expires, Hiders win

#### Mechanics:
- **Prop Manipulation**: Hiders can pick up, move, and arrange props to create hiding spots
- **Limited Sprint**: Players have a stamina bar that depletes while sprinting
- **Sound Indicators**: Making noise (running, moving objects) creates visual sound indicators for nearby players
- **Tagging**: Seekers must physically touch (or shoot with a tagging gun) Hiders to find them
- **Hint System**: Every 60 seconds, a subtle hint appears for Seekers (sound pulse from hiding players)

### Infection

A survival horror-inspired mode where infection spreads through the player base.

#### Rules:
- Players start as either **Survivors** or the initial **Infected** (initially just one player)
- The Infected must spread the infection by tagging Survivors
- Survivors must evade the Infected until the time limit expires (5 minutes)
- Each time a Survivor is infected, they join the Infected team
- If all players become Infected before the time limit, the Infected team wins
- If any Survivors remain when time expires, the Survivors win
- Survivors can barricade areas using physics objects to create temporary safe zones

#### Mechanics:
- **Barricading**: Survivors can move and stack objects to block pathways
- **Infected Vision**: Infected players gain a special vision mode that highlights nearby Survivors
- **Survivor Tools**: Survivors have access to tools that can temporarily stun Infected (flashlights, noise makers)
- **Infected Abilities**: Infected move slightly faster than Survivors and can sense nearby movement
- **Safe Zones**: Certain areas can be activated as temporary safe zones with cooldowns

### Sandbox Mode

A free-form creative mode for players to experiment with the physics system and build contraptions.

#### Features:
- **Full Physics System**: All objects follow realistic physics rules
- **Building Tools**: Players can connect objects together to create complex structures
- **Item Spawner**: Spawn various objects and items into the world
- **Weapon Playground**: Test different weapons and their effects on the environment
- **Vehicle Workshop**: Create and customize vehicles from component parts
- **No Objectives**: Pure creativity with no win/loss conditions

## Maps

VibeMod features recreations of iconic Call of Duty maps, reimagined with enhanced detail and interactivity:

1. **Nuketown**: A small suburban testing ground with houses and vehicles
2. **Terminal**: An airport terminal with multiple levels and hiding spots
3. **Shipment**: A compact cargo container yard for intense gameplay
4. **Warehouse**: An original map featuring a large industrial space with machinery
5. **Mansion**: A luxury estate with extensive grounds and multiple buildings
6. **Office**: A multi-floor office building with cubicles, meeting rooms, and ventilation systems

Each map features:
- **Destructible Elements**: Certain walls and objects can be broken
- **Interactive Props**: Most objects can be moved and manipulated
- **Dynamic Lighting**: Time of day changes and dynamic lighting effects
- **Secret Areas**: Hidden rooms and passages for advanced hiding spots
- **Environmental Hazards**: Some areas contain hazards that players must avoid

## Player Abilities

All players have access to:

1. **Object Manipulation**:
   - Grab and move objects with realistic weight and physics
   - Rotate and place objects with precision
   - Stack and balance objects to create structures
   - Throw objects with force proportional to their weight

2. **Movement**:
   - Walk and sprint (with stamina limitations)
   - Jump and crouch
   - Lean around corners
   - Climb certain surfaces and ladders

3. **Tools**:
   - Gravity Gun: Pick up and manipulate objects from a distance
   - Welding Tool: Connect objects together
   - Rope Tool: Create rope connections between objects
   - Flashlight: Illuminate dark areas
   - Camera: Take screenshots of the game world

## Physics System

The heart of VibeMod is its advanced physics system:

- **Realistic Weight**: Objects have appropriate weight and mass affecting how they move
- **Material Properties**: Different materials (wood, metal, glass) have appropriate physical properties
- **Collision Dynamics**: Objects collide realistically with appropriate sound and visual effects
- **Constraint System**: Objects can be connected with various types of constraints (fixed, hinged, ropes)
- **Breakable Objects**: Certain objects can break or deform when sufficient force is applied
- **Fluid Dynamics**: Basic simulation of water and other fluids

## Progression System

While the game is free-to-play, it includes a progression system:

- **Player Level**: Gain XP from participating in matches and completing objectives
- **Unlockable Tools**: New tools and abilities unlock as players level up
- **Customization Options**: Unlock visual customizations for player characters
- **Blueprint System**: Save and share contraptions built in sandbox mode
- **Seasonal Events**: Limited-time maps and game modes with special rewards

## Art Style

VibeMod features ultra-realistic graphics with:

- **Photorealistic Textures**: High-resolution textures with appropriate PBR properties
- **Advanced Lighting**: Global illumination, volumetric lighting, and dynamic shadows
- **Particle Effects**: High-fidelity particle systems for explosions, fire, and other effects
- **Animation**: Smooth, motion-captured animations for player characters
- **Post-Processing**: Modern post-processing effects including ambient occlusion, screen space reflections, and depth of field

## Sound Design

Audio is crucial for immersion and gameplay:

- **3D Spatial Audio**: Sounds are properly positioned in 3D space
- **Material-Based Sound Effects**: Different materials make appropriate sounds when interacting
- **Ambient Sound**: Dynamic ambient sound systems based on environment and events
- **Voice Chat**: Proximity-based voice chat with directional audio
- **Music**: Dynamic music system that adapts to the current game state

## Monetization

As a free-to-play game, VibeMod includes non-intrusive monetization:

- **Cosmetic Items**: Character customization options that don't affect gameplay
- **Battle Pass**: Seasonal battle passes with cosmetic rewards
- **Tool Skins**: Visual variations of in-game tools
- **Ad Integration**: Optional ad viewing for bonus rewards
- **Premium Maps**: Some specialized maps may be offered as premium content

The core gameplay experience remains completely free, with monetization focused on optional enhancements that don't affect game balance. 