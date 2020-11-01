# Moe Translation Overlay
A program for translating any screen region through OCR and overlaying the translation with a non interactive overlay.

[Example usage video](https://youtu.be/6CS_JHkEoxA)

Also any kind of feedback/feature request is appreciated. 
Either create an issue or contact me via email: `github@aris.moe` or discord `Amiron#8210`

## Usage

This is still an early alpha and only Google Cloud is the only available option for OCR and Translation. To use this what you will need a `service account` 
giving access to a `project` where the `Cloud Translation API` and `Cloud Vision API` is enabled.

- [Create](https://console.cloud.google.com/projectcreate) a new project,
- Enable `Cloud Vision API` and `Cloud Translation API`
- [Create](https://cloud.google.com/docs/authentication/production#create_service_account) a `service account` assigned to your created `project` and give it Project > Owner Role
- This should give you a JSON file with a private key to use.
- Put that file in a Folder called `.private` next to the executable and name it `key.json`
- Edit the appsettings.json and set the `Google.ProjectId` to the `projectId` the service account is assigned to 

So what you need is a Google Cloud [service account key file](https://cloud.google.com/bigquery/docs/authentication/service-account-file)
which you can create as explained by their [documentation](https://cloud.google.com/docs/authentication/production#create_service_account).
that has access to Tra

## Planned features

- Other non google reliant OCR and Translation services
- Companion Extension for Chrome/Firefox to directly overlay translations over browser images
- Non shitty GUI.