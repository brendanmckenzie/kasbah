# Kasbah Analytics

The purpose of the Kasbah Analytics module is to provide an means of tracking user engagement with content.

## Usage

 1. Add the `Kasbah.Analytics` Nuget package as a reference to your project
 2. Call the `.AddAnalytics()` and `.UseAnalytics()` extension methods to register and initialise the module in your `Startup` class

The underlying data storage provider is not part of this package, however the `Kasbah.Provider.Npgsql` provides an integration with PostgreSQL to run alongside the Kasbah core content provider.

## Dependencies

The only dependency will be on the Kasbah core module for tracking engagement with content stored in the Kasbah data structure.

## Integrations

The prime integration will be with the Kasbah Web module.

## Interface

There are three levels of interaction with this module.

 1. Tracking
 2. Reporting
 3. Management

### Tracking

The tracking methods provide a low level means of storing tracking data.

The methods are accessible via the `TrackingService` class.

 * `CreateProfile` creates a new profile and returns a unique identifier for the newly created profile
 * `TrackEvent` tracks an event against a profile
 * `TrackCampaign` tracks a campaign against a profile
 * `MergeProfiles` merges two profiles into one, merging all attributes, events and campaigns together into one record
 * `CreateAttribute` creates a new attribute and assigns it to a profile

### Reporting

The reporting methods provide the functionality for extracting previously reported data.

The methods are acessible via the `ReportingService` class.

The `List` methods can be paginated and configured to only return specific attributes.  The `Get` methods, by default, return all attributes, campaigns and events associated with the requested profile.

  * `ListProfiles` lists all stored profiles
  * `ListProfilesByAttribute` lists all profiles with the matching attribute
  * `ListProfilesByCampaign` lists all profiles that have been tracked in a particular campaign
  * `GetProfile` returns all information relating to an individual profile

### Management

The management methods provide the functionality to configure the analytics module.

The methods are accessible via the `ManagementService` class.

 * `CreateCampaign` creates a new campaign
 * `ListCampaigns` lists all campaigns

## Profile attributes

When attributes are created against a profile old values are retained when the same `alias` is used.

The `List` methods on the reporting interface will by default only take the most recent value for any given attribute.

## Campaign events

Campaign interactions are tracked as events against a profile, they are given the `alias` of "campaign"
