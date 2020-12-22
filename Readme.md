# Moe Translation Overlay
A program for translating any screen region through OCR and overlaying the translation with a non interactive overlay.

[Example usage video](https://youtu.be/6CS_JHkEoxA)

Also any kind of feedback/feature request is appreciated. 
Either create an issue or contact me via email: `github@aris.moe` or discord `Amiron#8210`

## Available OCR Services

You can set which OCR provider to use by writing the provider name inside the `appsettings.json` under `OcrProvider`

Default is `"Tesseract"`

|Provider Name| 👍 | 👎|
|-|-|-|
|"Google"|Accurate, can handle even the most wacky fonts| Online only, Costs money (~ 2€ per 1000 images), hassle to get the api|
|"Tesseract"|Free, Offline, just works MOSTLY|Can only interpret normal looking fonts(pretty ok though), need to download additional |

## How 2 Setup Google API access

This is still an early alpha and only Google Cloud is the only available option for Translation and optionally for OCR. To use this what you will need a `service account` 
giving access to a `project` where the `Cloud Translation API` (optionally `Cloud Vision API`) is enabled.

- [Create](https://console.cloud.google.com/projectcreate) a new project,
- Enable `Cloud Translation API` (and optionally `Cloud Vision API`) 
- [Create](https://cloud.google.com/docs/authentication/production#create_service_account) a `service account` assigned to your created `project` and give it Project > Owner Role
- This should give you a JSON file with a private key to use.
- Put that file in a Folder called `.private` next to the executable and name it `key.json`
- Edit the `appsettings.json` and set the `Google.ProjectId` to the `projectId` the service account is assigned to 

## Planned features

- Other non google reliant OCR and Translation services
- Companion Extension for Chrome/Firefox to directly overlay translations over browser images
- Non shitty GUI.
