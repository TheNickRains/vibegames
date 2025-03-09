# Contributing to VibeMod

Thank you for your interest in contributing to VibeMod! This document outlines the process and guidelines for contributing to the project.

## Code Style

We follow the Unity C# style guidelines:
- Use PascalCase for class names, public methods, and properties
- Use camelCase for private fields and local variables
- Use meaningful names that clearly describe the purpose
- Keep methods short and focused on a single responsibility
- Comment complex algorithms and public API methods

## Git Workflow

1. Fork the repository
2. Create a feature branch from `develop`
3. Make your changes
4. Write or update tests as necessary
5. Ensure all tests pass
6. Submit a pull request to the `develop` branch

## Commit Messages

Follow the conventional commits format:
- `feat:` for new features
- `fix:` for bug fixes
- `docs:` for documentation changes
- `refactor:` for code changes that neither fix bugs nor add features
- `test:` for adding or updating tests
- `chore:` for changes to the build process, tools, etc.

Example: `feat: add player physics interaction system`

## Pull Request Process

1. Update the README.md with details of changes if applicable
2. Update documentation if needed
3. The PR should work on all supported platforms
4. PRs require review from at least one maintainer
5. Once approved, a maintainer will merge the PR

## Development Environment Setup

1. Install Unity 2022.3 LTS or newer
2. Install Git LFS to handle large binary files
3. Clone the repository using `git clone https://github.com/username/vibe-games.git`
4. Open the project from Unity Hub

## Testing

- Write unit tests for all new functionality
- Ensure existing tests pass before submitting PRs
- Integration tests should be written for major gameplay systems
- Performance tests are required for systems that might impact framerate

## Assets and Resources

- All assets must be properly licensed for commercial use
- Include attribution information in the `CREDITS.md` file
- Optimize assets before committing them
- Use asset variants for different quality settings

## Communication

- Use GitHub Issues for bug reports and feature requests
- Join our Discord server for real-time discussion
- Subscribe to our development newsletter for updates

Thank you for contributing to VibeMod! 