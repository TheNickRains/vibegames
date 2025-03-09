# VibeMod Deployment Guide

This document provides detailed instructions for setting up, building, and deploying the VibeMod MMORPG project. The project consists of a Unity client application and a Next.js web interface, both deployed to Vercel.

## Prerequisites

Before you begin, ensure you have the following:

- [Unity](https://unity.com/download) 2022.3 LTS or newer
- [Node.js](https://nodejs.org/) 18.x or newer
- [Photon PUN 2](https://www.photonengine.com/pun) Account and App ID
- [Vercel](https://vercel.com/) Account
- [Firebase](https://firebase.google.com/) Project (for authentication and database)
- [Git](https://git-scm.com/) and [Git LFS](https://git-lfs.github.com/) for version control

## Project Structure

The project is organized as follows:

```
vibegames/
│
├── unity-client/            # Unity MMORPG client
│   ├── Assets/              # Game assets and scripts
│   ├── Packages/            # Unity packages
│   └── ProjectSettings/     # Unity project settings
│
├── web-interface/           # Next.js web application
│   ├── public/              # Static assets
│   └── src/                 # React source code
│
├── server/                  # Backend server code
│   ├── auth/                # Authentication logic
│   ├── game-logic/          # Game server logic
│   └── api/                 # API endpoints
│
├── docs/                    # Project documentation
│   ├── setup/               # Setup guides
│   ├── architecture/        # Architecture diagrams
│   └── api/                 # API documentation
│
├── README.md                # Project overview
├── CONTRIBUTING.md          # Contribution guidelines
└── vercel.json              # Vercel deployment configuration
```

## Development Environment Setup

### Unity Client Setup

1. Download and install Unity Hub and Unity 2022.3 LTS.
2. Open Unity Hub and click "Add" to add the `unity-client` directory as a project.
3. Install required dependencies:
   - Open Package Manager (Window > Package Manager)
   - Install the High Definition Render Pipeline (HDRP)
   - Import Photon PUN 2 from the Asset Store

4. Set up Photon:
   - Create a Photon account at https://www.photonengine.com/
   - Create a new Photon PUN application
   - Copy your App ID
   - In Unity, go to Window > Photon Unity Networking > PUN Wizard
   - Paste your App ID and set up the project

5. Configure project settings:
   - Open Project Settings (Edit > Project Settings)
   - Set up the HDRP for high-quality graphics
   - Configure Input settings for player controls
   - Set up appropriate Physics settings

### Web Interface Setup

1. Navigate to the `web-interface` directory:
   ```bash
   cd web-interface
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Create a `.env.local` file in the `web-interface` directory with the following:
   ```
   NEXT_PUBLIC_PHOTON_APP_ID=your_photon_app_id
   NEXT_PUBLIC_FIREBASE_API_KEY=your_firebase_api_key
   NEXT_PUBLIC_FIREBASE_AUTH_DOMAIN=your_firebase_auth_domain
   NEXT_PUBLIC_FIREBASE_PROJECT_ID=your_firebase_project_id
   NEXT_PUBLIC_FIREBASE_STORAGE_BUCKET=your_firebase_storage_bucket
   NEXT_PUBLIC_FIREBASE_MESSAGING_SENDER_ID=your_firebase_messaging_sender_id
   NEXT_PUBLIC_FIREBASE_APP_ID=your_firebase_app_id
   ```

4. Start the development server:
   ```bash
   npm run dev
   ```

5. Open [http://localhost:3000](http://localhost:3000) in your browser to see the web interface.

## Building for Production

### Building the Unity Client

1. Open the Unity project in Unity Editor.

2. Set up WebGL build:
   - Go to File > Build Settings
   - Select WebGL platform
   - Click "Switch Platform" if not already selected
   - Click "Player Settings" and configure:
     - Set company and product name
     - Configure memory settings (recommended 2GB+)
     - Enable "Decompression Fallback" for better browser compatibility

3. Build the WebGL version:
   - Still in Build Settings, click "Build"
   - Choose `unity-client/WebGL/Build` as the output directory
   - Wait for the build process to complete (this may take a while)

4. For desktop builds (optional):
   - Switch platform to Windows/Mac/Linux
   - Configure appropriate settings
   - Build to `unity-client/Desktop/Build`

### Building the Web Interface

1. Navigate to the `web-interface` directory:
   ```bash
   cd web-interface
   ```

2. Build the production version:
   ```bash
   npm run build
   ```

3. Test the production build locally:
   ```bash
   npm run start
   ```

## Deployment to Vercel

The project is configured for deployment to Vercel, a cloud platform for static sites and serverless functions.

### Vercel Setup

1. Create an account on [Vercel](https://vercel.com/).

2. Install the Vercel CLI:
   ```bash
   npm install -g vercel
   ```

3. Login to Vercel:
   ```bash
   vercel login
   ```

### Setting up Environment Variables

1. Log in to your Vercel dashboard.

2. Navigate to your project settings > Environment Variables.

3. Add the following environment variables:
   - `PHOTON_APP_ID`: Your Photon application ID
   - `FIREBASE_API_KEY`: Your Firebase API key
   - `FIREBASE_AUTH_DOMAIN`: Your Firebase auth domain
   - `FIREBASE_PROJECT_ID`: Your Firebase project ID
   - `FIREBASE_STORAGE_BUCKET`: Your Firebase storage bucket
   - `FIREBASE_MESSAGING_SENDER_ID`: Your Firebase messaging sender ID
   - `FIREBASE_APP_ID`: Your Firebase app ID
   - `NEXT_PUBLIC_API_URL`: The URL for your API services

### Deployment

1. From the root directory of the project, run:
   ```bash
   vercel
   ```

2. Follow the prompts to set up your project.

3. For subsequent deployments, use:
   ```bash
   vercel --prod
   ```

### Automatic Deployments

The project is configured with GitHub integration for automatic deployments:

1. Connect your GitHub repository to Vercel.
2. Configure automatic deployments:
   - Production branch: `main`
   - Preview branches: `develop`

## Configuring Photon Servers

For better performance in production:

1. Log in to your Photon dashboard at https://dashboard.photonengine.com/
2. Select your application
3. Configure the regions where your game will be available
4. Note that the free tier has limitations on CCU (concurrent users)
5. For production, consider upgrading to a paid plan

## Monitoring and Analytics

Set up monitoring for your live application:

1. Use Unity Analytics for game performance and player behavior
2. Set up Firebase Analytics for web interface tracking
3. Configure error logging with Sentry or similar services
4. Set up Vercel Analytics for website performance monitoring

## Scaling Considerations

As your player base grows:

1. Consider using a dedicated game server solution for high CCU games
2. Implement server regions to reduce latency for global players
3. Use CDN for static assets to improve load times
4. Consider sharding your database for improved performance
5. Implement caching strategies for frequently accessed data

## Troubleshooting

Common deployment issues:

1. **WebGL Build Issues**:
   - Ensure your Unity WebGL templates are up to date
   - Check for JavaScript compatibility issues
   - Test on multiple browsers

2. **Vercel Deployment Failures**:
   - Check build logs in the Vercel dashboard
   - Ensure your build commands are correct
   - Verify environment variables are set correctly

3. **Photon Connection Issues**:
   - Check if your Photon App ID is correct
   - Verify firewall settings aren't blocking WebSocket connections
   - Check CCU limits on your Photon plan

## Support

If you encounter any issues during deployment, please:

1. Check our [troubleshooting guide](../support/TROUBLESHOOTING.md)
2. Search for similar issues in our [GitHub Issues](https://github.com/your-org/vibegames/issues)
3. Join our [Discord server](https://discord.gg/vibemod) for community support
4. Contact the development team at support@vibemod.com 