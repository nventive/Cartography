# Open Source Project Template

This repository contains a template to seed a repository for an Open Source
project.

## How to use this template

1. Check out this repository
2. Delete the `.git` folder
3. Git init this repository and start working on your project!
4. Prior to submitting your request for publication, make sure to review the
   [Open Source guidelines for publications](https://nventive.visualstudio.com/Internal/_wiki/wikis/Internal_wiki?wikiVersion=GBwikiMaster&pagePath=%2FOpen%20Source%2FPublishing&pageId=7120).

## Features (to keep as-is, configure or remove)
- [Mergify](https://mergify.io/) is configured. You can edit or remove [.mergify.yml](/.mergify.yml).
- [allcontributors](https://allcontributors.org/) is configured. It helps adding contributors to the README.
- [dependabot](https://dependabot.com/) is configured. This bot will open pull requests automatically to update nuget dependencies. This one could be annoying, feel free to remove the [.dependabot](/.dependabot) folder.

The following is the template for the final README.md file:

---

# Project Title

Cartography Refactor

Doing a complete refactor of cartography module.  

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)

## Getting Started

- Clone project.
- For seeing sample, build and install app with VS on the desire device (Android, IOS, or UWP)

## Features

- DynamicMap is an interactive map for Android, IOS or UWP.
- StaticMap just show a map.
- Map Service open the default user map service and show a place or show the direction from user current location.

## Changelog

Please consult the [CHANGELOG](CHANGELOG.md) for more information about version
history.

## License

This project is licensed under the Apache 2.0 license - see the
[LICENSE](LICENSE) file for details.

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on the process for
contributing to this project.

Be mindful of our [Code of Conduct](CODE_OF_CONDUCT.md).
