# Honyaku Translation Overlay

Do you like gibberish machine translations? Did you ever want that compounded by the unreliable nature of OCR? Then this is the project for you! The aim of this project is simple: Provide a hassle free way of translating the content of images to english and overlay the translation.

OCR is handled by google cloud vision and translating by Deepl.

Honyaku translation overlay consist of two different solutions: a [chrome extension](#honyaku-Windows-gui) that can translate any **publicly hosted** image free of charge and a "non free" [Windows GUI](#honyaku-Windows-gui) for private desktop use (Though it's non free in the sense that you need to shoulder the API cost by yourself).

> Be aware. Everything is still in wacky alpha state and not reliable or bug free. And so far the current feature set is enough for my own personal needs. But at the same time, only by releasing can I figure out what people need or may want.

Any kind of feedback/feature request is appreciated.
Either create an issue, contact me via email `honyaku-github@aris.moe` or discord `Amiron#8210`

## Honyaku Chrome Extension

[Will insert chrome web store link here once it is approved]

Is a small and simple extension that is capable of translating any **publicly hosted** image free of charge into english. In theory. My own use case of japanese to english works but for other languages I may need to change a bit in the server so I don't keep sending non translatable images to Deepl.

> There is a maximum amount of approximately 40000 images the server will process per month, after that it will refuse any further translation requests.

## Honyaku Windows GUI

A program for translating any screen region through OCR and overlaying the translation with a non interactive overlay. Sadly this is not free and you would need to provide your own API keys to use this.

> Please be aware that on the first startup the program downloads QT in the background.
> I have recently implemented a progress bar feature but haven't gotten yet around to feed the QT download progress into it.

[Download of latest Version](https://github.com/Amiron49/Aris.Moe.Ocr.Translation.Overlay/releases/latest)

[Example usage video](https://youtu.be/6CS_JHkEoxA)

### Available OCR Services

You can set which OCR provider to use by writing the provider name inside the `appsettings.json` under `OcrProvider`

Default is `"Google"`

|Provider Name| 👍 | 👎|
|-|-|-|
|"Google"|Accurate, can handle even the most wacky fonts| Online only, Costs money (~ 2€ per 1000 images), hassle to get the api key|
|"Tesseract"|Free, Offline, just works MOSTLY|Can only interpret normal looking fonts(pretty ok though), needs to download additional additional files when selected, is optimised for manga rn|

### Available Translation Services

You can set which OCR provider to use by writing the provider name inside the `appsettings.json` under `OcrProvider`

Default is `"Deepl"`

|Provider Name| 👍 | 👎|
|-|-|-|
|"Google"|Adequate, First 500k Characters per month are free| Online only, Costs money (20$ per million characters after the first 500k), hassle to get the api|
|"Deepl"|Best Translator currently on the market, getting a key is easy|Online only, Monthly base fee (5$), Costs money (20$ per million characters)|

#### Are there free or offline alternatives?

I really don't know about free as my main usecase is ja -> en and anything less than Deepl/Google is borderline unreadable, so I didn't bother looking too much into that.

I did try to look around for good STS based MTL libraries but literally none had included support for jpn -> en

### How 2 Setup Google API access

This is still an early alpha and only Google Cloud is the only available option for Translation and optionally for OCR. To use this what you will need a `service account`
giving access to a `project` where the `Cloud Translation API` (optionally `Cloud Vision API`) is enabled.

- [Create](https://console.cloud.google.com/projectcreate) a new project,
- Enable `Cloud Translation API` (and optionally `Cloud Vision API`)
- [Create](https://cloud.google.com/docs/authentication/production#create_service_account) a `service account` assigned to your created `project` and give it Project > Owner Role
- This should give you a JSON file with a private key to use.
- Put that file in a Folder called `.private` next to the executable and name it `key.json`
- Edit the `appsettings.json` and set the `Google.ProjectId` to the `projectId` the service account is assigned to

## Planned Features
- Webp Support. Still waiting for the library I use to get support for that.
- A real big boy Database and not just Sqlite it in memory :v