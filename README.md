# MillviVideoDownloader

Download video as mp4 embedded by millvi player.

# Usage

## Edit configuration json

Edit `appsettings.json` as follows:

```json
{
  "loginPageUrl": "https://www.somefunclub.com/login",
  "videoPageUrl": "https://www.somefunclub.com/pages/voice",
  "fileNameFormat": [
    "#(\\d+)_(\\d+)-(\\d+)-(\\d+)\\((Sun|Mon|Tue|Wed|Thu|Fri|Sat)\\) Updated",
    "#$1-$2.$3.$4.mp4"
  ],
  "downloadDirectory": "D:\\SomeFunClub\\Voices"
}
```

`fileNameFormat` is the regular expression.

If you want to use multi byte character, save json as UTF-8.

## Put ffmpeg

Put [ffmpeg](https://ffmpeg.org/) in same directory with MillviVideoDownloader.dll.

## Pass authentication info as command line arguments

Run command line as follows:

```
> dotnet "{your MillviVideoDownloader.dll path}" --u {your user id} --p {your password}
```

example:

```
> dotnet "D:\MillviVideoDownloader\MillviVideoDownloader.dll" --u 01091 --p N!ceP@ssw0rd
```

## Hint

You can automate downloading by Windows Task Scheduler, cron, etc.

# Notice

You must use this program for **PRIVATE USE**. You **MUST NOT** publish, transfer, upload to anywhere pay contents.