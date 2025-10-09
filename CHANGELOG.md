# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.1.1] 2022-04-10

### Added

- ConverterBase for sample converters

### Removed

- Nventive.View package

## [0.4.2]

### Added

- Support for .NET 8 (Standalone, Android, iOS, Windows).

### Changed

- Updated documentation.
- New folder structure.
- Updated the sample application.
- Fixed issue with Android map not showing.
- Fixed issue with iOS pushpins not showing.
- Fixed an issue with Android pushpin scaling

### Deprecated

### Removed

- Support for Xamarin.
- Support for NetStandard2.0.

### Fixed

### Security

## [0.4.3]

### Changed

- Added more scaling suffixes for android.

## [0.5.0]

### Changed

- Added the option to disable Apple zoom animation.

## [0.6.0]

### Added

- The Google Map Control package for iOS (based on Dynamic Map).

## [0.6.1]

### Fixed

- The Google Maps icons are now correctly displayed on iOS.

## [0.6.2]

### Fixed

- The Google Maps bounds calculation from points of interest with padding.

## [0.6.3]

### Fixed

- Fixed the Google Maps initial view port race condition (View model vs Control initial value).
