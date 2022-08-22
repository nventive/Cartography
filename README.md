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

DynamicMap
1.	Show Map
   1.1.	Google Map
      1.1.1.	Android
      1.1.2.	IOS
      1.1.3.	UWP
   1.2.	IOS Map
      1.2.1.	IOS only
   1.3.	Bing Map
      1.3.1.	UWP only

2.	Show user location

3.	Show Pushpin
   3.1.	Filter Pushpin
   3.2.	Add Pushpin
   3.3.	Remove Pushpin
   3.4.	Customize pushpin
   3.5.	Group pushpin

4.	Map interaction
   4.1.	Drag
   4.2.	Zoom
   4.3.	Rotate
   4.4.	Select pushpin
   4.5.	Deselect pushpin
   4.6.	Zoom on pushpin
   4.7.	Add Pushpin
   4.8.	Remove Pushpin
   4.9.	Stop animation
   4.10.	Zoom on user
   4.11.	Show POI

5.	Follow User
   5.1.	Start follow user
   5.2.	Stop follow user
   
StaticMap
   1.	Show Map
      1.1.	Google Map
         1.1.1.	Android
         1.1.2.	IOS
         1.1.3.	UWP
      1.2.	IOS Map
         1.2.1.	IOS only
      1.3.	Bing Map
         1.3.1.	UWP only
   2.	Show user location
   3.	Show Pushpin

MapService
1.	Open user default map service and show location.
2.	Open user default map service and show direction.


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
