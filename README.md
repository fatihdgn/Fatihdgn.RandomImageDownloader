# Fatihdgn RandomImageDownloader

Downloads random image files from [picsum.photos](https://picsum.photos/). 

Build the application and run it directly, with parameters or run it with an input file.

```Count``` represents the number of images to download

```Paralellism``` represents the maximum parallel download limit.

```SavePath``` represents the path for the output directory.

### Running it directly
```sh
Fatihdgn.RandomImageDownloader.App.exe
```

### Running it with parameters
```sh
Fatihdgn.RandomImageDownloader.App.exe --count 200 --parallelism 5 --savepath ./outputs
```

### Running it with short parameter names
```sh
Fatihdgn.RandomImageDownloader.App.exe -c 200 -p 5 -s ./outputs
```

### Running it with an input file
```sh
Fatihdgn.RandomImageDownloader.App.exe -i input.json
```

### Sample input file
```json
{
 "Count": 200,
 "Parallelism": 5,
 "SavePath": "./outputs"
}

```

